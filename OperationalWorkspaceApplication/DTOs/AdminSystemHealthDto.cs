using System;
using System.Collections.Generic;
using System.Text;


namespace OperationalWorkspaceApplication.DTOs;

public class AdminSystemHealthDto
{
    public bool SageX3Connected { get; set; }

    public bool DatabaseConnected { get; set; }

    public string APIHealthStatus { get; set; } = string.Empty;

    public int FailedTransactions { get; set; }

    public int PendingSyncJobs { get; set; }

    public int ApiResponseTimeMs { get; set; }

    public DateTime LastCheckedUtc { get; set; }
}