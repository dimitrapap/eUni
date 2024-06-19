using eUni.Helpers;
using eUni.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace eUni.Pages.Teacher
{
    [RequireAuth]
    public class CoursesModel : PageModel
    {
        public List<CourseInfo> CoursesList = new List<CourseInfo>();
        public List<DilwmenaMathimataInfo> DilwmenaMathimataList = new List<DilwmenaMathimataInfo>();
        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // announcements per page 
        public string Role = string.Empty;
        private readonly string connectionString;

        public CoursesModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("eUniDBConnection");
        }

        public void OnGet()
        {
            page = 1;
            string requestPage = Request.Query["page"];
            string userId = HttpContext.Session.GetString("UserId");
            Guid teachersId = Guid.Empty;
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

                    string sqlTeacher = "SELECT TeachersId FROM Teachers WHERE UserId = @UserId";
                    using(SqlCommand command = new SqlCommand(sqlTeacher, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                teachersId = reader.GetGuid(reader.GetOrdinal("TeachersId"));
                            }
                        }
                    }

                    if(teachersId == Guid.Empty)
                    {
                        Role = "Mathitis";
                        string sqlMathimata = "SELECT * FROM DilwmenaCourses WHERE StudentsUserId = @UserId";
                        using(SqlCommand command = new SqlCommand( sqlMathimata, connection))
                        {
                            command.Parameters.AddWithValue("@UserId", userId);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    DilwmenaMathimataInfo dilwmenaMathimata = new DilwmenaMathimataInfo();
                                    dilwmenaMathimata.StudentsUserId = reader.GetGuid(reader.GetOrdinal("StudentsUserId"));
                                    dilwmenaMathimata.CourseId = reader.GetGuid(reader.GetOrdinal("CourseId"));

                                    DilwmenaMathimataList.Add(dilwmenaMathimata);
                                }
                            }
                        }
                        totalPages = (DilwmenaMathimataList.Count + pageSize - 1) / pageSize;
                        foreach (var item in DilwmenaMathimataList)
                        {
                            string sql = "SELECT * FROM Courses WHERE CourseId = @CourseId";
                            sql += " ORDER BY Title DESC";
                            sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@CourseId", item.CourseId);
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
                        }
                    }
                    else{

                        Role = "Kathgitis";
                        string sqlCount = "SELECT COUNT(*) FROM Courses WHERE TeachersId = @TeachersId";

                        using (SqlCommand command = new SqlCommand(sqlCount, connection))
                        {
                            command.Parameters.AddWithValue("@TeachersId", teachersId.ToString());
                            decimal count = (int)command.ExecuteScalar();
                            totalPages = (int)Math.Ceiling(count / pageSize);
                        }

                        string sql = "SELECT * FROM Courses WHERE TeachersId = @TeachersId";
                        sql += " ORDER BY Title DESC";
                        sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@TeachersId", teachersId.ToString());
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
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }
}
