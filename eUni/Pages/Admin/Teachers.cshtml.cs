using eUni.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace eUni.Pages.Admin
{
    public class TeachersModel : PageModel
    {
        public List<TeachersInfo> TeachersInfoList = new List<TeachersInfo>();

        public string Search = "";

        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // announcements per page 

        private readonly string connectionString;

        public TeachersModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("eUniDBConnection");
        }

        public void OnGet()
        {

            Search = Request.Query["search"];
            if (Search == null) Search = "";

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
                    string sqlCount = "SELECT COUNT(*) FROM Teachers";
                    if (Search.Length > 0)
                    {
                        sqlCount += " WHERE RegistrationNumber LIKE @search OR Name LIKE @search OR LastName LIKE @search";
                    }

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + Search + "%");

                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = "SELECT * FROM Teachers";
                    if (Search.Length > 0)
                    {
                        sql += " WHERE RegistrationNumber LIKE @search OR Name LIKE @search OR LastName LIKE @search";
                    }
                    sql += " ORDER BY LastName ASC";
                    sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + Search + "%");
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                TeachersInfo teacher = new TeachersInfo();
                                teacher.UserId = reader.GetGuid(reader.GetOrdinal("UserId"));
                                teacher.TeachersId = reader.GetGuid(reader.GetOrdinal("TeachersId"));
                                teacher.Name = reader.GetString(reader.GetOrdinal("Name"));
                                teacher.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                                teacher.Email = reader.GetString(reader.GetOrdinal("Email"));
                                teacher.PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"));
                                teacher.RegistrationNumber = reader.GetString(reader.GetOrdinal("RegistrationNumber"));
                                teacher.FathersName = reader.GetString(reader.GetOrdinal("FathersName"));
                                teacher.MothersName = reader.GetString(reader.GetOrdinal("MothersName"));
                                teacher.Department = reader.GetString(reader.GetOrdinal("Department"));
                                teacher.NumberOfCourses = reader.GetInt32(reader.GetOrdinal("NumberOfCourses"));
                                teacher.YearOfAdmission = reader.GetInt32(reader.GetOrdinal("YearOfAdmission"));

                                TeachersInfoList.Add(teacher);
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
