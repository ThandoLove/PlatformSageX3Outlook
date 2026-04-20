using System;
using System.Collections.Generic;
using System.Text;

    namespace OperationalWorkspaceApplication.DTOs;

    public class ReportDto
    {
        public string Code { get; set; } = string.Empty; // e.g., "BONLIV"
        public string Name { get; set; } = string.Empty; // e.g., "Daily Sales Orders"
        public string Category { get; set; } = "General";
    }


