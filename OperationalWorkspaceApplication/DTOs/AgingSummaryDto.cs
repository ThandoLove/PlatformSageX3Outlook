using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs
{
public class AgingSummaryDto
{
        public decimal Current { get; set; }
        public decimal Bucket30 { get; set; } // Renamed from Days30
        public decimal Bucket60 { get; set; } // Renamed from Days60
        public decimal Bucket90 { get; set; } // Renamed from Days90
        public decimal Bucket120Plus { get; set; }
        public decimal Total { get; set; }
    }
}