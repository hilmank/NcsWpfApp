using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Ncs.WpfApp.Services.Interfaces;
using Ncs.WpfApp.Helpers;

namespace Ncs.WpfApp.ViewModels
{
    public class OrdersConfirmationViewModel : INotifyPropertyChanged
    {
        private readonly IOrderService _orderService;
        private int _orderId;
        private string _currentStatus;

        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string>();

        private string _selectedAction;
        public string SelectedAction
        {
            get => _selectedAction;
            set
            {
                _selectedAction = value;
                OnPropertyChanged();
                ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged(); // Notify SubmitCommand to re-evaluate its CanExecute
            }
        }


        public ICommand SubmitCommand { get; }

        public OrdersConfirmationViewModel(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));

            SubmitCommand = new RelayCommand(async () => await SubmitAsync(), () => !string.IsNullOrEmpty(SelectedAction));
        }
        public void Initialize(int orderId, string currentStatus)
        {
            _orderId = orderId;
            _currentStatus = currentStatus;
            LoadStatusOptions();
        }
        private void LoadStatusOptions()
        {
            if (_currentStatus == "Ordered")
            {
                StatusOptions.Add("Cancel");
                StatusOptions.Add("InProcess");
            }
            else if (_currentStatus == "InProcess")
            {
                StatusOptions.Add("Cancel");
                StatusOptions.Add("Complete");
            }
        }

        private async Task SubmitAsync()
        {
            if (string.IsNullOrEmpty(SelectedAction))
            {
                MessageBox.Show("Please select an action.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string action = SelectedAction switch
            {
                "Cancel" => "cancel",
                "InProcess" => "inprocess",
                "Complete" => "complete",
                _ => throw new ArgumentException("Invalid action")
            };

            var response = await _orderService.SaveStatusActionAsync(action, _orderId);

            if (response.Success)
            {
                MessageBox.Show("Order status updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
            }
            else
            {
                MessageBox.Show("Failed to update order status.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
