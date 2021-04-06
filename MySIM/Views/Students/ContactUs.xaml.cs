using MySIM.ViewModels;
using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/* Visual Studio Generated Skeleton Code: 
namespace MySIM.Views.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactUs : ContentPage
    {
        public ContactUs()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactUs : TabbedPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();
        private readonly List<string> toAddress = new List<string>();
        //private static string studEmail;

        public ContactUs(string pageToLoad)
        {
            InitializeComponent();

            //Create Enquiry Form Tab.
            EnquiryForm ef = new EnquiryForm(pageToLoad);
            Children.Add(ef);

            //Set Enquiry Form as selected tab.
            if (pageToLoad.Equals("student services"))
            {
                CurrentPage = Children[1];
            }

            //Initialise & Load Page.
            Initialise();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Initialise();
        }

        private void Initialise()
        {
            //Check if user account type is student.
            CheckIfStudentAccount();

            //Initialise page controls.
            SetPageDefaultSettings();
        }

        //Check if UserSettingsController's stored user data is student data.
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
            Title = "Contact Us";
            SelectedTabColor = Color.White;
            UnselectedTabColor = Color.Black;

            //Reference: https://forums.xamarin.com/discussion/18342/best-way-to-make-clickable-text
            generalTel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => TelClicked(generalTel.Text)),
            });

            generalEmail.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => await EmailClicked(generalEmail.Text, "General Enquiry")),
            });

            studentTel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => TelClicked(studentTel.Text)),
            });

            studentEmail.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => await EmailClicked(studentEmail.Text, "Student Enquiry")),
            });

            programTel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => TelClicked(programTel.Text)),
            });

            programEmail.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => await EmailClicked(programEmail.Text, "Programme Enquiry")),
            });

            apptBook.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => ApptBookClicked()),
            });
        }

        private async void TelClicked(string telNo)
        {
            string action = await DisplayActionSheet("", "Cancel", null, "Call");
            if (action == "Call")
            {
                try
                { 
                    var phoneDialer = CrossMessaging.Current.PhoneDialer;
                    if (phoneDialer.CanMakePhoneCall)
                        phoneDialer.MakePhoneCall("+65" + telNo);
                }
                catch (ArgumentNullException anEx)
                {
                    await DisplayAlert("Error", "Failed to dial as phone number is empty.: " + anEx.Message + " (Contact Administrator)", "OK");
                }
                catch (FeatureNotSupportedException ex)
                {
                    await DisplayAlert("Error", "Phone dialing is not supported on this device.: " + ex.Message + " (Contact Administrator)", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Failed to dial: " + ex.Message + " (Contact Administrator)", "OK");
                }
            }
        }

        private async Task EmailClicked(string toAdd, string subject)
        {
            string action = await DisplayActionSheet("", "Cancel", null, "Compose Email");
            if (action == "Compose Email")
            {
                try
                {
                    toAddress.Clear();
                    toAddress.Add(toAdd);
                    var message = new EmailMessage
                    {
                        To = toAddress,
                        Subject = subject
                    };
                    await Email.ComposeAsync(message);
                }
                catch (FeatureNotSupportedException)
                {
                    await DisplayAlert("", "Email is not supported on this device.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Failed to email: " + ex.Message + " (Contact Administrator)", "OK");
                }
            }
        }
        
        private async void ApptBookClicked()
        {
            //Reference: https://docs.microsoft.com/en-us/xamarin/essentials/open-browser?context=xamarin%2Fandroid&tabs=android
            await Browser.OpenAsync("https://simrec.custhelp.com/ci/documents/detail/2/SIMGE_ConsultationReg_Purpose", new BrowserLaunchOptions
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Show,
                PreferredToolbarColor = Color.FromHex("#027B88"),
                PreferredControlColor = Color.FromHex("#FFD50B"),
            });
        }
    }
}