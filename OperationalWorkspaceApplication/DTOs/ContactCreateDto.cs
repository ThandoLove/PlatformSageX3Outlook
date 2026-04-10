using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace OperationalWorkspaceApplication.DTOs

{
    public class ContactCreateDto
    {
        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company is required")]
        public string Company { get; set; } = string.Empty;

        // ReadOnly in UI, but sent to API
        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;

        // Split for Sage X3 address mapping
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = "USA";

        public string SalesRepId { get; set; } = string.Empty;

        // Metadata for Sage X3
        public string Category { get; set; } = "BPC"; // Business Partner Customer
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
