using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Charter3HourLogin.Common;
using Charter3HourLogin.Common.Models;
using Charter3HourLogin.Common.Services;
using Prism.Navigation;
using Prism.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Charter3HourLogin.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        private IDataRepoService _dataRepoService { get; }
        private IPageDialogService _pageDialogService { get; }
        public ProfilePageViewModel(INavigationService navigationService, IDataRepoService dataRepoService, IPageDialogService pageDialogService) : base(navigationService)
        {
            _dataRepoService = dataRepoService;
            _pageDialogService = pageDialogService;    
            
            ValidatePasswordCommand = ReactiveCommand.Create(ValidatePasswordCommandExecute, null);
            ValidatePasswordCommand.ThrownExceptions.Subscribe(error =>
            {
                Debug.WriteLine(error);
                Debugger.Break();
            });
            
            ValidateDateCommand = ReactiveCommand.Create(ValidateDateCommandExecute, null);
            ValidateDateCommand.ThrownExceptions.Subscribe(error =>
            {
                Debug.WriteLine(error);
                Debugger.Break();
            });
            
            var canExecute = this.WhenAnyValue(
                x => x.User.FirstName, 
                x => x.User.LastName,
                x => x.User.UserName,
                x => x.User.Password,
                x => x.User.Phone,
                x => x.User.StartDate,
                x => x.ErrorMessage,
                (firstName, 
                        lastName, 
                        username, 
                        password, 
                        phone, 
                        startDate, 
                        errorMessage) =>
                    !string.IsNullOrEmpty(firstName) &&
                    !string.IsNullOrEmpty(lastName) &&
                    !string.IsNullOrEmpty(password) &&
                    !string.IsNullOrEmpty(phone) &&
                    startDate!=DateTime.Today &&
                    string.IsNullOrEmpty(errorMessage)

            );
            
            CreateAccountCommand = ReactiveCommand.CreateFromTask(CreateAccountCommandExecute, canExecute);
            CreateAccountCommand.ThrownExceptions.Subscribe(error =>
            {
                Debug.WriteLine(error);
                Debugger.Break();
            });
        }

        private async Task CreateAccountCommandExecute()
        {
            //save
            var ok = await _dataRepoService.SaveValue<UserModel>(User, _dataRepoService.UserSaveFile);
            if (!ok)
            {
                await _pageDialogService.DisplayAlertAsync("", "Uh Oh. We got problems", "Ok");
                return;
            }
            
            //navigate to next screen
            await NavigationService.NavigateAsync("/NavigationPage/ProfileConfirmationPage");
        }

        private void ValidateDateCommandExecute()
        {
            if (User.StartDate > DateTime.Now.Date.AddDays(30))
            {
                ErrorMessage = "Start Date can be up to 30 days in the future.\n" +
                               "Directions said throw error, but thinking an error message display is preferred.";
            }
            else
                ErrorMessage = string.Empty;
        }

        private void ValidatePasswordCommandExecute()
        {
            ValidatePassword(User.Password);
        }
        
        private bool ValidatePassword(string password)
        {
            var input = password;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,15}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one lower case letter.";
                return false;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one upper case letter.";
                return false;
            }
            else if (!hasMiniMaxChars.IsMatch(input))
            {
                ErrorMessage = "Password should not be lesser than 8 or greater than 15 characters.";
                return false;
            }
            else if (!hasNumber.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one numeric value.";
                return false;
            }

            else if (!hasSymbols.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one special case character.";
                return false;
            }
            else if(HasConsecutiveChars(input, 2))
            {
                ErrorMessage = "Password should not contain consecutive characters.";
                return false;
            }
            else if(HasRepeatedPattern(input))
            {
                ErrorMessage = "Password should not contain a repeated pattern of characters.";
                return false;
            }
            else
            {
                return true;
            }
        }
        
        private bool HasConsecutiveChars(string source, int sequenceLength)
        {
            if(string.IsNullOrEmpty(source) || source.Length == 1)
                return false;

            char lastSeen = source.First();
            var count = 1;

            foreach(var c in source.Skip(1))
            {
                if (lastSeen == c)
                    count++;
                else
                    count = 1;

                if (count == sequenceLength)
                    return true;

                lastSeen = c;
            }

            return false;
        }

        private bool HasRepeatedPattern(string source)
        {
            var positions = new List<int>();
            for (var index = 0; index < source.Length-1; index++)
            {
                string stringToFind = source.Substring(index, 2);
                positions = new List<int>();
                int pos = 0;
                while ((pos < source.Length) && (pos = source.IndexOf(stringToFind, pos)) != -1)
                {
                    positions.Add(pos);
                    pos += stringToFind.Count();
                }
            }

            return positions.Count > 1;
        }
        

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            
            User = new UserModel();
        }

        [Reactive]
        public string ErrorMessage { get; set; }
        [Reactive]
        public UserModel User { get; set; }
        public ReactiveCommand<Unit, Unit> ValidatePasswordCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ValidateDateCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CreateAccountCommand { get; set; }
    }
}
