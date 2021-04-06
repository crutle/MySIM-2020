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
    public partial class ViewEditOneStudent : TabbedPage
    {
        public ViewEditOneStudent()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Students_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewEditOneStudent : TabbedPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private readonly List<Schools> filteredList = new List<Schools>();
        private List<Schools> uniList = new List<Schools>();
        private List<Courses> crseList = new List<Courses>();
        private Users student, studentCourse = new Users();
        private static int userRecordID, schoolRecordID, courseRecordID;

        private string currentUniOption;
        private int rowsAffected, rowsAffectedTwo = 0;

        public ViewEditOneStudent(int recordID)
        {
            InitializeComponent();

            userRecordID = recordID;

            //Initialise & Load Page
            Initialise();

            //Create Modules Tab
            ViewEnrolledModules vm = new ViewEnrolledModules(userRecordID, schoolRecordID, courseRecordID);
            Children.Add(vm);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Initialise();
        }

        protected void EditBtn_Clicked(object sender, EventArgs args)
        {
            if (toolbarEdit.Text.Equals("Save"))
            {
                SaveBtn_Clicked(sender, args);
            }

            fName.IsEnabled = true;
            lName.IsEnabled = true;
            pwd.IsEnabled = true;

            editBtn.IsVisible = false;
            deleteBtn.IsVisible = true;
            saveBtn.IsVisible = true;
            cancelEditBtn.IsVisible = true;

            schoolPicker.IsEnabled = true;
            coursePicker.IsEnabled = true;

            toolbarEdit.Text = "Save";
        }

        protected async void UniversityIndexChanged(object sender, EventArgs args)
        {
            string selectedUniName;
            try
            {
                if (schoolPicker.SelectedItem != null)
                {
                    selectedUniName = ((Schools)schoolPicker.SelectedItem).School_Name;

                    //Different university option selected. 
                    if (!selectedUniName.Equals(currentUniOption) && selectedUniName != null)
                    {
                        LoadCoursePickerOptions(selectedUniName);
                        currentUniOption = selectedUniName;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to handle load course by university: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        protected async void SaveBtn_Clicked(object sender, EventArgs args)
        {
            if (schoolPicker.SelectedItem == null)
            {
                DisplayAlert("", "Please select a university to enroll student under.", "OK");
            }
            else if (coursePicker.SelectedItem == null)
            {
                DisplayAlert("", "Please select a course to enroll student under.", "OK");
            }
            else if (string.IsNullOrWhiteSpace(fName.Text) && string.IsNullOrWhiteSpace(lName.Text) && string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = true;
                lNameError.IsVisible = true;
                pwdError.IsVisible = true;
            }
            else if (!string.IsNullOrWhiteSpace(fName.Text) && string.IsNullOrWhiteSpace(lName.Text) && string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = false;
                lNameError.IsVisible = true;
                pwdError.IsVisible = true;
            }
            else if (string.IsNullOrWhiteSpace(fName.Text) && !string.IsNullOrWhiteSpace(lName.Text) && string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = true;
                lNameError.IsVisible = false;
                pwdError.IsVisible = true;
            }
            else if (string.IsNullOrWhiteSpace(fName.Text) && string.IsNullOrWhiteSpace(lName.Text) && !string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = true;
                lNameError.IsVisible = true;
                pwdError.IsVisible = false;
            }
            else if (!string.IsNullOrWhiteSpace(fName.Text) && !string.IsNullOrWhiteSpace(lName.Text) && string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = false;
                lNameError.IsVisible = false;
                pwdError.IsVisible = true;
            }
            else if (!string.IsNullOrWhiteSpace(fName.Text) && string.IsNullOrWhiteSpace(lName.Text) && !string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = false;
                lNameError.IsVisible = true;
                pwdError.IsVisible = false;
            }
            else if (string.IsNullOrWhiteSpace(fName.Text) && !string.IsNullOrWhiteSpace(lName.Text) && !string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = true;
                lNameError.IsVisible = false;
                pwdError.IsVisible = false;
            }          
            else
            {
                try
                {
                    Users u = new Users
                    {
                        User_RecordID = userRecordID,
                        User_FirstName = fName.Text,
                        User_LastName = lName.Text,
                        User_PUID = studPUID.Text,
                        User_LoginPwd = pwd.Text,
                        Student_SchoolRecordID = ((Schools)schoolPicker.SelectedItem).School_RecordID,
                        Student_CourseRecordID = ((Courses)coursePicker.SelectedItem).Course_RecordID,
                        EditedBy = userData.UserRecordID
                    };

                    int count = db.CheckIfStudentExists(userRecordID);
                    if (count != 0)
                    {
                        //Update student record.
                        rowsAffected = db.UpdateOneStudent(u);
                        if (rowsAffected == 1)
                        {
                            count = db.CheckIfStudentCourseExists(userRecordID);
                            if (count != 0)
                            {
                                rowsAffectedTwo = db.UpdateOneStudentCourse(userRecordID, u.Student_CourseRecordID, u.Student_SchoolRecordID, userData.UserRecordID);
                                if (rowsAffectedTwo != 0)
                                {
                                    await DisplayAlert("Success", "Student " + student.User_ID + " has been updated successfully.", "OK");
                                    //Load student details.
                                    SetPageDefaultSettings();
                                    LoadOneStudent();
                                }
                                else
                                {
                                    await DisplayAlert("Failure", "Failed to update student course.", "OK");
                                }
                            }
                            else
                            {
                                await DisplayAlert("Failure", "Student course does not exist.", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Failure", "Failed to update student record.", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Edit Failure", "Failed to update student. Student does not exist.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Failed to update student: " + ex.Message + " (Contact Administrator)", "OK");
                }
            }
        }

        protected void CancelEditBtn_Clicked(object sender, EventArgs args)
        {
            SetPageDefaultSettings();
            LoadOneStudent();
        }

        protected async void DeleteBtn_Clicked(object sender, EventArgs args)
        {
            try
            {
                var response = await DisplayAlert("Confirm Deletion", "Delete student " + student.User_ID + "?", "OK", "Cancel");

                //OK selected
                if (response)
                {
                    //Delete student record from database.
                    rowsAffected = db.DeleteOneStudent(student.User_RecordID, userData.UserRecordID);
                    if (rowsAffected != 0)
                    {
                        rowsAffectedTwo = db.DeleteOneStudentCourse(student.User_RecordID, userData.UserRecordID);
                        if (rowsAffectedTwo != 0)
                        {
                            await DisplayAlert("Success", "Student " + student.User_ID + " has been successfully deleted.", "OK");
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert("Failure", "Failed to delete student course.", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Failure", "Failed to delete student.", "OK");
                    }
                }
                //Cancel selected.
                else
                {
                    //Load student details.
                    SetPageDefaultSettings();
                    LoadOneStudent();
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

            //Initialise page controls.
            SetPageDefaultSettings();

            //Get & load student details.
            LoadOneStudent();

            Title = student.User_ID.ToString();
            SelectedTabColor = Color.White;
            UnselectedTabColor = Color.Black;
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
        private void SetPageDefaultSettings()
        {
            schoolPicker.IsEnabled = false;
            coursePicker.IsEnabled = false;

            //Reset fields.
            studNum.Text = "";
            studPUID.Text = "";
            loginID.Text = "";
            email.Text = "";

            fName.Text = "";
            lName.Text = "";
            pwd.Text = "";
            fName.IsEnabled = false;
            lName.IsEnabled = false;        
            pwd.IsEnabled = false;

            fNameError.IsVisible = false;
            lNameError.IsVisible = false;
            pwdError.IsVisible = false;

            toolbarEdit.IsEnabled = true;
            toolbarEdit.Text = "Edit";

            editBtn.IsVisible = true;
            deleteBtn.IsVisible = false;
            saveBtn.IsVisible = false;
            cancelEditBtn.IsVisible = false;


            fName.Completed += (s, e) => lName.Focus();
            lName.Completed += (s, e) => pwd.Focus();
            pwd.Completed += (s, e) => SaveBtn_Clicked(s, e);
        }

        //Load one student details.
        private void LoadOneStudent()
        {
            try
            {
                //Get student details from database.
                student = db.GetOneStudent(userRecordID);

                studNum.Text = student.User_ID.ToString();
                studPUID.Text = student.User_PUID.ToString();
                loginID.Text = student.User_LoginID;
                email.Text = student.User_Email;

                fName.Text = student.User_FirstName;
                lName.Text = student.User_LastName;
                pwd.Text = student.User_LoginPwd;

                studentCourse = db.GetOneStudentCourse(userRecordID);
                LoadSchoolPickerOptions();
                SetCoursePickerOptions();

                schoolRecordID = studentCourse.Student_SchoolRecordID;
                courseRecordID = studentCourse.Student_CourseRecordID;
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load student details: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        //Set picker options to list of universities.
        private void LoadSchoolPickerOptions()
        {
            try
            {
                //Get list of university names.
                uniList = GetUniList_SortedByName();
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
                    schoolPicker.ItemsSource = filteredList;

                    for (int i = 0; i < filteredList.Count; i++)
                    {
                        if (filteredList[i].School_RecordID.Equals(studentCourse.Student_SchoolRecordID))
                        {
                            schoolPicker.SelectedIndex = i;
                            break;
                        }
                    }
                    currentUniOption = db.GetSchoolName(studentCourse.Student_SchoolRecordID);
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

        //Set picker options to list of courses.
        private void LoadCoursePickerOptions(string uniName)
        {
            try
            {
                if (uniName == "" || uniName == null)
                {
                    //Get list of course names.
                    crseList = GetCrseList_SortedByName();
                }
                else
                {
                    crseList = GetCrseList_SortedByUni(uniName);
                    coursePicker.SelectedItem = null;
                }

                coursePicker.ItemsSource = crseList;
                coursePicker.SelectedItem = null;

            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load course picker options: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        //Set picker options to enrolled course.
        private void SetCoursePickerOptions()
        {
            try
            {
                crseList = GetCrseList_SortedByUni(currentUniOption);
                coursePicker.ItemsSource = crseList;

                for (int i = 0; i < crseList.Count; i++)
                {
                    if (crseList[i].Course_RecordID.Equals(studentCourse.Student_CourseRecordID))
                    {
                        coursePicker.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load default course picker options: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        private List<Courses> GetCrseList_SortedByName()
        {
            return db.GetAllCourses().OrderBy(x => x.Course_Name).ToList();
        }
        private List<Courses> GetCrseList_SortedByUni(string name)
        {
            return db.GetCoursesBySchools(name).OrderBy(x => x.Course_Name).ToList();
        }
    }
}
