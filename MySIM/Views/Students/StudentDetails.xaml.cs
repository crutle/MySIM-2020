using MySIM.Models;
using MySIM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
/* Visual Studio Generated Skeleton Code: 
namespace MySIM.Views.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StudentDetails : ContentPage
    {
        public StudentDetails()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StudentDetails : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private Users user, userCourse = new Users();

        public StudentDetails()
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



        private void Initialise()
        {
            //Check if user account type is student.
            CheckIfStudentAccount();

            //Initialise page controls.
            SetPageDefaultSettings();

        }

        //Check if UserSettingsController's stored user data is admin data.
        private void CheckIfStudentAccount()
        {
            try
            {
                int isStudent = db.IsStudent(userData.UserRecordID);

                if (isStudent == 0 || userData.ActiveSession == false)
                {
                    Application.Current.Properties.Clear();
                    Navigation.PopToRootAsync();
                    Application.Current.MainPage = new Login();
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
            try
            {
                user = db.GetOneStudent(userData.UserRecordID);
                userCourse = db.GetOneStudentCourse(userData.UserRecordID);

                if (user != null)
                {
                    if (userCourse != null)
                    {
                        studName.Text = user.User_LastName + " " + user.User_FirstName;
                        studID.Text = user.User_ID.ToString();
                        studPUID.Text = user.User_PUID;
                        studUni.Text = db.GetSchoolName(userCourse.Student_SchoolRecordID);
                        studCourse.Text = db.GetCourseName(userCourse.Student_CourseRecordID);
                    }
                    else
                    {
                        DisplayAlert("Error", "Failed to get student course.", "OK");
                    }
                }
                else
                {
                    DisplayAlert("Error", "Failed to get student details.", "OK");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load student details: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }
    }
}