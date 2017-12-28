using System;
using System.Globalization;
using System.Windows;

namespace AstraTestWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenSensor0(object sender, RoutedEventArgs e)
        {
            var withColor = checkBoxWithColor.IsChecked ?? false;
            OpenSensor(0, withColor);
        }

        private void OpenSensor1(object sender, RoutedEventArgs e)
        {
            var withColor = checkBoxWithColor.IsChecked ?? false;
            OpenSensor(1, withColor);
        }

        private void OpenSensor(int sensorIndx, bool withColor)
        {
            try
            {
                var connectionString = "device/sensor" + sensorIndx.ToString(CultureInfo.InvariantCulture);
                var set = Astra.StreamSet.Open(connectionString);
                var sensorViewModel = new SensorViewModel(Properties.Settings.Default, Dispatcher, set, withColor);

                var wnd = new SensorWindow(sensorViewModel) { Owner = this };
                wnd.Title += " #" + sensorIndx;
                wnd.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Cannot open depth sensor #{sensorIndx}:\r\n{exc.Message}", "Error");
            }
        }
    }
}
