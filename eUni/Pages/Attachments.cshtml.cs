using eUni.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace eUni.Pages
{
    public class AttachmentsModel : PageModel
    {
        public List<AttachmentsInfo> AttachmentsList = new List<AttachmentsInfo>();

        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // announcements per page 
        public string Role = string.Empty;
        private readonly string connectionString;

        [BindProperty]
        [Required(ErrorMessage = "The File is required")]
        public IFormFile File { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        private IWebHostEnvironment webHostEnvironment;

        public AttachmentsModel(IConfiguration config, IWebHostEnvironment env)
        {
            connectionString = config.GetConnectionString("eUniDBConnection");
            webHostEnvironment = env;
        }

        public void OnGet()
        {
            page = 1;
            string requestPage = Request.Query["page"];
            Role = Request.Query["role"];
            string courseId = Request.Query["id"];
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
                    string sqlCount = "SELECT COUNT(*) FROM Attachments WHERE CourseId = @CourseId";

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@CourseId", courseId);
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = "SELECT * FROM Attachments WHERE CourseId = @CourseId";
                    sql += " ORDER BY Name DESC";
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
                                AttachmentsInfo attachment = new AttachmentsInfo();
                                attachment.AttachmentId = reader.GetGuid(reader.GetOrdinal("AttachmentId"));
                                attachment.Name = reader.GetString(reader.GetOrdinal("Name"));
                                attachment.ContentType = reader.GetString(reader.GetOrdinal("ContentType"));
                                attachment.FileName = reader.GetString(reader.GetOrdinal("FileName"));
                                attachment.CourseId = reader.GetGuid(reader.GetOrdinal("CourseId"));

                                AttachmentsList.Add(attachment);
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
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }

            string courseId = Request.Query["id"];
            // save the image file on the server
            string newFileName = Path.GetFileNameWithoutExtension(File.FileName) + "_" + courseId;
            newFileName += Path.GetExtension(File.FileName);

            string filesFolder = webHostEnvironment.WebRootPath + "/files/";

            string fileFullPath = Path.Combine(filesFolder, newFileName);
            Console.WriteLine("New image: " + fileFullPath);

            using (var stream = System.IO.File.Create(fileFullPath))
            {
                File.CopyTo(stream);
            }

            // save the new book in the database
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Attachments " +
                    "(AttachmentId,Name,ContentType,CourseId,FileName) VALUES " +
                    "(@AttachmentId, @Name, @ContentType, @CourseId, @FileName);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AttachmentId", Guid.NewGuid().ToString());
                        command.Parameters.AddWithValue("@Name", File.FileName);
                        command.Parameters.AddWithValue("@ContentType", File.ContentType);
                        command.Parameters.AddWithValue("@CourseId", courseId);
                        command.Parameters.AddWithValue("@FileName", newFileName);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            successMessage = "Data saved correctly";
            Response.Redirect("/Attachments?id="+ courseId);
        }
    }
}
