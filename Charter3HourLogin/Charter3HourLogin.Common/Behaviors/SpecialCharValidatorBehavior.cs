using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Charter3HourLogin.Common.Behaviors
{
    public class SpecialCharValidatorBehavior : Behavior<Entry>
    {
        private const string emailRegex = "^[A-Za-z0-9 ]+$";

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(SpecialCharValidatorBehavior), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += HandleTextChanged;
        }

        void HandleTextChanged(object sender, TextChangedEventArgs args)
        {
            
            if(!string.IsNullOrWhiteSpace(args.NewTextValue)) 
            {
                IsValid = (Regex.IsMatch(args.NewTextValue, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
                ((Entry)sender).Text = IsValid ? args.NewTextValue : args.NewTextValue.Remove(args.NewTextValue.Length - 1);
            }
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;

        }
    }
}
