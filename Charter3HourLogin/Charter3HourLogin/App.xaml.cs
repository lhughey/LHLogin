using Charter3HourLogin.Common.Services;
using Charter3HourLogin.Common.ViewModels;
using Charter3HourLogin.Common.Views;
using Charter3HourLogin.ViewModels;
using Charter3HourLogin.Views;
using Prism;
using Prism.Ioc;
using Prism.Services;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace Charter3HourLogin
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/LoginPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.RegisterSingleton<IDataRepoService, DataRepoService>();
            containerRegistry.RegisterInstance<IFileSystem>(new FileSystemImplementation());

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<ProfilePage, ProfilePageViewModel>();
            containerRegistry.RegisterForNavigation<ActivityPopupPage, ActivityPopupPageViewModel>();
            containerRegistry.RegisterForNavigation<ProfileConfirmationPage, ProfileConfirmationPageViewModel>();
        }
    }
}
