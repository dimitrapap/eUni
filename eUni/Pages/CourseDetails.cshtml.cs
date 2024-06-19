using eUni.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace eUni.Pages
{
    public class CourseDetailsModel : PageModel
    {
        public CourseInfo Course = new CourseInfo();
        public TeachersInfo Teacher = new TeachersInfo();
        public List<CourseAnnouncementInfo> AnnouncementsList = new List<CourseAnnouncementInfo>();
        public string Role = string.Empty;
        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // announcements per page 

        private readonly string connectionString;

        public CourseDetailsModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("eUniDBConnection");
        }
        public void OnGet()
        {
            
            string id = Request.Query["id"];
            Role = Request.Query["role"];
            page = 1;
            string requestPage = Request.Query["page"];
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

                    string sql = "SELECT * FROM Courses WHERE CourseId = @CourseId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@CourseId", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Course.CourseId = reader.GetGuid(reader.GetOrdinal("CourseId"));
                                Course.Title = reader.GetString(reader.GetOrdinal("Title"));
                                Course.TeachersId = reader.GetGuid(reader.GetOrdinal("TeachersId"));
                                Course.Semester = reader.GetInt32(reader.GetOrdinal("Semester"));
                                Course.Base = reader.GetInt32(reader.GetOrdinal("Base"));
                                Course.CodeCourse = reader.GetString(reader.GetOrdinal("CodeCourse"));
                                Course.CourseType = reader.GetString(reader.GetOrdinal("CourseType"));
                                Course.Department = reader.GetString(reader.GetOrdinal("Department"));
                                Course.Credits = reader.GetInt32(reader.GetOrdinal("Credits"));
                                Course.CourseDetails = reader.GetString(reader.GetOrdinal("CourseDetails"));
                            }
                        }
                    }

                    string sqlTeacher = "SELECT * FROM Teachers WHERE TeachersId = @TeachersId";

                    using(SqlCommand command = new SqlCommand(sqlTeacher, connection))
                    {
                        command.Parameters.AddWithValue("@TeachersId", Course.TeachersId);
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Teacher.UserId = reader.GetGuid(reader.GetOrdinal("UserId"));
                                Teacher.TeachersId = reader.GetGuid(reader.GetOrdinal("TeachersId"));
                                Teacher.Name = reader.GetString(reader.GetOrdinal("Name"));
                                Teacher.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                                Teacher.Email = reader.GetString(reader.GetOrdinal("Email"));
                                Teacher.PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"));
                                Teacher.RegistrationNumber = reader.GetString(reader.GetOrdinal("RegistrationNumber"));
                                Teacher.FathersName = reader.GetString(reader.GetOrdinal("FathersName"));
                                Teacher.MothersName = reader.GetString(reader.GetOrdinal("MothersName"));
                                Teacher.Department = reader.GetString(reader.GetOrdinal("Department"));
                                Teacher.NumberOfCourses = reader.GetInt32(reader.GetOrdinal("NumberOfCourses"));
                                Teacher.YearOfAdmission = reader.GetInt32(reader.GetOrdinal("YearOfAdmission"));
                            }
                        }
                    }

                    string sqlCount = "SELECT COUNT(*) FROM CourseAnnouncement";

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sqlAnnouncement = "SELECT * FROM CourseAnnouncement where CourseId = @CourseId";
                    sqlAnnouncement += " ORDER BY CreatedOn DESC";
                    sqlAnnouncement += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";
                    using (SqlCommand command = new SqlCommand(sqlAnnouncement, connection))
                    {
                        command.Parameters.AddWithValue("@CourseId", id);
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CourseAnnouncementInfo announcement = new CourseAnnouncementInfo();
                                announcement.CourseAnnouncementId = reader.GetGuid(reader.GetOrdinal("CourseAnnouncementId"));
                                announcement.Title = reader.GetString(reader.GetOrdinal("Title"));
                                announcement.Content = reader.GetString(reader.GetOrdinal("Content"));
                                announcement.CourseId = reader.GetGuid(reader.GetOrdinal("CourseId"));
                                announcement.CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn"));

                                AnnouncementsList.Add(announcement);
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
