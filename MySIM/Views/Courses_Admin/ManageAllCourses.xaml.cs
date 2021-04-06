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
    public partial class ManageAllCourses : ContentPage
    {
        public ManageAllCourses()
        {
            InitializeComponent();
        }
    }
} 
*/

namespace MySIM.Views.Courses_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageAllCourses : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Schools> uniList = new List<Schools>();
        private List<Courses> crseList = new List<Courses>();

        private string currentSortOption = "";
        private int rowsAffected, rowsAffectedTwo = 0;

        public ManageAllCourses()
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

        protected void AddBtn_Clicked(object sender, EventArgs args)
        {
            try
            {
                Navigation.PushAsync(new AddOneCourse());
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to redirect to add new course: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void SearchBtn_Clicked(object sender, EventArgs args)
        {
            try
            {
                var selectedUniName = (Schools)sortCoursePicker.SelectedItem;

                //To reset or for accidental click. 
                if (string.IsNullOrWhiteSpace(searchField.Text))
                {
                    SetPageDefaultSettings();
                    LoadAllCourses();
                }

                //To search for course.
                else
                {
                    //University name option is selected.
                    if (selectedUniName != null)
                    {
                        //Search for courses using search text and selected university name.
                        crseList = GetSearchedCourseList_FilteredByUniName(searchField.Text, selectedUniName.School_Name);

                        //No results, display no course message.
                        if (crseList.Count == 0)
                        {
                            noCourseLbl.IsVisible = true;
                            uniNameLbl.IsVisible = false;
                            courseList.ItemsSource = null;
                        }

                        //Have results, display list of searched courses for university name selected.
                        else
                        {
                            uniNameLbl.Text = selectedUniName.School_Name.ToString() + " Courses";
                            uniNameLbl.IsVisible = true;
                            noCourseLbl.IsVisible = false;
                            courseList.ItemsSource = crseList;
                        }
                    }
                    //University name option not selected.
                    else
                    {
                        crseList = GetSearchedCourseList(searchField.Text);

                        //No results, display no course message.
                        if (crseList.Count == 0)
                        {
                            noCourseLbl.IsVisible = true;
                            uniNameLbl.IsVisible = false;
                            courseList.ItemsSource = null;
                        }

                        //Have results, display list of searched courses.
                        else
                        {
                            noCourseLbl.IsVisible = false;
                            uniNameLbl.IsVisible = false;
                            courseList.ItemsSource = crseList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to search course: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void SortIndexChanged(object sender, EventArgs args)
        {
            try
            {
                var selectedUniName = (Schools)sortCoursePicker.SelectedItem;

                //Sort picker after reset or onload.
                if (selectedUniName == null)
                {
                    sortCoursePicker.SelectedItem = null;
                    uniNameLbl.IsVisible = false;
                }

                //Sort picker option selected.
                else
                {
                    //Different option selected.
                    if (!selectedUniName.School_Name.Equals(currentSortOption))
                    {
                        //No searched text inputted, to sort courses by uni name.
                        if (string.IsNullOrWhiteSpace(searchField.Text))
                        {
                            crseList = GetAllCourses_FilterByUniName(selectedUniName.School_Name);
                        }

                        //Have searched text inputted, to sort courses by searched text and uni name.
                        else
                        {
                            crseList = GetSearchedCourseList_FilteredByUniName(searchField.Text, selectedUniName.School_Name);
                        }

                        //No results, display no course message.
                        if (crseList.Count == 0)
                        {
                            noCourseLbl.IsVisible = true;
                            uniNameLbl.IsVisible = false;
                            courseList.ItemsSource = null;
                        }

                        //Have results, display list of courses.
                        else
                        {
                            noCourseLbl.IsVisible = false;
                            uniNameLbl.IsVisible = true;
                            uniNameLbl.Text = selectedUniName.School_Name.ToString() + " Courses";

                            courseList.ItemsSource = crseList;
                        }
                        //Update current selected option. 
                        currentSortOption = selectedUniName.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to sort by university: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void RefreshBtn_Clicked(object sender, EventArgs args)
        {
            SetPageDefaultSettings();
            LoadAllCourses();
        }

        protected void ListViewItem_Clicked(object sender, SelectedItemChangedEventArgs args)
        {
            try
            {
                Navigation.PushAsync(new ViewEditOneCourse(((Courses)args.SelectedItem).Course_RecordID));
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to redirect to view course details: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void CourseList_Refreshing(object sender, EventArgs args)
        {
            //Refresh page to initial load.
            SetPageDefaultSettings();
            LoadAllCourses();
            courseList.EndRefresh();
        }

        protected async void Delete_Clicked(object sender, EventArgs args)
        {
            try
            {
                int selectedCourseRecordID = Int32.Parse(((MenuItem)sender).CommandParameter.ToString());

                var response = await DisplayAlert("Confirm Deletion", "Delete course?", "OK", "Cancel");

                //OK selected.
                if (response)
                {
                    //1. Delete Courses record from database.
                    rowsAffected = db.DeleteOneCourse(selectedCourseRecordID, userData.UserRecordID);
                    if (rowsAffected == 1)
                    {
                        //2. Delete Schools_Courses record from database.
                        rowsAffectedTwo = db.DeleteOneSchoolCourse(selectedCourseRecordID, userData.UserRecordID);

                        if (rowsAffectedTwo == 1)
                        {
                            int count = db.CheckIfCourseModuleExists(selectedCourseRecordID);
                            //No modules to be removed.
                            if (count == 0)
                            {
                                await DisplayAlert("Success", "Course has been successfully deleted.", "OK");
                                SetPageDefaultSettings();
                                LoadAllCourses();
                            }
                            //3. Modules to be removed.
                            else
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    db.DeleteOneCourseModule(selectedCourseRecordID, userData.UserRecordID);
                                }
                                count = db.CheckIfCourseModuleExists(selectedCourseRecordID);
                                if (count == 0)
                                {
                                    await DisplayAlert("Success", "Course has been successfully deleted.", "OK");
                                    await Navigation.PopAsync();
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("Failure", "Failed to delete Course record under the selected university.", "OK");
                        }
                    }
                    //Cancel selected.
                    else
                    {
                        await DisplayAlert("Failure", "Failed to delete Course record.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to delete course: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }



        private void Initialise()
        {
            //Check if user account type is admin.
            CheckIfAdminAccount();

            //Initialise page controls.
            SetPageDefaultSettings();

            //Get & load list of courses.
            LoadAllCourses();
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

            //Reset sortCoursePicker.
            sortCoursePicker.SelectedItem = null;
            LoadPickerOptions();

            //Reset fields.
            searchField.Text = "";
            uniNameLbl.IsVisible = false;
            noCourseLbl.IsVisible = false;

            //Clear course list view.
            courseList.ItemsSource = null;
        }

        //Set picker options to list of available universities.
        private void LoadPickerOptions()
        {
            try
            {
                //Get list of university names
                uniList = GetUniList_SortedByName();
                if (uniList.Count != 0)
                {
                    sortCoursePicker.ItemsSource = uniList;
                    sortCoursePicker.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load school picker options: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        private List<Schools> GetUniList_SortedByName()
        {
            return db.GetAllSchools().OrderBy(x => x.School_Name).ToList();
        }

        //Get & load list of courses.
        private void LoadAllCourses()
        {
            try
            {
                //Get list of all courses.
                crseList = GetAllCourses();

                //No record, display no courses message.
                if (crseList.Count == 0)
                {
                    noCourseLbl.IsVisible = true;
                    uniNameLbl.IsVisible = false;
                }
                //Have records, display list of courses.
                else
                {
                    noCourseLbl.IsVisible = false;
                    uniNameLbl.IsVisible = false;
                    courseList.ItemsSource = crseList;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load list of courses: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        private List<Courses> GetAllCourses()
        {
            return db.GetAllCourses().OrderBy(x => x.Course_Name).ToList();
        }

        private List<Courses> GetAllCourses_FilterByUniName(string selectedUniName)
        {
            return db.GetCoursesBySchools(selectedUniName).OrderBy(x => x.Course_Name).ToList();
        }

        private List<Courses> GetSearchedCourseList(string inputString)
        {
            return db.GetSearchedCourse(inputString);
        }

        private List<Courses> GetSearchedCourseList_FilteredByUniName(string searchedString, string selectedUniName)
        {
            return db.GetSearchedAndFilteredCourses(searchedString, selectedUniName).OrderBy(x => x.Course_Name).ToList();
        }
    }
}


