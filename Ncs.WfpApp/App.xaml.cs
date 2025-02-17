using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Ncs.WfpApp.Helpers;
using Ncs.WfpApp.Services;
using Ncs.WfpApp.Services.Interfaces;
using Ncs.WfpApp.ViewModels;
using Ncs.WfpApp.Views;
using System.Windows;

namespace Ncs.WfpApp
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

            // Register AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>(); // AutoMapper Configuration
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Register Services
            services.AddSingleton<IUserService, UserService>();

            // Register ViewModels
            services.AddSingleton<UserSignInViewModel>();

            // Register Views
            services.AddSingleton<UserSignInWindow>();

            // Build Service Provider
            ServiceProvider = services.BuildServiceProvider();

            // Start Main Window
            var mainWindow = ServiceProvider.GetService<UserSignInWindow>();
            mainWindow?.Show();
        }
    }

}
