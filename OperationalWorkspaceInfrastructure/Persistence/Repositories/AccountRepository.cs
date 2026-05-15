using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public class AccountRepository : IAccountRepository
{
    // =========================================================
    // 🗄 DATABASE TABLE: UserAccounts
    // =========================================================
    /*
        TABLE: UserAccounts

        Columns:
        --------------------------------------------------------
        Id            UNIQUEIDENTIFIER   PRIMARY KEY
        Username      NVARCHAR(100)      UNIQUE NOT NULL
        Email         NVARCHAR(255)      NOT NULL
        PasswordHash  NVARCHAR(MAX)      NOT NULL
        Role          NVARCHAR(50)       NOT NULL
        IsActive      BIT                NOT NULL DEFAULT 1
        CreatedAtUtc  DATETIME           NOT NULL
        UpdatedAtUtc  DATETIME           NULL
    */

    // =========================================================
    // 🗄 DATABASE TABLE: RefreshTokens
    // =========================================================
    /*
        TABLE: RefreshTokens

        Columns:
        --------------------------------------------------------
        Id            UNIQUEIDENTIFIER   PRIMARY KEY
        UserId        UNIQUEIDENTIFIER   FOREIGN KEY -> UserAccounts(Id)
        Token         NVARCHAR(500)      NOT NULL UNIQUE
        ExpiresAtUtc  DATETIME           NOT NULL
        CreatedAtUtc  DATETIME           NOT NULL
        RevokedAtUtc  DATETIME           NULL
        ReplacedBy    NVARCHAR(500)      NULL
        IsActive      BIT                NOT NULL DEFAULT 1
    */

    // TEMP MOCK STORAGE (replace with EF Core later)
    private static readonly List<UserAccount> _users = new();

    static AccountRepository()
    {
        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<UserAccount>();

        var admin = new UserAccount
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@local.com",
            Role = "Admin"

            // DB equivalents:
            // IsActive = true,
            // CreatedAtUtc = DateTime.UtcNow,
            // UpdatedAtUtc = null
        };

        admin.PasswordHash = hasher.HashPassword(admin, "password123");

        _users.Add(admin);
    }

    public Task<UserAccount?> FindAccountByUsernameAsync(string username)
    {
        var user = _users.Find(x => x.Username == username);
        return Task.FromResult(user);
    }

    public Task<UserAccount?> FindAccountByIdAsync(string id)
    {
        var user = _users.Find(x => x.Id.ToString() == id);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(UserAccount user)
    {
        var index = _users.FindIndex(x => x.Id == user.Id);
        if (index >= 0)
        {
            _users[index] = user;

            // DB equivalent:
            // UPDATE UserAccounts
            // SET Username = @Username,
            //     Email = @Email,
            //     PasswordHash = @PasswordHash,
            //     Role = @Role,
            //     UpdatedAtUtc = GETUTCDATE()
            // WHERE Id = @Id;
        }

        return Task.CompletedTask;
    }

    public Task SaveRefreshTokenAsync(RefreshToken token)
    {
        // DB equivalent:
        /*
            INSERT INTO RefreshTokens
            (Id, UserId, Token, ExpiresAtUtc, CreatedAtUtc, IsActive)
            VALUES
            (@Id, @UserId, @Token, @ExpiresAtUtc, GETUTCDATE(), 1);
        */

        return Task.CompletedTask;
    }

    public Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        // DB equivalent:
        /*
            SELECT TOP 1 *
            FROM RefreshTokens
            WHERE Token = @token
              AND IsActive = 1
              AND ExpiresAtUtc > GETUTCDATE();
        */

        return Task.FromResult<RefreshToken?>(null);
    }

    public Task UpdateRefreshTokenAsync(RefreshToken stored)
    {
        // DB equivalent:
        /*
            UPDATE RefreshTokens
            SET IsActive = 0,
                RevokedAtUtc = GETUTCDATE(),
                ReplacedBy = @NewToken
            WHERE Id = @Id;
        */

        return Task.CompletedTask;
    }
}