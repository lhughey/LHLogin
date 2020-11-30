using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Charter3HourLogin.Common.Views
{
    public partial class ActivityPopupPage
    {
        public ActivityPopupPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            while (IsAnimationEnabled)
            {
                await Task.WhenAll
                (cogBig.RelRotateTo(360, 6000, Easing.Linear),
                    cogLil.RelRotateTo(-360, 6000, easing: Easing.Linear)
                );
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            ViewExtensions.CancelAnimations(cogBig);
            ViewExtensions.CancelAnimations(cogLil);
            IsAnimationEnabled = false;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}