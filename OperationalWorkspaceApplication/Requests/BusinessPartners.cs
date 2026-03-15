using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Requests
{
public sealed record GetBusinessPartnerSnapshotRequest(string page, string PageSize, string BpCode);
public sealed record UpdateCreditLimitRequest(string BpCode, decimal NewLimit);
}