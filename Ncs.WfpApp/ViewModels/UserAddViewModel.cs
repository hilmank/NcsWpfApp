using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Ncs.WpfApp.Helpers;
using Ncs.WpfApp.Models;
using Ncs.WpfApp.Models.ApiResponse;
using Ncs.WpfApp.Services.Interfaces;
using Ncs.WpfApp.Views;

namespace Ncs.WpfApp.ViewModels
{
    public class UserAddViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;

        public UserAddViewModel(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            // Initialize collections
            Companies = new ObservableCollection<CompaniesDto>();
            PersonalIdTypes = new ObservableCollection<PersonalIdTypeDto>();
            Roles = new ObservableCollection<RolesDto>();

            // Load data
            LoadCompanies();
            LoadPersonalIdTypes();
            LoadRoles();

            // Define Commands
            SaveCommand = new RelayCommand(async () => await SaveUserAsync(), CanSave);
            CancelCommand = new RelayCommand(Cancel, () => true);
        }

        // Properties
        private string _rfidTag;
        public string RfidTag
        {
            get => _rfidTag;
            set { _rfidTag = value; OnPropertyChanged(); }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _firstname;
        public string Firstname
        {
            get => _firstname;
            set { _firstname = value; OnPropertyChanged(); }
        }

        private string _middlename;
        public string Middlename
        {
            get => _middlename;
            set { _middlename = value; OnPropertyChanged(); }
        }

        private string _lastname;
        public string Lastname
        {
            get => _lastname;
            set { _lastname = value; OnPropertyChanged(); }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        private string _employeeNumber;
        public string EmployeeNumber
        {
            get => _employeeNumber;
            set { _employeeNumber = value; OnPropertyChanged(); }
        }

        private int? _selectedCompanyId;
        public int? SelectedCompanyId
        {
            get => _selectedCompanyId;
            set { _selectedCompanyId = value; OnPropertyChanged(); }
        }

        private string _guestCompanyName;
        public string GuestCompanyName
        {
            get => _guestCompanyName;
            set { _guestCompanyName = value; OnPropertyChanged(); }
        }

        private int? _selectedPersonalIdType;
        public int? SelectedPersonalIdType
        {
            get => _selectedPersonalIdType;
            set { _selectedPersonalIdType = value; OnPropertyChanged(); }
        }

        private string _personalIdNumber;
        public string PersonalIdNumber
        {
            get => _personalIdNumber;
            set { _personalIdNumber = value; OnPropertyChanged(); }
        }

        private int _selectedRole;
        public int SelectedRole
        {
            get => _selectedRole;
            set { _selectedRole = value; OnPropertyChanged(); }
        }

        // Collections
        public ObservableCollection<CompaniesDto> Companies { get; set; }
        public ObservableCollection<PersonalIdTypeDto> PersonalIdTypes { get; set; }
        public ObservableCollection<RolesDto> Roles { get; set; }

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Load data methods
        private async void LoadCompanies()
        {
            var companies = await _userService.GetCompaniesAsync();
            Companies.Clear();
            foreach (var company in companies.Data)
                Companies.Add(company);
        }

        private async void LoadPersonalIdTypes()
        {
            var personalIdTypes = await _userService.GetPersonalIdTypesAsync();
            PersonalIdTypes.Clear();
            foreach (var type in personalIdTypes.Data)
                PersonalIdTypes.Add(type);
        }

        private async void LoadRoles()
        {
            var roles = await _userService.GetRolesAsync();
            Roles.Clear();
            foreach (var role in roles.Data)
                Roles.Add(role);
        }
//        private bool CanSave() => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(RfidTag);
        private bool CanSave() => true;

        private async Task SaveUserAsync()
        {
            var newUser = new UserAddModel
            {
                Username = Username,
                Firstname = Firstname,
                Middlename = string.IsNullOrWhiteSpace(Middlename) ? null : Middlename,
                Lastname = string.IsNullOrWhiteSpace(Lastname) ? null : Lastname,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Address = Address,
                EmployeeNumber = string.IsNullOrWhiteSpace(EmployeeNumber) ? null : EmployeeNumber,
                CompanyId = SelectedCompanyId > 0 ? SelectedCompanyId : null,
                PersonalTypeId = SelectedPersonalIdType > 0 ? SelectedPersonalIdType : null,
                PersonalIdNumber = string.IsNullOrWhiteSpace(PersonalIdNumber) ? null : PersonalIdNumber,
                GuestCompanyName = string.IsNullOrWhiteSpace(GuestCompanyName) ? null : GuestCompanyName,
                RfidTag = RfidTag,
                RolesIds = SelectedRole > 0 ? [SelectedRole] : []
            };

            var result = await _userService.SaveUserAsync(newUser);
            if (result.Success)
            {
                MessageBox.Show("User added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Windows.OfType<UserAddWindow>().FirstOrDefault()?.Close();
            }
            else
            {
                MessageBox.Show("Failed to add user." + Environment.NewLine + $"{result.MessageDetail}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            Application.Current.Windows.OfType<UserAddWindow>().FirstOrDefault()?.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
