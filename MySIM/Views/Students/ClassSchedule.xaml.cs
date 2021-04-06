using MySIM.Models;
using MySIM.ViewModels;
using Syncfusion.SfSchedule.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/* Visual Studio Generated Skeleton Code: 
namespace MySIM.Views.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClassSchedule : TabbedPage
    {
        public ClassSchedule()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClassSchedule : TabbedPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Modules> modList = new List<Modules>();
        private string moduleCodeTableName;

        public ClassSchedule()
        {
            InitializeComponent();

            //Create List View Tab
            ClassView cv = new ClassView();
            Children.Add(cv);

            //Initialise & Load Page.
            Initialise();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Initialise();
        }
        
        protected void ClassSchedule_CellTapped(object sender, CellTappedEventArgs args)
        { 
          if(args.Appointment != null)
            {
                var selectedCell = (Users_Students)args.Appointment;
                string moduleCodeName = selectedCell.EventName;
                DateTime startTime = selectedCell.From;
                DateTime endTime = selectedCell.To;
                
                string detailsStr = moduleCodeName + "\n\n START: " + startTime.ToString("g") + " \n END: " + endTime.ToString("g");

                DisplayAlert("Class Details", detailsStr, "OK");
            }
        }



        private void Initialise()
        {
            //Check if user account type is student.
            CheckIfStudentAccount();

            //Initialise page controls.
            SetPageDefaultSettings();

            //Get & load list of modules under one course.
            LoadOneStudentClasses();
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
            classSchedule.DataSource = null;

            SelectedTabColor = Color.White;
            UnselectedTabColor = Color.Black;
        }

        //Get & load list of modules classes under one student.
        private void LoadOneStudentClasses()
        {
            try
            {
                modList = db.GetEnrolledModuleList(userData.UserRecordID);

                var studentClasses = new ObservableCollection<Users_Students>();

                for (int i = 0; i < modList.Count; i++)
                {
                    if(modList[i].Module_Code.Contains(" "))
                    {
                        moduleCodeTableName = modList[i].Module_Code.Replace(" ", "_");
                    }
                    else
                    {
                        moduleCodeTableName = modList[i].Module_Code;
                    }
                    int classExists = db.CheckIfModuleClassTableExists(moduleCodeTableName);
                    if(classExists != 0)
                    {
                        List<Users_Students> tempList = db.GetStudentClassesByModule(userData.UserRecordID, modList[i].Module_Code, moduleCodeTableName);

                        for (int j = 0; j < tempList.Count; j++)
                        {
                            DateTime date = Convert.ToDateTime(tempList[j].ModuleClass_Date);
                            DateTime sTime = Convert.ToDateTime(tempList[j].ClassTimings_StartTime);
                            DateTime eTime = Convert.ToDateTime(tempList[j].ClassTimings_EndTime);

                            DateTime newStartDateTime = new DateTime(date.Year, date.Month, date.Day,
                                                sTime.Hour, sTime.Minute, sTime.Second);
                            DateTime newEndDateTime = new DateTime(date.Year, date.Month, date.Day,
                                                eTime.Hour, eTime.Minute, eTime.Second);

                            //Reference: https://help.syncfusion.com/xamarin/scheduler/data-bindings
                            Users_Students studClass = new Users_Students
                            {
                                From = newStartDateTime,
                                To = newEndDateTime,
                                EventName = tempList[j].FormattedTitle,
                                Color = Color.FromHex("#b7d192"),
                                ModuleClass_Location = tempList[j].ModuleClass_Location,
                                Id = tempList[j].ModuleClass_RecordID
                            };
                            studentClasses.Add(studClass);
                        }
                    }                   
                }

                classSchedule.DataSource = studentClasses;
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load list of student classes: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }
    }
}