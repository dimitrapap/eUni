using eUni.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace eUni.Pages.Student
{
    public class RequestsModel : PageModel
    {
        [BindProperty]
        public List<NotificationInfo> NotificationsList { get; set; } = new List<NotificationInfo>();
        [BindProperty]
        public ReportsEnum requestReport { get; set; }
        public int page = 1;
        public int totalPages = 0;
        private readonly int pageSize = 5;

        public string errorMessage = "";
        public string successMessage = "";

        private readonly string connectionString;

        public RequestsModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("eUniDBConnection");
        }

        public void OnGet()
        {
            page = 1;
            string requestPage = Request.Query["page"];
             string registrationNumber = HttpContext.Session.GetString("registrationNumber");
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
                    string sqlCount = "SELECT COUNT(*) FROM RequestReports WHERE ApplicantsRN = @registrationNumber";

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@registrationNumber", registrationNumber);
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = "SELECT * FROM RequestReports WHERE ApplicantsRN = @registrationNumber";
                    sql += " ORDER BY CreatedOn DESC";
                    sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@registrationNumber", registrationNumber);
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
            var typosBebaiwsis = Request.Form["requestReport"];
            string registrationNumber = HttpContext.Session.GetString("registrationNumber");

            int type = 1;
            if(ReportsEnum.BebaiwsiPeratwsisSpoudwn.ToString() == typosBebaiwsis[0])
            {
                type = (int)ReportsEnum.BebaiwsiPeratwsisSpoudwn;
            }
            else if(ReportsEnum.BebaiwsiSpoudwn.ToString() == typosBebaiwsis[0])
            {
                type = (int)ReportsEnum.BebaiwsiSpoudwn;
            }
            else if(ReportsEnum.AnalytikiBathmologia.ToString() == typosBebaiwsis[0])
            {
                type = (int)ReportsEnum.AnalytikiBathmologia;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "INSERT INTO RequestReports " +
                            "VALUES " +
                            "(@Type, @CreatedOn, @ApplicantsRN, @Completed);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Type", type);
                        command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        command.Parameters.AddWithValue("@ApplicantsRN", registrationNumber);
                        command.Parameters.AddWithValue("@Completed", 0);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            successMessage = "? ?????? ???????????? ????????!";
            Response.Redirect("/Student/Requests");
        }
    }
}
