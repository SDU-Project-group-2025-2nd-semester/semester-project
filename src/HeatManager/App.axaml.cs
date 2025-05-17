using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using HeatManager.Core.Extensions;
using HeatManager.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using MainWindow = HeatManager.Views.MainWindow;

namespace HeatManager
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // If you use CommunityToolkit, line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);

                // Register all the services needed for the application to run
                var collection = new ServiceCollection();
                collection.AddCommonServices();

                var window = desktop.MainWindow = new MainWindow();

                collection.AddSingleton<MainWindowViewModel>(serviceProvider => ActivatorUtilities.CreateInstance<MainWindowViewModel>(serviceProvider, window));

                // Creates a ServiceProvider containing services from the provided IServiceCollection
                var services = collection.BuildServiceProvider();


                var vm = services.GetRequiredService<MainWindowViewModel>();

                window.DataContext = vm;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}