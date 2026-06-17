using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceShared.CommonModels;

public class PaginationModel
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }
}