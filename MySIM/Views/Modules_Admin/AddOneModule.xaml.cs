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

/* Visual Studio Generated Skeleton Code: 
namespace MySIM.Views.Modules_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddOneModule : ContentPage
    {
        public AddOneModule()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Modules_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddOneModule : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Classes> classList = new List<Classes>();       
        private Classes cls = new Classes();

        private int currentClassOption,  rowsAffected = 0;

        public AddOneModule()
        {
            InitializeComponent();
            //Initialise & Load Page.
            Initialise();
        }

        protected void SaveBtn_Clicked(object sender, EventArgs args)
        {
            if (classTimePicker.SelectedItem == null)
            {
                DisplayAlert("", "Please select a class session.", "OK");
            }
            else if (string.IsNullOrWhiteSpace(modCode.Text) && string.IsNullOrWhiteSpace(modName.Text) && (modQty.Text == ""))
            {
                modCodeError.IsVisible = true;
                modNameError.IsVisible = true;
                modQtyError.IsVisible = true;
                requiredLbl.TextColor = Color.Red;
            }
            else if (!string.IsNullOrWhiteSpace(modCode.Text) && string.IsNullOrWhiteSpace(modName.Text) && (modQty.Text == ""))
            {
                modCodeError.IsVisible = false;
                modNameError.IsVisible = true;
                modQtyError.IsVisible = true;
                requiredLbl.TextColor = Color.Red;
            }
            else if (string.IsNullOrWhiteSpace(modCode.Text) && !string.IsNullOrWhiteSpace(modName.Text) && (modQty.Text == ""))
            {
                modCodeError.IsVisible = true;
                modNameError.IsVisible = false;
                modQtyError.IsVisible = true;
                requiredLbl.TextColor = Color.Red;
            }
            else if (string.IsNullOrWhiteSpace(modCode.Text) && string.IsNullOrWhiteSpace(modName.Text) && (modQty.Text != ""))
            {
                modCodeError.IsVisible = true;
                modNameError.IsVisible = true;
                modQtyError.IsVisible = false;
                requiredLbl.TextColor = Color.Red;
            }

            else if (!string.IsNullOrWhiteSpace(modCode.Text) && !string.IsNullOrWhiteSpace(modName.Text) && (modQty.Text == ""))
            {
                modCodeError.IsVisible = false;
                modNameError.IsVisible = false;
                modQtyError.IsVisible = true;
                requiredLbl.TextColor = Color.Red;
            }
            else if (!string.IsNullOrWhiteSpace(modCode.Text) && string.IsNullOrWhiteSpace(modName.Text) && (modQty.Text != ""))
            {
                modCodeError.IsVisible = false;
                modNameError.IsVisible = true;
                modQtyError.IsVisible = false;
                requiredLbl.TextColor = Color.Red;
            }
            else if (string.IsNullOrWhiteSpace(modCode.Text) && !string.IsNullOrWhiteSpace(modName.Text) && (modQty.Text != ""))
            {
                modCodeError.IsVisible = true;
                modNameError.IsVisible = false;
                modQtyError.IsVisible = false;
                requiredLbl.TextColor = Color.Red;
            }
            else if (Regex.IsMatch(modCode.Text, @"^\d"))
            {
                DisplayAlert("", "Module code cannot start with a number.", "OK");
            }
            else
            {
                bool intOnly = int.TryParse(modQty.Text, out int i);
                if (!intOnly)
                {
                    DisplayAlert("", "Please fill in Lesson Quantity with numbers only.", "OK");
                }
                else
                {
                    try
                    {
                        Modules mod = new Modules
                        {
                            Module_Code = modCode.Text,
                            Module_Name = modName.Text,
                            Module_Description = modDesc.Text,
                            Module_LessonQty = int.Parse(modQty.Text),
                            Module_LessonStartDate = modDate.Date.ToString(),
                            ClassTiming_RecordID = ((Classes)classTimePicker.SelectedItem).ClassTiming_RecordID,
                            CreatedBy = userData.UserRecordID
                        };

                        //Check if module code exists.
                        int count = db.CheckIfModuleCodeExists(modCode.Text);

                        //Module does not exist
                        if (count == 0)
                        {
                            //1. Add Modules record.
                            rowsAffected = db.AddOneModule(mod);
                            if (rowsAffected == 1)
                            {
                                DisplayAlert("Success", "Module record has been created successfully.", "OK");
                                Navigation.PopAsync();
                            }
                            else
                            {
                                DisplayAlert("Failure", "Module record failed to create.", "OK");
                            }
                        }
                        //Module exists.
                        else
                        {
                            DisplayAlert("Failure", "Failed to add new module. Module exists in the database.", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("UNIQUE KEY"))
                        {
                            DisplayAlert("Creation Failure", "Failed to add new module: A record with the module code exists in the database. Please check the database. (Contact Administrator)", "OK");
                        }
                        else
                        {
                            DisplayAlert("Error", "Failed to add new module: " + ex.Message + " (Contact Administrator)", "OK");
                        }
                    }
                }
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
                        startTime.IsVisible = true;
                        endTime.IsVisible = true;
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
            //Reset classTimePicker.
            classTimePicker.SelectedItem = null;
            LoadPickerOptions();
            startTime.IsVisible = false;
            endTime.IsVisible = false;

            modCodeError.IsVisible = false;
            modNameError.IsVisible = false;
            modQtyError.IsVisible = false;

            requiredLbl.TextColor = Color.Gray;

            //Reset fields.
            modCode.Text = "";
            modName.Text = "";
            modDesc.Text = "";
            modQty.Text = "";
            modDate.Date = DateTime.Today;
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
                    classTimePicker.SelectedItem = null;
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
    }
}

