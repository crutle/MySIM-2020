using System;
using System.Collections.Generic;
using System.Text;

namespace MySIM.Models
{
    class Modules
    {
        //Modules Table
        public int Module_RecordID { get; set; }
        public string Module_Code { get; set; }
        public string Module_Name { get; set; }
        public string Module_Description { get; set; }
        public int Module_LessonQty { get; set; }
        public string Module_LessonStartDate { get; set; }
        public int IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int EditedBy { get; set; }
        public string EditedOn { get; set; }
        public int DeletedBy { get; set; }
        public string DeletedOn { get; set; }

        public string FormattedModuleTitle
        {
            get
            {
                return string.Format("{0} {1}", Module_Code, Module_Name);
            }
        }

        //Courses_Modules Table, linking Course & Module
        public int Module_Course_RecordID { get; set; }
        public string Module_Course_Name { get; set; }

        //ClassTimings Table, linking modules and class timings for Module's Class Schedule Table
        public int ClassTiming_RecordID { get; set; }
        public string ClassTiming_Name { get; set; }

        //Student_Modules Table
        public int Student_Module_RecordID { get; set; }
        public int School_RecordID { get; set; }
        public int Course_RecordID { get; set; }
        public int Student_RecordID { get; set; }
    }
}

