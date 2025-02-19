using Microsoft.Extensions.DependencyInjection;
using Ncs.WpfApp.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Ncs.WpfApp.Views
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private DispatcherTimer _dataRefreshTimer;
        private DispatcherTimer _clockTimer;

        public AdminWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetService<AdminViewModel>();

            Loaded += AdminWindow_Loaded;
            StartClock();
            StartDataRefresh();
        }

        private async void AdminWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await ReloadDataAsync();
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
            if (DataContext is AdminViewModel viewModel)
            {
                if (viewModel.SearchCommandUsers.CanExecute(null))
                {
                    await viewModel.LoadAllDataAsync();
                }
            }
        }
        private void OrdersDataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("DataGrid Clicked!");
        }
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hyperlink Clicked!");
        }

    }

}
