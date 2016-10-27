using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using MessageBox = System.Windows.Forms.MessageBox;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using ACEntryListGenerator.Annotations;

namespace ACEntryListGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml					   
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly EntryList m_entryList;

        private readonly OpenFileDialog m_ofdEntryList;
        private readonly OpenFileDialog m_ofdRaceData;
        private readonly SaveFileDialog m_sfdEntryList;
        private readonly FolderBrowserDialog m_fbdEntryLists;

        private const int MAX_PER_SERVER = 24;
        private const int MIN_CARS_PER_CLASS = 5;

        private RaceData m_raceSessionData;
        private int m_maxPerServer;
        private int m_minCarsPerClass;
        private Dictionary<string, string> m_classes;

        private readonly string m_sizeDialogText = "The Entry list is larger than the maximum cars per server ({0})\n\n Split the entry list to fit in the {1} servers?";
        private string m_ballastStep;
        private string m_ballastStart;

        private enum SortingType
        {
            Random,
            Position,
            Time
        }

        public RaceData RaceSessionData
        {
            get
            {
                return m_raceSessionData;
            }
            private set
            {
                if( Equals( value, m_raceSessionData ) )
                    return;
                m_raceSessionData = value;
            }
        }

        public String BallastStep
        {
            get { return m_ballastStep; }
            set
            {
                if (value == m_ballastStep) return;
                m_ballastStep = value;
                OnPropertyChanged();
            }
        }

        public String BallastStart
        {
            get { return m_ballastStart; }
            set
            {
                if (value == m_ballastStart) return;
                m_ballastStart = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }


        public MainWindow()
        {
            InitializeComponent();

            ReadConfig();

            m_entryList = new EntryList();
            dgEntryList.DataContext = m_entryList.Entries;

            BallastStart = "30";
            BallastStep = "3";
            
            tbBallastStart.DataContext = tbBallastStep.DataContext = this;

            m_ofdEntryList = new OpenFileDialog();
            m_ofdEntryList.Title = "Select Entry List to Load";
            m_ofdEntryList.FileName = "entry_list.ini";
            m_ofdEntryList.Filter = "Entry List|*.ini";
            m_ofdEntryList.CheckPathExists = true;

            m_ofdRaceData = new OpenFileDialog();
            m_ofdRaceData.Title = "Select Race Data File to Load";
            m_ofdRaceData.FileName = String.Empty;
            m_ofdRaceData.Filter = "JSON File|*.json|Tab Separated File|*.tsv";
            m_ofdRaceData.CheckPathExists = true;

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

            if (!File.Exists("config.ini"))
            {
                m_maxPerServer = MAX_PER_SERVER;
                m_minCarsPerClass = MIN_CARS_PER_CLASS;
            }
            else
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile( "config.ini" );

                foreach( SectionData section in data.Sections )
                {
                    if( section.SectionName.ToLower() == "basic" )
                    {
                        foreach( KeyData key in section.Keys )
                        {
                            if( key.KeyName.ToLower() == "carsperserver" )
                            {
                                if( Int32.TryParse( key.Value, out m_maxPerServer ) == false )
                                    m_maxPerServer = MAX_PER_SERVER;
                            } else if( key.KeyName.ToLower() == "mincarsperclass" )
                            {
                                if( Int32.TryParse( key.Value, out m_minCarsPerClass ) == false )
                                    m_minCarsPerClass = MIN_CARS_PER_CLASS;
                            }
                        }
                    } else if( section.SectionName.ToLower() == "classes" )
                    {
                        foreach( KeyData key in section.Keys )
                        {
                            var cars = key.Value.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
                            foreach( string carName in cars )
                                m_classes[carName] = key.KeyName;
                        }
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

        private void btnLoadRaceData_Click( object sender, RoutedEventArgs e )
        {
            if( m_ofdRaceData.ShowDialog() != System.Windows.Forms.DialogResult.OK )
                return;

            if( Path.GetExtension(m_ofdRaceData.FileName) == ".json" )
            {
                RawRaceData rawData = JsonConvert.DeserializeObject<RawRaceData>( File.ReadAllText( m_ofdRaceData.FileName ) );
                RaceSessionData = RaceData.ParseRawJSONData( rawData ); 
            } else if (Path.GetExtension(m_ofdRaceData.FileName) == ".tsv")
            {
                RaceSessionData = RaceData.ParseRawTSVData( File.ReadAllLines( m_ofdRaceData.FileName ) );
            }
            else
            {
                MessageBox.Show("Error: Invalid race data extension.", "Error", MessageBoxButtons.OK);
                return;
            }

            lblDriverCount.Content = $"Drivers: {RaceSessionData.Drivers.Count}";

            if( RaceSessionData.AllSessions.Count > 0 )
            {
                cbSessions.IsEnabled = true;
                cbSessions.ItemsSource = RaceSessionData.AllSessions;
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

        private void btnApplyBallast_Click( object sender, RoutedEventArgs e )
        {
            Int32 ballast = 0, ballastStep = 0;

            if (!Int32.TryParse(BallastStart, out ballast) || !Int32.TryParse(BallastStep, out ballastStep))
                MessageBox.Show("Error parsing ballast input.\n\nPlease only enter positive numbers.", "Parse Error", MessageBoxButtons.OK);

            if (ballast < 0 || ballastStep < 0)
                MessageBox.Show("Error parsing ballast input.\n\nPlease only enter positive numbers.", "Parse Error", MessageBoxButtons.OK);

            foreach (Entry entry in m_entryList.Entries)
            {
                entry.Ballast = ballast.ToString();
                entry.HadBallastTag = true;

                ballast = Math.Max(0, ballast - ballastStep);
            }
        }
    }
}
