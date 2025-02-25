using AutoMapper;
using Ncs.WpfApp.Helpers;
using Ncs.WpfApp.Models;
using Ncs.WpfApp.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Ncs.WpfApp.ViewModels
{
    public class OrdersConfirmationViewModel : INotifyPropertyChanged
    {
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private int _orderId;
        private string _currentStatus;
        private string _selectedAction;
        private Visibility _reservationVisibility = Visibility.Collapsed;

        public OrdersConfirmationViewModel(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _mapper = mapper;
            SubmitCommand = new RelayCommand(async () => await SubmitAsync(), CanSubmit);
            StatusOptions = new ObservableCollection<string>();
            Orders = new ObservableCollection<OrderListModel>();
        }

        public ObservableCollection<string> StatusOptions { get; }
        public ObservableCollection<OrderListModel> Orders { get; }
        public ICommand SubmitCommand { get; }

        public string SelectedAction
        {
            get => _selectedAction;
            set
            {
                _selectedAction = value;
                OnPropertyChanged();
                ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
            }
        }

        public Visibility ReservationVisibility
        {
            get => _reservationVisibility;
            set
            {
                _reservationVisibility = value;
                OnPropertyChanged();
                ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
            }
        }

        public async Task InitializeAsync(int orderId, string currentStatus)
        {
            _orderId = orderId;
            _currentStatus = currentStatus;
            LoadStatusOptions();

            var result = await _orderService.GetReservationOrdersAsync(orderId);
            if (result.Success)
            {
                ReservationVisibility = Visibility.Visible;
                Orders.Clear();
                foreach (var order in result?.Data )
                {
                    Orders.Add(_mapper.Map<OrderListModel>(order));
                }
            }
            else
            {
                ReservationVisibility = Visibility.Collapsed;
            }
        }

        private void LoadStatusOptions()
        {
            StatusOptions.Clear();
            switch (_currentStatus)
            {
                case "Ordered":
                    StatusOptions.Add("Cancel");
                    StatusOptions.Add("InProcess");
                    break;
                case "InProcess":
                    StatusOptions.Add("Cancel");
                    StatusOptions.Add("Complete");
                    break;
            }
        }

        private bool CanSubmit()
        {
            return !string.IsNullOrEmpty(SelectedAction) || ReservationVisibility == Visibility.Visible;
        }

        private async Task SubmitAsync()
        {
            if (string.IsNullOrEmpty(SelectedAction))
            {
                MessageBox.Show("Please select an action.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string action = SelectedAction.ToLower();
            ApiResponseModel<bool> response;

            if (ReservationVisibility == Visibility.Visible && Orders.Any())
            {
                response = await _orderService.SaveStatussActionAsync(action, Orders.Select(x => x.OrdersId).ToList());
            }
            else
            {
                response = await _orderService.SaveStatusActionAsync(action, _orderId);
            }

            HandleResponse(response);
        }

        private void HandleResponse(ApiResponseModel<bool> response)
        {
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
