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
namespace MySIM.Views.Students_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnrollExistingModule : ContentPage
    {
        public EnrollExistingModule()
        {
            InitializeComponent();
        }
    }
} 
*/

namespace MySIM.Views.Students_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnrollExistingModule : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Modules> modList = new List<Modules>();
        private static int userRecordID, schoolRecordID, courseRecordID;
        private static string courseName, schoolName;

        private int rowsAffected = 0;

        public EnrollExistingModule(int recordID, int schID, int crseID)
        {
            InitializeComponent();

            userRecordID = recordID;
            schoolRecordID = schID;
            courseRecordID = crseID;
            courseName = db.GetCourseName(courseRecordID);
            schoolName = db.GetSchoolName(schoolRecordID);

            //Initialise & Load Page.
            Initialise();
        }

        protected void SearchBtn_Clicked(object sender, EventArgs args)
        {
            string searchStr = searchField.Text;

            //Only searched text.
            if (string.IsNullOrWhiteSpace(searchStr))
            {
                SetPageDefaultSettings();
                LoadModules("");
            }
            else
            {
                LoadModules(searchStr);
            }
        }

        protected void RefreshBtn_Clicked(object sender, EventArgs args)
        {
            SetPageDefaultSettings();
            LoadModules("");
        }

        protected async void ListViewItem_Clicked(object sender, SelectedItemChangedEventArgs args)
        {
            try
            {
                var selectedMod = (Modules)args.SelectedItem;

                var response = await DisplayAlert("Confirm Addition", "Enroll student in " + selectedMod.Module_Code + " " + selectedMod.Module_Name + "?", "OK", "Cancel");

                //OK selected.
                if (response)
                {
                    int count = db.CheckIfStudentModuleExists(userRecordID, selectedMod.Module_RecordID);
                    //Module added to course.
                    if (count == 1)
                    {
                        await DisplayAlert("Failure", "Failed to link module to student. Student has already enrolled module.", "OK");
                    }
                    else
                    {
                        //Add Module to Course.
                        rowsAffected = db.AddOneStudentModule(userRecordID, schoolRecordID, courseRecordID, selectedMod.Module_RecordID, userData.UserRecordID);//AddOneCourseModule(courseRecordID, selectedMod.Module_RecordID, userData.UserRecordID);
                        if (rowsAffected != 0)
                        {
                            await DisplayAlert("Success", "Module has been enrolled successfully.", "OK");
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert("Failure", "Failed to link module to student.", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to enroll module: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void ModuleList_Refreshing(object sender, EventArgs args)
        {
            SetPageDefaultSettings();
            LoadModules("");
            moduleList.EndRefresh();
        }


        private void Initialise()
        {
            //Check if user account type is admin.
            CheckIfAdminAccount();

            //Initialise page controls.
            SetPageDefaultSettings();

            //Get & load existing modules.
            LoadModules("");
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
            searchField.Text = "";
            searchField.Completed += (s, e) => SearchBtn_Clicked(s, e);

            //Reset fields.           
            noModulesLbl.IsVisible = false;

            //Reset existing module list view.
            moduleList.ItemsSource = null;
        }

        //Get & load existing modules.
        private void LoadModules(string str)
        {
            try
            {
                //Get list of all modules.
                if (string.IsNullOrEmpty(str))
                {
                    modList = GetSearchedModuleList_FilterByUniCourse("", schoolName, courseName);
                }
                else
                {
                    modList = GetSearchedModuleList_FilterByUniCourse(str, schoolName, courseName);
                }

                //No record, display no courses message.
                if (modList.Count == 0)
                {
                    noModulesLbl.IsVisible = true;
                    moduleList.ItemsSource = null;
                }
                //Have records, display list of courses.
                else
                {
                    noModulesLbl.IsVisible = false;
                    moduleList.ItemsSource = modList;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load list of all modules: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        private List<Modules> GetSearchedModuleList_FilterByUniCourse(string searchedString, string selectedUniName, string selectedCourseName)
        {
            return db.GetSearchedAndFilteredModules(selectedUniName, selectedCourseName, searchedString).OrderBy(x => x.Module_Code).ToList();
        }
    }
}