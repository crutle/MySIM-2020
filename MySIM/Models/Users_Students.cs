using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MySIM.Models
{
	class Users_Students
	{
		//Module's Class Schedule Table e.g. CO3310 Table
		public int ModuleClass_RecordID { get; set; }
		public int ModuleClass_TimingID { get; set; }
		public string ModuleClass_Date { get; set; }
		public string ModuleClass_Location { get; set; }

		//ClassTimings Table
		public string ClassTimings_StartTime { get; set; }
		public string ClassTimings_EndTime { get; set; }


		//Modules Table
		public string Module_Code { get; set; }
		public string Module_Name { get; set; }

		//Syncfusion Mandatory Appointment Fields
		public string EventName { get; set; }
		public DateTime From { get; set; }
		public DateTime To { get; set; }
		public Color Color { get; set; }

		public string FormattedTitle
		{
			get
			{
				if (ModuleClass_Location == null)
				{

					return string.Format("{0}" + Environment.NewLine + "{1}", Module_Code, Module_Name);
				}
				else
				{
					return string.Format("{0}" + Environment.NewLine + "{1}" + Environment.NewLine + Environment.NewLine + "{2}", Module_Code, Module_Name, ModuleClass_Location);
				}
			}
		}
		public string FormattedClassDetails
		{
			get
			{
				if (ModuleClass_Location == null)
				{
					return string.Format("{0} - {1}", ClassTimings_StartTime, ClassTimings_EndTime);
				}
				else
				{
					return string.Format("{0} - {1}" + Environment.NewLine + "{2}", ClassTimings_StartTime, ClassTimings_EndTime, ModuleClass_Location);
				}
			}
		}

		public string Loc { get; set; }
		public int Id { get; set; }

		
	}
}
