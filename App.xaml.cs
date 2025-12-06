using System;
using System.Windows;

namespace KickBlastJudoManager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Handle unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show($"Fatal Application Error:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                    "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Application Error:\n\n{e.Exception.Message}\n\nStack Trace:\n{e.Exception.StackTrace}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
