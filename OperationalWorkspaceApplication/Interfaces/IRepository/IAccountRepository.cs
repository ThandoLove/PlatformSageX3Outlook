using System;
using System.Collections.Generic;
using System.Text;
using OperationalWorkspace.Domain.Entities;

namespace OperationalWorkspaceApplication.Interfaces.IRepository;

public interface IAccountRepository
{
    Task<LoginUser?> FindAccountByUsernameAsync(string username);
}