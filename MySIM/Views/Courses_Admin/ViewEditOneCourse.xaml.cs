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
    public partial class ViewEditOneCourse : ContentPage
    {
        public ViewEditOneCourse()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Courses_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewEditOneCourse : TabbedPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private List<Schools> uniList = new List<Schools>();
        private Courses course = new Courses();
        private static int courseRecordID;

        private int rowsAffected, rowsAffectedTwo = 0;

        public ViewEditOneCourse(int recordID)
        {
            InitializeComponent();

            courseRecordID = recordID;

            //Create Modules Tab
            ViewModules vm = new ViewModules(courseRecordID);
            Children.Add(vm);

            //Initialise & Load Page.
            Initialise();
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

            schoolPicker.IsEnabled = true;
            crseName.IsEnabled = true;
            crseApplication.IsEnabled = true;
            crseOverview.IsEnabled = true;
            crseDates.IsEnabled = true;
            crseStructure.IsEnabled = true;
            crseAdmission.IsEnabled = true;
            crseExemptions.IsEnabled = true;
            crseFees.IsEnabled = true;
            crseProfiles.IsEnabled = true;
            crseAssessment.IsEnabled = true;
            crseGrading.IsEnabled = true;
            crsePromotion.IsEnabled = true;
            crseAttendance.IsEnabled = true;
            crseGrad.IsEnabled = true;
            crseSite.IsEnabled = true;

            toolbarEdit.Text = "Save";

            editBtn.IsVisible = false;

            saveBtn.IsVisible = true;
            cancelEditBtn.IsVisible = true;
            deleteBtn.IsVisible = true;
        }

        protected async void SaveBtn_Clicked(object sender, EventArgs args)
        {
            if (schoolPicker.SelectedItem == null)
            {
                await DisplayAlert("", "Please select a university to register course under.", "OK");
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
                            Course_RecordID = courseRecordID,
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
                            EditedBy = userData.UserRecordID
                        };

                        //1. Update Courses record.
                        rowsAffected = db.UpdateOneCourse(crse);
                        if (rowsAffected == 1)
                        {
                            int schoolRecordID = db.GetSchoolRecordID(((Schools)schoolPicker.SelectedItem).School_Name);
                            //Check if Schools_Courses record exists.
                            int count = db.CheckIfSchoolCourseExists(courseRecordID, schoolRecordID);

                            //No records found, to add.
                            if (count == 0)
                            {
                                //2.a Add Schools_Courses record.
                                rowsAffectedTwo = db.AddOneSchoolCourse(courseRecordID, schoolRecordID, crse.EditedBy);
                            }
                            //Records found, to update.
                            else
                            {
                                //2.b Update Schools_Courses record.
                                rowsAffectedTwo = db.UpdateOneSchoolCourse(courseRecordID, schoolRecordID, crse.EditedBy);
                            }

                            //Both success updates.
                            if (rowsAffectedTwo == 1)
                            {
                                await DisplayAlert("Success", "Course record has been updated successfully.", "OK");
                                //Reload course details.
                                SetPageDefaultSettings();
                                LoadOneCourse();
                            }
                            //Failed to update Schools_Courses record.
                            else
                            {
                                await DisplayAlert("Failure", "Course record failed to update under the selected university.", "OK");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", "Failed to update course: " + ex.Message + " (Contact Administrator)", "OK");
                    }
                }
            }
        }

        protected void CancelEditBtn_Clicked(object sender, EventArgs args)
        {
            //Reload course details.
            SetPageDefaultSettings();
            LoadOneCourse();
        }

        protected async void DeleteBtn_Clicked(object sender, EventArgs args)
        {
            try
            {
                var response = await DisplayAlert("Confirm Deletion", "Delete course?", "OK", "Cancel");

                //OK selected.
                if (response)
                {
                    //1. Delete Courses record from database.
                    rowsAffected = db.DeleteOneCourse(courseRecordID, userData.UserRecordID);
                    if (rowsAffected == 1)
                    {
                        //2. Delete Schools_Courses record from database.
                        rowsAffectedTwo = db.DeleteOneSchoolCourse(courseRecordID, userData.UserRecordID);
                        if (rowsAffectedTwo == 1)
                        {
                            int count = db.CheckIfCourseModuleExists(courseRecordID);
                            //No modules to be removed.
                            if (count == 0)
                            {
                                await DisplayAlert("Success", "Course has been successfully deleted.", "OK");
                                await Navigation.PopAsync();
                            }
                            //3. Modules to be removed.
                            else
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    db.DeleteOneCourseModule(courseRecordID, userData.UserRecordID);
                                }
                                count = db.CheckIfCourseModuleExists(courseRecordID);
                                if (count == 0)
                                {
                                    await DisplayAlert("Success", "Course has been successfully deleted.", "OK");
                                    await Navigation.PopAsync();
                                }
                            }
                        }
                        //Failed to delete Schools_Courses record.
                        else
                        {
                            await DisplayAlert("Failure", "Failed to delete Course record under the selected university.", "OK");
                        }
                    }
                    //Failed to delete Courses record.
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

            //Get & load one course.
            LoadOneCourse();
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
            schoolPicker.IsEnabled = false;

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

            crseName.IsEnabled = false;
            crseApplication.IsEnabled = false;
            crseOverview.IsEnabled = false;
            crseDates.IsEnabled = false;
            crseStructure.IsEnabled = false;
            crseAdmission.IsEnabled = false;
            crseExemptions.IsEnabled = false;
            crseFees.IsEnabled = false;
            crseProfiles.IsEnabled = false;
            crseAssessment.IsEnabled = false;
            crseGrading.IsEnabled = false;
            crsePromotion.IsEnabled = false;
            crseAttendance.IsEnabled = false;
            crseGrad.IsEnabled = false;
            crseSite.IsEnabled = false;

            crseNameError.IsVisible = false;
            crseSiteError.IsVisible = false;
            requiredLbl.TextColor = Color.Gray;

            toolbarEdit.Text = "Edit";

            editBtn.IsVisible = true;

            saveBtn.IsVisible = false;
            cancelEditBtn.IsVisible = false;
            deleteBtn.IsVisible = false;

            try
            {
                Title = db.GetCourseName(courseRecordID);
                SelectedTabColor = Color.White;
                UnselectedTabColor = Color.Black;
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to reset page controls: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        //Get & load one course.
        private void LoadOneCourse()
        {
            try
            {
                //Get course details from database.
                course = db.GetOneCourse(courseRecordID);

                schoolPicker.SelectedItem = course.Course_School_Name;

                crseName.Text = course.Course_Name;
                crseApplication.Text = course.Course_Application;
                crseOverview.Text = course.Course_Overview;
                crseDates.Text = course.Course_StartEndDate;
                crseStructure.Text = course.Course_Structure;
                crseAdmission.Text = course.Course_Admission_Criteria;
                crseExemptions.Text = course.Course_Exemptions;
                crseFees.Text = course.Course_Fees;
                crseProfiles.Text = course.Course_Profiles;
                crseAssessment.Text = course.Course_Assessments;
                crseGrading.Text = course.Course_Grading;
                crsePromotion.Text = course.Course_Promotion;
                crseAttendance.Text = course.Course_AttendanceReq;
                crseGrad.Text = course.Course_Graduation;
                crseSite.Text = course.WebsiteLink;

                LoadUniPickerOptions();
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load course details: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        //Set picker options to list of available universities.
        private void LoadUniPickerOptions()
        {
            try
            {
                //Get list of university names
                uniList = GetUniList_SortedByName();
                if (uniList.Count != 0)
                {
                    schoolPicker.ItemsSource = uniList;

                    for (int i = 0; i < uniList.Count; i++)
                    {
                        if (uniList[i].School_Name.Equals(course.Course_School_Name))
                        {
                            schoolPicker.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load university picker options: " + ex.Message + " (Contact Administrator)", "OK");
            }
        }

        private List<Schools> GetUniList_SortedByName()
        {
            return db.GetAllSchools().OrderBy(x => x.School_Name).ToList();
        }
    }
}