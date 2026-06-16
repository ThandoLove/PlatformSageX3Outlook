# Manifest URL → Blazor Route Map

All Outlook task pane URLs from `OperationalWorkspace.Addin/Manifest.xml` and where they resolve.

| Manifest resource ID | URL path | Blazor page / component | Status |
|----------------------|----------|-------------------------|--------|
| `tpWorkspaceUrl` | `/taskpane/workspace` | `Dashboard.razor` | Aligned |
| `tpCreateContactUrl` | `/taskpane/contact/create` | `NewContactForm.razor` | Aligned (alias added) |
| `tpViewOrdersUrl` | `/taskpane/orders/view` | `SalesOrders.razor` | Aligned (alias added) |
| `tpViewCustomerUrl` | `/taskpane/customer/view` | `CrmSnapshot.razor` | Aligned (alias added) |
| `tpAiSummaryUrl` | `/taskpane/ai/summary` | `TaskPaneAiSummary.razor` | Aligned (new page) |
| `tpCreateTaskUrl` | `/taskpane/task/create` | `CreateTaskPanel.razor` | Aligned (alias added) |
| `tpInsertTemplateUrl` | `/taskpane/compose/template` | `Dashboard.razor` | Aligned |
| `tpAiResponseUrl` | `/taskpane/compose/ai-response` | `Dashboard.razor` | Aligned |
| `tpAttachDocsUrl` | `/taskpane/compose/attach-documents` | `Dashboard.razor` | Aligned |
| `tpFollowUpUrl` | `/taskpane/compose/followup` | `Dashboard.razor` | Aligned |
| `funcFileUrl` | `/js/officeFunctionFile.html` | Static file (ExecuteFunction handlers) | Aligned |

## ExecuteFunction handlers (manifest → `officeFunctionFile.html`)

| Manifest `FunctionName` | Handler |
|-------------------------|---------|
| `onLinkToCrmClicked` | Links email sender via `officeBridge.checkSender()` |
| `onUploadAttachmentClicked` | `officeBridge.uploadCurrentDocument()` |
| `onOpenInSageX3Clicked` | `officeBridge.openSageRecord('BPCUSTOMER', 'read')` |
| `onInjectSignatureClicked` | Inserts signature into compose body |
| `onWorkflowActionClicked` | Placeholder (wire when Sage workflow API is available) |

## Related non-taskpane routes

| Route | Page |
|-------|------|
| `/dashboard` | `Dashboard.razor` (browser mode) |
| `/forms/add-contact`, `/add-contact` | `NewContactForm.razor` |
| `/orders` | `SalesOrders.razor` |
| `/contacts` | `BusinessPartner.razor` |
| `/crm-snapshot` | `CrmSnapshot.razor` |
| `/forms/admin-create-task` | `CreateTaskPanel.razor` |
