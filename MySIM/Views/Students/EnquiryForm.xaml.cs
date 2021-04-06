using MySIM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*  Visual Studio Generated Skeleton Code: 
namespace MySIM.Views.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnquiryForm : ContentPage
    {
        public EnquiryForm()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnquiryForm : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        
        public EnquiryForm(string to)
        {
            InitializeComponent();
            //Initialise & Load Page.
            Initialise(to);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Initialise("");
        }

        protected async void SendBtn_Clicked(object sender, EventArgs args)
        {
            if (string.IsNullOrEmpty(emailMsgEditor.Text) && string.IsNullOrEmpty(emailSubjEntry.Text))
            {
                await DisplayAlert("", "Please fill in a subject and a message before sending.", "OK");
            }
            else if (string.IsNullOrEmpty(emailMsgEditor.Text))
            {
                await DisplayAlert("", "Please write a message before sending.", "OK");
            } 
            else if ( string.IsNullOrEmpty(emailSubjEntry.Text))
            {
                await DisplayAlert("", "Please write in a subject before sending.", "OK");
            }
            else
            {
                bool answer = await DisplayAlert("Confirmation", "Confirm send?", "Yes", "No");
                if (answer)
                {
                    //For demonstration purpose, hardcoded from email address and email password. Actual: use logged in student's email (@mymail.sim.edu.sg). 
                    string toEmail = emailToPicker.SelectedItem.ToString();
                    string subject = emailSubjEntry.Text;
                    string message = emailMsgEditor.Text;              
                    string studentSIMPwd = "CO3320ProjectNov2020";

                    try
                    {
                        MailMessage mail = new MailMessage();
                        mail.To.Clear();
                        mail.Subject = "";
                        mail.Body = "";

                        mail.From = new MailAddress("chchan002@outlook.com");
                        //mail.To.Add("el2c.chanchuhan@gmail.com");
                        mail.To.Add(toEmail);
                        mail.Subject = subject;
                        mail.Body = message;

                        SmtpClient SmtpServer = new SmtpClient("smtp-mail.outlook.com");
                        //SmtpClient SmtpServer = new SmtpClient("smtp.office365.com");
                        SmtpServer.Port = 587;
                        SmtpServer.Host = "smtp-mail.outlook.com";
                        //SmtpServer.Host = "smtp.office365.com";
                        SmtpServer.EnableSsl = true;
                        SmtpServer.UseDefaultCredentials = false;
                        SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("chchan002@outlook.com", studentSIMPwd);

                        SmtpServer.Send(mail);

                        await DisplayAlert("Success", "Email has been sent successfully.", "OK");
                        SetPageDefaultSettings();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Email failed to send: ", ex.Message, "OK");
                    }
                }
            }            
        }



        private void Initialise(string to)
        {
            //Check if user account type is student.
            CheckIfStudentAccount();

            //Initialise page controls.
            SetPageDefaultSettings();

            if (!string.IsNullOrEmpty(to))
            {
                emailToPicker.SelectedIndex = 1;
            }
        }

        //Check if UserSettingsController's stored user data is admin data.
        private void CheckIfStudentAccount()
        {
            try
            {
                int isStudent = db.IsStudent(userData.UserRecordID);

                if (isStudent == 0 || userData.ActiveSession == false)
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
            emailSubjEntry.Text = "";
            emailMsgEditor.Text = "";

        }
    }
}