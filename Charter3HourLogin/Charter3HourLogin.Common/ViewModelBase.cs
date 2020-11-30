using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Charter3HourLogin.Common.Views;
using Prism.AppModel;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Rg.Plugins.Popup.Services;
using Splat;

namespace Charter3HourLogin.Common
{
    public class ViewModelBase : ReactiveObject, INavigationAware, IDestructible, IAbracadabra
    {
        protected INavigationService NavigationService { get; private set; }
        protected ILogger Logger { get; private set; }
        [Reactive]
        public string Title { get; set; }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;

            _isBusy = ObservableAsPropertyHelper<bool>.Default(false);
            _isNotBusy = this.WhenAnyValue(x => x.IsBusy)
                             .Select(x => !IsBusy)
                             .ToProperty(this, x => x.IsNotBusy, true);

            //Set indicator visibility based on IsBusy
            this.WhenAnyValue(x => x.IsBusy).Subscribe(async _ => { await SetIndictorState(true); });
            this.WhenAnyValue(x => x.IsNotBusy).Subscribe(async _ => { await SetIndictorState(false); });

            var canExecute = this.WhenAnyValue(x => x.IsNotBusy);
            AddIsBusyObservable(this.WhenAnyObservable(x => x.ExternalBrowseCommand.IsExecuting));
        }

        private bool indicatorRunning { get; set; }
        private IObservable<bool> _isBusyObservable;
        private ObservableAsPropertyHelper<bool> _isBusy;
        public bool IsBusy => _isBusy.Value;
        protected ObservableAsPropertyHelper<bool> _isNotBusy { get; }
        public bool IsNotBusy => _isNotBusy.Value;

        protected void AddIsBusyObservable(IObservable<bool> observable, bool reset = false)
        {
            if (reset)
            {
                _isBusyObservable = null;
            }

            if (_isBusyObservable == null)
            {
                _isBusyObservable = observable;
            }
            else
            {
                _isBusyObservable = Observable.Merge(_isBusyObservable, observable);
            }

            if (_isBusy != null)
            {
                _isBusy.Dispose();
            }

            _isBusyObservable.ToProperty(this, x => x.IsBusy, out _isBusy);


        }

        private async Task SetIndictorState(bool isBusyRequested)
        {
            if (isBusyRequested)
            {
                if (IsBusy)
                {
                    await PopupNavigation.Instance.PushAsync(new ActivityPopupPage(), true);
                }
            }
            else
            {
                if (IsNotBusy)
                {
                    if (PopupNavigation.Instance.PopupStack.Any())
                        await PopupNavigation.Instance.PopAsync(true);
                }
            }
        }

        public ReactiveCommand<string, Unit> ExternalBrowseCommand { get; set; }
        

        public virtual void Initialize(INavigationParameters parameters)
        {

        }


        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("Title"))
                Title = parameters["Title"].ToString();
            
        }

        public virtual void Destroy()
        {

        }

        public virtual Task InitializeAsync(INavigationParameters parameters)
        {
            return Task.CompletedTask;
        }

    }
}
