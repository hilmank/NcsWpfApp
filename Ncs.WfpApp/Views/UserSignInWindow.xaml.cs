using Microsoft.Extensions.DependencyInjection;
using Ncs.WfpApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ncs.WfpApp.Views
{
    /// <summary>
    /// Interaction logic for UserSignInWindow.xaml
    /// </summary>
    public partial class UserSignInWindow : Window
    {
        public UserSignInWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetService<UserSignInViewModel>();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserSignInViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
