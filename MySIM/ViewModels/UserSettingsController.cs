using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace MySIM.ViewModels
{
    /* UserSettingsContoller for Stay Logged In function:
     *      - Getters & Setters for declared variables
     * Reference: https://www.c-sharpcorner.com/article/learn-about-user-settings-in-xamarin-forms/
     */

    public class UserSettingsController : INotifyPropertyChanged
    {
        public INavigation _navigation;
        public int UserRecordID
        {
            get => UserSettings.UserRecordID;
            set
            {
                UserSettings.UserRecordID = value;
                NotifyPropertyChanged("UserRecordID");
            }
        }
        public bool IsAdmin
        {
            get => UserSettings.IsAdmin;
            set
            {
                UserSettings.IsAdmin = value;
                NotifyPropertyChanged("IsAdmin");
            }
        }
        public bool StayLoggedIn
        {
            get => UserSettings.StayLoggedIn;
            set
            {
                UserSettings.StayLoggedIn = value;
                NotifyPropertyChanged("StayLoggedIn");
            }
        }
        public bool ActiveSession
        {
            get => UserSettings.ActiveSession;
            set
            {
                UserSettings.ActiveSession = value;
                NotifyPropertyChanged("ActiveSession");
            }
        }

        /*  REUSE OF CODE (BELOW)
         * Title: Learn About User Settings In Xamarin.
         * Author: Venkata Swamy Balaraju
         * Date: 17 May 2018
         * Code version: NA.
         * Type: Source Code
         * Available at: https://www.c-sharpcorner.com/article/learn-about-user-settings-in-xamarin-forms/
         */
        #region INotifyPropertyChanged  
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}