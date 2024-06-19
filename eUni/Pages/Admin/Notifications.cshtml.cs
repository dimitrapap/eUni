using eUni.Helpers;
using eUni.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace eUni.Pages.Admin
{
    [RequireAuth]
    public class NotificationsModel : PageModel
    {
        [BindProperty]
        public List<NotificationInfo> NotificationsList { get; set; } = new List<NotificationInfo>();
        
        [BindProperty]
        public IFormFile File { get; set; }
        [BindProperty]
        public int NotificationId { get; set; }
        [BindProperty]
        public ReportsEnum Type { get; set; }
        [BindProperty]
        public DateTime CreatedOn { get; set; }
        [BindProperty]
        public string ApplicantsRN { get; set; }
        public int page = 1;
        public int totalPages = 0;
        private readonly int pageSize = 5;

        private IWebHostEnvironment webHostEnvironment;

        private readonly string connectionString;

        public NotificationsModel(IConfiguration config, IWebHostEnvironment env)
        {
            connectionString = config.GetConnectionString("eUniDBConnection");
            webHostEnvironment = env;
        }

        public void OnGet()
        {
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
                    string sqlCount = "SELECT COUNT(*) FROM RequestReports";

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = "SELECT * FROM RequestReports";
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
                                NotificationInfo notification = new NotificationInfo();
                                notification.NotificationId = reader.GetInt32(reader.GetOrdinal("RequestId"));
                                notification.Type = (ReportsEnum)reader.GetInt32(reader.GetOrdinal("Type"));
                                notification.ApplicantsRN = reader.GetString(reader.GetOrdinal("ApplicantsRN"));
                                notification.CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn"));
                                notification.Completed = reader.GetBoolean(reader.GetOrdinal("Completed"));

                                NotificationsList.Add(notification);
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
            string newFileName = Type + "_" + NotificationId + "_" + ApplicantsRN;
            newFileName += Path.GetExtension(File.FileName);

            string filesFolder = webHostEnvironment.WebRootPath + "/files/Applications";

            string fileFullPath = Path.Combine(filesFolder, newFileName);
            Console.WriteLine("New File Application: " + fileFullPath);

            using (var stream = System.IO.File.Create(fileFullPath))
            {
                File.CopyTo(stream);
            }

            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE RequestReports " +
                    "SET Completed = 1" +
                    "WHERE RequestId = @id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", NotificationId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                //errorMessage = ex.Message;
                return;
            }

            //successMessage = "Data saved correctly";
            Response.Redirect("/Admin/Notifications");
        }
    }

}
