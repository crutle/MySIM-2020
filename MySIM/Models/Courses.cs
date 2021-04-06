using System;
using System.Collections.Generic;
using System.Text;

namespace MySIM.Models
{
    class Courses
    {
        //Courses Table
        public int Course_RecordID { get; set; }
        public string Course_Name { get; set; }
        public string Course_Application { get; set; }
        public string Course_Overview { get; set; }
        public string Course_StartEndDate { get; set; }
        public string Course_Structure { get; set; }
        public string Course_Admission_Criteria { get; set; }
        public string Course_Exemptions { get; set; }
        public string Course_Fees { get; set; }
        public string Course_Profiles { get; set; }
        public string Course_Assessments { get; set; }
        public string Course_Grading { get; set; }
        public string Course_Promotion { get; set; }
        public string Course_AttendanceReq { get; set; }
        public string Course_Graduation { get; set; }
        public string WebsiteLink { get; set; }
        public int IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int EditedBy { get; set; }
        public string EditedOn { get; set; }
        public int DeletedBy { get; set; }
        public string DeletedOn { get; set; }

        //Schools_Courses Table
        public int Course_School_RecordID { get; set; }
        public string Course_School_Name { get; set; }

    }
}
