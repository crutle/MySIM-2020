using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySIM.ViewModels;
using MySIM.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/* Visual Studio Generated Skeleton Code: 
namespace MySIM.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();

        private static string userType;
        private static int isStudent = 0;
        private static int isAdmin = 0;

        public MenuPage()
        {
            InitializeComponent();

            //Initialise & Load Page.
            Initialise();

            //Reference: https://stackoverflow.com/questions/49169049/hamburger-menu-xamarin-forms-masterdetailpage
            BindingContext = new MenuPageViewModel();
            // this.Icon = "yourHamburgerIcon.png"; //only neeeded for ios
        }

        private void Initialise()
        {
            //Check if user account type is admin.
            CheckIfAccountType();

            //Initialise page controls.
            SetPageDefaultSettings();
        }

        //Check if UserSettingsController's stored user data is admin data.
        private void CheckIfAccountType()
        {
            try
            {
                if (userData.ActiveSession == false)
                {
                    Application.Current.Properties.Clear();
                    Navigation.PopToRootAsync(true);
                    Application.Current.MainPage = new Login();
                }
                else
                {
                    isStudent = db.IsStudent(userData.UserRecordID);
                    isAdmin = db.IsAdmin(userData.UserRecordID);

                    if (isAdmin == 1 && isStudent == 0)
                    {
                        userType = "admin";
                    }
                    else if (isAdmin == 0 && isStudent == 1)
                    {
                        userType = "student";
                    }
                    else
                    {
                        userType = "none";
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to check user settings: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        //Initialise page controls.
        private void SetPageDefaultSettings()
        {
            if (userType == null)
            {
                errorLbl.IsVisible = true;

                adminView.IsVisible = false;
                studentView.IsVisible = false;
            }
            else
            {
                if (userType.Equals("admin"))
                {
                    adminView.IsVisible = true;
                    studentView.IsVisible = false;

                }
                else if (userType.Equals("student"))
                {
                    adminView.IsVisible = false;
                    studentView.IsVisible = true;
                }
            }
        }
    }
}