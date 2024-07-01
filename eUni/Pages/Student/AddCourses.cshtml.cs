using eUni.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace eUni.Pages.Student
{
    public class AddCoursesModel : PageModel
    {
        [BindProperty]
        public List<CourseInfo> CoursesList { get; set; } = new List<CourseInfo>();
        public List<DilwmenaMathimataInfo> DilwmenaMathimata = new List<DilwmenaMathimataInfo> { }; 
        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // announcements per page 
        public string Role = string.Empty;
        private readonly string connectionString;

        public AddCoursesModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("eUniDBConnection");
        }

        public void OnGet()
        {
            page = 1;
            string requestPage = Request.Query["page"];
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

                    string sqlCount = "SELECT COUNT(*) FROM Courses";

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = "SELECT * FROM Courses";
                    sql += " ORDER BY Title DESC";
                    sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                CourseInfo course = new CourseInfo();
                                course.CourseId = reader.GetGuid(reader.GetOrdinal("CourseId"));
                                course.Title = reader.GetString(reader.GetOrdinal("Title"));
                                course.TeachersId = reader.GetGuid(reader.GetOrdinal("TeachersId"));
                                course.Semester = reader.GetInt32(reader.GetOrdinal("Semester"));
                                course.Base = reader.GetInt32(reader.GetOrdinal("Base"));
                                course.CodeCourse = reader.GetString(reader.GetOrdinal("CodeCourse"));
                                course.CourseType = reader.GetString(reader.GetOrdinal("CourseType"));
                                course.Department = reader.GetString(reader.GetOrdinal("Department"));
                                course.Credits = reader.GetInt32(reader.GetOrdinal("Credits"));
                                course.CourseDetails = reader.GetString(reader.GetOrdinal("CourseDetails"));

                                CoursesList.Add(course);
                            }
                        }
                    }

                    string dilwmenaSql = "SELECT * FROM DilwmenaCourses WHERE StudentsUserId = @userid";

                    using (SqlCommand command = new SqlCommand(dilwmenaSql, connection))
                    {
                        command.Parameters.AddWithValue("@userid", userId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                var courseId = reader.GetGuid(reader.GetOrdinal("CourseId"));
                                foreach (var item in CoursesList)
                                {
                                    if (item.CourseId == courseId)
                                    {
                                        item.Checked = true;
                                    }
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
            string userId = HttpContext.Session.GetString("UserId");
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (var item in CoursesList)
                    {
                        bool exists = false;
                        if (item.Checked)
                        {
                            string sqlExists = "SELECT CourseId FROM DilwmenaCourses WHERE CourseId = @CourseId and StudentsUserId = @userId";

                            using (SqlCommand command = new SqlCommand(sqlExists, connection))
                            {
                                command.Parameters.AddWithValue("@CourseId", item.CourseId);
                                command.Parameters.AddWithValue("@UserId", userId);

                                using (SqlDataReader reader = command.ExecuteReader())
                                {

                                    if (reader.Read())
                                    {
                                        exists = true;
         
                                    }
                                }
                            }

                            if (!exists)
                            {
                                string sql = "INSERT INTO DilwmenaCourses " +
                                                                            "VALUES " +
                                                                            "(@StudentsUserId, @CourseId, @grade);";

                                using (SqlCommand command1 = new SqlCommand(sql, connection))
                                {
                                    command1.Parameters.AddWithValue("@StudentsUserId", userId);
                                    command1.Parameters.AddWithValue("@CourseId", item.CourseId);
                                    command1.Parameters.AddWithValue("@grade", 0);

                                    command1.ExecuteNonQuery();
                                }
                            }
                            


                        }
                        
                    }
                }
                Response.Redirect("/Student/AddCourses");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            
        }

    }
}
