using Microsoft.Extensions.DependencyInjection;
using Ncs.WpfApp.ViewModels;
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

namespace Ncs.WpfApp.Views
{
    /// <summary>
    /// Interaction logic for OrdersConfirmationWindow.xaml
    /// </summary>
    public partial class OrdersConfirmationWindow : Window
    {
        public OrdersConfirmationWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetService<OrdersConfirmationViewModel>();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Optional: Can be used to track popup closure
            this.Close(); // Closes the popup
        }
    }
}
