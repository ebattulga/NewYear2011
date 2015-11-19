using System;
using System.Windows;

namespace NewYear2011.Dialog
{
    /// <summary>
    /// Interaction logic for winSettings.xaml
    /// </summary>
    public partial class winSettings : Window
    {
        public winSettings()
        {
            InitializeComponent();
        }
        public event EventHandler SetDefaultSettings;
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            Close();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (SetDefaultSettings != null)
                SetDefaultSettings(this, null);
        }

        
    }
}
