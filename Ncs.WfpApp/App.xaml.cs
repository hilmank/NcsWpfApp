using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Ncs.WpfApp.Helpers;
using Ncs.WpfApp.Services;
using Ncs.WpfApp.Services.Interfaces;
using Ncs.WpfApp.ViewModels;
using Ncs.WpfApp.Views;
using System.Net.Http;
using System.Windows;
using WpfScreenHelper;
namespace Ncs.WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            // Register HttpClient
            string apiBaseUrl = ConfigurationHelper.GetApiBaseUrl();

            // Register HttpClient globally
            services.AddSingleton<HttpClient>(provider =>
            {
                var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
                return httpClient;
            });

            // Register AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>(); // AutoMapper Configuration
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Register Services
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IOrderService, OrderService>();
            services.AddSingleton<IReservationService, ReservationService>();

            // Register ViewModels
            services.AddSingleton<UserSignInViewModel>();
            services.AddSingleton<AdminViewModel>();
            services.AddTransient<UserAddViewModel>();
            services.AddTransient<OrdersConfirmationViewModel>();
            services.AddTransient<CustomerViewModel>();
            services.AddTransient<CustomerMenuConfirmationViewModel>();

            // Register Views
            services.AddSingleton<UserSignInWindow>();
            services.AddSingleton<AdminWindow>();
            services.AddSingleton<OrdersConfirmationWindow>();
            services.AddSingleton<UserAddWindow>();
            services.AddSingleton<CustomerWindow>();
            services.AddSingleton<CustomerMenuConfirmationWindow>();


            // Build Service Provider
            ServiceProvider = services.BuildServiceProvider();

            // Start Main Window
            var mainWindow = ServiceProvider.GetService<UserSignInWindow>();
            mainWindow?.Show();

            /*
            base.OnStartup(e);

            // Get all screens
            var screens = Screen.AllScreens.ToList();
            AdminWindow adminWindow = new AdminWindow();
            if (screens.Count > 0)
            {
                var screen1 = screens[0];
                adminWindow.Left = screen1.Bounds.Left;
                adminWindow.Top = screen1.Bounds.Top;
                adminWindow.Width = screen1.Bounds.Width;
                adminWindow.Height = screen1.Bounds.Height;
            }
            adminWindow.Show();

            // Create and show Customer Window on the second screen (if available)
            CustomerWindow customerWindow = new CustomerWindow();
            if (screens.Count > 1)
            {
                var screen2 = screens[1];
                customerWindow.Left = screen2.Bounds.Left;
                customerWindow.Top = screen2.Bounds.Top;
                customerWindow.Width = screen2.Bounds.Width;
                customerWindow.Height = screen2.Bounds.Height;
            }
            else
            {
                MessageBox.Show("Only one screen detected. Running both windows on the same screen.");
            }
            customerWindow.Show();
        */
        }
    }
}
