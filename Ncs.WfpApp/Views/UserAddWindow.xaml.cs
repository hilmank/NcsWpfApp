using Microsoft.Extensions.DependencyInjection;
using Ncs.WpfApp.ViewModels;
using System.Windows;

namespace Ncs.WpfApp.Views
{
    /// <summary>
    /// Interaction logic for UserAddWindow.xaml
    /// </summary>
    public partial class UserAddWindow : Window
    {
        public UserAddWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetService<UserAddViewModel>();
        }
    }
}
