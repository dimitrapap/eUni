using eUni.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace eUni.Pages.Teacher
{
    public class AddGradesModel : PageModel
    {
        [BindProperty]
        public List<DilwmenaMathimataInfo> DilwmenaMathimata { get; set; } = new List<DilwmenaMathimataInfo> { };
        public List<StudentsInfo> StudentsInfoList = new List<StudentsInfo>();
        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // announcements per page 
        private readonly string connectionString;

        public AddGradesModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("eUniDBConnection");
        }
        public void OnGet()
        {
            page = 1;
            string requestPage = Request.Query["page"];
            string courseId = Request.Query["id"];
            string userId = HttpContext.Session.GetString("UserId");
            if (requestPage != null)
            {
                try
                {
                    page = int.Parse(requestPage);
                }
                catch (Exception ex)
                {
                    page = 1;
                }
            }
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlCount = "SELECT COUNT(*) FROM DilwmenaCourses WHERE CourseId = @CourseId AND Grade < 5";

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@CourseId", courseId);
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = "SELECT * FROM DilwmenaCourses WHERE CourseId = @CourseId";
                    sql += " ORDER BY StudentsUserId DESC";
                    sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@CourseId", courseId);
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                DilwmenaMathimataInfo course = new DilwmenaMathimataInfo();
                                course.CourseId = reader.GetGuid(reader.GetOrdinal("CourseId"));
                                course.StudentsUserId = reader.GetGuid(reader.GetOrdinal("StudentsUserId"));
                                course.Grade = !reader.IsDBNull(reader.GetOrdinal("Grade")) ? reader.GetInt32(reader.GetOrdinal("Grade")) : null;
                                DilwmenaMathimata.Add(course);
                            }
                        }
                    }

                    string studentSql = "SELECT * FROM Students WHERE UserId = @UserId";

                    foreach(var item in DilwmenaMathimata) 
                    {
                        using (SqlCommand command = new SqlCommand(studentSql, connection))
                        {
                            command.Parameters.AddWithValue("@UserId", item.StudentsUserId);

                            using(SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    StudentsInfo students = new StudentsInfo();
                                    students.UserId = reader.GetGuid(reader.GetOrdinal("UserId"));
                                    students.StudentsId = reader.GetGuid(reader.GetOrdinal("StudentsId"));
                                    students.Name = reader.GetString(reader.GetOrdinal("Name"));
                                    students.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                                    students.Email = reader.GetString(reader.GetOrdinal("Email"));
                                    students.PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"));
                                    students.RegistrationNumber = reader.GetString(reader.GetOrdinal("RegistrationNumber"));
                                    students.FathersName = reader.GetString(reader.GetOrdinal("FathersName"));
                                    students.MothersName = reader.GetString(reader.GetOrdinal("MothersName"));
                                    students.Department = reader.GetString(reader.GetOrdinal("Department"));
                                    students.Specialization = !reader.IsDBNull(reader.GetOrdinal("Specialization")) ? reader.GetString(reader.GetOrdinal("Specialization")) : string.Empty;
                                    students.YearOfAdmission = reader.GetInt32(reader.GetOrdinal("YearOfAdmission"));
                                    students.CurrentSemester = reader.GetInt32(reader.GetOrdinal("CurrentSemester"));

                                    StudentsInfoList.Add(students);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public void OnPost()
        {
            try
            {
                string courseId = Request.Query["id"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (var item in DilwmenaMathimata)
                    {
                        string sql = "UPDATE DilwmenaCourses SET Grade = @Grade WHERE StudentsUserId = @StudentsUserId AND CourseId = @CourseId";

                        using (SqlCommand command1 = new SqlCommand(sql, connection))
                        {
                            command1.Parameters.AddWithValue("@Grade", item.Grade);
                            command1.Parameters.AddWithValue("@StudentsUserId", item.StudentsUserId);
                            command1.Parameters.AddWithValue("@CourseId", courseId);
                            

                            command1.ExecuteNonQuery();
                        }

                    }
                }
                Response.Redirect("/Teacher/AddGrades?id=" + courseId.ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }
}
