using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Charter3HourLogin.Common;
using Charter3HourLogin.Common.Models;
using Charter3HourLogin.Common.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Charter3HourLogin.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private IDataRepoService _dataRepoService { get; }
        
        public LoginPageViewModel(INavigationService navigationService, IDataRepoService dataRepoService) : base(navigationService)
        {
            Title = "Login";

            _dataRepoService = dataRepoService;

            //Reset the error message when changing a form value
            this.WhenAnyValue(
                    x => x.UserName, x => x.Password,
                    (userName, password) =>
                        !string.IsNullOrEmpty(userName) &&
                        !string.IsNullOrEmpty(password))
                .Subscribe(x => ErrorMessage = string.Empty);
                            
            var canExecuteLogin = this.WhenAnyValue(
                x => x.UserName, x => x.Password,
                (userName, password) =>
                    !string.IsNullOrEmpty(userName) &&
                    !string.IsNullOrEmpty(password));

            var canExecute = this.WhenAnyValue(x => x.IsNotBusy);

            LoginUserCommand = ReactiveCommand.CreateFromTask(LoginUserCommandExecute, canExecuteLogin);
            LoginUserCommand.ThrownExceptions.Subscribe(error =>
            {
                Debug.WriteLine(error);
                Debugger.Break();
            });
            
            CreateAccountCommand = ReactiveCommand.CreateFromTask(CreateAccountCommandExecute, canExecute);
            CreateAccountCommand.ThrownExceptions.Subscribe(error =>
            {
                Debug.WriteLine(error);
                Debugger.Break();
            });
            
            AddIsBusyObservable(this.WhenAnyObservable(x => x.LoginUserCommand.IsExecuting));
            AddIsBusyObservable(this.WhenAnyObservable(x => x.CreateAccountCommand.IsExecuting));
        }

        private async Task CreateAccountCommandExecute()
        {
            await Task.Delay(500);
            var result = await NavigationService.NavigateAsync("ProfilePage");
            if (!result.Success)
            {
                Debugger.Break();
            }
        }

        private async Task LoginUserCommandExecute()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            
            //I would extract this to a UserService and pass in the login/password as parameters, but you see I can use DI (as evidenced by the dataRepoService)
            User = await _dataRepoService.GetSavedValue<UserModel>(_dataRepoService.UserSaveFile);
            if (User == null || User?.UserName.Trim().ToLowerInvariant() != UserName.Trim().ToLowerInvariant())
            {
                ErrorMessage = "Account does not exist";
            }
            else if (User != null && User.Password.Trim().ToLowerInvariant() != Password.Trim().ToLowerInvariant())
            {
                ErrorMessage = "Password is incorrect";
            }
            else if (User?.UserName.Trim().ToLowerInvariant() == UserName.Trim().ToLowerInvariant() &&
                     User.Password.Trim().ToLowerInvariant() == Password.Trim().ToLowerInvariant())
            {
                ErrorMessage = string.Empty;
                SuccessMessage = Constants.SuccessMessage;
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            UserName = string.Empty;
            Password = string.Empty;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        [Reactive]
        public UserModel User { get; set; }
        [Reactive]
        public string UserName { get; set; }
        [Reactive]
        public string Password { get; set; }
        [Reactive]
        public string ErrorMessage { get; set; } = string.Empty;
        [Reactive] 
        public string SuccessMessage { get; set; } 
        public ReactiveCommand<Unit, Unit> LoginUserCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CreateAccountCommand { get; set; }
    }
  
}
