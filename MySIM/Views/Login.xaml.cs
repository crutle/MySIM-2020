using MySIM.Menu;
using MySIM.ViewModels;
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
        public partial class Login : ContentPage
        {
            public Login()
            {
                InitializeComponent();
            }
        }
    }
 */

namespace MySIM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        private readonly DatabaseController db = new DatabaseController();
        private readonly UserSettingsController userData = new UserSettingsController();
        private int userRecordID = 0, isAdmin = 0, rowsAffected = 0;

        public Login()
        {
            InitializeComponent();
            //Initialise & Load Page.
            Initialise();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Initialise();
        }

        protected void ShowHidePwdBtn_OnClick(object sender, EventArgs args)
        {
            Button showHideBtn = (Button)sender;

            if (showHideBtn.Text == "show")
            {
                loginPwd.IsPassword = false;
                showHideBtn.Text = "hide";
            }
            else if (showHideBtn.Text == "hide")
            {
                loginPwd.IsPassword = true;
                showHideBtn.Text = "show";
            }
        }

        protected void LoginBtn_OnClick(object sender, EventArgs args)
        {
            try
            {
                //Check if user account exists.
                userRecordID = db.GetUserRecordID(loginID.Text, loginPwd.Text);

                //User does not exists.
                if (userRecordID == 0)
                {
                    errorMsg.IsVisible = true;
                }
                //User exists.
                else
                {
                    //Check if user account is admin account.
                    isAdmin = db.IsAdmin(userRecordID);
                    //Log in user account.
                    LoginUser(userRecordID, stayLoggedInBtn.IsChecked, isAdmin);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to log in user (1): " + ex.Message + " (Contact Administrator)", "OK");
            }
        }



        private void Initialise()
        {
            //Moves cursor down as Enter is hit
            loginID.Completed += (s, e) => loginPwd.Focus();
            loginPwd.Completed += (s, e) => LoginBtn_OnClick(s, e);

            loginID.Text = "";
            loginPwd.Text = "";

            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        //Log in User: update last logged in & redirect page.
        private void LoginUser(int recordID, bool stayLoggedIn, int isAdmin)
        {
            try
            {
                //Update last logged in of user.
                rowsAffected = db.UpdateLastLoggedIn(recordID);

                if (rowsAffected == 1)
                {
                    //Store user data in UserSettingsController.
                    userData.UserRecordID = recordID;
                    userData.StayLoggedIn = stayLoggedIn;
                    userData.ActiveSession = true;

                    if (isAdmin == 1)
                    {
                        userData.IsAdmin = true;
                    }
                    else
                    {
                        userData.IsAdmin = false;
                    }


                    //Make Home (student/admin) the new root page.
                    App.RootPage = new RootPage
                    {
                        Flyout = new MenuPage(),
                        Detail = new NavigationPage(new Home())
                    };

                    Application.Current.MainPage = App.RootPage;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to log in user: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }
    }
}