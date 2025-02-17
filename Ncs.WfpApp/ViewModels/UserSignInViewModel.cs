using Ncs.WfpApp.Models;
using Ncs.WfpApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Ncs.WfpApp.Helpers;

namespace Ncs.WfpApp.ViewModels
{
    public class UserSignInViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public UserSignInViewModel(IUserService userService)
        {
            _userService = userService;
            SignInCommand = new RelayCommand(async () => await SignInAsync(), CanSignIn);
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                ((RelayCommand)SignInCommand).RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                ((RelayCommand)SignInCommand).RaiseCanExecuteChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand SignInCommand { get; }

        private async Task SignInAsync()
        {
            var user = new UserSignInModel { Username = Username, Password = Password };
            bool isAuthenticated = await _userService.SignInAsync(user);

            if (isAuthenticated)
            {
                MessageBox.Show("Sign-in successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ErrorMessage = "Invalid username or password!";
            }
        }

        private bool CanSignIn() => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
