using Ncs.WpfApp.Helpers;
using Ncs.WpfApp.Models;
using Ncs.WpfApp.Services.Interfaces;
using Ncs.WpfApp.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Ncs.WpfApp.ViewModels
{
    public class UserSignInViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand SignInCommand { get; }
        public ICommand ExitCommand { get; }
        public UserSignInViewModel(IUserService userService)
        {
            _userService = userService;
            SignInCommand = new RelayCommand(async () => await SignInAsync(), CanSignIn);
            ExitCommand = new RelayCommand(ExitApplication, ()=>true);
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
        private bool _isSignedIn;
        public bool IsSignedIn
        {
            get => _isSignedIn;
            set
            {
                _isSignedIn = value;
                OnPropertyChanged();
                ((RelayCommand)SignInCommand).RaiseCanExecuteChanged();
            }
        }


        private async Task SignInAsync()
        {
            var user = new UserSignInModel { UsernameOrEmail = Username, Password = Password };
            var result = await _userService.SignInAsync(user);
            bool isAuthenticated = result.Success;
            ErrorMessage = string.Empty;
            if (isAuthenticated && result.Data != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var adminWindow = new AdminWindow();
                    adminWindow.Show();
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is UserSignInWindow userSignInWindow)
                        {
                            if (userSignInWindow.DataContext is UserSignInViewModel viewModel)
                            {
                                if (viewModel.IsSignedIn)
                                {
                                    CloseAllWindowsExcept(userSignInWindow);
                                    viewModel.IsSignedIn = false; // ✅ Reset sign-in status
                                    viewModel.ClearInputs();  // ✅ Clear all input fields
                                    SessionManager.ClearAdminSession();
                                }
                                else
                                {
                                    viewModel.IsSignedIn = true;  // ✅ User is now signed in
                                }
                            }
                            break;
                        }
                    }
                });
            }
            else
            {
                ErrorMessage = result.MessageDetail ?? "An unknown error occurred.";
            }
        }

        private bool CanSignIn() => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) || IsSignedIn;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void CloseAllWindowsExcept(Window keepWindow)
        {
            foreach (Window window in Application.Current.Windows.Cast<Window>().ToList())
            {
                if (window != keepWindow)
                {
                    window.Close();
                }
            }
        }
        public void ClearInputs()
        {
            Username = string.Empty;
            Password = string.Empty;
            ErrorMessage = string.Empty;
        }
        private void ExitApplication()
        {
            Application.Current.Shutdown(); 
        }
    }
}
