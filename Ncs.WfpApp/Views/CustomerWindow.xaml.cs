using Microsoft.Extensions.DependencyInjection;
using Ncs.WpfApp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Ncs.WpfApp.Views
{
    /// <summary>
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        private DispatcherTimer _dataRefreshTimer;
        private DispatcherTimer _clockTimer;
        public CustomerWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetService<CustomerViewModel>();

            Loaded += CustomerWindow_Loaded;
            StartClock();
            StartDataRefresh();

        }

        private async void CustomerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CustomerViewModel viewModel)
            {
                await viewModel.LoadMenuDataAsync();
            }
        }

        private void StartClock()
        {
            _clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Updates every second
            };
            _clockTimer.Tick += UpdateDateTime;
            _clockTimer.Start();
        }

        private void UpdateDateTime(object sender, EventArgs e)
        {
            DateTimeText.Text = DateTime.Now.ToString("dddd, MMMM dd yyyy - HH:mm:ss");
        }

        private void StartDataRefresh()
        {
            _dataRefreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5) // Refresh data every 5 seconds
            };
            _dataRefreshTimer.Tick += async (sender, e) => await ReloadDataAsync();
            _dataRefreshTimer.Start();
        }
        private async Task ReloadDataAsync()
        {
            if (DataContext is CustomerViewModel viewModel)
            {
                await viewModel.LoadMenuDataAsync();
            }
        }
        private async void txtRfidInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    // Wait a short delay to ensure scanning is complete
                    await Task.Delay(200);

                    // Call RFID processing method in ViewModel
                    await ((CustomerViewModel)DataContext).ProcessRfidInput();
                }
            }
        }

    }

}
