using MySIM.Menu;
using MySIM.ViewModels;
using MySIM.Views.Students;
using MySIM.Views.Students_Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
/* Visual Studio Generated Skeleton Code: 
namespace MySIM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        public Home()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();

        public Home()
        {
            InitializeComponent();
            //Initialise & Load Page
            Initialise();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Initialise();
        }

        protected void ToChatbotBtn_Clicked(object sender, EventArgs args)
        {
            Navigation.PushAsync(new ChatPage());
        }

        protected void LogoutBtn_Clicked(object sender, EventArgs args)
        {
            try
            {
                UserSettings.ClearAllData();
                //Remove all pages from stack then set MainPage as Login.
                Navigation.PopToRootAsync();
                Application.Current.MainPage = new Login();

            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to logout: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }



        private void Initialise()
        {
            NavigationPage.SetHasBackButton(this, false);

            //Check user account type.
            CheckUserLoginStatus();
            
        }

        //Check user account type and setting view accordingly.
        private void CheckUserLoginStatus()
        {
            try
            {
                if (userData.UserRecordID == 0 || userData.ActiveSession == false)
                {
                    Application.Current.Properties.Clear();
                    //Remove all pages from stack then set MainPage as Login.
                    Navigation.PopToRootAsync();                  
                    Application.Current.MainPage = new Login();

                }
                else
                {
                    //Assign views to admin and student accounts.
                    SetUserView(userData.IsAdmin);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to check user settings: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        private void SetUserView(bool isAdmin)
        {
            /*  Setting dynamic views according to type of account logged in
             *  Reference: https://www.xamboy.com/2019/01/18/using-control-templates-in-xamarin-forms/
             */
            ControlTemplate = null;
            ControlTemplate adminHomeView = new ControlTemplate(typeof(AdminHomeView));
            ControlTemplate studentHomeView = new ControlTemplate(typeof(StudentHomeView));

            //Admin View 
            if (isAdmin)
            {
                ControlTemplate = adminHomeView;
            }
            //Student View
            else
            {
                ControlTemplate = studentHomeView;
            }
        }
    }
}