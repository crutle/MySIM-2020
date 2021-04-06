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
    public partial class ViewEnrolledModules : ContentPage
    {
        public ViewEnrolledModules()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Students_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewEnrolledModules : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Modules> modList = new List<Modules>();
        private static int userRecordID, schoolRecordID, courseRecordID;

        private int rowsAffected = 0;

        public ViewEnrolledModules(int recordID, int schID, int crseID)
        {
            InitializeComponent();

            userRecordID = recordID;
            schoolRecordID = schID;
            courseRecordID = crseID;

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
            noEnrolledModulesLbl.IsVisible = false;
            LoadOneStudentModules();
        }

        protected void AddBtn_Clicked(object sender, EventArgs args)
        {
            try
            {
                Navigation.PushAsync(new EnrollExistingModule(userRecordID, schoolRecordID, courseRecordID));
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to redirect to add an existing module : " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void AddedModuleList_Refreshing(object sender, EventArgs args)
        {
            noEnrolledModulesLbl.IsVisible = false;
            LoadOneStudentModules();
            enrolledModuleList.EndRefresh();
        }

        protected async void Delete_Clicked(object sender, EventArgs args)
        {
            try
            {
                int selectedModuleRecordID = Int32.Parse(((MenuItem)sender).CommandParameter.ToString());

                var response = await DisplayAlert("Confirm Removal", "Remove module from student?", "OK", "Cancel");

                //OK selected.
                if (response)
                {
                    int exists = db.CheckIfStudentModuleExists(userRecordID, selectedModuleRecordID);
                    if (exists != 0)
                    {
                        //1. Delete Student_Modules record from database.
                        rowsAffected = db.DeleteOneStudentModule(userRecordID, selectedModuleRecordID, userData.UserRecordID);
                        if (rowsAffected != 0)
                        {
                            await DisplayAlert("Success", "Module has been removed successfully.", "OK");
                            SetPageDefaultSettings();
                            LoadOneStudentModules();
                        }
                        else
                        {
                            await DisplayAlert("Failure", "Failed to remove module from student.", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to remove module: Module has not been enrolled. (Contact Administrator)", "OK");
                        SetPageDefaultSettings();
                        LoadOneStudentModules();
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

            //Get & load list of modules enrolled under one student.
            LoadOneStudentModules();
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
            noEnrolledModulesLbl.IsVisible = false;

            //Clear added module list view.
            enrolledModuleList.ItemsSource = null;
            enrolledModuleList.SelectedItem = null;
        }

        //Get & load list of modules enrolled under one student.
        private void LoadOneStudentModules()
        {
            try
            {
                //Get list of all modules.
                modList = GetAllEnrolledModules();

                //No record, display no modules message.
                if (modList.Count == 0)
                {
                    noEnrolledModulesLbl.IsVisible = true;
                }
                //Have records, display list of added modules.
                else
                {
                    noEnrolledModulesLbl.IsVisible = false;
                    enrolledModuleList.ItemsSource = modList;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load list of enrolled modules: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        private List<Modules> GetAllEnrolledModules()
        {
            return db.GetOneStudentModules(userRecordID).OrderBy(x => x.Module_Code).ToList();
        }
    }
}
