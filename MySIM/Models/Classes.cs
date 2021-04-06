using System;
using System.Collections.Generic;
using System.Text;

namespace MySIM.Models
{
    class Classes
    {
        //ClassTimings Table
        public int ClassTiming_RecordID { get; set; }
        public string ClassTiming_Name { get; set; }
        public string ClassTiming_StartTime { get; set; }
        public string ClassTiming_EndTime { get; set; }
        public int IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int EditedBy { get; set; }
        public string EditedOn { get; set; }
        public int DeletedBy { get; set; }
        public string DeletedOn { get; set; }

        //Module's Class Schedule Table e.g CO3310
        public int Module_RecordID { get; set; }
        public int ModuleClass_RecordID { get; set; }
        public string ModuleClass_Date { get; set; }
        public string ModuleClass_Loc { get; set; }
        public string AttendanceCode { get; set; }
    }
}
