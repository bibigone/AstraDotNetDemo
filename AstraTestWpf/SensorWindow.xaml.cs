using System;
using System.Windows;
using System.Windows.Media.Imaging;

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

        private void CopyDepthImageToClipboard(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(ViewModel?.DepthImageSource);
        }

        private void CopyColorImageToClipboard(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(ViewModel?.ColorImageSource);
        }

        private static void CopyToClipboard(BitmapSource bitmap)
        {
            if (bitmap == null)
                Console.Beep();
            else
                Clipboard.SetImage(bitmap);
        }
    }
}
