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
    public partial class AddOneCourse : ContentPage
    {
        public AddOneCourse()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Courses_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddOneCourse : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Schools> uniList = new List<Schools>();

        private int rowsAffected, rowsAffectedTwo = 0;

        public AddOneCourse()
        {
            InitializeComponent();
            //Initialise & Load Page.
            Initialise();
        }

        protected void SaveBtn_Clicked(object sender, EventArgs args)
        {
            if (schoolPicker.SelectedItem == null)
            {
                DisplayAlert("", "Please select a university to register course under.", "OK");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(crseName.Text) && string.IsNullOrWhiteSpace(crseSite.Text))
                {
                    crseNameError.IsVisible = true;
                    crseSiteError.IsVisible = true;
                    requiredLbl.TextColor = Color.Red;
                }
                else if (string.IsNullOrWhiteSpace(crseName.Text) && !string.IsNullOrWhiteSpace(crseSite.Text))
                {
                    crseNameError.IsVisible = true;
                    crseSiteError.IsVisible = false;
                    requiredLbl.TextColor = Color.Red;
                }
                else if (!string.IsNullOrWhiteSpace(crseName.Text) && string.IsNullOrWhiteSpace(crseSite.Text))
                {

                    crseNameError.IsVisible = false;
                    crseSiteError.IsVisible = true;
                    requiredLbl.TextColor = Color.Red;
                }
                else
                {
                    try
                    {
                        Courses crse = new Courses
                        {
                            Course_Name = crseName.Text,
                            Course_Application = crseApplication.Text,
                            Course_Overview = crseOverview.Text,
                            Course_StartEndDate = crseDates.Text,
                            Course_Structure = crseStructure.Text,
                            Course_Admission_Criteria = crseAdmission.Text,
                            Course_Exemptions = crseExemptions.Text,
                            Course_Fees = crseFees.Text,
                            Course_Profiles = crseProfiles.Text,
                            Course_Assessments = crseAssessment.Text,
                            Course_Grading = crseGrading.Text,
                            Course_Promotion = crsePromotion.Text,
                            Course_AttendanceReq = crseAttendance.Text,
                            Course_Graduation = crseGrad.Text,
                            WebsiteLink = crseSite.Text,
                            CreatedBy = userData.UserRecordID
                        };

                        //Check if course name exists.
                        int count = db.CheckIfCourseNameExists(crse.Course_Name.ToString());

                        //Course does not exist.
                        if (count == 0)
                        {
                            //1. Add Courses record.
                            rowsAffected = db.AddOneCourse(crse);
                            if (rowsAffected == 1)
                            {
                                crse.Course_RecordID = db.GetCourseRecordID(crse.Course_Name.ToString());
                                var selectedSchool = (Schools)schoolPicker.SelectedItem;
                                //Check if Schools_Courses exists.
                                count = db.CheckIfSchoolCourseExists(crse.Course_RecordID, selectedSchool.School_RecordID);

                                //No records found, to add.
                                if (count == 0)
                                {
                                    //2. Add Schools_Courses record.
                                    rowsAffectedTwo = db.AddOneSchoolCourse(crse.Course_RecordID, selectedSchool.School_RecordID, crse.CreatedBy);

                                    if (rowsAffectedTwo == 1)
                                    {
                                        DisplayAlert("Success", "Course record has been successfully created.", "OK");
                                        Navigation.PopAsync();
                                    }
                                    else
                                    {
                                        DisplayAlert("Failure", "Course record failed to add under the selected university.", "OK");
                                    }
                                }
                                //Records found, to alert (courses record does not exist, created. schools_courses record exists error)
                                else
                                {
                                    DisplayAlert("Failure", "Course has already been added under this university.", "OK");
                                }
                            }
                        }
                        //Course exists.
                        else
                        {
                            DisplayAlert("Failure", "Failed to add new course. Course exists in the database.", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        if(ex.Message.Contains("UNIQUE KEY"))
                        {
                            DisplayAlert("Error", "Failed to add new course: A record with the course name exists in the database. Please check the database. (Contact Administrator)", "OK");
                        }
                        else
                        {
                            DisplayAlert("Error", "Failed to add new course: " + ex.Message + " (Contact Administrator)", "OK");
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

        //Initialise page controls.
        private void SetPageDefaultSettings()
        {
            //Reset schoolPicker.
            schoolPicker.SelectedItem = null;
            LoadPickerOptions();

            //Reset fields.
            crseName.Text = "";
            crseApplication.Text = "";
            crseOverview.Text = "";
            crseDates.Text = "";
            crseStructure.Text = "";
            crseAdmission.Text = "";
            crseExemptions.Text = "";
            crseFees.Text = "";
            crseProfiles.Text = "";
            crseAssessment.Text = "";
            crseGrading.Text = "";
            crsePromotion.Text = "";
            crseAttendance.Text = "";
            crseGrad.Text = "";
            crseSite.Text = "";

            crseNameError.IsVisible = false;
            crseSiteError.IsVisible = false;

            requiredLbl.TextColor = Color.Gray;
        }

        //Set picker options to list of available universities.
        private void LoadPickerOptions()
        {
            try
            {
                //Get list of university names.
                uniList = GetUniList_SortedByName();
                if (uniList.Count != 0)
                {
                    schoolPicker.ItemsSource = uniList;
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
    }
}