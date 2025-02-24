using Ncs.WpfApp.Helpers;
using Ncs.WpfApp.Models;
using Ncs.WpfApp.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;

public class CustomerMenuConfirmationViewModel : INotifyPropertyChanged
{
    private readonly IOrderService _orderService;
    private readonly IReservationService _reservationService;
    private int _menuItemsId;

    private double _windowWidth;
    public double WindowWidth
    {
        get => _windowWidth;
        set { _windowWidth = value; OnPropertyChanged(); }
    }

    private double _windowHeight;
    public double WindowHeight
    {
        get => _windowHeight;
        set { _windowHeight = value; OnPropertyChanged(); }
    }
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

    // New properties to control UI element visibility
    private Visibility _reservationVisibility = Visibility.Collapsed;
    public Visibility ReservationVisibility
    {
        get => _reservationVisibility;
        set { 
            _reservationVisibility = value; 
            OnPropertyChanged();
            ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
        }
    }

    private Visibility _comboBoxVisibility = Visibility.Visible;
    public Visibility ComboBoxVisibility
    {
        get => _comboBoxVisibility;
        set { 
            _comboBoxVisibility = value; 
            OnPropertyChanged();
            ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
        }
    }

    // Collection for reservation data (if needed for binding to the DataGrid)
    public ObservableCollection<ReservationListModel> Reservations { get; set; } = new ObservableCollection<ReservationListModel>();

    public CustomerMenuConfirmationViewModel(IOrderService orderService, IReservationService reservationService)
    {
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));

        SubmitCommand = new RelayCommand(async () => await SubmitAsync(), CanSubmit);
    }
    private bool CanSubmit()
    {
        // Enable button if ComboBox is visible and a variant is selected OR DataGrid is visible
        return (!string.IsNullOrEmpty(SelectedVariant) && ComboBoxVisibility == Visibility.Visible)
                || (ReservationVisibility == Visibility.Visible);
    }
    // Change Initialize to async so you can await the reservation service call.
    public async Task InitializeAsync(int menuItemsId, int userId)
    {
        _menuItemsId = menuItemsId;
        LoadVariantOptions();

        var reservationResponse = await _reservationService.GetReservationsByUserIdAsync(userId);
        if (reservationResponse.Success)
        {

            // Show the DataGrid and hide the ComboBox
            ReservationVisibility = Visibility.Visible;
            ComboBoxVisibility = Visibility.Collapsed;

            // Optionally, populate the DataGrid's items
            Reservations = new ObservableCollection<ReservationListModel>(reservationResponse.Data);
            OnPropertyChanged(nameof(Reservations));
            // Set window size for a successful response
            WindowWidth = 600;
            WindowHeight = 250;
        }
        else
        {
            // Hide the DataGrid and show the ComboBox
            ReservationVisibility = Visibility.Collapsed;
            ComboBoxVisibility = Visibility.Visible;
            WindowWidth = 340;
            WindowHeight = 200;
        }
    }

    private void LoadVariantOptions()
    {
        VariantOptions.Add("Regular");
        VariantOptions.Add("Spicy");
    }

    private async Task SubmitAsync()
    {
        OrderAddModel orderItem = null;

        if (ComboBoxVisibility == Visibility.Visible)
        {
            if (string.IsNullOrWhiteSpace(SelectedVariant))
            {
                MessageBox.Show("Please select a variant.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isSpicy = SelectedVariant.Equals("Spicy", StringComparison.OrdinalIgnoreCase);
            orderItem = new OrderAddModel
            {
                IsSpicy = isSpicy,
                MenuItemsId = _menuItemsId
            };
        }
        else
        {
            orderItem = GetOrderItemFromTable();
            if (orderItem == null)
            {
                MessageBox.Show("Failed to retrieve order details. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        try
        {
            var response = await _orderService.SaveOrderCustomerActionAsync(orderItem);
            if (response.Success)
            {
                MessageBox.Show("Order confirmed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
            }
            else
            {
                MessageBox.Show("Failed to confirm order." + Environment.NewLine + response.MessageDetail, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred while submitting the order: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private OrderAddModel? GetOrderItemFromTable()
    {
        // Validate if Reservations is null or empty
        if (Reservations == null || !Reservations.Any())
        {
            MessageBox.Show("No reservation data found.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return null;
        }

        var userReserved = Reservations.FirstOrDefault(x => x.ReservationsUserGuestId == null);
        if (userReserved == null)
        {
            MessageBox.Show("No valid reservation found for the user.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return null;
        }

        OrderAddModel order = new()
        {
            MenuItemsId = userReserved.MenuItemsId,
            IsSpicy = userReserved.MenuVariant?.Equals("Spicy", StringComparison.OrdinalIgnoreCase) ?? false,
            ReservationGuestsIds = []
        };

        foreach (var item in Reservations.Where(x => x.ReservationsUserGuestId != null))
        {
            if (item.ReservationsUserGuestId.HasValue)
            {
                order.ReservationGuestsIds.Add(item.ReservationsUserGuestId.Value);
            }
        }

        return order;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
