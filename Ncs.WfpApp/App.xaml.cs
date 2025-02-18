using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Ncs.WpfApp.Helpers;
using Ncs.WpfApp.Services;
using Ncs.WpfApp.Services.Interfaces;
using Ncs.WpfApp.ViewModels;
using Ncs.WpfApp.Views;
using System.Net.Http;
using System.Windows;

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
            services.AddSingleton<AdminWindowViewModel>();
            services.AddTransient<UserAddViewModel>();
            services.AddTransient<OrdersConfirmationViewModel>();

            // Register Views
            services.AddSingleton<UserSignInWindow>();
            services.AddSingleton<AdminWindow>();
            services.AddSingleton<OrdersConfirmationWindow>();
            services.AddSingleton<CustomerWindow>();
            services.AddSingleton<UserAddWindow>();


            // Build Service Provider
            ServiceProvider = services.BuildServiceProvider();

            // Start Main Window
            var mainWindow = ServiceProvider.GetService<UserSignInWindow>();
            mainWindow?.Show();
        }
    }

}
