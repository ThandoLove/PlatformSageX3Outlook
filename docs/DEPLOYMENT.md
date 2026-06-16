# Deployment — Outlook Add-in (simple, no Docker)

This add-in is deployed as **one HTTPS web app**. The API project hosts both REST endpoints and the Blazor UI.

## Architecture

```text
Outlook (desktop / web)
    └── loads Manifest.xml
            └── task panes → https://YOUR-DOMAIN/taskpane/...
            └── function file → https://YOUR-DOMAIN/js/officeFunctionFile.html
                    └── OperationalWorkspaceAPI (Kestrel or IIS)
                            ├── REST  /api/v1/*
                            └── Blazor  /taskpane/*, /dashboard, …
```

You do **not** need a separate UI process in production.

## 1. Publish the API (includes UI)

```powershell
cd c:\Users\user\Desktop\temp\PlatformSageX3Outlook

dotnet publish OperationalWorkspaceAPI/OperationalWorkspaceAPI.csproj `
  -c Release `
  -o ./publish/addin-host
```

## 2. Configure secrets (production)

Set environment variables on the host (or IIS app pool):

| Variable | Example |
|----------|---------|
| `ConnectionStrings__DefaultConnection` | SQL Server connection string |
| `Jwt__Key` | At least 32 random characters |
| `Jwt__Issuer` | `OperationalWorkspaceAPI` |
| `Jwt__Audience` | `PlatformSageX3Outlook` |
| `SageX3__RestBaseUrl` | `https://sage-server/api1/x3/erp/SEED` |
| `SageSecurityOptions__ClientId` | Sage OAuth client id |
| `SageSecurityOptions__ClientSecret` | Sage OAuth secret |
| `SageSecurityOptions__TokenEndpoint` | `https://sage-server/api/oauth2/token` |
| `AddIn__PublicBaseUrl` | `https://addin.yourcompany.com` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

## 3. Run on HTTPS

Outlook requires **HTTPS** for add-in URLs.

**Option A — Kestrel with certificate (simplest for pilot)**

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ASPNETCORE_URLS = "https://0.0.0.0:443"
dotnet ./publish/addin-host/OperationalWorkspaceAPI.dll
```

**Option B — IIS reverse proxy**

1. Install ASP.NET Core Hosting Bundle.
2. Create site bound to HTTPS (443).
3. Point site physical path to `./publish/addin-host`.
4. Set environment variables in IIS → Configuration Editor or `web.config`.

## 4. Update the Outlook manifest

Edit `OperationalWorkspace.Addin/Manifest.xml` — replace every `https://localhost:7173` with your public URL (`AddIn:PublicBaseUrl`).

Also add your domain under `<AppDomains>`:

```xml
<AppDomain>https://addin.yourcompany.com</AppDomain>
```

Icon and task pane URLs must all use the same host.

See [MANIFEST_ROUTES.md](./MANIFEST_ROUTES.md) for the full URL → page map.

## 5. Sideload the add-in (testing)

**Outlook desktop (Windows)**

1. Copy `Manifest.xml` to a network share or local folder.
2. File → Get Add-ins → My Add-ins → Add a custom add-in → Add from file.
3. Select the manifest.

**Microsoft 365 admin (organization)**

Upload the manifest via Microsoft 365 admin center → Integrated apps (when ready for wider rollout).

## 6. Verify

| Check | URL |
|-------|-----|
| Health | `https://YOUR-DOMAIN/health` |
| Task pane | `https://YOUR-DOMAIN/taskpane/workspace` |
| Function file | `https://YOUR-DOMAIN/js/officeFunctionFile.html` |
| API auth | `POST https://YOUR-DOMAIN/api/v1/auth/login` |

## Security notes (already in code)

- Dev login bypass only works when `ASPNETCORE_ENVIRONMENT=Development`.
- All API controllers require JWT except `/api/v1/auth/*`.
- Hangfire dashboard requires Admin/Manager role outside Development.
- CSP `frame-ancestors` allows Outlook hosts; `X-Frame-Options: DENY` removed for task pane embedding.
- CORS policy `OutlookAddInPolicy` restricts origins in Production.

## What you can skip

- Docker / container images (`Deployments/Containers/` — not required for add-in hosting).
- Separate `OperationalWorkspaceUI` process — only needed for local UI-only debugging.
- Azure AD / OpenID Connect — not configured yet; current auth is JWT username/password until you add external IdP.
