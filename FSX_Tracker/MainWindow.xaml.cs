using System;
using System.Windows;



namespace FSX_Tracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                FSXConnectionManager.TestSimconnect();
            }
            catch (System.IO.FileLoadException e)
            {
                MessageBox.Show("Please install SimConnect SDK");
                Environment.Exit(-1);
            }
            
            InitializeComponent();
            this.DataContext = new MasterViewModel();

        }
    }

}
