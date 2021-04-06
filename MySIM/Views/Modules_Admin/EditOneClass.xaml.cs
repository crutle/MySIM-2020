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
    public partial class EditOneClass : ContentPage
    {
        public EditOneClass()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Modules_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditOneClass : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Classes> classList = new List<Classes>();
        private Classes cls = new Classes();
        private static int modClassRecordID, moduleRecordID;
        private static string moduleCode, moduleCodeTableName;

        private int currentClassOption, rowsAffected, rowsAffectedTwo = 0;

        public EditOneClass(string modCode, int recordID)
        {
            InitializeComponent();

            moduleCode = modCode;
            modClassRecordID = recordID;
            moduleRecordID = db.GetModuleRecordID(moduleCode);

            //Initialise & Load Page.
            Initialise();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Initialise();
        }

        protected void GenAttCodeBtn_Clicked(object sender, EventArgs args)
        {
            string clsCode = GenerateRandomString();
            classCode.Text = clsCode;
        }

        protected void EditBtn_Clicked(object sender, EventArgs args)
        {
            if (toolbarEdit.Text.Equals("Save"))
            {
                SaveBtn_Clicked(sender, args);
            }

            classTimePicker.IsEnabled = true;
            classDate.IsEnabled = true;
            classLoc.IsEnabled = true;
            classCode.IsEnabled = false;

            genAttCodeBtn.IsVisible = true;
            editBtn.IsVisible = false;
            deleteBtn.IsVisible = true;
            saveBtn.IsVisible = true;
            cancelEditBtn.IsVisible = true;

            toolbarEdit.Text = "Save";
        }

        protected async void ClassTimingChanged(object sender, EventArgs args)
        {
            try
            {
                var selectedClassTiming = (Classes)classTimePicker.SelectedItem;

                //Picker after reset or onload.
                if (selectedClassTiming == null)
                {
                    SetPageDefaultSettings();
                    LoadOneModuleClass();
                }
                else
                {
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
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to change class timing: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void SaveBtn_Clicked(object sender, EventArgs args)
        {
            try
            {
                Classes c = new Classes()
                {
                    ModuleClass_RecordID = modClassRecordID,
                    ClassTiming_RecordID = ((Classes)classTimePicker.SelectedItem).ClassTiming_RecordID,
                    ModuleClass_Date = classDate.Date.ToString(),
                    ModuleClass_Loc = classLoc.Text,
                    AttendanceCode = classCode.Text,
                    EditedBy = userData.UserRecordID

                };
                rowsAffected = UpdateClass(c);
                if (rowsAffected != 0)
                {
                    DisplayAlert("Success", "Class record has been updated successfully.", "OK");
                    SetPageDefaultSettings();
                    LoadOneModuleClass();
                }
                else
                {
                    DisplayAlert("Failure", "Class record failed to update.", "OK");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to update class record: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void CancelEditBtn_Clicked(object sender, EventArgs args)
        {
            SetPageDefaultSettings();
            LoadOneModuleClass();
        }

        protected async void DeleteBtn_Clicked(object sender, EventArgs args)
        {
            try
            {
                var response = await DisplayAlert("Confirm Deletion", "Delete class?", "OK", "Cancel");

                //OK selected.
                if (response)
                {
                    int oldQty = db.GetLessonQuantity(moduleRecordID);

                    //1. Delete Module's Class record from database.
                    if(moduleCode.Contains(" "))
                    {
                        rowsAffected = db.DeleteOneClass(moduleCode.Replace(" ","_"), modClassRecordID, userData.UserRecordID);
                    }
                    else
                    {
                        rowsAffected = db.DeleteOneClass(moduleCode, modClassRecordID, userData.UserRecordID);
                    }
                    
                    if (rowsAffected != 0)
                    {
                        rowsAffectedTwo = db.UpdateOneModuleQty(oldQty - 1, moduleCode, userData.UserRecordID);
                        if (rowsAffectedTwo != 0)
                        {
                            await DisplayAlert("Success", "Class has been deleted successfully.", "OK");
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert("Failure", "Failed to update lesson quantity.", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Failure", "Failed to delete class.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to delete class: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }



        private void Initialise()
        {
            //Check if user account type is admin.
            CheckIfAdminAccount();

            CheckIfTableExists();

            //Initialise page controls.
            SetPageDefaultSettings();

            //Get & load one module's class record.
            LoadOneModuleClass();
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

        //Check if UserSettingsController's stored user data is admin data.
        private void CheckIfTableExists()
        {
            try
            {
                int exists;
                if (moduleCode.Contains(" "))
                {
                    exists = db.CheckIfModuleClassTableExists(moduleCode.Replace(" ", "_"));
                }
                else
                {
                    exists = db.CheckIfModuleClassTableExists(moduleCode);
                }
             
                if (exists == 0)
                {
                    Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to check if table exists: " + ex.Message + " (Contact Administrator)", "OK");
            }

        }

        //Initialise page controls.
        private void SetPageDefaultSettings()
        {
            classLoc.Text = "";
            classDate.Date = DateTime.Today;

            classTimePicker.IsEnabled = false;
            classDate.IsEnabled = false;
            classLoc.IsEnabled = false;
            classCode.IsEnabled = false;

            genAttCodeBtn.IsVisible = false;
            editBtn.IsVisible = true;
            toolbarEdit.Text = "Edit";

            deleteBtn.IsVisible = false;
            saveBtn.IsVisible = false;
            cancelEditBtn.IsVisible = false;
        }

        private void LoadOneModuleClass()
        {
            try
            {
                //Get class details from database.
                if (moduleCode.Contains(" "))
                {
                    cls = db.GetOneModuleClass(moduleCode.Replace(" ","_"), modClassRecordID);
                }
                else
                {
                    cls = db.GetOneModuleClass(moduleCode, modClassRecordID);
                }              

                //Load fields.
                startTime.Text = "Start Time: " + cls.ClassTiming_StartTime;
                endTime.Text = "End Time: " + cls.ClassTiming_EndTime;
                classDate.Date = Convert.ToDateTime(cls.ModuleClass_Date);
                classLoc.Text = cls.ModuleClass_Loc;
                classCode.Text = cls.AttendanceCode;

                LoadPickerOptions();
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load class details: " + ex.Message + " (Contact Administrator)", "OK");
            }
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
                    for (int i = 0; i < classList.Count; i++)
                    {
                        if (classList[i].ClassTiming_Name.Equals(cls.ClassTiming_Name))
                        {
                            classTimePicker.SelectedIndex = i;
                            break;
                        }
                    }
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

        private int UpdateClass(Classes c)
        {
            if(moduleCode.Contains(" "))
            {
                return db.UpdateOneClass(c, moduleCode.Replace(" ","_"));
            }
            else
            {
                return db.UpdateOneClass(c, moduleCode);
            }           
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
