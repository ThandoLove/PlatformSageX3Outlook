using Microsoft.Extensions.Configuration;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces;
using OperationalWorkspaceInfrastructure.Attachments;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceInfrastructure.Providers;

public class SmartAttachmentProvider : IAttachmentProvider
{
    private readonly IConfiguration _config;

    private readonly MockAttachmentProvider _mock;

    private readonly SageAttachmentProvider _sage;

    public SmartAttachmentProvider(
        IConfiguration config,
        MockAttachmentProvider mock,
        SageAttachmentProvider sage)
    {
        _config = config;

        _mock = mock;

        _sage = sage;
    }

    public async Task<List<AttachmentDto>> GetAttachmentsAsync(
        string ownerType,
        string ownerId,
        CancellationToken cancellationToken)
    {
        bool useMock =
            _config.GetValue<bool>("SageX3:UseMockData");

        if (useMock)
        {
            return await _mock.GetAttachmentsAsync(
                ownerType,
                ownerId,
                cancellationToken);
        }

        return await _sage.GetAttachmentsAsync(
            ownerType,
            ownerId,
            cancellationToken);
    }
}