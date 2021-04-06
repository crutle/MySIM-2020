using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Settings;
using Plugin.Settings.Abstractions;


namespace MySIM
{
    /*  UserSettings for Stay Logged In function:
     *  - Xam.Plugins.Settings NuGet package
     *  - Declaration of variables for persistent data 
     * Reference: https://www.c-sharpcorner.com/article/learn-about-user-settings-in-xamarin-forms/
     */
    public static class UserSettings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        public static int UserRecordID
        {
            get => AppSettings.GetValueOrDefault(nameof(UserRecordID), 0);
            set => AppSettings.AddOrUpdateValue(nameof(UserRecordID), value);
        }

        public static bool IsAdmin
        {
            get => AppSettings.GetValueOrDefault(nameof(IsAdmin), false);
            set => AppSettings.AddOrUpdateValue(nameof(IsAdmin), value);
        }

        public static bool StayLoggedIn
        {
            get => AppSettings.GetValueOrDefault(nameof(StayLoggedIn), false);
            set => AppSettings.AddOrUpdateValue(nameof(StayLoggedIn), value);
        }
        public static bool ActiveSession
        {
            get => AppSettings.GetValueOrDefault(nameof(ActiveSession), false);
            set => AppSettings.AddOrUpdateValue(nameof(ActiveSession), value);
        } 
       
        public static void ClearAllData()
        {
            AppSettings.Clear();
        }
    }
}