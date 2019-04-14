﻿using System;
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

        private void OpenSensor0(object sender, RoutedEventArgs e) => OpenSensor(0);

        private void OpenSensor1(object sender, RoutedEventArgs e) => OpenSensor(1);

        private void OpenSensor(int sensorIndx)
        {
            var withDepth = checkBoxWithDepth.IsChecked ?? false;
            var withColor = checkBoxWithColor.IsChecked ?? false;
            var withInfrared = checkBoxWithInfrared.IsChecked ?? false;
            var withBodyTracking = checkBoxWithBodyTracking.IsChecked ?? false;
            OpenSensor(sensorIndx, withDepth, withColor, withInfrared, withBodyTracking);
        }

        private void OpenSensor(int sensorIndx, bool withDepth, bool withColor, bool withInfrared, bool withBodyTracking)
        {
            try
            {
                var connectionString = "device/sensor" + sensorIndx.ToString(CultureInfo.InvariantCulture);
                var set = Astra.StreamSet.Open(connectionString);
                
                var sensorViewModel = new SensorViewModel(Properties.Settings.Default, Dispatcher, set, 
                    withDepth: withDepth,
                    withColor:withColor, 
                    withInfrared: withInfrared, 
                    withBodyTracking: withBodyTracking);

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
