using System.Windows;

namespace AstraTestWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            // To resolve reference to AstraDotNet.dll depending on architecture (32- or 64-bit)
            AstraDotNetAssemblyResolver.Init();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialization of Astra SDK
            Astra.Context.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // Deinitialization of Astra SDK
            // Without this call application will crash on exit
            Astra.Context.Terminate();
        }
    }
}
