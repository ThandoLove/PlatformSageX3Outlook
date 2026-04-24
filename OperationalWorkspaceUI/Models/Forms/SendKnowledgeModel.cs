using System.ComponentModel.DataAnnotations;

namespace OperationalWorkspaceUI.Models.Forms
{
 

    public class SendKnowledgeModel
    {
        [Required(ErrorMessage = "Search keywords are mandatory to find the correct Sage X3 article.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Search query must be between 3 and 100 characters.")]
        public string SearchQuery { get; set; } = string.Empty;

        [Required(ErrorMessage = "A personal message provides context for the recipient.")]
        [StringLength(1000, ErrorMessage = "Personal message cannot exceed 1000 characters.")]
        public string Message { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Please provide a valid email format for the recipient.")]
        public string? RecipientEmail { get; set; }

        public bool IncludeLink { get; set; } = true;
        public bool TrackOpen { get; set; } = true;
    }
}
