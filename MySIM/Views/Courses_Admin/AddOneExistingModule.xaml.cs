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
namespace MySIM.Views.Courses_Admin
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

namespace MySIM.Views.Courses_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddOneExistingModule : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Schools> uniList = new List<Schools>();
        private List<Courses> crseList = new List<Courses>();
        private List<Modules> modList = new List<Modules>();
        private static int courseRecordID;

        private int rowsAffected = 0;
        private string currentUniOption, currentCrseOption;

        public AddOneExistingModule(int recordID)
        {
            InitializeComponent();

            courseRecordID = recordID;

            //Initialise & Load Page.
            Initialise();
        }
        
        protected void SearchBtn_Clicked(object sender, EventArgs args)
        {
            string searchStr = searchField.Text, uniName, crseName;
            int exists, crseRecordID, schRecordID;

            if (sortByUniPicker.SelectedItem == null)
            {
                uniName = "";
            }
            else
            {
                uniName = ((Schools)sortByUniPicker.SelectedItem).School_Name;
            }

            if (sortByCoursePicker.SelectedItem == null)
            {
                crseName = "";
            }
            else
            {
                crseName = ((Courses)sortByCoursePicker.SelectedItem).Course_Name;
            }

            //Only searched text.
            if (string.IsNullOrWhiteSpace(searchStr))
            {
                SetPageDefaultSettings();
                LoadAllModules();
            }
            else
            {
                if(uniName != "" && crseName != "")
                {
                    crseRecordID = db.GetCourseRecordID(crseName);
                    schRecordID = db.GetSchoolRecordID(uniName);
                    exists = db.CheckIfSchoolCourseExists(crseRecordID, schRecordID);

                    if(exists == 0)
                    {

                        modList = GetSearchedModuleList_FilterByUniCourse(searchStr, uniName, "");
                    }
                    else
                    {
                        modList = GetSearchedModuleList_FilterByUniCourse(searchStr, uniName, crseName);
                    }
                }
                else
                {
                    modList = GetSearchedModuleList_FilterByUniCourse(searchStr, uniName, crseName);
                }   
                

                if (modList.Count == 0)
                {
                    noModulesLbl.IsVisible = true;
                    uniNameLbl.IsVisible = false;
                    courseNameLbl.IsVisible = false;
                    moduleList.ItemsSource = null;
                }
                else
                {
                    //Display results for searched text.
                    if (uniName == "" && crseName == "")
                    {
                        noModulesLbl.IsVisible = false;
                        uniNameLbl.IsVisible = false;
                        courseNameLbl.IsVisible = false;
                    }
                    //Display results for searched text & uniName.
                    else if (uniName != "" && crseName == "")
                    {
                        noModulesLbl.IsVisible = false;
                        uniNameLbl.IsVisible = true;
                        uniNameLbl.Text = uniName;
                        courseNameLbl.IsVisible = false;
                    }
                    //Display results for searched text & crseName.
                    else if (uniName == "" && crseName != "")
                    {
                        noModulesLbl.IsVisible = false;
                        uniNameLbl.IsVisible = false;
                        courseNameLbl.IsVisible = true;
                        courseNameLbl.Text = crseName + " Modules";
                    }
                    //Display results for searched text & crseName & uniName.
                    else
                    { 
                        noModulesLbl.IsVisible = false;
                        uniNameLbl.IsVisible = true;
                        uniNameLbl.Text = uniName;
                        courseNameLbl.IsVisible = true;
                        courseNameLbl.Text = crseName + " Modules";       
                    }

                    moduleList.ItemsSource = modList;
                }
            }
        }

        protected async void SortUniversityIndexChanged(object sender, EventArgs args)
        {
            string selectedUniName, searchStr, crseName;
            int crseRecordID, schRecordID, exists;

            try
            {
                searchStr = searchField.Text;
                //Check & Set University picker's selection.
                if (sortByUniPicker.SelectedItem == null)
                {
                    selectedUniName = "";
                }
                else
                {
                    selectedUniName = ((Schools)sortByUniPicker.SelectedItem).School_Name;
                }

                //Check & Set Course picker's selection.
                if (sortByCoursePicker.SelectedItem == null)
                {
                    crseName = "";
                }
                else
                {
                    crseName = ((Courses)sortByCoursePicker.SelectedItem).Course_Name;
                }

                //Check if course belongs to uni.
                if(!string.IsNullOrEmpty(selectedUniName) && !string.IsNullOrEmpty(crseName))
                {
                    crseRecordID = db.GetCourseRecordID(crseName);
                    schRecordID = db.GetSchoolRecordID(selectedUniName);
                    exists = db.CheckIfSchoolCourseExists(crseRecordID, schRecordID);

                    //Update module list.
                    if (!selectedUniName.Equals(currentUniOption))
                    { 
                        //Use university & text to search. (course does not belong to university)
                        if(exists == 0)
                        {
                            modList = GetSearchedModuleList_FilterByUniCourse(searchStr, selectedUniName, "");
                            if (modList.Count == 0)
                            {
                                noModulesLbl.IsVisible = true;
                                uniNameLbl.IsVisible = false;
                                courseNameLbl.IsVisible = false;
                                moduleList.ItemsSource = null;
                            }
                            else
                            {
                                noModulesLbl.IsVisible = false;
                                uniNameLbl.IsVisible = true;
                                uniNameLbl.Text = selectedUniName + " Modules";
                                courseNameLbl.IsVisible = false;
                                moduleList.ItemsSource = modList;
                            }
                        }
                        //Use university & text & course to search. (course belongs to university.)
                        else
                        {
                            modList = GetSearchedModuleList_FilterByUniCourse(searchStr, selectedUniName, crseName);
                            if (modList.Count == 0)
                            {
                                noModulesLbl.IsVisible = true;
                                uniNameLbl.IsVisible = false;
                                courseNameLbl.IsVisible = false;
                                moduleList.ItemsSource = null;
                            }
                            else
                            {
                                noModulesLbl.IsVisible = false;
                                uniNameLbl.IsVisible = true;
                                uniNameLbl.Text = selectedUniName;
                                courseNameLbl.IsVisible = true;
                                courseNameLbl.Text = crseName + " Modules";
                                moduleList.ItemsSource = modList;

                            }
                        }
                        currentUniOption = selectedUniName;
                    }
                }
                //course or university is null.
                else
                {
                    if (!selectedUniName.Equals(currentUniOption))
                    {
                        modList = GetSearchedModuleList_FilterByUniCourse(searchStr, selectedUniName, crseName);
                        if (modList.Count == 0)
                        {
                            noModulesLbl.IsVisible = true;
                            uniNameLbl.IsVisible = false;
                            courseNameLbl.IsVisible = false;
                            moduleList.ItemsSource = null;
                        }
                        else
                        {
                            noModulesLbl.IsVisible = false;
                            
                            if (selectedUniName != "" && crseName != "")
                            {
                                uniNameLbl.IsVisible = true;
                                uniNameLbl.Text = selectedUniName;
                                courseNameLbl.IsVisible = true;
                                courseNameLbl.Text = crseName + " Modules";                              
                            }
                            else if(selectedUniName == "" && crseName != "")
                            {
                                uniNameLbl.IsVisible = false;
                                courseNameLbl.IsVisible = true;
                                courseNameLbl.Text = crseName + " Modules";
                            }
                            else if (selectedUniName != "" && crseName == "")
                            {
                                uniNameLbl.IsVisible = true;
                                uniNameLbl.Text = selectedUniName + " Modules";
                                courseNameLbl.IsVisible = false;
                            }
                            else
                            {
                                uniNameLbl.IsVisible = false;
                            }

                            moduleList.ItemsSource = modList;
                        }
                        currentUniOption = selectedUniName;
                    }
                }
                LoadSortCoursePickerOptions(selectedUniName);                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to handle sort university index: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected async void SortCourseIndexChanged(object sender, EventArgs args)
        {
            string selectedCrseName = "", searchStr, uniName = "";
            int crseRecordID, schRecordID, exists;

            try
            {
                searchStr = searchField.Text;
                //Check & Set University picker's selection.
                if (sortByUniPicker.SelectedItem != null)
                {
                    uniName = ((Schools)sortByUniPicker.SelectedItem).School_Name;
                }

                //Check & Set Course picker's selection.
                if (sortByCoursePicker.SelectedItem != null)
                {
                    selectedCrseName = ((Courses)sortByCoursePicker.SelectedItem).Course_Name;
                }
               

                //Check if course belongs to uni.
                if (!string.IsNullOrEmpty(selectedCrseName) && !string.IsNullOrEmpty(uniName))
                {
                    crseRecordID = db.GetCourseRecordID(selectedCrseName);
                    schRecordID = db.GetSchoolRecordID(uniName);
                    exists = db.CheckIfSchoolCourseExists(crseRecordID, schRecordID);

                    //Update module list.
                    if (!selectedCrseName.Equals(currentCrseOption))
                    {
                        //Use course & text to search. (course does not belong to university)
                        if (exists == 0)
                        {
                            modList = GetSearchedModuleList_FilterByUniCourse(searchStr, "", selectedCrseName);
                            if (modList.Count == 0)
                            {
                                noModulesLbl.IsVisible = true;
                                uniNameLbl.IsVisible = false;
                                courseNameLbl.IsVisible = false;
                                moduleList.ItemsSource = null;
                            }
                            else
                            {
                                noModulesLbl.IsVisible = false;
                                uniNameLbl.IsVisible = false;
                                courseNameLbl.IsVisible = true;
                                courseNameLbl.Text = selectedCrseName + " Modules";
                                moduleList.ItemsSource = modList;
                            }
                        }
                        //Use university & text & course to search. (course belongs to university.)
                        else
                        {
                            modList = GetSearchedModuleList_FilterByUniCourse(searchStr, uniName, selectedCrseName);
                            if (modList.Count == 0)
                            {
                                noModulesLbl.IsVisible = true;
                                uniNameLbl.IsVisible = false;
                                courseNameLbl.IsVisible = false;
                                moduleList.ItemsSource = null;
                            }
                            else
                            {
                                noModulesLbl.IsVisible = false;
                                uniNameLbl.IsVisible = true;
                                uniNameLbl.Text = uniName;
                                courseNameLbl.IsVisible = true;
                                courseNameLbl.Text = selectedCrseName + " Modules";
                                moduleList.ItemsSource = modList;

                            }
                        }
                        currentCrseOption = selectedCrseName;
                    }
                }

                //course or university is null.
                else
                {
                    if (!selectedCrseName.Equals(currentCrseOption))
                    {
                        modList = GetSearchedModuleList_FilterByUniCourse(searchStr, uniName, selectedCrseName);
                        if (modList.Count == 0)
                        {
                            noModulesLbl.IsVisible = true;
                            uniNameLbl.IsVisible = false;
                            courseNameLbl.IsVisible = false;
                            moduleList.ItemsSource = null;
                        }
                        else
                        {
                            noModulesLbl.IsVisible = false;

                            if (uniName != "" && selectedCrseName != "")
                            {
                                uniNameLbl.IsVisible = true;
                                uniNameLbl.Text = uniName;
                                courseNameLbl.IsVisible = true;
                                courseNameLbl.Text = selectedCrseName + " Modules";
                                moduleList.ItemsSource = modList;
                            }
                            else if (uniName == "" && selectedCrseName != "")
                            {
                                uniNameLbl.IsVisible = false;
                                courseNameLbl.IsVisible = true;
                                courseNameLbl.Text = selectedCrseName + " Modules";
                                moduleList.ItemsSource = modList;
                            }
                            else if (uniName != "" && selectedCrseName == "")
                            {
                                uniNameLbl.IsVisible = true;
                                uniNameLbl.Text = uniName + " Modules";
                                courseNameLbl.IsVisible = false;
                                moduleList.ItemsSource = modList;
                            }
                            else
                            {
                                uniNameLbl.IsVisible = false;
                                moduleList.ItemsSource = modList;
                            }
                                                       
                        }
                        currentCrseOption = selectedCrseName;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to handle sort course index: " + ex.Message + " (Contact Administrator)", "OK");
            }

            /*
        string selectedCrseName, searchStr, uniName;
        int crseRecordID, schRecordID, exists;

        try
        {
            searchStr = searchField.Text;
            //Check & Set University picker's selection.
            if (sortByUniPicker.SelectedItem == null)
            {
                uniName = "";
            }
            else
            {
                uniName = ((Schools)sortByUniPicker.SelectedItem).School_Name;
            }

            //Check & Set Course picker's selection.
            if (sortByCoursePicker.SelectedItem == null)
            {
                selectedCrseName = "";
            }
            else
            {
                selectedCrseName = ((Courses)sortByCoursePicker.SelectedItem).Course_Name;
            }

            //Check if course belongs to uni.
            if (!string.IsNullOrEmpty(selectedCrseName) && !string.IsNullOrEmpty(uniName))
            {
                crseRecordID = db.GetCourseRecordID(selectedCrseName);
                schRecordID = db.GetSchoolRecordID(uniName);
                exists = db.CheckIfSchoolCourseExists(crseRecordID, schRecordID);

                //Update module list.
                if (!selectedCrseName.Equals(currentUniOption))
                {
                    //Use course & text to search. (course does not belong to university)
                    if (exists == 0)
                    {
                        modList = GetSearchedModuleList_FilterByUniCourse(searchStr, "", selectedCrseName);
                        if (modList.Count == 0)
                        {
                            noModulesLbl.IsVisible = true;
                            uniNameLbl.IsVisible = false;
                            courseNameLbl.IsVisible = false;
                            moduleList.ItemsSource = null;
                        }
                        else
                        {
                            noModulesLbl.IsVisible = false;
                            uniNameLbl.IsVisible = false;
                            courseNameLbl.IsVisible = true;
                            courseNameLbl.Text = selectedCrseName + " Modules";
                            moduleList.ItemsSource = modList;
                        }
                    }
                    //Use university & text & course to search. (course belongs to university.)
                    else
                    {
                        modList = GetSearchedModuleList_FilterByUniCourse(searchStr, uniName, selectedCrseName);
                        if (modList.Count == 0)
                        {
                            noModulesLbl.IsVisible = true;
                            uniNameLbl.IsVisible = false;
                            courseNameLbl.IsVisible = false;
                            moduleList.ItemsSource = null;
                        }
                        else
                        {
                            noModulesLbl.IsVisible = false;
                            uniNameLbl.IsVisible = true;
                            uniNameLbl.Text = uniName;
                            courseNameLbl.IsVisible = true;
                            courseNameLbl.Text = selectedCrseName + " Modules";
                            moduleList.ItemsSource = modList;

                        }
                    }
                    currentCrseOption = selectedCrseName;
                }
            }
            //course or university is null.
            else
            {
                if (!selectedCrseName.Equals(currentCrseOption))
                {
                    modList = GetSearchedModuleList_FilterByUniCourse(searchStr, uniName, selectedCrseName);
                    if (modList.Count == 0)
                    {
                        noModulesLbl.IsVisible = true;
                        uniNameLbl.IsVisible = false;
                        courseNameLbl.IsVisible = false;
                        moduleList.ItemsSource = null;
                    }
                    else
                    {
                        noModulesLbl.IsVisible = false;

                        if (uniName != "" && selectedCrseName != "")
                        {
                            uniNameLbl.IsVisible = true;
                            uniNameLbl.Text = uniName;
                            courseNameLbl.IsVisible = true;
                            courseNameLbl.Text = selectedCrseName + " Modules";
                        }
                        else if (uniName == "" && selectedCrseName != "")
                        {
                            uniNameLbl.IsVisible = false;
                            courseNameLbl.IsVisible = true;
                            courseNameLbl.Text = selectedCrseName + " Modules";
                        }
                        else if (uniName != "" && selectedCrseName == "")
                        {
                            uniNameLbl.IsVisible = true;
                            uniNameLbl.Text = uniName + " Modules";
                            courseNameLbl.IsVisible = false;
                        }
                        else
                        {
                            uniNameLbl.IsVisible = false;
                        }

                        moduleList.ItemsSource = modList;
                    }
                    currentCrseOption = selectedCrseName;
                }
            }
            LoadSortCoursePickerOptions("");

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to handle sort course index: " + ex.Message + " (Contact Administrator)", "OK");
        }
             */
        }

        protected void RefreshBtn_Clicked(object sender, EventArgs args)
        {
            SetPageDefaultSettings();
            LoadAllModules();
        }

        protected async void ListViewItem_Clicked(object sender, SelectedItemChangedEventArgs args)
        {
            try
            {
                var selectedMod = (Modules)args.SelectedItem;
                string crseName = db.GetCourseName(courseRecordID);

                var response = await DisplayAlert("Confirm Addition", "Add " + selectedMod.Module_Code + " " + selectedMod.Module_Name + " to " + crseName + "?", "OK", "Cancel");

                //OK selected.
                if (response)
                {
                    int count = db.CheckIfCourseModuleExists(selectedMod.Module_RecordID, courseRecordID);
                    //Module added to course.
                    if (count == 1)
                    {
                        await DisplayAlert("Failure", "Failed to link module to course. Module has already been added to course.", "OK");
                    }
                    else
                    {
                        //Add Module to Course.
                        rowsAffected = db.AddOneCourseModule(courseRecordID, selectedMod.Module_RecordID, userData.UserRecordID);
                        if (rowsAffected == 1)
                        {
                            await DisplayAlert("Success", "Module has been linked to course successfully.", "OK");
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert("Failure", "Failed to link module to course.", "OK");
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to link module: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void ModuleList_Refreshing(object sender, EventArgs args)
        {
            SetPageDefaultSettings();
            LoadAllModules();
            moduleList.EndRefresh();
        }



        private void Initialise()
        {
            //Check if user account type is admin.
            CheckIfAdminAccount();

            //Initialise page controls.
            SetPageDefaultSettings();

            //Get & load existing modules.
            LoadAllModules();
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

            //Reset pickers.
            LoadSortUniPickerOptions();
            LoadSortCoursePickerOptions("");

            //Reset fields.
            currentUniOption = "";
            currentCrseOption = "";
            noModulesLbl.IsVisible = false;
            uniNameLbl.IsVisible = false;
            courseNameLbl.IsVisible = false;

            //Reset existing module list view.
            moduleList.ItemsSource = null;
        }

        //Set sort picker options to list of available universities.
        private void LoadSortUniPickerOptions()
        {
            List<Schools> filteredList = new List<Schools>();
            try
            {
                //Get list of university names
                uniList = GetUniversityList();
                if (uniList.Count != 0)
                {
                    foreach (Schools s in uniList)
                    {
                        int count = db.CheckIfUniHasCourses(s.School_RecordID);
                        if (count != 0)
                        {
                            filteredList.Add(s);
                        }
                    }

                    sortByUniPicker.ItemsSource = filteredList;
                    sortByUniPicker.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load sort picker(university) options: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        //Set sort picker options to list of courses.
        private void LoadSortCoursePickerOptions(string uni)
        {
            try
            {
                sortByCoursePicker.ItemsSource = null;

                if (string.IsNullOrEmpty(uni))
                {
                    crseList = GetCourseList();
                }
                else
                {
                    crseList = GetCourseList_FilterByUniName(uni);
                }

                if (crseList.Count != 0)
                {
                    sortByCoursePicker.ItemsSource = crseList;
                    sortByCoursePicker.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load sort picker(courses) options: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        //Get & load existing modules.
        private void LoadAllModules()
        {
            try
            {
                //Get list of all modules.
                modList = GetAllModules();

                //No record, display no courses message.
                if (modList.Count == 0)
                {
                    noModulesLbl.IsVisible = true;
                    uniNameLbl.IsVisible = false;
                    courseNameLbl.IsVisible = false;
                }
                //Have records, display list of courses.
                else
                {
                    noModulesLbl.IsVisible = false;
                    uniNameLbl.IsVisible = false;
                    courseNameLbl.IsVisible = false;
                    moduleList.ItemsSource = modList;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load list of all modules: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        private List<Modules> GetAllModules()
        {
            return db.GetAllModules().OrderBy(x => x.Module_Code).ToList();
        }

        private List<Schools> GetUniversityList()
        {
            return db.GetAllSchools().OrderBy(x => x.School_Name).ToList();
        }

        private List<Courses> GetCourseList()
        {
            return db.GetAllCourses().OrderBy(x => x.Course_Name).ToList();
        }

        private List<Courses> GetCourseList_FilterByUniName(string selectedUniName)
        {
            return db.GetCoursesBySchools(selectedUniName).OrderBy(x => x.Course_Name).ToList();
        }

        private List<Modules> GetSearchedModuleList_FilterByUniCourse(string searchedString, string selectedUniName, string selectedCourseName)
        {
            return db.GetSearchedAndFilteredModules(selectedUniName, selectedCourseName, searchedString).OrderBy(x => x.Module_Code).ToList();
        }
    }
}