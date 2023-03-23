using System.Windows;

namespace WpfSpheres
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SpheresViewModel _viewModel;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var model = new SpheresModel();
            model.RandomiseSpeeds = true;

            _viewModel = new SpheresViewModel(model);

            var window = new SpheresWindow();
            window.DataContext = _viewModel;
            window.Closing += Window_Closing;
            window.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Need to stop the timer
            _viewModel.Dispose();
        }
    }

    
}
