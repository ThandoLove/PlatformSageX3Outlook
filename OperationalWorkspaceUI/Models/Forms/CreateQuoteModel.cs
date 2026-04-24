namespace OperationalWorkspaceUI.Models.Forms
{
    public class CreateQuoteModel
    {
        // For identifying the client from the email context
        public string ClientEmail { get; set; } = string.Empty;

        // The Sage X3 Business Partner code
        public string BusinessPartnerCode { get; set; } = string.Empty;

        // The reference name for the quote
        public string Name { get; set; } = string.Empty;

        // Description pulled from email subject or manual entry
        public string Description { get; set; } = string.Empty;

        // Financial details
        public decimal Amount { get; set; }

        public string PaymentTerms { get; set; } = "Net30";

        // Required for the FluentDatePicker binding (must be nullable DateTime?)
        public DateTime? ValidUntil { get; set; } = DateTime.Today.AddDays(30);
    }
}
