using MySIM.ViewModels;
using MySIM.Views;
using MySIM.Views.Courses_Admin;
using MySIM.Views.Modules_Admin;
using MySIM.Views.Students;
using MySIM.Views.Students_Admin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

/* Visual Studio Generated Skeleton Code: 
 namespace MySIM.Menu
{
    public class MenuPageViewModel
    {
    }
} 
*/

namespace MySIM.Menu
{
    public class MenuPageViewModel
    {
        //Reference: https://stackoverflow.com/questions/49169049/hamburger-menu-xamarin-forms-masterdetailpage
        public ICommand HomeCommand { get; set; }
        public ICommand CoursesCommand { get; set; }
        public ICommand ModulesCommand { get; set; }
        public ICommand StudentsCommand { get; set; }

        public ICommand ChatbotCommand { get; set; }
        public ICommand StudentDetailsCommand { get; set; }
        public ICommand StudentAttendanceCommand { get; set; }
        public ICommand StudentClassCommand { get; set; }
        public ICommand ContactCommand { get; set; }

        public ICommand LogoutCommand { get; set; }

        public UserSettingsController userData = new UserSettingsController();

        public MenuPageViewModel()
        {
            HomeCommand = new Command(GoHome);
            CoursesCommand = new Command(GoCourses);
            ModulesCommand = new Command(GoModules);
            StudentsCommand = new Command(GoStudents);

            ChatbotCommand = new Command(GoChatbot);
            StudentDetailsCommand = new Command(GoStudentDetails);
            StudentAttendanceCommand = new Command(GoStudentAttendance);
            StudentClassCommand = new Command(GoStudentClass);
            ContactCommand = new Command(GoContact);

            LogoutCommand = new Command(Logout);
        }

        void GoHome(object obj)
        {
            if (App.NavigationPage != null)
            {
                App.NavigationPage.Navigation.PopToRootAsync();
                App.MenuIsPresented = false;
            }
            else
            {
                Application.Current.MainPage = new RootPage
                {
                    Flyout = new MenuPage(),
                    Detail = new NavigationPage(new Home())
                };

            }
        }

        void GoCourses(object obj)
        {
            if (App.NavigationPage != null)
            {
                App.NavigationPage.Navigation.PopToRootAsync();
                App.NavigationPage.Navigation.PushAsync(new ManageAllCourses());
                App.MenuIsPresented = false;
            }
            else
            {
                Application.Current.MainPage = new RootPage
                {
                    Flyout = new MenuPage(),
                    Detail = new NavigationPage(new ManageAllCourses())
                };             
            }
        }

        void GoModules(object obj)
        {
            if (App.NavigationPage != null)
            {
                App.NavigationPage.Navigation.PopToRootAsync();
                App.NavigationPage.Navigation.PushAsync(new ManageAllModules());
                App.MenuIsPresented = false;
            }
            else
            {
                Application.Current.MainPage = new RootPage
                {
                    Flyout = new MenuPage(),
                    Detail = new NavigationPage(new ManageAllModules())
                };
            }
        }

        void GoStudents(object obj)
        {
            if (App.NavigationPage != null)
            {
                App.NavigationPage.Navigation.PopToRootAsync();
                App.NavigationPage.Navigation.PushAsync(new ManageAllStudents());
                App.MenuIsPresented = false;
            }
            else
            {
                Application.Current.MainPage = new RootPage
                {
                    Flyout = new MenuPage(),
                    Detail = new NavigationPage(new ManageAllStudents())
                };
            }
        }

        void GoChatbot(object obj)
        {
            if (App.NavigationPage != null)
            {
                App.NavigationPage.Navigation.PopToRootAsync();
                App.NavigationPage.Navigation.PushAsync(new ChatPage());
                App.MenuIsPresented = false;
            }
            else
            {
                Application.Current.MainPage = new RootPage
                {
                    Flyout = new MenuPage(),
                    Detail = new NavigationPage(new ChatPage())
                };
            }
        }

        void GoStudentDetails(object obj)
        {
            if (App.NavigationPage != null)
            {
                App.NavigationPage.Navigation.PopToRootAsync();
                App.NavigationPage.Navigation.PushAsync(new StudentDetails());
                App.MenuIsPresented = false;
            }
            else
            {
                Application.Current.MainPage = new RootPage
                {
                    Flyout = new MenuPage(),
                    Detail = new NavigationPage(new StudentDetails())
                };
            }
        }

        void GoStudentAttendance(object obj)
        {
            if (App.NavigationPage != null)
            {
                App.NavigationPage.Navigation.PopToRootAsync();
                App.NavigationPage.Navigation.PushAsync(new Attendance());
                App.MenuIsPresented = false;
            }
            else
            {
                Application.Current.MainPage = new RootPage
                {
                    Flyout = new MenuPage(),
                    Detail = new NavigationPage(new Attendance())
                };
            }
        }

        void GoStudentClass(object obj)
        {
            if (App.NavigationPage != null)
            {
                App.NavigationPage.Navigation.PopToRootAsync();
                App.NavigationPage.Navigation.PushAsync(new ClassSchedule());
                App.MenuIsPresented = false;
            }
            else
            {
                Application.Current.MainPage = new RootPage
                {
                    Flyout = new MenuPage(),
                    Detail = new NavigationPage(new ClassSchedule())
                };
            }
        }

        void GoContact(object obj)
        {
            if (App.NavigationPage != null)
            {
                App.NavigationPage.Navigation.PopToRootAsync();
                App.NavigationPage.Navigation.PushAsync(new ContactUs(""));
                App.MenuIsPresented = false;
            }
            else
            {
                Application.Current.MainPage = new RootPage
                {
                    Flyout = new MenuPage(),
                    Detail = new NavigationPage(new ContactUs(""))
                };
            }
        }

        void Logout(object obj)
        {
            UserSettings.ClearAllData();
            
            if (App.NavigationPage != null)
            {
                App.NavigationPage.Navigation.PopToRootAsync();
                App.MenuIsPresented = false;
            }
            
            Application.Current.MainPage = new Login();
        }
    }
}
