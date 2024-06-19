using eUni.Helpers;
using eUni.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace eUni.Pages.Admin
{
    [RequireAuth]
    public class AnnouncementsModel : PageModel
    {
        public List<AnnouncementsInfo> AnnouncementsList = new List<AnnouncementsInfo>();

        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // announcements per page 

        public string Role = string.Empty;

        private readonly string connectionString;

        public AnnouncementsModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("eUniDBConnection");
        }
        public void OnGet()
        {
            page = 1;
            Role = HttpContext.Session.GetString("role");
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
                    string sqlCount = "SELECT COUNT(*) FROM Announcements";

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = "SELECT * FROM Announcements";
                    sql += " ORDER BY CreatedOn DESC";
                    sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                AnnouncementsInfo announcement = new AnnouncementsInfo();
                                announcement.AnnouncementId = reader.GetGuid(reader.GetOrdinal("AnnouncementId"));
                                announcement.Title = reader.GetString(reader.GetOrdinal("Title"));
                                announcement.Content = reader.GetString(reader.GetOrdinal("Content"));
                                announcement.Author = (AuthorEnum)reader.GetInt32(reader.GetOrdinal("Author"));
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
