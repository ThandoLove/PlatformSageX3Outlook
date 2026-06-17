# Sage X3 REST endpoints in this codebase

Paths follow the Sage X3 Syracuse REST Web Services format:

```text
{RestBaseUrl}/{ENTITY}?representation={ENTITY}.$query
{RestBaseUrl}/{ENTITY}('{key}')?representation={ENTITY}.$details
{RestBaseUrl}/{ENTITY}?representation={ENTITY}.$create
```

`RestBaseUrl` is configured in `appsettings.json` → `SageX3:RestBaseUrl`  
Example: `https://your-sage-server/api1/x3/erp/SEED`

## Where endpoints are defined

| File | Purpose |
|------|---------|
| `OperationalWorkspaceInfrastructure/ExternalServices/SageX3/SageX3RestEndpoints.cs` | **Canonical entity codes and URL builders** |
| `OperationalWorkspaceInfrastructure/ExternalServices/SageX3/SageX3Client.cs` | HTTP calls using the builders above |
| `OperationalWorkspaceApplication/Services/SageRestService.cs` | Generic REST helper (`BPCUSTOMER`, `BPSUPPLIER`) |
| `OperationalWorkspaceInfrastructure/ERPAuthentication/SageAuthService.cs` | OAuth2 token: `SageSecurityOptions:TokenEndpoint` |

## Entity codes (X3 objects)

| Constant | X3 object | Typical use |
|----------|-----------|-------------|
| `BPCUSTOMER` | Customer master | Lookup by `BPCNUM`, credit, open orders |
| `BPSUPPLIER` | Supplier master | Supplier financial snapshot |
| `CONTACT` | Contact | Find partner by email (`WEB` field) |
| `SORDER` | Sales order | Open orders, order details |
| `SQUOTE` | Sales quote | Quotes |
| `SIH` | Sales invoice header | Invoices, AR aging |
| `ITMMASTER` | Product master | Item details |
| `STOCK` | Stock by site | Warehouse availability |
| `MVTSTO` | Stock movement | Adjustments |

## Methods wired in `SageX3Client`

| Method | Sage REST path (via `SageX3RestEndpoints`) |
|--------|---------------------------------------------|
| `GetCustomerDataAsync(bpCode)` | `BPCUSTOMER('{bpCode}')?representation=BPCUSTOMER.$details` |
| `FindPartnerByEmailAsync(email)` | `CONTACT?representation=CONTACT.$query&where=WEB eq '{email}'` |
| `GetPartnerFinancialSnapshotAsync` | `BPCUSTOMER('{bpCode}')?representation=BPCUSTOMER.$details` |
| `GetActivePartnersCountAsync` | `BPCUSTOMER?representation=BPCUSTOMER.$query&count=1` |
| `GetWarehouseStockAsync(site)` | `STOCK?representation=STOCK.$query&where=STOFCY eq '{site}'` |
| `GetInvoicesPagedAsync` | `SIH?representation=SIH.$query&count={n}` |

Until Sage X3 credentials and network access are configured, these calls log a warning and return empty/null — the app continues to use mock services in Development.

## Configuration when you connect

Set in User Secrets or environment variables:

```json
"SageX3": {
  "UseMockData": false,
  "RestBaseUrl": "https://YOUR-SERVER/api1/x3/erp/YOUR-FOLDER"
},
"SageSecurityOptions": {
  "ClientId": "your-client-id",
  "ClientSecret": "your-client-secret",
  "TokenEndpoint": "https://YOUR-SERVER/api/oauth2/token"
}
```

Run the API in **Production** environment (or disable Development mock registrations in `Program.cs`) to use the real `SageX3Client`.
