using System;

public class FormController : SurfaceController
{
    public ActionResult RenderForm()
    {
        return View("Form"); // Create the "Form" view
    }

    [HttpPost]
    public ActionResult HandleFormSubmit(FormViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Process form data and send email
            // ...

            // Create a new content node based on "FormSubmission" Document Type
            var contentService = Services.ContentService;
            var submissionNode = contentService.CreateContent(model.Name, CurrentPage.Id, "formSubmissionAlias");
            submissionNode.SetValue("propertyAliasForName", model.Name);
            submissionNode.SetValue("propertyAliasForEmail", model.Email);
            submissionNode.SetValue("propertyAliasForMessage", model.Message);
            contentService.SaveAndPublishWithStatus(submissionNode);


            // Insert the form data into the custom database table
            var connectionString = "your_connection_string_here";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "INSERT INTO FormSubmissions (Name, Email, Message, SubmissionDate) VALUES (@Name, @Email, @Message, @SubmissionDate)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", model.Name);
                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@Message", model.Message);
                    command.Parameters.AddWithValue("@SubmissionDate", DateTime.UtcNow);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

            return RedirectToUmbracoPage(123); // Redirect to a thank you page
        }

        return CurrentUmbracoPage();
    }
}
