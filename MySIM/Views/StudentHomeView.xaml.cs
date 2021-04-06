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
        public partial class StudentHomeView : ContentView
        {
            public StudentHomeView()
            {
                InitializeComponent();
            }
        }
    }
*/

namespace MySIM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StudentHomeView : ContentView
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();

        public StudentHomeView()
        {
            InitializeComponent();

            //Check if user account type is student.
            CheckIfStudentAccount();
        }

        protected void RedirectToStudentCardDetails(object sender, EventArgs args)
        {
            try
            {
                Navigation.PushAsync(new StudentDetails());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error", "Failed redirect to view student details: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void RedirectToClassSchedules(object sender, EventArgs args)
        {            
            try
            {
                Navigation.PushAsync(new ClassSchedule());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error", "Failed redirect to view class schedule: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void RedirectToAttendance(object sender, EventArgs args)
        {
            try
            {
                Navigation.PushAsync(new Attendance());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error", "Failed redirect to take attendance: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void RedirectToChatbot(object sender, EventArgs args)
        {          
            try
            {
                Navigation.PushAsync(new ChatPage());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error", "Failed redirect to chatbot: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void RedirectToContact(object sender, EventArgs args)
        {
           
            try
            {
                Navigation.PushAsync(new ContactUs(""));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error", "Failed redirect to view contact details: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        //Check if UserSettingsController's stored user data is student data.
        private void CheckIfStudentAccount()
        {
            try
            {

                int isStudent = db.IsStudent(userData.UserRecordID);
                if (isStudent == 0 || userData.ActiveSession == false)
                {
                    Application.Current.Properties.Clear();
                    Navigation.PopToRootAsync(true);
                    Navigation.PushAsync(new NavigationPage(new Login()));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error", "Failed to check if account type is student: " + ex.Message + " (Contact Administrator)", "OK");
            }            
        }
    }
}