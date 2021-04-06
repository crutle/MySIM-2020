using MySIM.ViewModels;
using MySIM.Views.Courses_Admin;
using MySIM.Views.Modules_Admin;
using MySIM.Views.Students_Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/* Visual Studio Generated Skeleton Code: 
    namespace MySIM
    {
        [XamlCompilation(XamlCompilationOptions.Compile)]
        public partial class AdminHomeView : ContentView
        {
            public AdminHomeView()
            {
                InitializeComponent();
            }
        }
    }
 */

namespace MySIM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminHomeView : ContentView
    {
        private readonly UserSettingsController userData = new UserSettingsController();

        public AdminHomeView()
        {
            InitializeComponent();

            //Check if user account type is admin.
            CheckIfAdminAccount();
        }

        protected void RedirectToManageCourses(object sender, EventArgs args)
        {          
            try
            {
                Navigation.PushAsync(new ManageAllCourses());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error", "Failed redirect to view courses: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void RedirectToManageModules(object sender, EventArgs args)
        {
            try
            {
                Navigation.PushAsync(new ManageAllModules());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error", "Failed redirect to view modules: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void RedirectToManageStudents(object sender, EventArgs args)
        {
            try
            {
                Navigation.PushAsync(new ManageAllStudents());
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error", "Failed redirect to view user information: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        //Check if UserSettingsController's stored user data is admin data.
        private void CheckIfAdminAccount()
        {
            try
            {
                if (userData.UserRecordID != 1 || userData.ActiveSession == false)
                {
                    Application.Current.Properties.Clear();
                    Navigation.PopToRootAsync(true);
                    Navigation.PushAsync(new NavigationPage(new Login()));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error", "Failed to check if account type is admin: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }
    }
}