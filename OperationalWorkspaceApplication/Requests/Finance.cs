using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Requests
{

    public sealed record GetAgingReportRequest(Guid BusinessPartnerId);

    public sealed record RegisterPaymentRequest(
        Guid BusinessPartnerId,
        decimal Amount);
}
