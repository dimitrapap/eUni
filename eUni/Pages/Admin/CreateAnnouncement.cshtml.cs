using eUni.Helpers;
using eUni.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace eUni.Pages.Admin
{
    [RequireAuth]
    [BindProperties]
    public class CreateAnnouncementModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Ο τίτλος είναι υποχρεωτικός")]
        [MaxLength(500, ErrorMessage = "Ο τίτλος δεν μπορεί να υπερβαίνει τους 100 χαρακτήρες")]
        public string Title { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Το περιεχόμενο είναι υποχρεωτικό")]
        public string AnnouncementContent { get; set; } = "";

        public AuthorEnum Author { get; set; }
        public DateTime CreatedOn { get; set; }

        public string errorMessage = "";
        public string successMessage = "";


        private readonly string connectionString;

        public CreateAnnouncementModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("eUniDBConnection");
        }
        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Η αποθήκευση νέας ανακοίνωσης παρουσίασε σφάλμα!";
                return;
            }
            //insert in database

            string requestId = Request.Query["id"];
            if (string.IsNullOrEmpty(requestId))
            {
                try
                {

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "INSERT INTO Announcements " +
                        "VALUES " +
                        "(@id, @title, @content, @author, @createdOn);";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@id", Guid.NewGuid());
                            command.Parameters.AddWithValue("@title", Title);
                            command.Parameters.AddWithValue("@content", AnnouncementContent);
                            command.Parameters.AddWithValue("@author", (int)AuthorEnum.Grammateia);

                            command.Parameters.AddWithValue("@createdOn", DateTime.Now);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return;
                }

                successMessage = "Η νέα ανακοίνωση αποθηκεύτηκε επιτυχώς!";
                Response.Redirect("/Admin/Announcements");
            }
            else
            {
                try
                {

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "INSERT INTO CourseAnnouncement " +
                        "VALUES " +
                        "(@id, @title, @content, @coursId, @createdOn);";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@id", Guid.NewGuid());
                            command.Parameters.AddWithValue("@title", Title);
                            command.Parameters.AddWithValue("@content", AnnouncementContent);
                            command.Parameters.AddWithValue("@coursId", requestId);
                            command.Parameters.AddWithValue("@createdOn", DateTime.Now);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return;
                }

                successMessage = "Η νέα ανακοίνωση αποθηκεύτηκε επιτυχώς!";
                Response.Redirect("/CourseDetails?id="+requestId.ToString());
            }
            
        }
    }
}
