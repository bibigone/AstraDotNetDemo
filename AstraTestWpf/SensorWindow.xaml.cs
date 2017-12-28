using System;
using System.Windows;

namespace AstraTestWpf
{
    /// <summary>
    /// Interaction logic for SensorWindow.xaml
    /// </summary>
    public partial class SensorWindow : Window
    {
        public SensorWindow()
        {
            InitializeComponent();
        }

        internal SensorWindow(SensorViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ViewModel?.Dispose();
            Owner?.Activate();
        }

        private SensorViewModel ViewModel => DataContext as SensorViewModel;
    }
}
