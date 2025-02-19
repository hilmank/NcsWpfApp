using Microsoft.Extensions.DependencyInjection;
using Ncs.WpfApp.ViewModels;
using System.Windows;

namespace Ncs.WpfApp.Views
{
    /// <summary>
    /// Interaction logic for CustomerMenuConfirmationWindow.xaml
    /// </summary>
    public partial class CustomerMenuConfirmationWindow : Window
    {
        public CustomerMenuConfirmationWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetService<CustomerMenuConfirmationViewModel>();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Optional: Can be used to track popup closure
            this.Close(); // Closes the popup
        }
    }
}
