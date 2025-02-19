using AutoMapper;
using Ncs.WpfApp.Helpers;
using Ncs.WpfApp.Models;
using Ncs.WpfApp.Services.Interfaces;
using Ncs.WpfApp.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ncs.WpfApp.ViewModels
{
    public class CustomerViewModel : INotifyPropertyChanged
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        private string _customerInfo;
        private string _rfidInput;

        private int _menuId1;
        private string _menuName1;
        private string _menuDescription1;
        private string _menuCalories1;
        private string _menuStock1;
        private string _menuImage1;

        private int _menuId2;
        private string _menuName2;
        private string _menuDescription2;
        private string _menuCalories2;
        private string _menuStock2;
        private string _menuImage2;

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand CancelCommand { get; }
        public ICommand RfidEnterCommand { get; }
        public ICommand SelectMenuCommand { get; }
        public CustomerViewModel(IMapper mapper, IUserService userService, IOrderService orderService)
        {
            _ = new RelayCommand(async () => await LoadCustomerInfo(), () => true);
            _ = new RelayCommand(async () => await LoadMenuDataAsync(), () => true);
            RfidEnterCommand = new RelayCommand(async () => await ProcessRfidInput(), () => !string.IsNullOrEmpty(RfidInput));
            CancelCommand = new RelayCommand(CancelAction, () => true);
            SelectMenuCommand = new RelayCommand<int>(menuId => OpenMenuConfirmation(menuId));

            _mapper = mapper;
            _userService = userService;
            _orderService = orderService;
            CancelAction();
        }

        public string CustomerInfo
        {
            get => _customerInfo;
            set { _customerInfo = value; OnPropertyChanged(); }
        }

        public string RfidInput
        {
            get => _rfidInput;
            set { _rfidInput = value; OnPropertyChanged(); }
        }

        public int MenuId1
        {
            get => _menuId1;
            set { _menuId1 = value; OnPropertyChanged(); }
        }
        public string MenuName1
        {
            get => _menuName1;
            set { _menuName1 = value; OnPropertyChanged(); }
        }

        public string MenuDescription1
        {
            get => _menuDescription1;
            set { _menuDescription1 = value; OnPropertyChanged(); }
        }

        public string MenuCalories1
        {
            get => _menuCalories1;
            set { _menuCalories1 = value; OnPropertyChanged(); }
        }

        public string MenuStock1
        {
            get => _menuStock1;
            set { _menuStock1 = value; OnPropertyChanged(); }
        }

        public string MenuImage1
        {
            get => _menuImage1;
            set { _menuImage1 = value; OnPropertyChanged(); }
        }

        public int MenuId2
        {
            get => _menuId2;
            set { _menuId2 = value; OnPropertyChanged(); }
        }
        public string MenuName2
        {
            get => _menuName2;
            set { _menuName2 = value; OnPropertyChanged(); }
        }

        public string MenuDescription2
        {
            get => _menuDescription2;
            set { _menuDescription2 = value; OnPropertyChanged(); }
        }

        public string MenuCalories2
        {
            get => _menuCalories2;
            set { _menuCalories2 = value; OnPropertyChanged(); }
        }

        public string MenuStock2
        {
            get => _menuStock2;
            set { _menuStock2 = value; OnPropertyChanged(); }
        }

        public string MenuImage2
        {
            get => _menuImage2;
            set { _menuImage2 = value; OnPropertyChanged(); }
        }

        public async Task LoadCustomerInfo()
        {
            var response = await _userService.RfidSignInAsync(RfidInput);
            if (!response.Success)
            {
                MessageBox.Show("Failed to confirm order." + Environment.NewLine + response.MessageDetail, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
            {
                CustomerInfo = $"{response?.Data?.User.EmployeeNumber} / {response?.Data?.User.Fullname} / {(response?.Data?.User.Company==null? response?.Data?.User.GuestCompanyName: response?.Data?.User.Company.Name)}";
            }
        }

        public async Task LoadMenuDataAsync()
        {
            var result = await _orderService.GetTodayMenus();
            if (result?.Data != null && result.Success)
            {
                var menu1 = _mapper.Map<DailyMenuModel>(result.Data.ToList()[0]);
                MenuId1 = menu1.MenuId;
                MenuName1 = menu1?.MenuName;
                MenuDescription1 = menu1?.MenuDescription;
                MenuCalories1 = menu1?.MenuCalories;
                MenuStock1 = menu1?.MenuStock;
                MenuImage1 = menu1?.MenuImage;
                if (result.Data.Count() > 1)
                {
                    var menu2 = _mapper.Map<DailyMenuModel>(result.Data.ToList()[1]);
                    MenuId2 = menu2.MenuId;
                    MenuName2 = menu2?.MenuName;
                    MenuDescription2 = menu2?.MenuDescription;
                    MenuCalories2 = menu2?.MenuCalories;
                    MenuStock2 = menu2?.MenuStock;
                    MenuImage2 = menu2?.MenuImage;

                }
            }
        }

        private void CancelAction()
        {
            SessionManager.ClearCustomerSession();
            CustomerInfo = string.Empty;
            RfidInput = string.Empty;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = Application.Current.Windows.OfType<CustomerWindow>().FirstOrDefault();
                if (window != null)
                {
                    window.txtRfidInput.Focus();
                }
            });
        }
        private async Task ProcessRfidInput()
        {
            if (string.IsNullOrEmpty(RfidInput)) return;

            var response = await _userService.RfidSignInAsync(RfidInput);
            if (!response.Success)
            {
                MessageBox.Show("Failed to confirm order." + Environment.NewLine + response.MessageDetail, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            RfidInput = string.Empty;
            CustomerInfo = $"{response?.Data?.User.EmployeeNumber} / {response?.Data?.User.Fullname} / {(response?.Data?.User.Company == null ? response?.Data?.User.GuestCompanyName : response?.Data?.User.Company.Name)}";
        }
        private void OpenMenuConfirmation(int menuId)
        {
            var viewModel = new CustomerMenuConfirmationViewModel(_orderService);
            viewModel.Initialize(menuId); // Pass MenuId to the confirmation window

            var confirmationWindow = new CustomerMenuConfirmationWindow
            {
                DataContext = viewModel
            };
            confirmationWindow.ShowDialog();
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
