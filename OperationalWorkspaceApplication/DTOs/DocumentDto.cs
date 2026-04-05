using System;
using System.Collections.Generic;
using System.Text;


namespace OperationalWorkspaceApplication.DTOs;

public class DocumentDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty; // .pdf, .docx
    public string Size { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string Source { get; set; } = "Sage X3"; // ERP or CRM
}
