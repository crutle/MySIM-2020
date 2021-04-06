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
    namespace MySIM.Views.Students_Admin
    {
       [XamlCompilation(XamlCompilationOptions.Compile)]
       public partial class ManageAllStudents : ContentPage
       {
            public ManageAllStudents()
            {
               InitializeComponent();
            }
       }
    }
*/

namespace MySIM.Views.Students_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageAllStudents : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Users> studList = new List<Users>();

        private int currentSortOption, rowsAffected, rowsAffectedTwo = 0;

        public ManageAllStudents()
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
                Navigation.PushAsync(new AddOneStudent());
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to redirect to add new student: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void SearchBtn_Clicked(object sender, EventArgs args)
        {
            try
            {
                var sortByPicker = sortPicker.SelectedIndex;

                //To reset or for accidental click.
                if (string.IsNullOrWhiteSpace(searchField.Text))
                {
                    SetPageDefaultSettings();
                    LoadAllStudents();
                }

                //To search for student.
                else
                {
                    studList = GetSearchedStudentList(searchField.Text);

                    //No Results, display no user message.
                    if (studList.Count == 0)
                    {
                        noUserLbl.IsVisible = true;
                        studentList.ItemsSource = null;
                    }

                    //Have Results, display list of students.
                    else
                    {
                        //Sort results by option.
                        if (sortByPicker == 0)
                        {
                            noUserLbl.IsVisible = false;
                            studentList.ItemsSource = SortListByID(studList);
                        }
                        else
                        {
                            noUserLbl.IsVisible = false;
                            studentList.ItemsSource = SortListByName(studList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to search student: " + ex.Message + " (Contact Administrator)", "OK");
            }

        }

        protected void SortIndexChanged(object sender, EventArgs args)
        {
            try
            {
                var index = sortPicker.SelectedIndex;

                //Different sort option selected. 
                if (index != currentSortOption)
                {
                    //No search text inputted, to sort students by sort option selected.
                    if (string.IsNullOrWhiteSpace(searchField.Text))
                    {
                        studList = GetAllStudentList();
                    }

                    //Have search text inputted, to sort students by searched text and sort option.
                    else
                    {
                        studList = GetSearchedStudentList(searchField.Text);
                    }

                    //No results, display no student message.
                    if (studList.Count == 0)
                    {
                        sortPicker.SelectedIndex = 0;
                        noUserLbl.IsVisible = true;
                        studentList.ItemsSource = null;
                    }

                    //Have results, display list of searched students.
                    else
                    {
                        switch (index)
                        {
                            case 0:
                                //Sort list of student by student id.
                                studentList.ItemsSource = SortListByID(studList);
                                noUserLbl.IsVisible = false;
                                break;
                            case 1:
                                //Sort list of student by student last name.
                                studentList.ItemsSource = SortListByName(studList);
                                noUserLbl.IsVisible = false;
                                break;
                        }
                        //Update current selected option. 
                        currentSortOption = index;
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to sort list of students: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }
        protected void RefreshBtn_Clicked(object sender, EventArgs args)
        {
            SetPageDefaultSettings();
            LoadAllStudents();
        }

        protected void ListViewItem_Clicked(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                Navigation.PushAsync(new ViewEditOneStudent(((Users)e.SelectedItem).User_RecordID));
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to redirect to view student details: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected void StudentList_Refreshing(object sender, EventArgs args)
        {
            //Refresh page to initial load.
            SetPageDefaultSettings();
            LoadAllStudents();
            studentList.EndRefresh();
        }

        protected async void Delete_Clicked(object sender, EventArgs args)
        {
            try
            {
                int selectedUserID = int.Parse(((MenuItem)sender).CommandParameter.ToString());
                int studentRecordID = db.GetUserRecordID(selectedUserID);

                var response = await DisplayAlert("Confirm Deletion", "Delete student " + selectedUserID + "?", "OK", "Cancel");

                if (response)
                {
                    //Mark student record as deleted.
                    rowsAffected = db.DeleteOneStudent(studentRecordID, userData.UserRecordID);
                    if (rowsAffected != 0)
                    {
                        int count = db.CheckIfStudentCourseExists(studentRecordID);
                        if(count != 0)
                        {
                            rowsAffectedTwo = db.DeleteOneStudentCourse(studentRecordID, userData.UserRecordID);
                            if (rowsAffectedTwo != 0)
                            {
                                await DisplayAlert("Success", "Student " + selectedUserID + " has been successfully deleted.", "OK");
                                SetPageDefaultSettings();
                                LoadAllStudents();
                            }
                            else
                            {
                                await DisplayAlert("Failure", "Failed to delete student course.", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Success", "Student " + selectedUserID + " has been successfully deleted.", "OK");
                            SetPageDefaultSettings();
                            LoadAllStudents();
                        }
                    }
                    else
                    {
                        await DisplayAlert("Delete Failure", "Failed to delete student.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to delete student: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }



        private void Initialise()
        {
            //Check if user account type is admin.
            CheckIfAdminAccount();

            //Initialise page fields.
            SetPageDefaultSettings();

            //Get & load list of students.
            LoadAllStudents();
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

        //Set page to default setings.
        private void SetPageDefaultSettings()
        {
            searchField.Text = "";
            searchField.Completed += (s, e) => SearchBtn_Clicked(s, e);

            //Reset sortPicker.
            sortPicker.SelectedIndex = 0;

            //Reset fields.
            searchField.Text = "";
            noUserLbl.IsVisible = false;

            //Clear student list view.
            studentList.ItemsSource = null;
        }

        //Load list of students.
        private void LoadAllStudents()
        {
            try
            {
                //Sort list of student by student id.
                studList = GetAllStudentList();

                //No record, display no user message.
                if (studList.Count == 0)
                {
                    noUserLbl.IsVisible = true;
                }
                //Have records, display list of students.
                else
                {
                    noUserLbl.IsVisible = false;
                    studentList.ItemsSource = SortListByID(studList);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load list of students: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        private List<Users> GetAllStudentList()
        {
            return db.GetAllStudents();
        }

        private List<Users> GetSearchedStudentList(string inputString)
        {
            return db.GetSearchedStudent(inputString);
        }

        private List<Users> SortListByID(List<Users> list)
        {
            return list.OrderBy(x => x.User_ID).ToList();
        }

        private List<Users> SortListByName(List<Users> list)
        {
            return list.OrderBy(x => x.User_LastName).ToList();
        }
    }
}