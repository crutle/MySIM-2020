using MySIM.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace MySIM.ViewModels
{
    class DatabaseController
    {
        private readonly static string connString = "";
      
        private readonly SqlCommand cmd = new SqlCommand();
        private readonly SqlConnection cn = new SqlConnection(connString);
        private readonly TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore");

        /**********************************************************************************************************************************
                                                                USER
        **********************************************************************************************************************************/
        public int IsAdmin(int recordID)
        {
            int isAdmin;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT IsAdmin FROM Users WHERE IsDeleted = 0 AND IsStudent = 0 AND Users_RecordID = @record;";

            cmd.Parameters.Add("@record", SqlDbType.Int).Value = recordID;

            try
            {
                cn.Open();
                isAdmin = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting admin status: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return isAdmin;
        }

        public int IsStudent(int recordID)
        {
            int isStudent;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT IsStudent FROM Users WHERE IsDeleted = 0 AND Users_RecordID = @record;";

            cmd.Parameters.Add("@record", SqlDbType.Int).Value = recordID;

            try
            {
                cn.Open();
                isStudent = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting admin status: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return isStudent;
        }

        public int GetUserRecordID(string loginID, string loginPwd)
        {
            int recordID;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Users_RecordID FROM Users WHERE IsDeleted = 0 AND Users_LoginID = @loginID AND Users_LoginPwd = @pwd;";

            cmd.Parameters.Add("@loginid", SqlDbType.VarChar, 255).Value = loginID;
            cmd.Parameters.Add("@pwd", SqlDbType.VarChar, 255).Value = loginPwd;

            try
            {
                cn.Open();
                recordID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting user record id: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return recordID;
        }

        public int GetUserRecordID(int studentNumber)
        {
            int recordID;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Users_RecordID FROM Users WHERE IsDeleted = 0 AND Users_ID = @studNum;";

            cmd.Parameters.Add("@studNum", SqlDbType.VarChar, 255).Value = studentNumber.ToString();

            try
            {
                cn.Open();
                recordID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting user record id: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return recordID;
        }

        public int UpdateLastLoggedIn(int userRecordID)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Users SET LastLoggedIn = @lastLogged WHERE IsDeleted = 0 AND Users_RecordID = " + userRecordID.ToString();

            cmd.Parameters.Add("@lastLogged", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);      

            try
            {
                cn.Open();
                rowsAffected = (int)cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error updating user last logged in: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        /**********************************************************************************************************************************
                                                                STUDENT
        **********************************************************************************************************************************/
        public int CheckIfLoginIDExists(string loginID)
        {
            int count;
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(Users_ID) FROM Users WHERE IsAdmin = 0 AND Users_LoginID LIKE '" + loginID + "%';";

            try
            {
                cn.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error count of login id: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }
        public int CheckIfStudentCourseExists(int studentRecordID)
        {
            int count;
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) FROM Student_Course WHERE IsDeleted = 0 AND Student_RecordID =" + studentRecordID + ";";

            try
            {
                cn.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error checking if student course exists: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }

        public int CheckIfStudentModuleExists(int studentRecordID, int moduleRecordID)
        {
            int count;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) FROM Student_Modules WHERE IsDeleted = 0 AND Student_RecordID = @studID AND Module_RecordID = @modID;";

            cmd.Parameters.Add("@studID", SqlDbType.Int).Value = studentRecordID;
            cmd.Parameters.Add("@modID", SqlDbType.Int).Value = moduleRecordID;


            try
            {
                cn.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error checking if course module(s) exists: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }


        public int CheckIfStudentExists(int studentRecordID)
        {
            int count;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE IsDeleted = 0 AND IsStudent = 1 AND Users_RecordID = @recordID;";

            cmd.Parameters.Add("@recordID", SqlDbType.Int).Value = studentRecordID;
            try
            {
                cn.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting checking if course exists: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }

        public string GetStudentEmail(int studentRecordID)
        {
            string email;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Users_Email FROM Users WHERE IsDeleted = 0 AND Users_RecordID = @id;";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = studentRecordID;
            try
            {
                cn.Open();
                email = (string)cmd.ExecuteScalar();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting student email: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return email;
        }

        public string GetStudentPassword(int studentRecordID)
        {
            string pwd;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Users_LoginPwd FROM Users WHERE IsDeleted = 0 AND Users_RecordID = @id;";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = studentRecordID;
            try
            {
                cn.Open();
                pwd = (string)cmd.ExecuteScalar();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting student email: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return pwd;
        }

        public List<Users> GetAllStudents()
        {
            List<Users> userList = new List<Users>();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT * FROM Users WHERE IsAdmin = 0 AND IsDeleted = 0;";

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Users u = new Users()
                    {
                        User_RecordID = int.Parse(dr["Users_RecordID"].ToString()),
                        User_FirstName = dr["Users_FirstName"].ToString(),
                        User_LastName = dr["Users_LastName"].ToString(),
                        User_LoginID = dr["Users_LoginID"].ToString(),
                        User_LoginPwd = dr["Users_LoginPwd"].ToString(),
                        User_PUID = dr["Users_PUID"].ToString(),
                        User_ID = int.Parse(dr["Users_ID"].ToString())
                    };
                    userList.Add(u);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of all students: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return userList;
        }

        public List<Users> GetSearchedStudent(string searchString)
        {
            List<Users> userList = new List<Users>();
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT * FROM Users WHERE IsAdmin = 0 AND IsDeleted = 0 AND (Users_FirstName LIKE @searchString OR Users_LastName LIKE @searchString OR Users_ID = @searchStringID);";

            cmd.Parameters.Add("@searchString", SqlDbType.VarChar, 255).Value = "%" + searchString + "%";
            cmd.Parameters.Add("@searchStringID", SqlDbType.VarChar, 255).Value = searchString;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Users u = new Users()
                    {
                        User_RecordID = int.Parse(dr["Users_RecordID"].ToString()),
                        User_FirstName = dr["Users_FirstName"].ToString(),
                        User_LastName = dr["Users_LastName"].ToString(),
                        User_LoginID = dr["Users_LoginID"].ToString(),
                        User_LoginPwd = dr["Users_LoginPwd"].ToString(),
                        User_PUID = dr["Users_PUID"].ToString(),
                        User_ID = int.Parse(dr["Users_ID"].ToString())
                    };
                    userList.Add(u);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of searched students: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return userList;
        }

        public Users GetOneStudent(int userRecordID)
        {
            Users student = new Users();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT * FROM Users WHERE IsDeleted = 0 AND IsAdmin = 0 AND IsStudent = 1 AND Users_RecordID = " + userRecordID;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                student.User_RecordID = int.Parse(dr["Users_RecordID"].ToString());
                student.User_FirstName = dr["Users_FirstName"].ToString();
                student.User_LastName = dr["Users_LastName"].ToString();
                student.User_LoginID = dr["Users_LoginID"].ToString();
                student.User_LoginPwd = dr["Users_LoginPwd"].ToString();
                student.User_PUID = dr["Users_PUID"].ToString();
                student.User_ID = int.Parse(dr["Users_ID"].ToString());
                student.User_Email = dr["Users_Email"].ToString();

            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting details of one student: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return student;
        }

        public Users GetOneStudentCourse(int userRecordID)
        {
            Users student = new Users();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Course_RecordID, School_RecordID FROM Student_Course WHERE IsDeleted = 0 AND Student_RecordID = " + userRecordID + ";";

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                student.Student_CourseRecordID = int.Parse(dr["Course_RecordID"].ToString());
                student.Student_SchoolRecordID = int.Parse(dr["School_RecordID"].ToString());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting details of one student course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return student;
        }

        public List<Modules> GetOneStudentModules(int studentRecordID)
        {
            List<Modules> moduleList = new List<Modules>();
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT sm.StudentModules_RecordID, sm.Student_RecordID, sm.School_RecordID, sm.Course_RecordID, sm.Module_RecordID, m.Modules_Code, m.Modules_Name " +
                                "FROM Student_Modules sm, Modules m " +
                                "WHERE sm.IsDeleted = 0 AND m.IsDeleted = 0 AND sm.Module_RecordID = m.Modules_RecordID AND sm.Student_RecordID = @studID;";

            cmd.Parameters.Add("@studID", SqlDbType.Int).Value = studentRecordID;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Modules mod = new Modules()
                    {
                        Student_Module_RecordID = int.Parse(dr["StudentModules_RecordID"].ToString()),
                        Student_RecordID = int.Parse(dr["Student_RecordID"].ToString()),
                        School_RecordID = int.Parse(dr["School_RecordID"].ToString()),
                        Course_RecordID = int.Parse(dr["Course_RecordID"].ToString()),
                        Module_RecordID = int.Parse(dr["Module_RecordID"].ToString()),
                        Module_Code = dr["Modules_Code"].ToString(),
                        Module_Name = dr["Modules_Name"].ToString(),
                    };
                    moduleList.Add(mod);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of modules of one student: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return moduleList;
        }

        public int AddOneStudent(Users u)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "INSERT INTO Users (Users_FirstName, Users_LastName, Users_LoginID, Users_LoginPwd, Users_ID, Users_Email, Users_PUID, IsAdmin, IsStudent, CreatedBy) VALUES (@fName, @lName, @loginID, @loginPwd, @studID, @email, @PUID, 0, 1, @creator);";

            cmd.Parameters.Add("@fName", SqlDbType.VarChar, 255).Value = u.User_FirstName;
            cmd.Parameters.Add("@lName", SqlDbType.VarChar, 255).Value = u.User_LastName;
            cmd.Parameters.Add("@loginID", SqlDbType.VarChar, 255).Value = u.User_LoginID;
            cmd.Parameters.Add("@loginPwd", SqlDbType.VarChar, 255).Value = u.User_LoginPwd;
            cmd.Parameters.Add("@studID", SqlDbType.Char, 12).Value = u.User_ID;
            cmd.Parameters.Add("@PUID", SqlDbType.VarChar, 255).Value = u.User_PUID;
            cmd.Parameters.Add("@email", SqlDbType.VarChar, 255).Value = u.User_Email;
            cmd.Parameters.Add("@creator", SqlDbType.Int).Value = u.CreatedBy;

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }

            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error adding one student: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int AddOneStudentCourse(Users u)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "INSERT INTO Student_Course (Student_RecordID, School_RecordID, Course_RecordID, CreatedBy) VALUES (@userID, @schoolID, @courseID, @creator);";

            cmd.Parameters.Add("@userID", SqlDbType.Int).Value = u.User_RecordID;
            cmd.Parameters.Add("@schoolID", SqlDbType.Int).Value = u.Student_SchoolRecordID;
            cmd.Parameters.Add("@courseID", SqlDbType.Int).Value = u.Student_CourseRecordID;
            cmd.Parameters.Add("@creator", SqlDbType.Int).Value = u.CreatedBy;

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }

            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error adding one student course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int AddOneStudentModule(int studentRecordID, int schoolRecordID, int courseRecordID, int moduleRecordID, int createdBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "INSERT INTO Student_Modules(Student_RecordID, School_RecordID, Course_RecordID, Module_RecordID, CreatedBy) VALUES" +
                " (@studID, @schID, @crseID, @modID, @creator);";

            cmd.Parameters.Add("@studID", SqlDbType.Int).Value = studentRecordID;
            cmd.Parameters.Add("@schID", SqlDbType.Int).Value = schoolRecordID;
            cmd.Parameters.Add("@crseID", SqlDbType.Int).Value = courseRecordID;
            cmd.Parameters.Add("@modID", SqlDbType.Int).Value = moduleRecordID;
            cmd.Parameters.Add("@creator", SqlDbType.Int).Value = createdBy;

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error enrolling student to module: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int UpdateOneStudent(Users u)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Users SET Users_FirstName = @fName, Users_LastName = @lName, Users_LoginPwd = @loginPwd, Users_PUID = @PUID, EditedBy = @editor, EditedOn = @editedOn WHERE Users_RecordID = @recordID AND IsDeleted = 0;";

            cmd.Parameters.Add("@fName", SqlDbType.VarChar, 255).Value = u.User_FirstName;
            cmd.Parameters.Add("@lName", SqlDbType.VarChar, 255).Value = u.User_LastName;
            cmd.Parameters.Add("@loginPwd", SqlDbType.VarChar, 255).Value = u.User_LoginPwd;
            cmd.Parameters.Add("@PUID", SqlDbType.VarChar, 255).Value = u.User_PUID;
            cmd.Parameters.Add("@recordID", SqlDbType.Char, 12).Value = u.User_RecordID;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = u.EditedBy;
            cmd.Parameters.Add("@editedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error updating one student: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int UpdateOneStudentCourse(int studentRecordID, int courseRecordID, int schoolRecordID, int editedBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Student_Course SET School_RecordID = @schoolID, Course_RecordID = @crseID, EditedOn = @editedOn, EditedBy = @editor WHERE Student_RecordID = @userID AND IsDeleted = 0;";

            cmd.Parameters.Add("@schoolID", SqlDbType.Int).Value = schoolRecordID;
            cmd.Parameters.Add("@crseID", SqlDbType.Int).Value = courseRecordID;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = editedBy;
            cmd.Parameters.Add("@editedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            cmd.Parameters.Add("@userID", SqlDbType.Int).Value = studentRecordID;

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error updating link of one course to school: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int DeleteOneStudent(int userRecordID, int deletedBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Users SET IsDeleted = 1, DeletedBy = @editor, DeletedOn = @deletedOn WHERE IsDeleted = 0 AND IsAdmin = 0 AND Users_RecordID = @recordID;";

            cmd.Parameters.Add("@recordID", SqlDbType.Int).Value = userRecordID;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = deletedBy;
            cmd.Parameters.Add("@deletedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error deleting one student: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int DeleteOneStudentCourse(int studentRecordID, int deletedBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Student_Course SET IsDeleted = 1, DeletedBy = @editor, DeletedOn = @deletedOn WHERE IsDeleted = 0 AND Student_RecordID = @studID;";

            cmd.Parameters.Add("@studID", SqlDbType.Int).Value = studentRecordID;

            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = deletedBy;
            cmd.Parameters.Add("@deletedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error deleting one school course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int DeleteOneStudentModule(int studentRecordID, int moduleRecordID, int deletedBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Student_Modules SET IsDeleted = 1, DeletedBy = @editor, DeletedOn = @deletedOn WHERE IsDeleted = 0 AND Student_RecordID = @studID AND Module_RecordID = @modID;";

            cmd.Parameters.Add("@studID", SqlDbType.Int).Value = studentRecordID;
            cmd.Parameters.Add("@modID", SqlDbType.Int).Value = moduleRecordID;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = deletedBy;
            cmd.Parameters.Add("@deletedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error deleting one student module: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }


        /**********************************************************************************************************************************
                                                                SCHOOL
        **********************************************************************************************************************************/
        public int GetSchoolRecordID(int courseRecordID)
        {
            int recordID;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT s.Schools_RecordID FROM Schools_Courses sc, Schools s, Courses c WHERE c.IsDeleted = 0 AND sc.Schools_ID = s.Schools_RecordID AND sc.Courses_ID = c.Courses_RecordID AND c.Courses_RecordID = @recordID;";

            cmd.Parameters.Add("@recordID", SqlDbType.Int).Value = courseRecordID;
            try
            {
                cn.Open();
                recordID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting school record id using course record id: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return recordID;
        }

        public int GetSchoolRecordID(string schoolName)
        {
            int recordID;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Schools_RecordID FROM Schools WHERE IsDeleted = 0 AND Schools_Name = @name;";

            cmd.Parameters.Add("@name", SqlDbType.VarChar, 255).Value = schoolName;
            try
            {
                cn.Open();
                recordID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting school record id using school name: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return recordID;
        }
        public string GetSchoolName(int schoolRecordID)
        {
            string name;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Schools_Name FROM Schools WHERE IsDeleted = 0 AND Schools_RecordID = @recordID;";

            cmd.Parameters.Add("@recordID", SqlDbType.Int).Value = schoolRecordID;
            try
            {
                cn.Open();
                name = cmd.ExecuteScalar().ToString();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting school record id using course record id: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return name;
        }

        public List<Schools> GetAllSchools()
        {
            List<Schools> uniList = new List<Schools>();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT * FROM Schools WHERE IsDeleted = 0;";

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Schools uni = new Schools()
                    {
                        School_RecordID = int.Parse(dr["Schools_RecordID"].ToString()),
                        School_Name = dr["Schools_Name"].ToString(),
                        School_Desc = dr["Schools_Desc"].ToString(),
                        City = dr["City"].ToString(),
                        Country = dr["Country"].ToString(),
                        WebsiteLink = dr["WebsiteLink"].ToString(),
                    };
                    uniList.Add(uni);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of all schools: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return uniList;
        }

        /**********************************************************************************************************************************
                                                                COURSES
        **********************************************************************************************************************************/
        public int CheckIfCourseNameExists(string courseName)
        {
            int count;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) FROM Courses WHERE IsDeleted = 0 AND Courses_Name = @name;";

            cmd.Parameters.Add("@name", SqlDbType.VarChar, 255).Value = courseName;
            try
            {
                cn.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting checking if course exists: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }

        public int CheckIfSchoolCourseExists(int courseRecordID, int schoolRecordID)
        {
            int recordID;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) FROM Schools_Courses WHERE Schools_ID = @schoolID AND Courses_ID = @crseID AND IsDeleted = 0;";

            cmd.Parameters.Add("@crseID", SqlDbType.Int).Value = courseRecordID;
            cmd.Parameters.Add("@schoolID", SqlDbType.Int).Value = schoolRecordID;
            try
            {
                cn.Open();
                recordID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error checking if school course exists: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return recordID;
        }

        public int CheckIfUniHasCourses(int schoolRecordID)
        {
            int count;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) FROM Schools_Courses sc, Schools s, Courses c WHERE sc.Courses_ID = c.Courses_RecordID AND sc.Schools_ID = s.Schools_RecordID AND sc.Schools_ID = @schID;";

            cmd.Parameters.Add("@schID", SqlDbType.Int).Value = schoolRecordID;
            try
            {
                cn.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting checking if university has courses: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }

        public int GetCourseRecordID(string courseName)
        {
            int recordID;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Courses_RecordID FROM Courses WHERE IsDeleted = 0 AND Courses_Name = @name;";

            cmd.Parameters.Add("@name", SqlDbType.VarChar, 255).Value = courseName;
            try
            {
                cn.Open();
                recordID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting course record id: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return recordID;
        }
        public string GetCourseName(int courseRecordID)
        {
            string courseName;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Courses_Name FROM Courses WHERE IsDeleted = 0 AND Courses_RecordID= @id;";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = courseRecordID;
            try
            {
                cn.Open();
                courseName = (string)cmd.ExecuteScalar();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting course name: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return courseName;
        }

        public List<Courses> GetAllCourses()
        {
            List<Courses> courseList = new List<Courses>();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT c.Courses_RecordID, c.Courses_Name, s.Schools_Name, s.Schools_RecordID" +
                " FROM Schools_Courses sc, Schools s, Courses c WHERE c.IsDeleted = 0 AND sc.IsDeleted = 0 AND sc.Schools_ID = s.Schools_RecordID AND sc.Courses_ID = c.Courses_RecordID;";

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Courses course = new Courses()
                    {
                        Course_RecordID = int.Parse(dr["Courses_RecordID"].ToString()),
                        Course_Name = dr["Courses_Name"].ToString(),
                        Course_School_RecordID = int.Parse(dr["Schools_RecordID"].ToString()),
                        Course_School_Name = dr["Schools_Name"].ToString()
                    };
                    courseList.Add(course);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of all courses: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return courseList;
        }

        public List<Courses> GetCoursesBySchools(string uniName)
        {
            List<Courses> courseList = new List<Courses>();
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT c.Courses_RecordID, c.Courses_Name, s.Schools_Name, s.Schools_RecordID" +
                " FROM Schools_Courses sc, Schools s, Courses c WHERE c.IsDeleted = 0 AND sc.IsDeleted = 0 AND sc.Schools_ID = s.Schools_RecordID AND sc.Courses_ID = c.Courses_RecordID AND Schools_Name = @uniName;";

            cmd.Parameters.Add("@uniName", SqlDbType.VarChar, 255).Value = uniName;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Courses course = new Courses()
                    {
                        Course_RecordID = int.Parse(dr["Courses_RecordID"].ToString()),
                        Course_Name = dr["Courses_Name"].ToString(),
                        Course_School_RecordID = int.Parse(dr["Schools_RecordID"].ToString()),
                        Course_School_Name = dr["Schools_Name"].ToString()
                    };
                    courseList.Add(course);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of all courses by school: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return courseList;
        }

        public List<Courses> GetSearchedCourse(string searchString)
        {
            List<Courses> courseList = new List<Courses>();
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT c.Courses_RecordID, c.Courses_Name, s.Schools_Name " +
                " FROM Schools_Courses sc, Schools s, Courses c WHERE c.IsDeleted = 0 AND sc.IsDeleted = 0 AND sc.Schools_ID = s.Schools_RecordID AND sc.Courses_ID = c.Courses_RecordID AND (c.Courses_Name LIKE @search OR s.Schools_Name LIKE @search);";

            cmd.Parameters.Add("@search", SqlDbType.VarChar, 255).Value = "%" + searchString + "%";

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Courses course = new Courses()
                    {
                        Course_RecordID = int.Parse(dr["Courses_RecordID"].ToString()),
                        Course_Name = dr["Courses_Name"].ToString(),
                        Course_School_Name = dr["Schools_Name"].ToString()
                    };
                    courseList.Add(course);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of searched courses: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return courseList;
        }

        public List<Courses> GetSearchedAndFilteredCourses(string searchString, string uniName)
        {
            List<Courses> courseList = new List<Courses>();
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT c.Courses_RecordID, c.Courses_Name, s.Schools_Name " +
                " FROM Schools_Courses sc, Schools s, Courses c WHERE c.IsDeleted = 0 AND sc.IsDeleted = 0 AND sc.Schools_ID = s.Schools_RecordID AND sc.Courses_ID = c.Courses_RecordID AND (c.Courses_Name LIKE @search AND s.Schools_Name = @uniname);";

            cmd.Parameters.Add("@search", SqlDbType.VarChar, 255).Value = "%" + searchString + "%";
            cmd.Parameters.Add("@uniname", SqlDbType.VarChar, 255).Value = uniName;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Courses course = new Courses()
                    {
                        Course_RecordID = int.Parse(dr["Courses_RecordID"].ToString()),
                        Course_Name = dr["Courses_Name"].ToString(),
                        Course_School_Name = dr["Schools_Name"].ToString()
                    };
                    courseList.Add(course);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of searched and filtered courses: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return courseList;
        }

        public Courses GetOneCourse(int courseRecordID)
        {
            Courses course = new Courses();
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT c.Courses_RecordID, c.Courses_Name, c.Courses_Application, c.Courses_Overview, c.Courses_StartEndDate, c.Courses_Structure, c.Courses_Admission_Criteria, c.Courses_Exemptions, c.Courses_Fees, c.Courses_Profiles, c.Courses_Assessment, c.Courses_Grading, c.Courses_Promotion, c.Courses_AttendanceReq, c.Courses_Graduation, c.WebsiteLink, sc.Schools_ID, s.Schools_Name " +
            " FROM Schools_Courses sc, Schools s, Courses c WHERE c.IsDeleted = 0  AND sc.Schools_ID = s.Schools_RecordID AND sc.Courses_ID = c.Courses_RecordID AND c.Courses_RecordID = @courseRecordID;";

            cmd.Parameters.Add("@courseRecordID", SqlDbType.Int).Value = courseRecordID;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();

                course.Course_RecordID = int.Parse(dr["Courses_RecordID"].ToString());
                course.Course_Name = dr["Courses_Name"].ToString();
                course.Course_Application = dr["Courses_Application"].ToString();
                course.Course_Overview = dr["Courses_Overview"].ToString();
                course.Course_StartEndDate = dr["Courses_StartEndDate"].ToString();
                course.Course_Structure = dr["Courses_Structure"].ToString();
                course.Course_Admission_Criteria = dr["Courses_Admission_Criteria"].ToString();
                course.Course_Fees = dr["Courses_Fees"].ToString();
                course.Course_Exemptions = dr["Courses_Exemptions"].ToString();
                course.Course_Profiles = dr["Courses_Profiles"].ToString();
                course.Course_Assessments = dr["Courses_Assessment"].ToString();
                course.Course_Grading = dr["Courses_Grading"].ToString();
                course.Course_Promotion = dr["Courses_Promotion"].ToString();
                course.Course_AttendanceReq = dr["Courses_AttendanceReq"].ToString();
                course.Course_Graduation = dr["Courses_Graduation"].ToString();
                course.WebsiteLink = dr["WebsiteLink"].ToString();
                course.Course_School_RecordID = int.Parse(dr["Schools_ID"].ToString());
                course.Course_School_Name = dr["Schools_Name"].ToString();

            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting details of one course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return course;
        }

        public int AddOneCourse(Courses crse)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "INSERT INTO Courses (Courses_Name, Courses_Application, Courses_Overview, Courses_StartEndDate, Courses_Structure, Courses_Admission_Criteria, Courses_Fees, Courses_Exemptions, Courses_Profiles, Courses_Assessment, Courses_Grading, Courses_Promotion, Courses_AttendanceReq, Courses_Graduation, WebsiteLink, CreatedBy) " +
                 "VALUES(@crseName, @crseApplication, @crseOverview, @crseDates, @crseStructure, @crseAdmission, @crseFees, @crseExemptions, @crseProfiles, @crseAssessments, @crseGrading, @crsePromotion, @crseAttendance, @crseGrad, @crseSite, @creator);";


            cmd.Parameters.Add("@crseName", SqlDbType.VarChar, 255).Value = crse.Course_Name;
            cmd.Parameters.Add("@crseApplication", SqlDbType.VarChar, 255).Value = crse.Course_Application;
            cmd.Parameters.Add("@crseOverview", SqlDbType.VarChar, 255).Value = crse.Course_Overview;
            cmd.Parameters.Add("@crseDates", SqlDbType.VarChar, 255).Value = crse.Course_StartEndDate;
            cmd.Parameters.Add("@crseStructure", SqlDbType.VarChar, 255).Value = crse.Course_Structure;
            cmd.Parameters.Add("@crseAdmission", SqlDbType.VarChar, 255).Value = crse.Course_Admission_Criteria;
            cmd.Parameters.Add("@crseFees", SqlDbType.VarChar, 255).Value = crse.Course_Fees;
            cmd.Parameters.Add("@crseExemptions", SqlDbType.VarChar, 255).Value = crse.Course_Exemptions;
            cmd.Parameters.Add("@crseProfiles", SqlDbType.VarChar, 255).Value = crse.Course_Profiles;
            cmd.Parameters.Add("@crseAssessments", SqlDbType.VarChar, 255).Value = crse.Course_Assessments;
            cmd.Parameters.Add("@crseGrading", SqlDbType.VarChar, 255).Value = crse.Course_Grading;
            cmd.Parameters.Add("@crsePromotion", SqlDbType.VarChar, 255).Value = crse.Course_Promotion;
            cmd.Parameters.Add("@crseAttendance", SqlDbType.VarChar, 255).Value = crse.Course_AttendanceReq;
            cmd.Parameters.Add("@crseGrad", SqlDbType.VarChar, 255).Value = crse.Course_Graduation;
            cmd.Parameters.Add("@crseSite", SqlDbType.VarChar, 255).Value = crse.WebsiteLink;
            cmd.Parameters.Add("@creator", SqlDbType.Int).Value = crse.CreatedBy;

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }

            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error adding one course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int AddOneSchoolCourse(int courseRecordID, int schoolRecordID, int createdBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "INSERT INTO Schools_Courses(Schools_ID, Courses_ID, CreatedBy) VALUES (@schoolID, @crseID, @creator);";

            cmd.Parameters.Add("@crseID", SqlDbType.Int).Value = courseRecordID;
            cmd.Parameters.Add("@schoolID", SqlDbType.Int).Value = schoolRecordID;
            cmd.Parameters.Add("@creator", SqlDbType.Int).Value = createdBy;

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error linking one course to school: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int UpdateOneCourse(Courses crse)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Courses SET Courses_Name = @crseName, Courses_Application = @crseApplication, Courses_Overview = @crseOverview, Courses_StartEndDate = @crseDates, Courses_Structure = @crseStructure, Courses_Admission_Criteria = @crseAdmission, Courses_Fees = @crseFees, Courses_Exemptions = @crseExemptions, Courses_Profiles = @crseProfiles, Courses_Assessment = @crseAssessments, Courses_Grading = @crseGrading, Courses_Promotion = @crsePromotion, Courses_AttendanceReq = @crseAttendance, Courses_Graduation = @crseGrad, WebsiteLink = @crseSite, EditedOn = @editedOn, EditedBy = @editor " +
                "WHERE IsDeleted = 0 AND Courses_RecordID = @recordID;";

            cmd.Parameters.Add("@recordID", SqlDbType.Int).Value = crse.Course_RecordID;
            cmd.Parameters.Add("@crseName", SqlDbType.VarChar, 255).Value = crse.Course_Name;
            cmd.Parameters.Add("@crseApplication", SqlDbType.VarChar, 255).Value = crse.Course_Application;
            cmd.Parameters.Add("@crseOverview", SqlDbType.VarChar, 255).Value = crse.Course_Overview;
            cmd.Parameters.Add("@crseDates", SqlDbType.VarChar, 255).Value = crse.Course_StartEndDate;
            cmd.Parameters.Add("@crseStructure", SqlDbType.VarChar, 255).Value = crse.Course_Structure;
            cmd.Parameters.Add("@crseAdmission", SqlDbType.VarChar, 255).Value = crse.Course_Admission_Criteria;
            cmd.Parameters.Add("@crseFees", SqlDbType.VarChar, 255).Value = crse.Course_Fees;
            cmd.Parameters.Add("@crseExemptions", SqlDbType.VarChar, 255).Value = crse.Course_Exemptions;
            cmd.Parameters.Add("@crseProfiles", SqlDbType.VarChar, 255).Value = crse.Course_Profiles;
            cmd.Parameters.Add("@crseAssessments", SqlDbType.VarChar, 255).Value = crse.Course_Assessments;
            cmd.Parameters.Add("@crseGrading", SqlDbType.VarChar, 255).Value = crse.Course_Grading;
            cmd.Parameters.Add("@crsePromotion", SqlDbType.VarChar, 255).Value = crse.Course_Promotion;
            cmd.Parameters.Add("@crseAttendance", SqlDbType.VarChar, 255).Value = crse.Course_AttendanceReq;
            cmd.Parameters.Add("@crseGrad", SqlDbType.VarChar, 255).Value = crse.Course_Graduation;
            cmd.Parameters.Add("@crseSite", SqlDbType.VarChar, 255).Value = crse.WebsiteLink;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = crse.EditedBy;
            cmd.Parameters.Add("@editedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }

            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error updating one course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int UpdateOneSchoolCourse(int courseRecordID, int schoolRecordID, int editedBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Schools_Courses SET Schools_ID = @schoolID, EditedOn = @editedOn, EditedBy = @editor WHERE Courses_ID = @crseID AND IsDeleted = 0;";

            cmd.Parameters.Add("@crseID", SqlDbType.Int).Value = courseRecordID;
            cmd.Parameters.Add("@schoolID", SqlDbType.Int).Value = schoolRecordID;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = editedBy;
            cmd.Parameters.Add("@editedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error updating link of one course to school: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int DeleteOneCourse(int courseRecordID, int deletedBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Courses SET IsDeleted = 1, DeletedBy = @editor, DeletedOn = @deletedOn WHERE IsDeleted = 0 AND Courses_RecordID = @recordID;";

            cmd.Parameters.Add("@recordID", SqlDbType.Int).Value = courseRecordID;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = deletedBy;
            cmd.Parameters.Add("@deletedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error deleting one course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int DeleteOneSchoolCourse(int courseRecordID, int deletedBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Schools_Courses SET IsDeleted = 1, DeletedBy = @editor, DeletedOn = @deletedOn WHERE IsDeleted = 0 AND Courses_ID = @recordID; ";

            cmd.Parameters.Add("@recordID", SqlDbType.Int).Value = courseRecordID;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = deletedBy;
            cmd.Parameters.Add("@deletedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error deleting one school course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        /**********************************************************************************************************************************
                                                              MODULES
        **********************************************************************************************************************************/
        public int CheckIfModuleCodeExists(string modCode)
        {
            int count;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) FROM Modules WHERE IsDeleted = 0 AND Modules_Code = @code;";

            cmd.Parameters.Add("@code", SqlDbType.VarChar, 255).Value = modCode;
            try
            {
                cn.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting checking if module exists: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }

        public int CheckIfCourseModuleExists(string moduleCode)
        {
            int count;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(cm.Course_Module_RecordID) FROM Courses_Modules cm, Modules m, Courses c WHERE cm.Courses_ID = c.Courses_RecordID AND cm.Modules_ID = m.Modules_RecordID AND cm.IsDeleted = 0 AND m.Modules_Code = @modCode;";

            cmd.Parameters.Add("@modCode", SqlDbType.VarChar, 255).Value = moduleCode;

            try
            {
                cn.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error checking if course module(s) exists: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }

        public int CheckIfCourseModuleExists(int courseRecordID)
        {
            int count;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) FROM Courses_Modules WHERE Courses_ID = @crseID AND IsDeleted = 0;";

            cmd.Parameters.Add("@crseID", SqlDbType.Int).Value = courseRecordID;

            try
            {
                cn.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error checking if course module(s) exists: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }

        public int CheckIfCourseModuleExists(int moduleRecordID, int courseRecordID)
        {
            int count;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) FROM Courses_Modules WHERE Modules_ID = @modID AND Courses_ID = @crseID AND IsDeleted = 0;";

            cmd.Parameters.Add("@crseID", SqlDbType.Int).Value = courseRecordID;
            cmd.Parameters.Add("@modID", SqlDbType.Int).Value = moduleRecordID;

            try
            {
                cn.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error checking if course module exists: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }

        public int GetModuleRecordID(string moduleCode)
        {
            int recordID;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Modules_RecordID FROM Modules WHERE IsDeleted = 0 AND Modules_Code = @code;";

            cmd.Parameters.Add("@code", SqlDbType.VarChar, 255).Value = moduleCode;
            try
            {
                cn.Open();
                recordID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting module record id: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return recordID;
        }

        public string GetModuleCode(int moduleRecordID)
        {
            string moduleCode;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Modules_Code FROM Modules WHERE IsDeleted = 0 AND Modules_RecordID = @id;";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = moduleRecordID;
            try
            {
                cn.Open();
                moduleCode = (string)cmd.ExecuteScalar();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting module code: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return moduleCode;
        }

        public string GetModuleName(int moduleRecordID)
        {
            string moduleCode;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Modules_Name FROM Modules WHERE IsDeleted = 0 AND Modules_RecordID = @id;";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = moduleRecordID;
            try
            {
                cn.Open();
                moduleCode = (string)cmd.ExecuteScalar();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting module name: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return moduleCode;
        }

        public int GetLessonQuantity(int moduleRecordID)
        {
            int qty;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Modules_LessonsQty FROM Modules WHERE IsDeleted = 0 AND Modules_RecordID = @id;";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = moduleRecordID;
            try
            {
                cn.Open();
                qty = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting module lesson quantity: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return qty;
        }

        public int GetLessonQuantity(string moduleCode)
        {
            int qty;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Modules_LessonsQty FROM Modules WHERE IsDeleted = 0 AND Modules_Code = @code;";

            cmd.Parameters.Add("@code", SqlDbType.VarChar, 255).Value = moduleCode;
            try
            {
                cn.Open();
                qty = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting module lesson quantity: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return qty;
        }

        public int GetClassTimingRecordID(int moduleRecordID)
        {
            int qty;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT ClassTimings_ID FROM Modules WHERE IsDeleted = 0 AND Modules_RecordID = @id;";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = moduleRecordID;
            try
            {
                cn.Open();
                qty = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting class timing record id: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return qty;
        }

        public List<Modules> GetAllModules()
        {
            List<Modules> moduleList = new List<Modules>();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT DISTINCT Modules_RecordID, Modules_Code, Modules_Name FROM Modules WHERE IsDeleted = 0;";

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Modules mod = new Modules()
                    {
                        Module_RecordID = int.Parse(dr["Modules_RecordID"].ToString()),
                        Module_Code = dr["Modules_Code"].ToString(),
                        Module_Name = dr["Modules_Name"].ToString()
                    };
                    moduleList.Add(mod);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of all modules: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return moduleList;
        }

        public List<Modules> GetModulesBySchools(string uniName)
        {
            List<Modules> moduleList = new List<Modules>();
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT DISTINCT m.Modules_RecordID, m.Modules_Code, m.Modules_Name FROM Modules m, Courses c, Schools s, Schools_Courses sc, Courses_Modules cm " +
                                " WHERE sc.Courses_ID = c.Courses_RecordID AND sc.Schools_ID = s.Schools_RecordID AND " +
                                " cm.Courses_ID = c.Courses_RecordID AND cm.Modules_ID = m.Modules_RecordID AND " +
                                " m.IsDeleted = 0 AND s.Schools_Name = @name;";

            cmd.Parameters.Add("@name", SqlDbType.VarChar, 255).Value = uniName;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Modules mod = new Modules()
                    {
                        Module_RecordID = int.Parse(dr["Modules_RecordID"].ToString()),
                        Module_Code = dr["Modules_Code"].ToString(),
                        Module_Name = dr["Modules_Name"].ToString(),
                    };
                    moduleList.Add(mod);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of all modules by schools: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return moduleList;
        }

        public List<Modules> GetModulesByCourses(string crseName)
        {
            List<Modules> moduleList = new List<Modules>();
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT m.Modules_RecordID, m.Modules_Code, m.Modules_Name FROM Modules m, Courses c, Schools s, Schools_Courses sc, Courses_Modules cm " +
                                " WHERE sc.Courses_ID = c.Courses_RecordID AND sc.Schools_ID = s.Schools_RecordID AND " +
                                " cm.Courses_ID = c.Courses_RecordID AND cm.Modules_ID = m.Modules_RecordID AND " +
                                " m.IsDeleted = 0 AND c.Courses_Name = @name;";

            cmd.Parameters.Add("@name", SqlDbType.VarChar, 255).Value = crseName;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Modules mod = new Modules()
                    {
                        Module_RecordID = int.Parse(dr["Modules_RecordID"].ToString()),
                        Module_Code = dr["Modules_Code"].ToString(),
                        Module_Name = dr["Modules_Name"].ToString(),
                    };
                    moduleList.Add(mod);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of all modules by course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return moduleList;
        }

        public List<Modules> GetSearchedAndFilteredModules(string uniName, string courseName, string searchString)
        {
            List<Modules> moduleList = new List<Modules>();
            cmd.Parameters.Clear();
            cmd.Connection = cn;

            //searchString
            if (string.IsNullOrEmpty(uniName) && string.IsNullOrEmpty(courseName) && !string.IsNullOrEmpty(searchString))
            {
                cmd.CommandText = "SELECT Modules_RecordID, Modules_Code, Modules_Name FROM Modules " +
                    "WHERE IsDeleted = 0 AND (Modules_Code LIKE @search OR Modules_Name LIKE @search);";
            }

            //uniName
            else if (!string.IsNullOrEmpty(uniName) && string.IsNullOrEmpty(courseName) && string.IsNullOrEmpty(searchString))
            {
                cmd.CommandText = "SELECT DISTINCT m.Modules_RecordID, m.Modules_Code, m.Modules_Name FROM Modules m, Courses c, Schools s, Schools_Courses sc, Courses_Modules cm " +
                            " WHERE sc.Courses_ID = c.Courses_RecordID AND sc.Schools_ID = s.Schools_RecordID AND sc.IsDeleted = 0 AND " +
                            " cm.Courses_ID = c.Courses_RecordID AND cm.Modules_ID = m.Modules_RecordID AND cm.IsDeleted = 0 AND " +
                            " m.IsDeleted = 0 AND s.Schools_Name = @name;";
            }

            //courseName
            else if (string.IsNullOrEmpty(uniName) && !string.IsNullOrEmpty(courseName) && string.IsNullOrEmpty(searchString))
            {
                cmd.CommandText = "SELECT DISTINCT m.Modules_RecordID, m.Modules_Code, m.Modules_Name FROM Modules m, Courses c, Schools s, Schools_Courses sc, Courses_Modules cm " +
                               " WHERE sc.Courses_ID = c.Courses_RecordID AND sc.Schools_ID = s.Schools_RecordID AND sc.IsDeleted = 0 AND " +
                               " cm.Courses_ID = c.Courses_RecordID AND cm.Modules_ID = m.Modules_RecordID AND cm.IsDeleted = 0 AND " +
                               " m.IsDeleted = 0 AND c.Courses_Name = @crseName";
            }

            //searchString & uniName
            else if (!string.IsNullOrEmpty(uniName) && string.IsNullOrEmpty(courseName) && !string.IsNullOrEmpty(searchString))
            {
                cmd.CommandText = "SELECT DISTINCT m.Modules_RecordID, m.Modules_Code, m.Modules_Name FROM Modules m, Courses c, Schools s, Schools_Courses sc, Courses_Modules cm " +
                              " WHERE sc.Courses_ID = c.Courses_RecordID AND sc.Schools_ID = s.Schools_RecordID AND " +
                              " cm.Courses_ID = c.Courses_RecordID AND cm.Modules_ID = m.Modules_RecordID AND " +
                              " m.IsDeleted = 0 AND s.Schools_Name = @name AND (m.Modules_Code LIKE @search OR m.Modules_Name LIKE @search);";
            }

            //searchString & courseName
            else if (string.IsNullOrEmpty(uniName) && !string.IsNullOrEmpty(courseName) && !string.IsNullOrEmpty(searchString))
            {
                cmd.CommandText = "SELECT DISTINCT m.Modules_RecordID, m.Modules_Code, m.Modules_Name FROM Modules m, Courses c, Schools s, Schools_Courses sc, Courses_Modules cm " +
                              " WHERE sc.Courses_ID = c.Courses_RecordID AND sc.Schools_ID = s.Schools_RecordID AND " +
                              " cm.Courses_ID = c.Courses_RecordID AND cm.Modules_ID = m.Modules_RecordID AND " +
                              " m.IsDeleted = 0 AND c.Courses_Name = @crseName AND (m.Modules_Code LIKE @search OR m.Modules_Name LIKE @search);";
            }

            //courseName & uniName
            else if (!string.IsNullOrEmpty(uniName) && !string.IsNullOrEmpty(courseName) && string.IsNullOrEmpty(searchString))
            {
                cmd.CommandText = "SELECT DISTINCT m.Modules_RecordID, m.Modules_Code, m.Modules_Name FROM Modules m, Courses c, Schools s, Schools_Courses sc, Courses_Modules cm " +
                              " WHERE sc.Courses_ID = c.Courses_RecordID AND sc.Schools_ID = s.Schools_RecordID AND " +
                              " cm.Courses_ID = c.Courses_RecordID AND cm.Modules_ID = m.Modules_RecordID AND " +
                              " m.IsDeleted = 0 AND s.Schools_Name = @name AND c.Courses_Name = @crseName;";
            }

            //uniName & courseName & searchString
            else
            {
                cmd.CommandText = "SELECT DISTINCT m.Modules_RecordID, m.Modules_Code, m.Modules_Name FROM Modules m, Courses c, Schools s, Schools_Courses sc, Courses_Modules cm " +
                              " WHERE sc.Courses_ID = c.Courses_RecordID AND sc.Schools_ID = s.Schools_RecordID AND " +
                              " cm.Courses_ID = c.Courses_RecordID AND cm.Modules_ID = m.Modules_RecordID AND " +
                              " m.IsDeleted = 0 AND s.Schools_Name = @name AND c.Courses_Name = @crseName AND (m.Modules_Code LIKE @search OR m.Modules_Name LIKE @search) ";
            }

            cmd.Parameters.Add("@name", SqlDbType.VarChar, 255).Value = uniName;
            cmd.Parameters.Add("@crseName", SqlDbType.VarChar, 255).Value = courseName;
            cmd.Parameters.Add("@search", SqlDbType.VarChar, 255).Value = "%" + searchString + "%";

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Modules mod = new Modules()
                    {
                        Module_RecordID = int.Parse(dr["Modules_RecordID"].ToString()),
                        Module_Code = dr["Modules_Code"].ToString(),
                        Module_Name = dr["Modules_Name"].ToString()
                    };
                    moduleList.Add(mod);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of searched and filtered modules: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return moduleList;
        }

        public List<Modules> GetModulesByCourses(int courseRecordID)
        {
            List<Modules> moduleList = new List<Modules>();
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT m.Modules_RecordID, m.Modules_Code, m.Modules_Name FROM Modules m, Courses c, Schools s, Schools_Courses sc, Courses_Modules cm " +
                                " WHERE sc.Courses_ID = c.Courses_RecordID AND sc.Schools_ID = s.Schools_RecordID AND " +
                                " cm.Courses_ID = c.Courses_RecordID AND cm.Modules_ID = m.Modules_RecordID AND " +
                                " m.IsDeleted = 0 AND cm.IsDeleted = 0 AND c.Courses_RecordID = @crseID;";

            cmd.Parameters.Add("@crseID", SqlDbType.Int).Value = courseRecordID;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Modules mod = new Modules()
                    {
                        Module_RecordID = int.Parse(dr["Modules_RecordID"].ToString()),
                        Module_Code = dr["Modules_Code"].ToString(),
                        Module_Name = dr["Modules_Name"].ToString(),
                    };
                    moduleList.Add(mod);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of all modules by school and course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return moduleList;
        }

        public Modules GetOneModule(int moduleRecordID)
        {
            Modules module = new Modules();
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Modules_RecordID, Modules_Code,Modules_Name,Modules_Description,Modules_LessonsQty,Modules_LessonStartDate,ClassTimings_ID FROM Modules WHERE IsDeleted = 0 AND Modules_RecordID = @moduleRecordID;";

            cmd.Parameters.Add("@moduleRecordID", SqlDbType.Int).Value = moduleRecordID;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();

                module.Module_RecordID = int.Parse(dr["Modules_RecordID"].ToString());
                module.Module_Code = dr["Modules_Code"].ToString();
                module.Module_Name = dr["Modules_Name"].ToString();
                module.Module_Description = dr["Modules_Description"].ToString();
                module.Module_LessonQty = int.Parse(dr["Modules_LessonsQty"].ToString());
                module.Module_LessonStartDate = dr["Modules_LessonStartDate"].ToString();
                if (dr["ClassTimings_ID"] == null)
                {
                    module.ClassTiming_RecordID = 0;
                }
                else
                {
                    module.ClassTiming_RecordID = int.Parse(dr["ClassTimings_ID"].ToString());
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting details of one module: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return module;
        }

        public Modules GetOneModule(string moduleCode)
        {
            Modules module = new Modules();
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Modules_RecordID, Modules_Code,Modules_Name,Modules_Description,Modules_LessonsQty,Modules_LessonStartDate,ClassTimings_ID FROM Modules WHERE IsDeleted = 0 AND Modules_Code = @modCode;";

            cmd.Parameters.Add("@modCode", SqlDbType.VarChar, 255).Value = moduleCode;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();

                module.Module_RecordID = int.Parse(dr["Modules_RecordID"].ToString());
                module.Module_Code = dr["Modules_Code"].ToString();
                module.Module_Name = dr["Modules_Name"].ToString();
                module.Module_Description = dr["Modules_Description"].ToString();
                module.Module_LessonQty = int.Parse(dr["Modules_LessonsQty"].ToString());
                module.Module_LessonStartDate = dr["Modules_LessonStartDate"].ToString();
                if (dr["ClassTimings_ID"] == null)
                {
                    module.ClassTiming_RecordID = 0;
                }
                else
                {
                    module.ClassTiming_RecordID = int.Parse(dr["ClassTimings_ID"].ToString());
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting details of one module: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return module;
        }

        public int AddOneModule(Modules mod)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "INSERT INTO Modules(Modules_Code, Modules_Name, Modules_Description, Modules_LessonsQty, Modules_LessonStartDate, ClassTimings_ID, CreatedBy) VALUES (@modCode, @modName, @modDesc, @modQty, @startDate, @classID, @creator);";

            cmd.Parameters.Add("@modCode", SqlDbType.VarChar, 255).Value = mod.Module_Code;
            cmd.Parameters.Add("@modName", SqlDbType.VarChar, 255).Value = mod.Module_Name;
            cmd.Parameters.Add("@modDesc", SqlDbType.VarChar, 255).Value = mod.Module_Description;
            cmd.Parameters.Add("@modQty", SqlDbType.VarChar, 255).Value = mod.Module_LessonQty;
            cmd.Parameters.Add("@startDate", SqlDbType.VarChar, 255).Value = mod.Module_LessonStartDate;
            cmd.Parameters.Add("@classID", SqlDbType.Int).Value = mod.ClassTiming_RecordID;
            cmd.Parameters.Add("@creator", SqlDbType.Int).Value = mod.CreatedBy;

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }

            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error adding one module: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int AddOneCourseModule(int courseID, int modID, int createdBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "INSERT INTO Courses_Modules(Courses_ID, Modules_ID, CreatedBy) VALUES (@crseID, @moduleID, @creator);";

            cmd.Parameters.Add("@crseID", SqlDbType.Int).Value = courseID;
            cmd.Parameters.Add("@moduleID", SqlDbType.Int).Value = modID;
            cmd.Parameters.Add("@creator", SqlDbType.Int).Value = createdBy;

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error linking one module to course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int UpdateOneModuleQty(int qty, string moduleCode, int editedBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Modules SET Modules_LessonsQty = @qty, EditedBy = @editor, EditedOn = @editedOn WHERE IsDeleted = 0 AND Modules_Code = @code;";

            cmd.Parameters.Add("@qty", SqlDbType.Int).Value = qty;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = editedBy;
            cmd.Parameters.Add("@editedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            cmd.Parameters.Add("@code", SqlDbType.VarChar, 255).Value = moduleCode;

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }

            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error updating one module lesson quantity: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int UpdateOneModule(Modules mod)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Modules SET Modules_Code = @modCode, Modules_Name = @modName, Modules_Description = @modDesc, Modules_LessonsQty = @modQty, Modules_LessonStartDate = @startDate, ClassTimings_ID = @classID, EditedBy = @editor, EditedOn = @editedOn WHERE Modules_RecordID = @recordID AND IsDeleted = 0;";

            cmd.Parameters.Add("@recordID", SqlDbType.Int).Value = mod.Module_RecordID;
            cmd.Parameters.Add("@modCode", SqlDbType.VarChar, 255).Value = mod.Module_Code;
            cmd.Parameters.Add("@modName", SqlDbType.VarChar, 255).Value = mod.Module_Name;
            cmd.Parameters.Add("@modDesc", SqlDbType.VarChar, 255).Value = mod.Module_Description;
            cmd.Parameters.Add("@modQty", SqlDbType.VarChar, 255).Value = mod.Module_LessonQty;
            cmd.Parameters.Add("@startDate", SqlDbType.VarChar, 255).Value = mod.Module_LessonStartDate;
            cmd.Parameters.Add("@classID", SqlDbType.Int).Value = mod.ClassTiming_RecordID;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = mod.EditedBy;
            cmd.Parameters.Add("@editedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error updating one student: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int DeleteOneModule(int moduleRecordID, int deletedBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Modules SET IsDeleted = 1, DeletedBy = @editor, DeletedOn = @deletedOn WHERE IsDeleted = 0 AND Modules_RecordID = @recordID;";

            cmd.Parameters.Add("@recordID", SqlDbType.Int).Value = moduleRecordID;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = deletedBy;
            cmd.Parameters.Add("@deletedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error deleting one course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int DeleteOneCourseModule(int courseRecordID, int deletedBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Courses_Modules SET IsDeleted = 1, DeletedBy = @editor, DeletedOn = @deletedOn WHERE IsDeleted = 0 AND Courses_ID = @crseID;";

            cmd.Parameters.Add("@crseID", SqlDbType.Int).Value = courseRecordID;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = deletedBy;
            cmd.Parameters.Add("@deletedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error deleting one school course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int DeleteOneCourseModule(int moduleRecordID, int courseRecordID, int deletedBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Courses_Modules SET IsDeleted = 1, DeletedBy = @editor, DeletedOn = @deletedOn WHERE IsDeleted = 0 AND Courses_ID = @crseID AND Modules_ID = @modID;";

            cmd.Parameters.Add("@crseID", SqlDbType.Int).Value = courseRecordID;
            cmd.Parameters.Add("@modID", SqlDbType.Int).Value = moduleRecordID;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = deletedBy;
            cmd.Parameters.Add("@deletedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error deleting one school course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int DeleteOneCourseModuleByMod(int moduleRecordID, int deletedBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Courses_Modules SET IsDeleted = 1, DeletedBy = @editor, DeletedOn = @deletedOn WHERE IsDeleted = 0 AND Modules_ID = @modID;";

            cmd.Parameters.Add("@modID", SqlDbType.Int).Value = moduleRecordID;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = deletedBy;
            cmd.Parameters.Add("@deletedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error deleting one school course: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        /**********************************************************************************************************************************
                                                             CLASSES
        **********************************************************************************************************************************/
        public int CheckIfModuleClassTableExists(string moduleCode)
        {
            int count;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + moduleCode + "';";

            //cmd.Parameters.Add("@modCode", SqlDbType.VarChar,255).Value = moduleCode;
            try
            {
                cn.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting checking if class table exists: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }

        public int GetClassQuantity(string moduleCode)
        {
            int qty;
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) FROM " + moduleCode + " WHERE IsDeleted = 0;";

            try
            {
                cn.Open();
                qty = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting class timing record id: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return qty;
        }

        public List<Classes> GetAllClassTimings()
        {
            List<Classes> classList = new List<Classes>();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT * FROM ClassTimings WHERE IsDeleted = 0;";

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Classes c = new Classes()
                    {
                        ClassTiming_RecordID = int.Parse(dr["ClassTimings_RecordID"].ToString()),
                        ClassTiming_Name = dr["ClassTimings_Name"].ToString(),
                        ClassTiming_StartTime = Convert.ToDateTime(dr["ClassTimings_StartTime"].ToString()).ToString("hh:mm tt"),
                        ClassTiming_EndTime = Convert.ToDateTime(dr["ClassTimings_EndTime"].ToString()).ToString("hh:mm tt"),
                    };
                    classList.Add(c);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of all class timings: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return classList;
        }

        public List<Classes> GetAllModuleClasses(string modCode)
        {
            List<Classes> classList = new List<Classes>();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT m.ModuleClass_RecordID, m.ModuleClass_TimingID, m.ModuleClass_Date, m.ModuleClass_Location, ct.ClassTimings_Name, ct.ClassTimings_StartTime, ct.ClassTimings_EndTime FROM " + modCode + " m, ClassTimings ct " +
                                "WHERE m.ModuleClass_TimingID = ct.ClassTimings_RecordID AND m.IsDeleted = 0;";

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Classes c = new Classes()
                    {
                        ModuleClass_RecordID = int.Parse(dr["ModuleClass_RecordID"].ToString()),
                        ClassTiming_RecordID = int.Parse(dr["ModuleClass_TimingID"].ToString()),
                        ModuleClass_Date = Convert.ToDateTime(dr["ModuleClass_Date"].ToString()).ToString("dd MMM yyy"),
                        ModuleClass_Loc = dr["ModuleClass_Location"].ToString(),
                        ClassTiming_Name = dr["ClassTimings_Name"].ToString(),
                        ClassTiming_StartTime = Convert.ToDateTime(dr["ClassTimings_StartTime"].ToString()).ToString("hh:mm tt"),
                        ClassTiming_EndTime = Convert.ToDateTime(dr["ClassTimings_EndTime"].ToString()).ToString("hh:mm tt"),
                    };
                    classList.Add(c);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of all class timings: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return classList;
        }

        public Classes GetOneClassTimings(int classTimingRecordID)
        {
            Classes c = new Classes();
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT * FROM ClassTimings WHERE IsDeleted = 0 AND ClassTimings_RecordID = @id;";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = classTimingRecordID;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();

                c.ClassTiming_RecordID = int.Parse(dr["ClassTimings_RecordID"].ToString());
                c.ClassTiming_Name = dr["ClassTimings_Name"].ToString();
                c.ClassTiming_StartTime = Convert.ToDateTime(dr["ClassTimings_StartTime"].ToString()).ToString("hh:mm tt");
                c.ClassTiming_EndTime = Convert.ToDateTime(dr["ClassTimings_EndTime"].ToString()).ToString("hh:mm tt");
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of all class timings: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return c;
        }

        public Classes GetOneModuleClass(string moduleCode, int recordID)
        {
            Classes c = new Classes();
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT m.ModuleClass_TimingID, m.ModuleClass_Date, m.ModuleClass_Location, m.AttendanceCode, ct.ClassTimings_Name, ct.ClassTimings_StartTime, ct.ClassTimings_EndTime " +
                                "FROM " + moduleCode + " m, ClassTimings ct WHERE m.ModuleClass_TimingID = ct.ClassTimings_RecordID AND m.IsDeleted = 0 AND m.ModuleClass_RecordID = @id;";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = recordID;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();

                c.ClassTiming_RecordID = int.Parse(dr["ModuleClass_TimingID"].ToString());
                c.ClassTiming_Name = dr["ClassTimings_Name"].ToString();
                c.ModuleClass_Date = Convert.ToDateTime(dr["ModuleClass_Date"].ToString()).ToString("dd MMM yyy");
                c.ModuleClass_Loc = dr["ModuleClass_Location"].ToString();
                c.AttendanceCode = dr["AttendanceCode"].ToString();
                c.ClassTiming_StartTime = Convert.ToDateTime(dr["ClassTimings_StartTime"].ToString()).ToString("hh:mm tt");
                c.ClassTiming_EndTime = Convert.ToDateTime(dr["ClassTimings_EndTime"].ToString()).ToString("hh:mm tt");

            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting details of one module: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return c;
        }

        public int DropClassTable(string moduleCode)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "DROP TABLE " + moduleCode + ";";

            cmd.Parameters.Add("@moduleCode", SqlDbType.VarChar, 255).Value = moduleCode;
            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error dropping class table: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
            ;
        }

        public int CreateClassTable(string moduleCode)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "CREATE TABLE " + moduleCode + "(" +
                "       ModuleClass_RecordID int NOT NULL IDENTITY(1,1)," +
                "		ModuleClass_TimingID int NOT NULL," +
                "		ModuleClass_Date DateTime NOT NULL," +
                "       ModuleClass_Location varchar(max)," +
                "       AttendanceCode varchar(max)," +
                "       IsDeleted bit DEFAULT(0)," +
                "		CreatedBy int NOT NULL," +
                "		CreatedOn DateTime NOT NULL DEFAULT(getDate())," +
                "		EditedBy int," +
                "       EditedOn DateTime," +
                "       DeletedBy int," +
                "       DeletedOn DateTime" +
                "      Primary Key(ModuleClass_RecordID));";

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error creating class table: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }

        public int AddClasses(string moduleCode, int timingID, DateTime classDate, int createdBy)
        {
            int count;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "INSERT INTO " + moduleCode + "(ModuleClass_TimingID, ModuleClass_Date, CreatedBy) VALUES(@timingID, @date, @creator);";

            cmd.Parameters.Add("@timingID", SqlDbType.Int).Value = timingID;
            cmd.Parameters.Add("@date", SqlDbType.Date).Value = classDate;
            cmd.Parameters.Add("@creator", SqlDbType.Int).Value = createdBy;

            try
            {
                cn.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error checking if generating module class table: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }

        public int AddClasses(string moduleCode, int timingID, DateTime classDate, string attCode, int createdBy)
        {
            int count;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "INSERT INTO " + moduleCode + "(ModuleClass_TimingID, ModuleClass_Date, AttendanceCode, CreatedBy) VALUES(@timingID, @date, @code, @creator);";

            cmd.Parameters.Add("@timingID", SqlDbType.Int).Value = timingID;
            cmd.Parameters.Add("@date", SqlDbType.Date).Value = classDate;
            cmd.Parameters.Add("@code", SqlDbType.VarChar).Value = attCode;
            cmd.Parameters.Add("@creator", SqlDbType.Int).Value = createdBy;

            try
            {
                cn.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error checking if generating module class table: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }

        public int UpdateOneClass(Classes c, string moduleCode)
        {
            int count;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE " + moduleCode + " SET ModuleClass_TimingID = @timingID, ModuleClass_Date = @date, ModuleClass_Location = @loc, AttendanceCode=@attCode, EditedBy = @editor, EditedOn = @editedOn WHERE IsDeleted = 0 AND ModuleClass_RecordID = @id;";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = c.ModuleClass_RecordID;
            cmd.Parameters.Add("@timingID", SqlDbType.Int).Value = c.ClassTiming_RecordID;
            cmd.Parameters.Add("@date", SqlDbType.Date).Value = c.ModuleClass_Date;
            cmd.Parameters.Add("@loc", SqlDbType.VarChar, 255).Value = c.ModuleClass_Loc;
            cmd.Parameters.Add("@attCode", SqlDbType.VarChar, 255).Value = c.AttendanceCode;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = c.EditedBy;
            cmd.Parameters.Add("@editedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error checking if updating table: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }

        public int DeleteOneClass(string modCode, int modClassRecordID, int deletedBy)
        {
            int rowsAffected;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE " + modCode + " SET IsDeleted = 1, DeletedBy = @editor, DeletedOn = @deletedOn WHERE IsDeleted = 0 AND ModuleClass_RecordID = @id;";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = modClassRecordID;
            cmd.Parameters.Add("@editor", SqlDbType.Int).Value = deletedBy;
            cmd.Parameters.Add("@deletedOn", SqlDbType.DateTime).Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            try
            {
                cn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error deleting one class: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return rowsAffected;
        }



        /**********************************************************************************************************************************
                                                               STUDENT VIEW - Student_Modules
        **********************************************************************************************************************************/
        public Users_Students CheckIfAttendanceCodeExists(string modCode, string attendanceCode)
        {
            Users_Students us;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT ModuleClass_RecordID FROM " + modCode + "  WHERE IsDeleted = 0 AND AttendanceCode = @attCode;";

            cmd.Parameters.Add("@attCode", SqlDbType.VarChar, 255).Value = attendanceCode;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                us = new Users_Students
                {
                    ModuleClass_RecordID = int.Parse(dr["ModuleClass_RecordID"].ToString())
                };
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error checking if attendance code valid: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                if (ex.Message.Contains("no data is present"))
                {
                    return null;
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }
            cn.Close();

            return us;
        }

        public Users_Students GetAttendanceCodeStartEndDateTime(string modCode, int moduleClsRecordID)
        {
            Users_Students us;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT ct.ClassTimings_StartTime, ct.ClassTimings_EndTime, c.ModuleClass_Date FROM ClassTimings ct, " + modCode + " c " +
                "WHERE c.ModuleClass_TimingID = ct.ClassTimings_RecordID AND c.IsDeleted = 0 AND c.ModuleClass_RecordID = @id;";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = moduleClsRecordID;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                us = new Users_Students
                {
                    ClassTimings_StartTime = Convert.ToDateTime(dr["ClassTimings_StartTime"].ToString()).ToString("hh:mm tt"),
                    ClassTimings_EndTime = Convert.ToDateTime(dr["ClassTimings_EndTime"].ToString()).ToString("hh:mm tt"),
                    ModuleClass_Date = dr["ModuleClass_Date"].ToString()
                };

            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting start and end datetime of attendance code: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return us;
        }

        public int CheckIfAttendanceCodeIsValid(string modCode, string attendanceCode, DateTime now, DateTime startDt, DateTime endDt)
        {
            int count;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) FROM  " + modCode + " WHERE IsDeleted = 0 AND AttendanceCode = @attCode " +
                 "AND @now >= @start " +
                 "AND @now < DATEADD(minute, 1, @end);";

            cmd.Parameters.Add("@attCode", SqlDbType.VarChar, 255).Value = attendanceCode;
            cmd.Parameters.Add("@now", SqlDbType.DateTime).Value = now;
            cmd.Parameters.Add("@start", SqlDbType.DateTime).Value = startDt;
            cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = endDt;

            try
            {
                cn.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error checking if attendance code valid: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }

        public int TakeAttendance(int studentRecordID, int modRecordID, string modCode, int modClsRecordID, int createdBy)
        {
            int count;
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "INSERT Student_ClassAttendance (Student_RecordID, Module_RecordID, Module_Code, ModuleClass_RecordID, IsPresent, CreatedBy) " +
                "VALUES (@studID, @modRecordID, @modCode, @modClsID, 1, @creator);";

            cmd.Parameters.Add("@studID", SqlDbType.Int).Value = studentRecordID;
            cmd.Parameters.Add("@modRecordID", SqlDbType.Int).Value = modRecordID;
            cmd.Parameters.Add("@modCode", SqlDbType.VarChar, 255).Value = modCode;
            cmd.Parameters.Add("@modClsID", SqlDbType.Int).Value = modClsRecordID;
            cmd.Parameters.Add("@creator", SqlDbType.Int).Value = createdBy;

            try
            {
                cn.Open();
                count = Convert.ToInt32(cmd.ExecuteNonQuery());
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error taking attendance: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return count;
        }

        public List<Modules> GetEnrolledModuleList(int studentRecordID)
        {
            List<Modules> modList = new List<Modules>();
            cmd.Parameters.Clear();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT m.Modules_Code, m.Modules_Name FROM Student_Modules sm, Modules m " +
                "WHERE sm.Module_RecordID = m.Modules_RecordID AND sm.IsDeleted = 0 AND Student_RecordID = @id;";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = studentRecordID;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Modules m = new Modules()
                    {
                        Module_Code = dr["Modules_Code"].ToString(),
                        Module_Name = dr["Modules_Name"].ToString(),
                    };
                    modList.Add(m);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting list of module codes: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return modList;
        }

        public List<Users_Students> GetStudentClassesByModule(int studentRecordID, string modCode, string tableName)
        {
            List<Users_Students> classList = new List<Users_Students>();
            cmd.Parameters.Clear();
            cmd.Connection = cn;

            cmd.CommandText = "SELECT DISTINCT m.Modules_Code, m.Modules_Name, ct.ClassTimings_StartTime, ct.ClassTimings_EndTime, c.ModuleClass_Date, c.ModuleClass_Location" +
                " FROM Student_Modules sm, Modules m, ClassTimings ct, " + tableName + " c WHERE sm.Module_RecordID = m.Modules_RecordID" +
                " AND m.Modules_Code = '" + modCode + "'" +
                " AND c.ModuleClass_TimingID = ct.ClassTimings_RecordID" +
                " AND c.IsDeleted = 0 AND sm.Student_RecordID = @id;";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = studentRecordID;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Users_Students studClass = new Users_Students()
                    {
                        Module_Code = dr["Modules_Code"].ToString(),
                        Module_Name = dr["Modules_Name"].ToString(),
                        ClassTimings_StartTime = Convert.ToDateTime(dr["ClassTimings_StartTime"].ToString()).ToString("hh:mm tt"),
                        ClassTimings_EndTime = Convert.ToDateTime(dr["ClassTimings_EndTime"].ToString()).ToString("hh:mm tt"),
                        ModuleClass_Date = Convert.ToDateTime(dr["ModuleClass_Date"].ToString()).ToString("dd MMM yyy"),
                        ModuleClass_Location = dr["ModuleClass_Location"].ToString(),

                    };

                    classList.Add(studClass);
                }
            }
            catch (SqlException sqlEx)
            {
                cn.Close();
                Console.WriteLine("(SQL) Error getting student's list of classes: " + sqlEx.Message);
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {
                cn.Close();
                throw new Exception(ex.Message);
            }
            cn.Close();

            return classList;
        }
    }
}


