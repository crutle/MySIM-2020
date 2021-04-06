using MySIM.Models;
using MySIM.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/* Visual Studio Generated Skeleton Code: 
    namespace MySIM.Views.Students_Admin
    {
        [XamlCompilation(XamlCompilationOptions.Compile)]
        public partial class AddOneStudent : ContentPage
        {
            public AddOneStudent()
            {
                 InitializeComponent();
            }
        }
     }
 */

namespace MySIM.Views.Students_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddOneStudent : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private readonly List<Schools> filteredList = new List<Schools>();
        private List<Schools> uniList = new List<Schools>();
        private List<Courses> crseList = new List<Courses>();

        private int rowsAffected, rowsAffectedTwo, idCount = 0;
        private string currentUniOption;
        private string generatedFirstName, generatedLastName, generatedLoginID, finalLoginID;

        public AddOneStudent()
        {
            InitializeComponent();
            //Initialise & Load Page
            Initialise();
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

        protected void SaveBtn_Clicked(object sender, EventArgs args)
        {
            if (schoolPicker.SelectedItem == null)
            {
                DisplayAlert("", "Please select a university to enroll student under.", "OK");
            }
            else if (coursePicker.SelectedItem == null)
            {
                DisplayAlert("", "Please select a course to enroll student under.", "OK");
            }
            else if (string.IsNullOrWhiteSpace(fName.Text) && string.IsNullOrWhiteSpace(lName.Text) && string.IsNullOrWhiteSpace(studID.Text) && string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = true;
                lNameError.IsVisible = true;
                studIDError.IsVisible = true;
                pwdError.IsVisible = true;
            }
            else if (!string.IsNullOrWhiteSpace(fName.Text) && string.IsNullOrWhiteSpace(lName.Text) && string.IsNullOrWhiteSpace(studID.Text) && string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = false;
                lNameError.IsVisible = true;
                studIDError.IsVisible = true;
                pwdError.IsVisible = true;
            }
            else if (string.IsNullOrWhiteSpace(fName.Text) && !string.IsNullOrWhiteSpace(lName.Text) && string.IsNullOrWhiteSpace(studID.Text) && string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = true;
                lNameError.IsVisible = false;
                studIDError.IsVisible = true;
                pwdError.IsVisible = true;
            }
            else if (string.IsNullOrWhiteSpace(fName.Text) && string.IsNullOrWhiteSpace(lName.Text) && !string.IsNullOrWhiteSpace(studID.Text) && string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = true;
                lNameError.IsVisible = true;
                studIDError.IsVisible = false;
                pwdError.IsVisible = true;
            }
            else if (string.IsNullOrWhiteSpace(fName.Text) && string.IsNullOrWhiteSpace(lName.Text) && string.IsNullOrWhiteSpace(studID.Text) && !string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = true;
                lNameError.IsVisible = true;
                studIDError.IsVisible = true;
                pwdError.IsVisible = false;
            }

            else if (!string.IsNullOrWhiteSpace(fName.Text) && !string.IsNullOrWhiteSpace(lName.Text) && string.IsNullOrWhiteSpace(studID.Text) && string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = false;
                lNameError.IsVisible = false;
                studIDError.IsVisible = true;
                pwdError.IsVisible = true;
            }
            else if (!string.IsNullOrWhiteSpace(fName.Text) && string.IsNullOrWhiteSpace(lName.Text) && !string.IsNullOrWhiteSpace(studID.Text) && string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = false;
                lNameError.IsVisible = true;
                studIDError.IsVisible = false;
                pwdError.IsVisible = true;
            }
            else if (!string.IsNullOrWhiteSpace(fName.Text) && string.IsNullOrWhiteSpace(lName.Text) && string.IsNullOrWhiteSpace(studID.Text) && !string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = false;
                lNameError.IsVisible = true;
                studIDError.IsVisible = true;
                pwdError.IsVisible = false;
            }

            else if (string.IsNullOrWhiteSpace(fName.Text) && !string.IsNullOrWhiteSpace(lName.Text) && !string.IsNullOrWhiteSpace(studID.Text) && string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = true;
                lNameError.IsVisible = false;
                studIDError.IsVisible = false;
                pwdError.IsVisible = true;
            }
            else if (string.IsNullOrWhiteSpace(fName.Text) && !string.IsNullOrWhiteSpace(lName.Text) && string.IsNullOrWhiteSpace(studID.Text) && !string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = true;
                lNameError.IsVisible = false;
                studIDError.IsVisible = true;
                pwdError.IsVisible = false;
            }

            else if (string.IsNullOrWhiteSpace(fName.Text) && string.IsNullOrWhiteSpace(lName.Text) && !string.IsNullOrWhiteSpace(studID.Text) && !string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = true;
                lNameError.IsVisible = true;
                studIDError.IsVisible = false;
                pwdError.IsVisible = false;
            }

            else if (!string.IsNullOrWhiteSpace(fName.Text) && !string.IsNullOrWhiteSpace(lName.Text) && !string.IsNullOrWhiteSpace(studID.Text) && string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = false;
                lNameError.IsVisible = false;
                studIDError.IsVisible = false;
                pwdError.IsVisible = true;
            }
            else if (!string.IsNullOrWhiteSpace(fName.Text) && string.IsNullOrWhiteSpace(lName.Text) && !string.IsNullOrWhiteSpace(studID.Text) && !string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = false;
                lNameError.IsVisible = true;
                studIDError.IsVisible = false;
                pwdError.IsVisible = false;
            }
            else if (!string.IsNullOrWhiteSpace(fName.Text) && !string.IsNullOrWhiteSpace(lName.Text) && string.IsNullOrWhiteSpace(studID.Text) && !string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = false;
                lNameError.IsVisible = false;
                studIDError.IsVisible = true;
                pwdError.IsVisible = false;
            }
            else if (string.IsNullOrWhiteSpace(fName.Text) && !string.IsNullOrWhiteSpace(lName.Text) && !string.IsNullOrWhiteSpace(studID.Text) && !string.IsNullOrWhiteSpace(pwd.Text))
            {
                fNameError.IsVisible = true;
                lNameError.IsVisible = false;
                studIDError.IsVisible = false;
                pwdError.IsVisible = false;
            }
            else
            {
                bool intOnly = int.TryParse(studID.Text, out int i);

                if (!intOnly)
                {
                    DisplayAlert("", "Please fill in only numbers for Student Number.", "OK");
                }
                else
                {

                    try
                    {
                        Users u = new Users
                        {
                            User_FirstName = fName.Text,
                            User_LastName = lName.Text,
                            User_LoginID = GenerateLoginID(fName.Text, lName.Text),
                            User_LoginPwd = pwd.Text,
                            User_ID = Int32.Parse(studID.Text),
                            User_PUID = studPUID.Text,
                            User_Email = GenerateEmail(finalLoginID),
                            Student_SchoolRecordID = ((Schools)schoolPicker.SelectedItem).School_RecordID,
                            Student_CourseRecordID = ((Courses)coursePicker.SelectedItem).Course_RecordID,
                            CreatedBy = userData.UserRecordID
                        };

                        //Add new student record.
                        rowsAffected = db.AddOneStudent(u);
                        if (rowsAffected == 1)
                        {
                            //Check if student course exists.
                            u.User_RecordID = db.GetUserRecordID(u.User_ID);
                            int count = db.CheckIfStudentCourseExists(u.User_RecordID);
                            if (count != 0)
                            {
                                DisplayAlert("Failure", "Failed to enroll student under selected course. Student is already enrolled.", "OK");
                            }
                            else
                            {
                                rowsAffectedTwo = db.AddOneStudentCourse(u);
                                if (rowsAffectedTwo == 1)
                                {
                                    DisplayAlert("Success", "Student record has been successfully created. The login ID is: " + finalLoginID, "OK");
                                    Navigation.PopAsync();
                                }
                                else
                                {
                                    DisplayAlert("Failure", "Failed to enroll student under selected course. Student is already enrolled.", "OK");
                                }
                            }
                        }
                        else
                        {
                            DisplayAlert("Creation Failure", "Student record failed to create.", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("UNIQUE KEY constraint"))
                        {
                            DisplayAlert("Creation Failure", "Failed to add new student: Student number exists in the database.", "OK");
                        }
                        else
                        {
                            DisplayAlert("Error", "Failed to add new student: " + ex.Message + " (Contact Administrator)", "OK");
                        }
                    }
                }
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

        private void SetPageDefaultSettings()
        {
            //Reset pickers.
            LoadSchoolPickerOptions();
            LoadCoursePickerOptions("");


            //Reset fields.
            fName.Text = "";
            lName.Text = "";
            studID.Text = "";
            studPUID.Text = "";
            pwd.Text = "";

            fNameError.IsVisible = false;
            lNameError.IsVisible = false;
            studIDError.IsVisible = false;
            pwdError.IsVisible = false;

            fName.Completed += (s, e) => lName.Focus();
            lName.Completed += (s, e) => studID.Focus();
            studID.Completed += (s, e) => pwd.Focus();
            pwd.Completed += (s, e) => SaveBtn_Clicked(s, e);
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
                    schoolPicker.SelectedItem = null;
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
                if (uniName == "")
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

        private List<Courses> GetCrseList_SortedByName()
        {
            return db.GetAllCourses().OrderBy(x => x.Course_Name).ToList();
        }
        private List<Courses> GetCrseList_SortedByUni(string name)
        {
            return db.GetCoursesBySchools(name).OrderBy(x => x.Course_Name).ToList();
        }


        //Generate Login ID from first and last name.
        private string GenerateLoginID(string firstName, string lastName)
        {
            generatedLoginID = FormatFirstName(firstName).ToLower() + FormatLastName(lastName).ToLower();
            idCount = db.CheckIfLoginIDExists(generatedLoginID);
            finalLoginID = generatedLoginID + (idCount + 1).ToString("D3");
            return finalLoginID;
        }

        private string FormatFirstName(string firstname)
        {
            var fNames = firstname.Split(' ');
            foreach (string name in fNames)
            {
                generatedFirstName += name.Substring(0, 1);
            }
            return generatedFirstName;
        }

        private string FormatLastName(string lastname)
        {
            if (lastname.Contains(" "))
            {
                var lName = lastname.Split(' ');
                foreach (string name in lName)
                {
                    generatedLastName += name;
                }
                return generatedLastName;
            }
            else
            {
                return lastname;
            }
        }

        //Generate Email from Login ID.
        private string GenerateEmail(string loginID)
        {
            return loginID + "@mymail.sim.edu.sg";
        }
    }
}