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
namespace MySIM.Views.Modules_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewEditClasses : ContentPage
    {
        public ViewEditClassSchedule()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Modules_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewEditClasses : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Classes> classList = new List<Classes>();
        private static int moduleRecordID, moduleClassRecordID;
        private static string moduleCode, moduleCodeTableName;

        private int rowsAffected, rowsAffectedTwo = 0;

        public ViewEditClasses(int modID)
        {
            InitializeComponent();

            moduleRecordID = modID;
            moduleCode = db.GetModuleCode(moduleRecordID);

            //Initialise & Load Page.
            Initialise();
        }

        protected override void OnAppearing()
        {  
            base.OnAppearing();
            Initialise();
        }

        protected async void GenerateClassesBtn_Clicked(object sender, EventArgs args)
        {
            //Check if module class table exists.
            if (moduleCode.Contains(" "))
            {
                moduleCodeTableName = moduleCode.Replace(" ", "_");
            }
            else
            {
                moduleCodeTableName = moduleCode;              
            }

            int exists = db.CheckIfModuleClassTableExists(moduleCodeTableName);
            Modules m = db.GetOneModule(moduleCode);         
        
            //Table does not exist.
            if (exists == 0)
            {
                //Create table & insert records.
                GenerateTable(m);
            }
            //Table exist.
            else
            {
                int actualQty = db.GetClassQuantity(moduleCodeTableName); 
                int recordQty = db.GetLessonQuantity(moduleCode);

                // To re-populate.
                if (actualQty != recordQty)
                {
                    var response = await DisplayAlert("Confirm Replacement", "Replace " + moduleCode + "'s class table?", "ok", "cancel");
                    if (response)
                    {
                        db.DropClassTable(moduleCodeTableName);
                        //Create table & insert records.
                        GenerateTable(m);
                    }
                }
                //Same qty, no changes.
                else
                {
                    LoadOneModuleClasses();
                }
            }
        }

        protected void RefreshClassesBtn_Clicked(object sender, EventArgs args)
        {
            SetPageDefaultSettings();
            LoadOneModuleClasses();
        }

        protected void AddBtn_Clicked(object sender, EventArgs args)
        {
            try
            {
                Navigation.PushAsync(new AddOneClass(moduleCode));
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to redirect to add a class: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void ClassListViewItem_Clicked(object sender, SelectedItemChangedEventArgs args)
        {
            try
            {
                if (args.SelectedItem != null)
                {
                    moduleClassRecordID = ((Classes)args.SelectedItem).ModuleClass_RecordID;
                    Navigation.PushAsync(new EditOneClass(moduleCode, moduleClassRecordID));
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to redirect to view module details: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void ClassList_Refreshing(object sender, EventArgs args)
        {
            SetPageDefaultSettings();
            LoadOneModuleClasses();
            classListView.EndRefresh();
        }

        protected async void Delete_Clicked(object sender, EventArgs args)
        {
            try
            {
                var response = await DisplayAlert("Confirm Deletion", "Delete class?", "OK", "Cancel");

                //OK selected.
                if (response)
                {
                    int selectedModClassRecordID = Int32.Parse(((MenuItem)sender).CommandParameter.ToString());
                    int oldQty = db.GetLessonQuantity(moduleRecordID);

                    //1. Delete Module's Class record from database.
                    if(moduleCode.Contains(" "))
                    {
                        rowsAffected = db.DeleteOneClass(moduleCode.Replace(" ", "_"), selectedModClassRecordID, userData.UserRecordID);
                    }
                    else
                    {
                        rowsAffected = db.DeleteOneClass(moduleCode, selectedModClassRecordID, userData.UserRecordID);
                    }

                    if (rowsAffected == 1)
                    {
                        rowsAffectedTwo = db.UpdateOneModuleQty(oldQty - 1, moduleCode, userData.UserRecordID);
                        if (rowsAffectedTwo == 1)
                        {
                            await DisplayAlert("Success", "Class has been deleted successfully.", "OK");
                            SetPageDefaultSettings();
                            LoadOneModuleClasses();
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

            //Initialise page controls.
            SetPageDefaultSettings();

            //Get & load list of class of one module.
            LoadOneModuleClasses();
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
            noClassesLbl.IsVisible = false;

            //Clear added module list view.
            classListView.ItemsSource = null;
            classListView.SelectedItem = null;
        }

        //Get & load list of classes under one module.
        private void LoadOneModuleClasses()
        {
            try
            {
                //Check if module class table exists.
                if(moduleCode.Contains(" "))
                {
                    moduleCodeTableName = moduleCode.Replace(" ", "_");
                }
                else
                {
                    moduleCodeTableName = moduleCode;                   
                }
                int exists = db.CheckIfModuleClassTableExists(moduleCodeTableName);

                //Table does not exist.
                if (exists == 0)
                {
                    noClassesLbl.IsVisible = true;
                }

                //Table exist.
                else
                {
                    classList = GetModuleClassList(moduleCodeTableName);

                    //No record, display no classes message.
                    if (classList.Count == 0)
                    {
                        noClassesLbl.IsVisible = true;
                    }
                    //Have records, display list of classes.
                    else
                    {
                        noClassesLbl.IsVisible = false;
                        classListView.ItemsSource = classList;
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load list of classes    : " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        private List<Classes> GetModuleClassList(string moduleCode)
        {
            return db.GetAllModuleClasses(moduleCode).OrderBy(x => x.ModuleClass_RecordID).ToList();
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
                string startDate = m.Module_LessonStartDate;
                DateTime d;
                if (startDate == "")
                {
                    d = DateTime.Now;
                }
                else
                {
                    d = Convert.ToDateTime(m.Module_LessonStartDate);
                }


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
                    await DisplayAlert("Insert Failure", "Failed to create records.", "ok");
                }
                //Insert success.
                else
                {
                    noClassesLbl.IsVisible = false;
                    LoadOneModuleClasses();
                }
            }
            //Table not created.
            else
            {
                await DisplayAlert("Create Failure", "Failed to create " + moduleCode + "'s class table.", "ok");
            }
        }
    }
}
