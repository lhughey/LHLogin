using Xamarin.Forms;

namespace Charter3HourLogin.Views
{
    public partial class LoginPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        
        //Disable the back button on Droid for the login
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
