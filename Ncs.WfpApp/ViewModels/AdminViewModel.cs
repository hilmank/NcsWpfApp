using AutoMapper;
using Ncs.WpfApp.Helpers;
using Ncs.WpfApp.Models;
using Ncs.WpfApp.Services;
using Ncs.WpfApp.Services.Interfaces;
using Ncs.WpfApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Ncs.WpfApp.ViewModels
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IReservationService _reservationService;

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
            StatusActionCommandOrders = new RelayCommand<object>(param => StatusAction(param), param => param != null);

            SearchCommandAllOrders = new RelayCommand(async () => await LoadDataAllOrdersAsync(), () => true);
            RefreshCommandAllOrders = new RelayCommand(async () => await RefreshDataAllOrdersAsync(), () => true);

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

            #region Reservations
            _reservationService = reservationService ?? throw new ArgumentNullException(nameof(ReservationService));
            Reservations = new ObservableCollection<ReservationListModel>();
            SearchCommandReservations = new RelayCommand(async () => await LoadDataReservationsAsync(), () => true);
            RefreshCommandReservations = new RelayCommand(async () => await RefreshDataReservationsAsync(), () => true);
            FirstPageCommandReservations = new RelayCommand(async () => await NavigateToFirstPageReservations(), () => true);
            PreviousPageCommandReservations = new RelayCommand(async () => await NavigateToPreviousPageReservations(), () => true);
            NextPageCommandReservations = new RelayCommand(async () => await NavigateToNextPageReservations(), () => true);
            LastPageCommandReservations = new RelayCommand(async () => await NavigateToLastPageReservations(), () => true);
            #endregion
        }
        public async Task LoadAllDataAsync()
        {
            await LoadDataOrdersAsync();
            await LoadDataAllOrdersAsync();
            await LoadDataUsersAsync();
            await LoadDataReservationsAsync();
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
        private ObservableCollection<OrderListModel> _orders = [];
        public ObservableCollection<OrderListModel> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders)); 
            }
        }
        public ICommand StatusActionCommandOrders { get; }
        public async Task LoadDataOrdersAsync()
        {
            List<string> orderStatuss = ["Ordered", "InProcess"];
            var result = await _orderService.GetOrderTodayAsync(string.Empty, orderStatuss);
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

        #region All AllOrders
        private ObservableCollection<OrderListModel> _allOrders = [];
        public ObservableCollection<OrderListModel> AllOrders
        {
            get => _allOrders;
            set
            {
                _allOrders = value;
                OnPropertyChanged(nameof(AllOrders));
            }
        }
        private string _searchTextAllOrders;
        public string SearchTextAllOrders
        {
            get => _searchTextAllOrders;
            set { _searchTextAllOrders = value; OnPropertyChanged(); }
        }

        public ICommand SearchCommandAllOrders { get; }
        public ICommand RefreshCommandAllOrders { get; }
        public async Task LoadDataAllOrdersAsync()
        {
            var result = await _orderService.GetOrderTodayAsync(SearchTextAllOrders, null);
            if (result?.Data != null && result.Success)
            {
                Menu1Name = result.Data.Menu1Name;
                Menu1Available = result.Data.Menu1Available;
                Menu2Name = result.Data.Menu2Name;
                Menu2Available = result.Data.Menu2Available;
                AllOrders.Clear();
                foreach (var order in _mapper.Map<List<OrderListModel>>(result.Data.Orders))
                {
                    AllOrders.Add(order);
                }
            }
        }
        private async Task RefreshDataAllOrdersAsync() => await LoadDataAllOrdersAsync();
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
        public ICommand SearchCommandUsers { get; }
        public ICommand RefreshCommandUsers { get; }
        public ICommand FirstPageCommandUsers { get; }
        public ICommand PreviousPageCommandUsers { get; }
        public ICommand NextPageCommandUsers { get; }
        public ICommand LastPageCommandUsers { get; }
        public ICommand AddCommandUsers { get; }
        public string SearchTextUsers { get; set; }

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
        #region Reservations
        private ObservableCollection<ReservationListModel> _reservations = new ObservableCollection<ReservationListModel>();
        public ObservableCollection<ReservationListModel> Reservations
        {
            get => _reservations;
            set
            {
                _reservations = value;
                OnPropertyChanged(nameof(Users)); // ✅ Notify UI when changed
            }
        }
        public ICommand SearchCommandReservations { get; }
        public ICommand RefreshCommandReservations { get; }
        public ICommand FirstPageCommandReservations { get; }
        public ICommand PreviousPageCommandReservations { get; }
        public ICommand NextPageCommandReservations { get; }
        public ICommand LastPageCommandReservations { get; }
        public ICommand AddCommandReservations { get; }
        public string SearchTextReservations { get; set; }
        public async Task LoadDataReservationsAsync()
        {
            var result = await _reservationService.GetReservationsTodayAsync(SearchTextReservations, null);
            if (result?.Data != null && result.Success)
            {
                Reservations.Clear();
                foreach (var user in _mapper.Map<List<ReservationListModel>>(result.Data))
                {
                    Reservations.Add(user);
                }
            }
        }
        private async Task RefreshDataReservationsAsync() => await LoadDataReservationsAsync();
        private async Task NavigateToFirstPageReservations() { /* Pagination logic */ }
        private async Task NavigateToPreviousPageReservations() { /* Pagination logic */ }
        private async Task NavigateToNextPageReservations() { /* Pagination logic */ }
        private async Task NavigateToLastPageReservations() { /* Pagination logic */ }
        #endregion
    }
}
