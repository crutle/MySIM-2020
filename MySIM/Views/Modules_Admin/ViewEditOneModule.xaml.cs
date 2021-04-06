using MySIM.Models;
using MySIM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*Visual Studio Generated Skeleton Code: 
namespace MySIM.Views.Modules_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewEditOneModule : TabbedPage
    {
        public ViewEditOneModule()
        {
            InitializeComponent();
        }
    }
}*/

namespace MySIM.Views.Modules_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewEditOneModule : TabbedPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Classes> classList = new List<Classes>();
        private Modules module = new Modules();
        private Classes cls = new Classes();

        private static int moduleRecordID;
        private static string moduleCode;

        private int currentClassOption, rowsAffected, rowsAffectedTwo = 0;
     

        public ViewEditOneModule(int modID)
        {
            InitializeComponent();

            moduleRecordID = modID;
            moduleCode = db.GetModuleCode(moduleRecordID);

            //Create Classes Tab
            ViewEditClasses classTab = new ViewEditClasses(moduleRecordID);
            Children.Add(classTab);

            //Initialise & Load Page.
            Initialise();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing(); 
            Initialise();      
        }
        
        protected void RefreshClassesBtn_Clicked(object sender, EventArgs args)
        {
            SetPageDefaultSettings();
            LoadOneModule();
        }

        protected void EditBtn_Clicked(object sender, EventArgs args)
        {
            if (toolbarEdit.Text.Equals("Save"))
            {
                SaveBtn_Clicked(sender, args);
            }

            classTimePicker.IsEnabled = true;

            modName.IsEnabled = true;
            modDesc.IsEnabled = true;
            modQty.IsEnabled = true;
            modDate.IsEnabled = true;

            editBtn.IsVisible = false;

            saveBtn.IsVisible = true;
            cancelEditBtn.IsVisible = true;
            deleteBtn.IsVisible = true;

            toolbarEdit.Text = "Save";

        }
        protected async void SaveBtn_Clicked(object sender, EventArgs args)
        {
            if(classTimePicker.SelectedItem == null)
            {
                DisplayAlert("", "Please select a class session.", "OK");
            }
            else if (string.IsNullOrWhiteSpace(modName.Text) && (modQty.Text == ""))
            {
                modNameError.IsVisible = true;
                modQtyError.IsVisible = true;
                requiredLbl.TextColor = Color.Red;
            }          
            else if (!string.IsNullOrWhiteSpace(modName.Text) && (modQty.Text == ""))
            {
                modNameError.IsVisible = false;
                modQtyError.IsVisible = true;
                requiredLbl.TextColor = Color.Red;
            }
            else if (string.IsNullOrWhiteSpace(modName.Text) && (modQty.Text != ""))
            {
                modNameError.IsVisible = true;
                modQtyError.IsVisible = false;
                requiredLbl.TextColor = Color.Red;
            }
            else if (Regex.IsMatch(modCode.Text, @"^\d"))
            {
                DisplayAlert("", "Module code cannot start with a number.", "OK");
            }
            else
            {
                try
                {
                    Modules mod = new Modules
                    {
                        Module_RecordID = moduleRecordID,
                        Module_Code = modCode.Text,
                        Module_Name = modName.Text,
                        Module_Description = modDesc.Text,
                        Module_LessonQty = int.Parse(modQty.Text),
                        Module_LessonStartDate = modDate.Date.ToString(),
                        ClassTiming_RecordID = ((Classes)classTimePicker.SelectedItem).ClassTiming_RecordID,
                        EditedBy = userData.UserRecordID
                    };

                    //Update Modules record.
                    rowsAffected = db.UpdateOneModule(mod);
                    if (rowsAffected == 1)
                    {
                        await DisplayAlert("Success", "Module record has been updated successfully.", "OK");
                        SetPageDefaultSettings();
                        LoadOneModule();
                    }
                    else
                    {
                        await DisplayAlert("Failure", "Module record failed to udpate.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Failed to udpate module: " + ex.Message + " (Contact Administrator)", "OK");
                }
            }
        }

        protected void CancelEditBtn_Clicked(object sender, EventArgs args)
        {
            SetPageDefaultSettings();
            LoadOneModule();
        }

        protected async void DeleteBtn_Clicked(object sender, EventArgs args)
        {
            try
            {
                var response = await DisplayAlert("Confirm Deletion", "Delete module?", "OK", "Cancel");

                //OK selected.
                if (response)
                {
                    int count = db.CheckIfCourseModuleExists(moduleCode);
                    //1. Courses_Modules to be removed.
                    if (count != 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            db.DeleteOneCourseModuleByMod(moduleRecordID, userData.UserRecordID);
                        }
                    }

                    //2. Delete Modules record from database.
                    rowsAffected = db.DeleteOneModule(moduleRecordID, userData.UserRecordID);
                    if (rowsAffected != 0)
                    {
                        int exists = db.CheckIfModuleClassTableExists(moduleCode);
                        if (exists != 0)
                        {
                            //3. Delete Classes.
                            rowsAffectedTwo = db.DropClassTable(moduleCode);
                            if (rowsAffectedTwo != 0)
                            {
                                await DisplayAlert("Success", "Module has been successfully deleted.", "OK");
                                await Navigation.PopAsync();
                            }
                            else
                            {
                                await DisplayAlert("Failure", "Failed to delete classes.", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Success", "Module has been successfully deleted.", "OK");
                            await Navigation.PopAsync();
                        }                   
                    }
                    else
                    {
                        await DisplayAlert("Failure", "Failed to delete module.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to delete module: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void ClassTimingChanged(object sender, EventArgs args)
        {
            try
            {
                var selectedClassTiming = (Classes)classTimePicker.SelectedItem;
                if(selectedClassTiming != null)
                {
                    //Different option selected.
                    if (!selectedClassTiming.ClassTiming_RecordID.Equals(currentClassOption))
                    {
                        cls = db.GetOneClassTimings(selectedClassTiming.ClassTiming_RecordID);
                        if (cls != null)
                        {
                            startTime.Text = "Start Time: " + cls.ClassTiming_StartTime;
                            endTime.Text = "End Time: " + cls.ClassTiming_EndTime;
                            startTime.IsVisible = true;
                            endTime.IsVisible = true;
                        }
                        currentClassOption = selectedClassTiming.ClassTiming_RecordID;
                    }
                }
               
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to change class timing: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }


        public void Initialise()
        {
            //Check if user account type is admin.
            CheckIfAdminAccount();

            //Initialise page controls.
            SetPageDefaultSettings();

            //Get & load one module.
            LoadOneModule();
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
            classTimePicker.SelectedItem = null;
            classTimePicker.IsEnabled = false;

            //Reset fields.
            modCode.Text = "";
            modName.Text = "";
            modDesc.Text = "";
            modQty.Text = "";
            modDate.Date = DateTime.Today;

            modCode.IsEnabled = false;
            modName.IsEnabled = false;
            modDesc.IsEnabled = false;
            modQty.IsEnabled = false;
            modDate.IsEnabled = false;

            modNameError.IsVisible = false;
            modQtyError.IsVisible = false;
            requiredLbl.TextColor = Color.Gray;

            editBtn.IsVisible = true;
            toolbarEdit.Text = "Edit";

            saveBtn.IsVisible = false;
            cancelEditBtn.IsVisible = false;
            deleteBtn.IsVisible = false;

            try
            {
                Title = db.GetModuleName(moduleRecordID);
                SelectedTabColor = Color.White;
                UnselectedTabColor = Color.Black;
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to reset page controls: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        //Set picker options to list of class timings.
        private void LoadPickerOptions(int classID)
        {
            try
            {
                //Get list of class timing names.
                classList = GetClassList();
                if (classList.Count != 0)
                {
                    classTimePicker.ItemsSource = classList;

                    if (classID != 0)
                    {
                        for (int i = 0; i < classList.Count; i++)
                        {
                            if (classList[i].ClassTiming_RecordID.Equals(classID))
                            {
                                classTimePicker.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        classTimePicker.SelectedItem = null;
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

        private void LoadOneModule()
        {
            try
            {
                //Get course details from database.
                module = db.GetOneModule(moduleRecordID);

                //Load fields.
                modCode.Text = module.Module_Code;
                modName.Text = module.Module_Name;
                modDesc.Text = module.Module_Description;
                modQty.Text = module.Module_LessonQty.ToString();
                if (!string.IsNullOrEmpty(module.Module_LessonStartDate))
                {
                    modDate.Date = Convert.ToDateTime(module.Module_LessonStartDate);
                }

                LoadPickerOptions(module.ClassTiming_RecordID);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load module details: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }
    }
}