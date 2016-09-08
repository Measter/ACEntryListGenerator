﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using MessageBox = System.Windows.Forms.MessageBox;
using System;
using System.Windows.Controls;

namespace ACEntryListGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml					   
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly EntryList m_entryList;

        private readonly OpenFileDialog m_ofdEntryList;
        private readonly OpenFileDialog m_ofdJson;
        private readonly SaveFileDialog m_sfdEntryList;
        private readonly FolderBrowserDialog m_fbdEntryLists;

        private RaceData m_jsonData;
        private int m_maxPerServer;
        private int m_minCarsPerClass;
        private Dictionary<string, string> m_classes;

        private readonly string m_sizeDialogText = "The Entry list is larger than the maximum cars per server ({0})\n\n Split the entry list to fit in the {1} servers?";

        private enum SortingType
        {
            Random,
            Position,
            Time
        }

        public RaceData JSONData
        {
            get
            {
                return m_jsonData;
            }
            private set
            {
                if( Equals( value, m_jsonData ) )
                    return;
                m_jsonData = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            ReadConfig();

            m_entryList = new EntryList();
            dgEntryList.DataContext = m_entryList.Entries;

            m_ofdEntryList = new OpenFileDialog();
            m_ofdEntryList.Title = "Select Entry List to Load";
            m_ofdEntryList.FileName = "entry_list.ini";
            m_ofdEntryList.Filter = "Entry List|*.ini";
            m_ofdEntryList.CheckPathExists = true;

            m_ofdJson = new OpenFileDialog();
            m_ofdJson.Title = "Select JSON File to Load";
            m_ofdJson.FileName = "race_out.json";
            m_ofdJson.Filter = "JSON File|*.json";
            m_ofdJson.CheckPathExists = true;

            m_sfdEntryList = new SaveFileDialog();
            m_sfdEntryList.Title = "Choose Location of Entry List";
            m_sfdEntryList.FileName = "entry_list.ini";
            m_sfdEntryList.Filter = "Entry List|*.ini";

            m_fbdEntryLists = new FolderBrowserDialog();
            m_fbdEntryLists.Description = "Select Folder to save entry lists. (Existing entries will be overwritten.)";
            m_fbdEntryLists.SelectedPath = Environment.CurrentDirectory;

            cbSortingType.Items.Add( SortingType.Random );
            cbSortingType.Items.Add( SortingType.Position );
            cbSortingType.Items.Add( SortingType.Time );
            cbSortingType.SelectedIndex = 0;
        }

        private void ReadConfig()
        {
            m_classes = new Dictionary<string, string>();

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile( "config.ini" );

            foreach( SectionData section in data.Sections )
            {
                if( section.SectionName.ToLower() == "basic" )
                {
                    foreach (KeyData key in section.Keys)
                    {
                        if (key.KeyName.ToLower() == "carsperserver")
                        {
                            if( Int32.TryParse( key.Value, out m_maxPerServer ) == false )
                                m_maxPerServer = 24;
                        } else if (key.KeyName.ToLower() == "mincarsperclass")
                        {
                            if (Int32.TryParse(key.Value, out m_minCarsPerClass) == false)
                                m_minCarsPerClass = 5;
                        }
                    }
                } else if( section.SectionName.ToLower() == "classes" )
                {
                    foreach( KeyData key in section.Keys )
                    {
                        var cars = key.Value.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string carName in cars)
                            m_classes[carName] = key.KeyName;
                    }
                }
            }
        }

        private void btnOpenEntryList_Click( object sender, RoutedEventArgs e )
        {
            if( m_ofdEntryList.ShowDialog() != System.Windows.Forms.DialogResult.OK )
                return;

            cbReverseList.SelectedIndex = -1;
            cbReverseList.Items.Clear();

            List<Entry> newEntries = Entry.ParseEntryList( File.ReadAllLines( m_ofdEntryList.FileName ) );
            m_entryList.AddEntries( newEntries );

            for( int i = 1; i <= m_entryList.Entries.Count; i++ )
                cbReverseList.Items.Add( i );

            cbReverseList.SelectedIndex = 9;
        }

        private void btnLoadJson_Click( object sender, RoutedEventArgs e )
        {
            if( m_ofdJson.ShowDialog() != System.Windows.Forms.DialogResult.OK )
                return;

            RawRaceData rawData = JsonConvert.DeserializeObject<RawRaceData>( File.ReadAllText( m_ofdJson.FileName ) );
            JSONData = RaceData.ParseRawData( rawData );

            lblDriverCount.Content = $"Drivers: {JSONData.Drivers.Count}";

            if( JSONData.AllSessions.Count > 0 )
            {
                cbSessions.IsEnabled = true;
                cbSessions.ItemsSource = JSONData.AllSessions;
                cbSessions.SelectedIndex = 0;

                btnUpdateEntry.IsEnabled = true;
            } else
            {
                cbSessions.IsEnabled = false;
                cbSessions.SelectedIndex = -1;
                cbSessions.ItemsSource = null;

                btnUpdateEntry.IsEnabled = false;
            }
        }

        private void btnSortEntries_Click( object sender, RoutedEventArgs e )
        {
            SortingType type = (SortingType)cbSortingType.SelectionBoxItem;

            switch( type )
            {
                case SortingType.Random:
                    m_entryList.SortEntryListByRandom();
                    break;
                case SortingType.Position:
                    m_entryList.SortEntryListByPosition( (int)cbReverseList.SelectionBoxItem );
                    break;
                case SortingType.Time:
                    m_entryList.SortEntryListByLapTime( (int)cbReverseList.SelectionBoxItem );
                    break;
            }
        }

        private void btnUpdateEntry_Click( object sender, RoutedEventArgs e )
        {
            CleanSession ses = cbSessions.SelectedItem as CleanSession;

            m_entryList.UpdateSessionData( ses, cbUpdateMissing.IsChecked.HasValue && cbUpdateMissing.IsChecked.Value );
        }

        private void btnSaveEntryList_Click( object sender, RoutedEventArgs e )
        {
            int serverCount = (int) Math.Ceiling(m_entryList.Entries.Count/(float) m_maxPerServer);

            if( m_entryList.Entries.Count <= m_maxPerServer || MessageBox.Show( String.Format(m_sizeDialogText, m_maxPerServer, serverCount), "Entry List Too Big", MessageBoxButtons.YesNo ) != System.Windows.Forms.DialogResult.Yes )
            {
                if( m_sfdEntryList.ShowDialog() != System.Windows.Forms.DialogResult.OK )
                    return;

                Entry.SaveEntryList( m_entryList.Entries.ToList(), m_sfdEntryList.FileName );

                return;
            }

            if( m_fbdEntryLists.ShowDialog() != System.Windows.Forms.DialogResult.OK )
                return;

            Entry.SaveEntryListSimpleSplit( m_entryList.Entries.ToList(), m_maxPerServer, m_minCarsPerClass, m_classes, m_fbdEntryLists.SelectedPath );
        }

        private void btnDeleteEntry_Click( object sender, RoutedEventArgs e )
        {
            if( MessageBox.Show( "Are you sure you wish to delete the selected entrees?", "Delete Entries?",
                                  MessageBoxButtons.YesNo ) == System.Windows.Forms.DialogResult.No )
            {
                return;
            }

            List<Entry> selItems = dgEntryList.SelectedItems.OfType<Entry>().ToList();

            if( !selItems.Any() )
                return;

            m_entryList.DeleteEntries( selItems );
        }

        private void dgEntryList_LoadingRow( object sender, System.Windows.Controls.DataGridRowEventArgs e )
        {
            e.Row.Header = ( e.Row.GetIndex() ).ToString();
        }

        private void BtnMassSetRegion_OnClick( object sender, RoutedEventArgs e )
        {
            List<Entry> selItems = dgEntryList.SelectedItems.OfType<Entry>().ToList();

            if( !selItems.Any() )
                return;

            SetRegionDialog box = new SetRegionDialog();

            if( box.ShowDialog() == false )
                return; // User Cancelled

            foreach( Entry ent in selItems )
                ent.Region = box.Region;
        }
        
    }
}