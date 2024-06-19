using eUni.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace eUni.Pages.Student
{
    public class GradesModel : PageModel
    {
        public List<CourseInfo> CoursesList { get; set; } = new List<CourseInfo>();
        public List<DilwmenaMathimataInfo> DilwmenaMathimata = new List<DilwmenaMathimataInfo> { };
        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // announcements per page 
        public string Role = string.Empty;
        private readonly string connectionString;
        public int CreditsTotal;
        public decimal GradeAverage;

        public GradesModel(IConfiguration config)
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
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlCount = "SELECT COUNT(*) FROM DilwmenaCourses WHERE StudentsUserId = @UserId";

                    using (SqlCommand command = new SqlCommand(sqlCount, conn))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string dilwmenaSql = "SELECT * FROM DilwmenaCourses WHERE StudentsUserId = @UserId";
                    dilwmenaSql += " ORDER BY StudentsUserId DESC";
                    dilwmenaSql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand command = new SqlCommand(dilwmenaSql, conn))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DilwmenaMathimataInfo dilwmenaMathimata = new DilwmenaMathimataInfo();
                                dilwmenaMathimata.CourseId = reader.GetGuid(reader.GetOrdinal("CourseId"));
                                dilwmenaMathimata.StudentsUserId = reader.GetGuid(reader.GetOrdinal("StudentsUserId"));
                                dilwmenaMathimata.Grade = !reader.IsDBNull(reader.GetOrdinal("Grade")) ? reader.GetInt32(reader.GetOrdinal("Grade")) : null;
                                DilwmenaMathimata.Add(dilwmenaMathimata);
                            }
                        }
                    }

                    string CourseDetailSql = "SELECT * FROM Courses WHERE CourseId = @CourseId";


                    foreach (var item in DilwmenaMathimata)
                    {
                        using (SqlCommand command = new SqlCommand(CourseDetailSql, conn))
                        {
                            command.Parameters.AddWithValue("@CourseId", item.CourseId);

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

                int PerasmenaMathimataCount = 0;
                int GradesSum = 0;
                int creditsSum = 0;
                foreach(var item in DilwmenaMathimata)
                {
                    if(item.Grade >= 5)
                    {
                        PerasmenaMathimataCount++;
                        GradesSum = GradesSum + item.Grade.Value;

                        var course = CoursesList.Find(x => x.CourseId == item.CourseId);
                        creditsSum = creditsSum + course.Credits;
                    }
                }
                GradeAverage = GradesSum / PerasmenaMathimataCount;
                CreditsTotal = creditsSum;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }
}
