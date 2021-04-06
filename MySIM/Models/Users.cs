using System;
using System.Collections.Generic;
using System.Text;

namespace MySIM.Models
{
    class Users
    {
        //Users Table
        //Student/Admin Details
        public int User_RecordID { get; set; }
        public string User_FirstName { get; set; }
        public string User_LastName { get; set; }
        public string User_LoginID { get; set; }
        public string User_LoginPwd { get; set; }
        public int User_ID { get; set; }
        public string User_PUID { get; set; }
        public string User_Email { get; set; }
        public bool IsAdmin { get; set; }
        public int IsDeleted { get; set; }
        public string LastLoggedIn { get; set; }

        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int EditedBy { get; set; }
        public string EditedOn { get; set; }
        public int DeletedBy { get; set; }
        public string DeletedOn { get; set; }

        //Student_Module & Student_Course Table.
        public int Student_SchoolRecordID { get; set; }
        public int Student_CourseRecordID { get; set; }
    }
}
