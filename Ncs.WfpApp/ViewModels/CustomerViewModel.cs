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
        private readonly IReservationService _reservationService;

        private string _customerInfo;
        private string _rfidInput;
        private int? _userId;

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
        public CustomerViewModel(IMapper mapper, IUserService userService, IOrderService orderService, IReservationService reservationService)
        {
            _ = new RelayCommand(async () => await LoadCustomerInfo(), () => true);
            _ = new RelayCommand(async () => await LoadMenuDataAsync(), () => true);
            RfidEnterCommand = new RelayCommand(async () => await ProcessRfidInput(), () => !string.IsNullOrEmpty(RfidInput));
            CancelCommand = new RelayCommand(CancelAction, () => true);
            SelectMenuCommand = new RelayCommand<int>(menuId => OpenMenuConfirmation(menuId));

            _mapper = mapper;
            _userService = userService;
            _orderService = orderService;
            _reservationService = reservationService;
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

        public int? UserId
        {
            get => _userId;
            set { _userId = value; OnPropertyChanged(); }
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
                UserId = response?.Data?.User.Id;
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
            UserId = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = Application.Current.Windows.OfType<CustomerWindow>().FirstOrDefault();
                if (window != null)
                {
                    window.txtRfidInput.Focus();
                }
            });
        }
        public async Task ProcessRfidInput()
        {
            if (string.IsNullOrEmpty(RfidInput)) return;

            // 🔹 Automatically Trim Extra Spaces or Special Characters
            RfidInput = RfidInput.Trim();

            // 🔹 Check if the RFID contains only valid characters (numbers or letters)
            if (!System.Text.RegularExpressions.Regex.IsMatch(RfidInput, @"^[A-Za-z0-9]+$"))
            {
                MessageBox.Show("Invalid RFID format. Only alphanumeric characters are allowed.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                RfidInput = string.Empty; // Clear input
                return;
            }

            // 🔹 Detect RFID Length Dynamically
            int rfidLength = RfidInput.Length;

            // Common RFID tag lengths (adjust if necessary)
            HashSet<int> validRfidLengths = new HashSet<int> { 4, 7, 10, 12, 24, 32 };

            if (!validRfidLengths.Contains(rfidLength))
            {
                MessageBox.Show($"Invalid RFID length ({rfidLength} characters).",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                RfidInput = string.Empty; // Clear input
                return;
            }

            // Proceed with authentication
            var response = await _userService.RfidSignInAsync(RfidInput);
            if (!response.Success)
            {
                MessageBox.Show("Failed to confirm order." + Environment.NewLine + response.MessageDetail,
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 🔹 Clear Input After Successful Scan
            RfidInput = string.Empty;

            // 🔹 Display Customer Information
            CustomerInfo = $"{(response?.Data?.User.EmployeeNumber == null ? response?.Data?.User?.PersonalIdNumber : response?.Data?.User.EmployeeNumber)} / {response?.Data?.User.Fullname} / " +
                           $"{(response?.Data?.User.Company == null ? response?.Data?.User.GuestCompanyName : response?.Data?.User.Company.Name)}";
            UserId = response?.Data?.User.Id;
        }


        private void OpenMenuConfirmation(int menuId)
        {
            var viewModel = new CustomerMenuConfirmationViewModel(_orderService, _reservationService);
            _ = viewModel.InitializeAsync(menuId, UserId ?? 0); // Pass MenuId to the confirmation window, default to 0 if UserId is null

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
