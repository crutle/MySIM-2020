using MySIM.Menu;
using MySIM.ViewModels;
using MySIM.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/* Visual Studio Generated Skeleton Code: 
namespace MySIM
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
*/

namespace MySIM
{ 
    public partial class App : Application
    {
        private DateTime? SleepTime = null;
        private readonly UserSettingsController userData = new UserSettingsController();
        public static NavigationPage NavigationPage { get; private set; }
        public static RootPage RootPage;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
            CheckInternetConnection();
            LoadPage();
        }

        protected override void OnSleep()
        {
            SleepTime = DateTime.Now;
        }

        protected override void OnResume()
        {
            /*  REUSE OF CODE (BELOW)
             *  Title: Local Security Measures in Xamarin.Forms
             *  Author: Judson Bandy
             *  Date: 28 Feb 2020
             *  Code version: NA
             *  Type: Source Code
             *  Available at: https://hangzone.com/local-security-measures-xamarin-forms/
             */

            //Auto log-out function ("Stay Logged In" not selected): Detect up to 5 minutes of inactivity, then auto logs out user.
            if (userData.StayLoggedIn == false && SleepTime != null)
            {
                if (DateTime.Now.Subtract((DateTime)SleepTime) > TimeSpan.FromSeconds(300))
                {
                    LogOutUser();
                }
                else
                {
                    CheckInternetConnection();
                }
            }
        }

        //Log out user: Clear stored data, redirect to Login page.
        public void LogOutUser()
        {
            UserSettings.ClearAllData();

            if (App.NavigationPage != null)
            {
                App.NavigationPage.Navigation.PopToRootAsync();
                App.MenuIsPresented = false;
            }

            Application.Current.MainPage = new Login();
        }

        //Reference: https://stackoverflow.com/questions/49169049/hamburger-menu-xamarin-forms-masterdetailpage             
        public static bool MenuIsPresented
        {
            get {  return RootPage.IsPresented; }
            set { RootPage.IsPresented = value; }

        }

        private async void CheckInternetConnection()
        {
            var current = Connectivity.NetworkAccess;

            while (current != NetworkAccess.Internet)
            {
                await Task.Yield(); 
                // Just a simulation with 10 tries to get the data
                
                await Task.Delay(500);
                await MainPage.DisplayAlert(
                    "Connection Error",
                    "Unable to connect with the server. Check your internet connection and try again",
                    "Try again");

                await Task.Delay(2000);
                current = Connectivity.NetworkAccess;
            }               
        }      
        
        private void LoadPage()
        {
            MainPage = new RootPage
            {
                Flyout = new MenuPage(),
                Detail = new NavigationPage(new Home())
            };
        }
    }
}

