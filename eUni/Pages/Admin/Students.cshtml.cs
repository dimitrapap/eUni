using eUni.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace eUni.Pages.Admin
{
    public class StudentsModel : PageModel
    {
        public List<StudentsInfo> StudentsInfoList = new List<StudentsInfo>();

        public string Search = "";

        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // announcements per page 

        private readonly string connectionString;

        public StudentsModel(IConfiguration config)
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
                    string sqlCount = "SELECT COUNT(*) FROM Students";
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

                    string sql = "SELECT * FROM Students";
                    if (Search.Length > 0)
                    {
                        sqlCount += " WHERE RegistrationNumber LIKE @search OR Name LIKE @search OR LastName LIKE @search";
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }
}
