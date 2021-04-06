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
    public partial class ClassView : ContentPage
    {
        public ClassView()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClassView : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Users_Students> classList = new List<Users_Students>();
        private List<Modules> modList = new List<Modules>();
        private string currentModuleOption, moduleCodeTableName;
        public ClassView()
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

        protected void SortIndexChanged(object sender, EventArgs args)
        {
            try
            {
                if(sortModulePicker.SelectedItem != null)
                {
                    var selectedModule = (Modules)sortModulePicker.SelectedItem;
                    int index = sortModulePicker.SelectedIndex;
               
                    //Different option selected.
                    if(!selectedModule.Module_Code.Equals(currentModuleOption))
                    {
                        
                        if (selectedModule.Module_Code.Contains(" "))
                        {
                            moduleCodeTableName = selectedModule.Module_Code.Replace(" ", "_");
                        }
                        else
                        {
                            moduleCodeTableName = selectedModule.Module_Code;
                        }

                        int classExists = db.CheckIfModuleClassTableExists(moduleCodeTableName);

                        //Have classes.
                        if (classExists != 0)
                        {
                            LoadOneStudentModules(selectedModule.Module_Code, moduleCodeTableName);
                            currentModuleOption = ((Modules)sortModulePicker.SelectedItem).Module_Code;
                            noClassesLbl.IsVisible = false;
                        }
                        //No classes.
                        else
                        {
                            noClassesLbl.IsVisible = true;
                            enrolledModuleList.ItemsSource = null;
                        }
                        currentModuleOption = selectedModule.Module_Code;
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load list of enrolled modules: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void EnrolledModuleList_Refreshing(object sender, EventArgs args)
        {
            SetPageDefaultSettings();
            LoadOneStudentModules("", "");
            enrolledModuleList.EndRefresh();
        }





        private void Initialise()
        {
            //Check if user account type is student.
            CheckIfStudentAccount();

            //Initialise page controls.
            SetPageDefaultSettings();

            //Get & load list of modules enrolled under one student.
            LoadOneStudentModules("", "");
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
            noEnrolledModulesLbl.IsVisible = false;
            noClassesLbl.IsVisible = false;

            //Clear enrolled module list view.
            enrolledModuleList.ItemsSource = null;
            enrolledModuleList.SelectedItem = null;

            LoadPickerOptions();
        }

        //Set picker options to list of enrolled modules.
        private void LoadPickerOptions()
        {
            try
            {
                //Get list of university names
                modList = db.GetEnrolledModuleList(userData.UserRecordID);
                if (modList.Count != 0)
                {
                    sortModulePicker.ItemsSource = modList;
                    sortModulePicker.SelectedIndex = 0;
                    noEnrolledModulesLbl.IsVisible = true;
                }
                else
                {
                    noEnrolledModulesLbl.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load module picker options: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        //Set picker options to list of enrolled modules.
        private void LoadOneStudentModules(string selectedModCode, string tableName)
        {
            try
            {
                if (selectedModCode != "")
                {
                    classList = GetStudentClasses(selectedModCode, tableName);                    
                }
                //No record, display no modules message.
                if (classList.Count == 0)
                {
                    noEnrolledModulesLbl.IsVisible = false;
                    noClassesLbl.IsVisible = true;
                }
                //Have records, display list of enrolled modules.
                else
                {
                    noEnrolledModulesLbl.IsVisible = false;
                    noClassesLbl.IsVisible = false;  
                    enrolledModuleList.ItemsSource = classList.OrderBy(x => x.ModuleClass_Date);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load enrolled module class schedule options: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }
        private List<Users_Students> GetStudentClasses(string modCode, string tableName)
        {
            return db.GetStudentClassesByModule(userData.UserRecordID, modCode, tableName).OrderBy(x => x.ModuleClass_RecordID).ToList();
        }
    }
}
