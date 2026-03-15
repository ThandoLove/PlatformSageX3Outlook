using OperationalWorkspaceApplication.DTOs;


namespace OperationalWorkspaceApplication.Responses;


public sealed record BusinessPartnersResponse(BusinessPartnerSnapshotDto Snapshot);
public sealed record UpdateCreditLimitResponse(bool Success);


public record BusinessPartnerSnapshotResponse(BusinessPartnerSnapshotDto Data);



