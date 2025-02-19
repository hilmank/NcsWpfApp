using Ncs.WpfApp.Helpers;
using Ncs.WpfApp.Models;
using Ncs.WpfApp.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Ncs.WpfApp.ViewModels
{
    public class CustomerMenuConfirmationViewModel : INotifyPropertyChanged
    {
        private readonly IOrderService _orderService;
        private int _menuItemsId;

        public ObservableCollection<string> VariantOptions { get; } = new ObservableCollection<string>();

        private string _selectedVariant;
        public string SelectedVariant
        {
            get => _selectedVariant;
            set
            {
                _selectedVariant = value;
                OnPropertyChanged();
                ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged(); 
            }
        }


        public ICommand SubmitCommand { get; }

        public CustomerMenuConfirmationViewModel(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));

            SubmitCommand = new RelayCommand(async () => await SubmitAsync(), () => !string.IsNullOrEmpty(SelectedVariant));
        }
        public void Initialize(int menuItemsId)
        {
            _menuItemsId = menuItemsId;
            LoadVariantOptions();
        }
        private void LoadVariantOptions()
        {
            VariantOptions.Add("Regular");
            VariantOptions.Add("Spicy");
        }

        private async Task SubmitAsync()
        {
            if (string.IsNullOrEmpty(SelectedVariant))
            {
                MessageBox.Show("Please select a variant.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isSpicy = SelectedVariant == "Spicy";
            OrderAddModel orderItem = new OrderAddModel { IsSpicy = isSpicy, MenuItemsId =_menuItemsId };
             var response = await _orderService.SaveOrderCustomerActionAsync(orderItem);

            if (response.Success)
            {
                MessageBox.Show("Order confirm successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
            }
            else
            {
                MessageBox.Show("Failed to confirm order." + Environment.NewLine+ response.MessageDetail, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
