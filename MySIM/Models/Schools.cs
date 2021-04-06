using System;
using System.Collections.Generic;
using System.Text;

namespace MySIM.Models
{
	class Schools
	{
		//Schools Table
		public int School_RecordID { get; set; }
		public string School_Name { get; set; }
		public string School_Desc { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public string WebsiteLink { get; set; }
		public int IsDeleted { get; set; }

		public int CreatedBy { get; set; }
		public string CreatedOn { get; set; }
		public int EditedBy { get; set; }
		public string EditedOn { get; set; }
		public int DeletedBy { get; set; }
		public string DeletedOn { get; set; }
	}
}
