using AutoMapper;
using Ncs.WpfApp.Helpers;
using Ncs.WpfApp.Models;
using Ncs.WpfApp.Services.Interfaces;
using Ncs.WpfApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Ncs.WpfApp.ViewModels
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public AdminViewModel(
            IOrderService orderService,
            IReservationService reservationService,
            IUserService userService,
            IMapper mapper)
        {
            _mapper = mapper;
            #region Orders
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            Orders = new ObservableCollection<OrderListModel>();
            SearchCommandOrders = new RelayCommand(async () => await LoadDataOrdersAsync(SearchTextOrders), () => true);
            RefreshCommandOrders = new RelayCommand(async () => await RefreshDataOrdersAsync(), () => true);
            StatusActionCommandOrders = new RelayCommand<object>(async param => StatusAction(param), param => param != null);

            #endregion

            #region User
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            Users = new ObservableCollection<UserListModel>();
            SearchCommandUsers = new RelayCommand(async () => await LoadDataUsersAsync(SearchTextUsers), () => true);
            RefreshCommandUsers = new RelayCommand(async () => await RefreshDataUsersAsync(), () => true);
            FirstPageCommandUsers = new RelayCommand(async () => await NavigateToFirstPageUsers(), () => true);
            PreviousPageCommandUsers = new RelayCommand(async () => await NavigateToPreviousPageUsers(), () => true);
            NextPageCommandUsers = new RelayCommand(async () => await NavigateToNextPageUsers(), () => true);
            LastPageCommandUsers = new RelayCommand(async () => await NavigateToLastPageUsers(), () => true);
            AddCommandUsers = new RelayCommand(OpenUserAddWindow, () => true);
            #endregion
        }
        public async Task LoadAllDataAsync()
        {
            await LoadDataUsersAsync();
            await LoadDataOrdersAsync();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region Orders
        private string _menu1Name;
        public string Menu1Name
        {
            get => _menu1Name;
            set { _menu1Name = value; OnPropertyChanged(); }
        }

        private string _menu1Available;
        public string Menu1Available
        {
            get => _menu1Available;
            set { _menu1Available = value; OnPropertyChanged(); }
        }

        private string _menu2Name;
        public string Menu2Name
        {
            get => _menu2Name;
            set { _menu2Name = value; OnPropertyChanged(); }
        }

        private string _menu2Available;
        public string Menu2Available
        {
            get => _menu2Available;
            set { _menu2Available = value; OnPropertyChanged(); }
        }
        private string _statusActionOrders;
        public string StatusActionOrders
        {
            get => _statusActionOrders;
            set { _statusActionOrders = value; OnPropertyChanged(); }
        }
        private ObservableCollection<OrderListModel> _orders = new ObservableCollection<OrderListModel>();
        public ObservableCollection<OrderListModel> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders)); 
            }
        }
        public ICommand SearchCommandOrders { get; }
        public ICommand RefreshCommandOrders { get; }
        public ICommand StatusActionCommandOrders { get; }
        public string SearchTextOrders { get; set; }
        public async Task LoadDataOrdersAsync(string searchText = "")
        {
            var result = await _orderService.GetOrderAsync(searchText);
            if (result?.Data != null && result.Success)
            {
                Menu1Name = result.Data.Menu1Name;
                Menu1Available = result.Data.Menu1Available;
                Menu2Name = result.Data.Menu2Name;
                Menu2Available = result.Data.Menu2Available;
                Orders.Clear();
                foreach (var order in _mapper.Map<List<OrderListModel>>(result.Data.Orders))
                {
                    Orders.Add(order);
                }
            }
        }
        private async Task RefreshDataOrdersAsync() => await LoadDataOrdersAsync();
        private void StatusAction(object parameter)
        {
            if (parameter is OrderParameters orderParam)
            {

                var viewModel = new OrdersConfirmationViewModel(_orderService);
                viewModel.Initialize(orderParam.OrderId, orderParam.OrderStatus);

                var confirmationWindow = new OrdersConfirmationWindow { DataContext = viewModel };
                confirmationWindow.ShowDialog();
            }
        }





        #endregion
        #region Users
        private ObservableCollection<UserListModel> _users = new ObservableCollection<UserListModel>();
        public ObservableCollection<UserListModel> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users)); // ✅ Notify UI when changed
            }
        }
        #region Users Command
        public ICommand SearchCommandUsers { get; }
        public ICommand RefreshCommandUsers { get; }
        public ICommand FirstPageCommandUsers { get; }
        public ICommand PreviousPageCommandUsers { get; }
        public ICommand NextPageCommandUsers { get; }
        public ICommand LastPageCommandUsers { get; }
        public ICommand AddCommandUsers { get; }
        public string SearchTextUsers { get; set; }
        #endregion

        public async Task LoadDataUsersAsync(string searchText = "")
        {
            var result = await _userService.GetUsersAsync(searchText);
            if (result?.Data != null && result.Success)
            {
                Users.Clear();
                foreach (var user in _mapper.Map<List<UserListModel>>(result.Data))
                {
                    Users.Add(user);
                }
            }
        }
        private async Task RefreshDataUsersAsync() => await LoadDataUsersAsync();
        private async Task NavigateToFirstPageUsers() { /* Pagination logic */ }
        private async Task NavigateToPreviousPageUsers() { /* Pagination logic */ }
        private async Task NavigateToNextPageUsers() { /* Pagination logic */ }
        private async Task NavigateToLastPageUsers() { /* Pagination logic */ }

        private static void OpenUserAddWindow()
        {
            var userAddWindow = new UserAddWindow();
            userAddWindow.ShowDialog();
        }
        #endregion

    }
}
