using MySIM.Models;
using MySIM.ViewModels;
using MySIM.Views.Modules_Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/* Visual Studio Generated Skeleton Code: 
namespace MySIM.Views.Courses_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewModules : ContentPage
    {
        public ViewModules()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Courses_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewModules : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Modules> modList = new List<Modules>();
        private static int courseRecordID, schoolRecordID;

        private int rowsAffected = 0;

        public ViewModules(int crseID)
        {
            InitializeComponent();

            courseRecordID = crseID;
            schoolRecordID = db.GetSchoolRecordID(courseRecordID);

            //Initialise & Load Page.
            Initialise();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Initialise();
        }

        protected void RefreshAddedModulesBtn_Clicked(object sender, EventArgs args)
        {
            noAddedModulesLbl.IsVisible = false;
            LoadOneCourseModules();
        }
        protected void AddBtn_Clicked(object sender, EventArgs args)
        {
            try
            {
                Navigation.PushAsync(new AddOneExistingModule(courseRecordID));
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to redirect to add an existing module : " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void AddedModuleList_Refreshing(object sender, EventArgs args)
        {
            noAddedModulesLbl.IsVisible = false;
            LoadOneCourseModules();
            addedModuleList.EndRefresh();
        }

        protected async void Delete_Clicked(object sender, EventArgs args)
        {
            try
            {
                int selectedModuleRecordID = Int32.Parse(((MenuItem)sender).CommandParameter.ToString());

                var response = await DisplayAlert("Confirm Removal", "Remove module from course?", "OK", "Cancel");

                //OK selected.
                if (response)
                {
                    //1. Delete Courses_Modules record from database.
                    rowsAffected = db.DeleteOneCourseModule(selectedModuleRecordID, courseRecordID, userData.UserRecordID);
                    if (rowsAffected == 1)
                    {
                        await DisplayAlert("Success", "Module has been removed successfully.", "OK");
                        SetPageDefaultSettings();
                        LoadOneCourseModules();
                    }
                    else
                    {
                        await DisplayAlert("Failure", "Failed to remove module from course.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to remove module: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }



        private void Initialise()
        {
            //Check if user account type is admin.
            CheckIfAdminAccount();

            //Initialise page controls.
            SetPageDefaultSettings();

            //Get & load list of modules under one course.
            LoadOneCourseModules();
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
            //Reset fields.
            noAddedModulesLbl.IsVisible = false;

            //Clear added module list view.
            addedModuleList.ItemsSource = null;
            addedModuleList.SelectedItem = null;
        }

        //Get & load list of modules under one course.
        private void LoadOneCourseModules()
        {
            try
            {
                //Get list of all modules.
                modList = GetAllAddedModules();

                //No record, display no modules message.
                if (modList.Count == 0)
                {
                    noAddedModulesLbl.IsVisible = true;
                }
                //Have records, display list of added modules.
                else
                {
                    noAddedModulesLbl.IsVisible = false;
                    addedModuleList.ItemsSource = modList;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load list of added modules: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        private List<Modules> GetAllAddedModules()
        {
            return db.GetModulesByCourses(courseRecordID).OrderBy(x => x.Module_Code).ToList();
        }
    }
}
