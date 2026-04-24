namespace OperationalWorkspaceUI.Models.Forms
{
    // CODE START
    public class CreateTaskModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
   
        public DateTime? DueDate { get; set; } = DateTime.Now;
        public string AssignedTo { get; set; } = string.Empty;
        public string StatusDescription { get; set; } = string.Empty;
    }
    // CODE END
}
