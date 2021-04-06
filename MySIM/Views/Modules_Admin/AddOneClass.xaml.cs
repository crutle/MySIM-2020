using MySIM.Models;
using MySIM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*Visual Studio Generated Skeleton Code: 
namespace MySIM.Views.Modules_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddOneClass : ContentPage
    {
        public AddOneClass()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Modules_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddOneClass : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Classes> classList = new List<Classes>();
        private Classes cls = new Classes();
        private static string moduleCode, moduleCodeTableName;

        private int currentClassOption, rowsAffected, rowsAffectedTwo;

        public AddOneClass(string code)
        {
            InitializeComponent();

            moduleCode = code;

            //Initialise & Load Page.
            Initialise();
        }

        protected void GenAttCodeBtn_Clicked(object sender, EventArgs args)
        {
            string clsCode = GenerateRandomString();
            classCode.Text = clsCode;
        }

        protected void SaveBtn_Clicked(object sender, EventArgs args)
        {
            Modules m = db.GetOneModule(moduleCode);
            cls.ClassTiming_RecordID = ((Classes)classTimePicker.SelectedItem).ClassTiming_RecordID;
            cls.ModuleClass_Date = classDate.Date.ToString();
            cls.AttendanceCode = classCode.Text;
            int oldQty = db.GetLessonQuantity(moduleCode);

            try
            {
                //1. Check if table exists.
                if (moduleCode.Contains(" "))
                {
                    moduleCodeTableName = moduleCode.Replace(" ", "_");
                }
                else
                {
                    moduleCodeTableName = moduleCode;
                }

                int exists = db.CheckIfModuleClassTableExists(moduleCodeTableName);

                //Table exists.
                if (exists != 0)
                {
                    //a.2 Insert into table.
                    rowsAffected = InsertClasses(cls);
                    if (rowsAffected != 0)
                    {
                        int newQty = db.GetClassQuantity(moduleCodeTableName);                        

                        //a.3 Update lesson qty.
                        if (oldQty != newQty)
                        {
                            rowsAffectedTwo = UpdateClassQty(newQty);
                            if (rowsAffectedTwo != 0)
                            {
                                DisplayAlert("Success", "Class record has been created successfully.", "OK");
                                Navigation.PopAsync();
                            }
                            else
                            {
                                DisplayAlert("Failure", "Module's lesson quantity failed to update.", "OK");
                            }
                        }
                        else
                        {
                            DisplayAlert("Success", "Class record has been created successfully.", "OK");
                            Navigation.PopAsync();
                        }

                    }
                    else
                    {
                        DisplayAlert("Failure", "Class record failed to create.", "OK");
                    }
                }
                //Table does not exist.
                else
                {
                    //b.2 Create Default Table.
                    GenerateTable(m);
                    //b.3 Insert record.
                    rowsAffected = InsertClasses(cls);
                    if (rowsAffected != 0)
                    {
                        //b.4 Update lesson qty.
                        rowsAffectedTwo = UpdateClassQty(oldQty + 1);
                        if (rowsAffectedTwo != 0)
                        {
                            DisplayAlert("Success", "Class record has been created successfully.", "OK");
                            Navigation.PopAsync();
                        }
                        else
                        {
                            DisplayAlert("Failure", "Module's lesson quantity failed to update.", "OK");
                        }
                    }
                    else
                    {
                        DisplayAlert("Failure", "Class record failed to create.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to add new class: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void ClassTimingChanged(object sender, EventArgs args)
        {
            try
            {
                var selectedClassTiming = (Classes)classTimePicker.SelectedItem;

                //Different option selected.
                if (!selectedClassTiming.ClassTiming_RecordID.Equals(currentClassOption))
                {
                    cls = db.GetOneClassTimings(selectedClassTiming.ClassTiming_RecordID);
                    if (cls != null)
                    {
                        startTime.Text = "Start Time: " + cls.ClassTiming_StartTime;
                        endTime.Text = "End Time: " + cls.ClassTiming_EndTime;
                    }
                    currentClassOption = selectedClassTiming.ClassTiming_RecordID;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to change class timing: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }



        private void Initialise()
        {
            //Check if user account type is admin.
            CheckIfAdminAccount();

            //Initialise page controls.
            SetPageDefaultSettings();
        }

        //Check if UserSettingsController's stored user data is admin data.
        private void CheckIfAdminAccount()
        {
            try
            {
                if (userData.UserRecordID != 1 || userData.ActiveSession == false)
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
            LoadPickerOptions();

            classDate.Date = DateTime.Today;
            classCode.IsEnabled = false;

            classLoc.Text = "";
            classCode.Text = "";
        }

        //Set picker options to list of class timings.
        private void LoadPickerOptions()
        {
            try
            {
                //Get list of class timing names.
                classList = GetClassList();
                if (classList.Count != 0)
                {
                    classTimePicker.ItemsSource = classList;
                    classTimePicker.SelectedIndex = 0;

                    startTime.Text = "Start Time: " + classList[0].ClassTiming_StartTime;
                    endTime.Text = "End Time: " + classList[0].ClassTiming_EndTime;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load class picker options: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        private List<Classes> GetClassList()
        {
            return db.GetAllClassTimings().OrderBy(x => x.ClassTiming_Name).ToList();
        }

        //Create table & insert records.
        private async void GenerateTable(Modules m)
        {
            int createdCount, insertCount = 0;
            int days = 0;
            //Create Table.
            if (moduleCode.Contains(" "))
            {
                moduleCodeTableName = moduleCode.Replace(" ", "_");
            }
            else
            {
                moduleCodeTableName = moduleCode;
            }

            createdCount = db.CreateClassTable(moduleCodeTableName);

            //Table Created.
            if (createdCount == -1)
            {
                DateTime d = Convert.ToDateTime(m.Module_LessonStartDate);

                for (int i = 0; i < m.Module_LessonQty; i++)
                {
                    DateTime newDate = d.AddDays(days);
                    db.AddClasses(moduleCodeTableName, m.ClassTiming_RecordID, newDate, userData.UserRecordID);
                    days += 7;
                    insertCount++;
                }
                //Insert failed.
                if (insertCount == 0)
                {
                    await DisplayAlert("Failure", "Failed to create default records.", "ok");

                }
            }
            //Table not created.
            else
            {
                await DisplayAlert("Failure", "Failed to create " + moduleCode + "'s class table.", "ok");
            }
        }

        //Insert Record.
        private int InsertClasses(Classes c)
        {
            if(moduleCode.Contains(" "))
            {
                return db.AddClasses(moduleCode.Replace(" ", "_"), c.ClassTiming_RecordID, Convert.ToDateTime(c.ModuleClass_Date), c.AttendanceCode, userData.UserRecordID);
            }
            else
            {
                return db.AddClasses(moduleCode, c.ClassTiming_RecordID, Convert.ToDateTime(c.ModuleClass_Date), c.AttendanceCode, userData.UserRecordID);
            }
        }

        //Update qty.
        private int UpdateClassQty(int qty)
        {
            return db.UpdateOneModuleQty(qty, moduleCode, userData.UserRecordID);
        }

        //Reference: https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
        private string GenerateRandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
        }
    }
}