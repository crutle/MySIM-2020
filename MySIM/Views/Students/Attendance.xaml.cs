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
    public partial class Attendance : ContentPage
    {
        public Attendance()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Attendance : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Modules> modList = new List<Modules>();
        private Users_Students obj, obj2;
        private static int modClassRecordID;
        private string moduleCodeTableName;
        public Attendance()
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

        protected async void SubmitBtn_Clicked(object sender, EventArgs args)
        {
            bool attCodeExists, attCodeValid, attTaken;

            if (sortModulePicker.SelectedItem == null)
            {
                await DisplayAlert("", "Please select a module to register attendance for.", "OK");
            }
            else
            {
                try
                {
                    var selectedModule = (Modules)sortModulePicker.SelectedItem;
                    string modCode = selectedModule.Module_Code;
                    if(modCode.Contains(" "))
                    {
                        moduleCodeTableName = modCode.Replace(" ", "_");
                    }
                    else
                    {
                        moduleCodeTableName = modCode;
                    }
                    string attCode = codeInput.Text;

                    int i = userData.UserRecordID;

                    //1. Check if Attendance Code exist under ModuleCode table.
                    attCodeExists = AttendanceCodeExists(moduleCodeTableName, attCode);

                    if (attCodeExists == true)
                    {
                        //2.a Get Attendance code valid DateTime range.
                        GetAttendanceCodeValidRange(moduleCodeTableName, modClassRecordID);
                        if (obj2 != null)
                        {
                            DateTime date = Convert.ToDateTime(obj2.ModuleClass_Date);
                            DateTime sTime = Convert.ToDateTime(obj2.ClassTimings_StartTime);
                            DateTime eTime = Convert.ToDateTime(obj2.ClassTimings_EndTime);

                            DateTime newStartDateTime = new DateTime(date.Year, date.Month, date.Day,
                                                sTime.Hour, sTime.Minute, sTime.Second) ;

                            DateTime newEndDateTime = new DateTime(date.Year, date.Month, date.Day,
                                                eTime.Hour, eTime.Minute, eTime.Second);

                            //3. Check if Attendance code is valid.
                            attCodeValid = AttendanceCodeValidity(moduleCodeTableName, attCode, newStartDateTime, newEndDateTime);
                            //3.a Valid
                            if (attCodeValid == true)
                            {
                                //4. Take attendance.                        
                                attTaken = TakeModAttendance(modCode, modClassRecordID);
                                if (attTaken == true)
                                {
                                    await DisplayAlert("Success", "Attendance taken.", "OK");
                                    SetPageDefaultSettings();
                                }
                                else
                                {
                                    await DisplayAlert("Failure", "Failed to take attendance.", "OK");
                                }
                            }
                            //3.b Invalid.
                            else
                            {
                                await DisplayAlert("Invalid", "Attendance code is invalid. Please try again when the class starts.", "OK");
                            }
                        }
                        //2.b Attendance code valid range does not exist.
                        else
                        {
                            await DisplayAlert("Invalid", "Attendance code does not exist.", "OK");
                        }

                    //1.b Attendance code does not exist.
                    }
                    else
                    {
                        await DisplayAlert("Invalid", "Attendance code does not exist.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Failed to take attendance: " + ex.Message + " (Contact Administrator)", "OK");

                }
            }
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
            //Reset fields.
            codeInput.Text = "";

            LoadPickerOptions();
        }

        //Set picker options to list of enrolled modules.
        private void LoadPickerOptions()
        {
            try
            {
                //Get list of module names
                modList = db.GetEnrolledModuleList(userData.UserRecordID);
                if (modList.Count != 0)
                {
                    //Remove modules without classes.
                    for(int i = 0; i < modList.Count; i++)
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
                        if (classExists == 0)
                        {
                            modList.RemoveAt(i);
                        }
                    }

                    if(modList.Count != 0)
                    {
                        sortModulePicker.ItemsSource = modList;
                        sortModulePicker.SelectedItem = null;
                    }
                    else
                    {
                        sortModulePicker.ItemsSource = null;
                        sortModulePicker.SelectedItem = null;
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load module picker options: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        private bool AttendanceCodeExists(string modCode, string attCode)
        {
            obj = db.CheckIfAttendanceCodeExists(modCode, attCode);
            if (obj == null)
            {
                return false;
            }
            else
            {
                modClassRecordID = obj.ModuleClass_RecordID;
                return true;
            }
        }

        private Users_Students GetAttendanceCodeValidRange(string modCode, int modClass_RecordID)
        {
            obj2 = db.GetAttendanceCodeStartEndDateTime(modCode, modClass_RecordID);
            if (obj2 == null)
            {
                return null;
            }
            else
            {
                return obj2;
            }
        }

        private bool AttendanceCodeValidity(string modCode, string attCode, DateTime startDt, DateTime endDt)
        {
            int count = db.CheckIfAttendanceCodeIsValid(modCode, attCode, DateTime.Now, startDt, endDt);
            if (count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool TakeModAttendance(string modCode, int modClass_RecordID)
        {
            int count = db.TakeAttendance(userData.UserRecordID, db.GetModuleRecordID(modCode), modCode, modClass_RecordID, userData.UserRecordID);
            if (count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
