using MySIM.Models;
using MySIM.ViewModels;
using Syncfusion.XForms.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/* Visual Studio Generated Skeleton Code: 
namespace MySIM.Views.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public ChatPage()
        {
            InitializeComponent();
        }
    }
}
*/

namespace MySIM.Views.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly DatabaseController db = new DatabaseController();

        private SfChat botChat;
        private SfChatViewModel viewModel;

        public ChatPage()
        {
            InitializeComponent();
            //Initialise & Load Page.
            Initialise();
        }



        private void Initialise()
        {
            //Check if user account type is student.
            CheckIfStudentAccount();

            LoadChatPage();
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

        private void LoadChatPage()
        {
            //Reference: https://github.com/SyncfusionExamples/Chat-GettingStarted-in-Xamarin-Forms

            botChat = new SfChat();
            viewModel = new SfChatViewModel(Navigation);
            this.BindingContext = viewModel;
            botChat.SendMessage += viewModel.SendMessage;
            botChat.Messages = viewModel.Messages;
            botChat.CurrentUser = viewModel.CurrentUser;
            this.Content = botChat;
        }
    }
}
