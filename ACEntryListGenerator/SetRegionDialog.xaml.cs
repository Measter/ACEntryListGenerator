using System;
using System.Windows;

namespace ACEntryListGenerator
{
    /// <summary>
    /// Interaction logic for SetRegionDialog.xaml
    /// </summary>
    public partial class SetRegionDialog : Window
    {
        public Region Region { get; private set; }

        public SetRegionDialog()
        {
            InitializeComponent();
            cbRegionBox.ItemsSource = Enum.GetValues(typeof(Region));
            cbRegionBox.SelectedIndex = 0;
        }

        private void bnCancel_Click( object sender, RoutedEventArgs e )
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnOkay_Click( object sender, RoutedEventArgs e )
        {
            this.DialogResult = true;
            this.Region = (Region)cbRegionBox.SelectedItem;
            this.Close();
        }
    }
}
