using System;
using System.Collections.Generic;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceApplication.ApplicationState;

/// <summary>
/// Manages the state container for an individual Blazor Server circuit session.
/// IMPORTANT: Must be registered in Dependency Injection using AddScoped service scope.
/// </summary>
public class AppStateContainer : IDisposable
{
    // ======================================================
    // AUTH & DECOUPLED ERP ENVIRONMENT MATRIX
    // ======================================================
    public bool IsAuthenticated { get; private set; }
    public string? AccessToken { get; private set; }

    // Core structural properties required for native Sage X3 endpoints context mapping
    public string ActiveSageEndpoint { get; private set; } = "SEED";
    public string ActiveFolder { get; private set; } = "SEED";
    public string CurrentCompany { get; private set; } = "";
    public string CurrentSite { get; private set; } = "";
    public string UserRole { get; private set; } = "";
    public string LanguageCode { get; private set; } = "en-US";

    // ======================================================
    // COOPERATIVE CORE ENGINES
    // ======================================================
    public List<string> AutomationLog { get; private set; } = new();
    public string EmailCategory { get; private set; } = "";
    public bool HasInvoiceRisk { get; private set; }
    public string TaskPriority { get; private set; } = "Normal";
    public int ActivityCount { get; private set; }
    public int TaskCount { get; private set; }

    // ======================================================
    // MAIL OVERLAYS
    // ======================================================
    public EmailInsightDto? CurrentEmail { get; private set; }
    public string? CurrentEmailId => CurrentEmail?.Id.ToString();
    public string? CurrentSubject => CurrentEmail?.Subject;

    // ======================================================
    // ERP & CRM TRANSACTION METRICS
    // ======================================================
    public BusinessPartnerSnapshotDto? MatchedClient { get; private set; }
    public List<OpenOrderDto> LinkedOrders { get; private set; } = new();
    public List<TaskDto> LinkedTasks { get; private set; } = new();
    public List<SalesOrderDto> SalesOrders { get; private set; } = new();
    public List<InvoiceDto> Invoices { get; private set; } = new();
    public List<ActivityDto> Activities { get; private set; } = new();

    // ======================================================
    // PRESENTATION & UI CANVAS ARCHITECTURES
    // ======================================================
    public bool IsBusy { get; private set; }

    // ======================================================
    // SAFE EVENT DISPATCH ROUTINES
    // ======================================================
    public event Action? OnChange;

    // ======================================================
    // AUTH & SITE HANDSHAKE SETTERS
    // ======================================================
    public void SetAuthentication(string token)
    {
        IsAuthenticated = true;
        AccessToken = token;
        Notify();
    }

    /// <summary>
    /// Populates the foundational execution landscape properties for Sage X3 communications.
    /// </summary>
    public void SetSageEnvironmentContext(
        string endpoint,
        string folder,
        string company,
        string site,
        string role,
        string language)
    {
        ActiveSageEndpoint = endpoint ?? "SEED";
        ActiveFolder = folder ?? "SEED";
        CurrentCompany = company ?? "";
        CurrentSite = site ?? "";
        UserRole = role ?? "";
        LanguageCode = language ?? "en-US";
        Notify();
    }

    public void SetActiveSageEndpoint(string folder)
    {
        ActiveSageEndpoint = folder;
        Notify();
    }

    public void ClearAuthentication()
    {
        IsAuthenticated = false;
        AccessToken = null;
        ActiveSageEndpoint = "SEED";
        ActiveFolder = "SEED";
        CurrentCompany = "";
        CurrentSite = "";
        UserRole = "";
        LanguageCode = "en-US";
        Notify();
    }

    // ======================================================
    // AUTOMATION PIPELINE DISPATCHERS
    // ======================================================
    public void AddAutomation(string message)
    {
        AutomationLog.Insert(0, $"{DateTime.Now:HH:mm:ss}  {message}");
        Notify();
    }

    public void ClearAutomation()
    {
        AutomationLog.Clear();
        Notify();
    }

    public void SetEmailCategory(string category)
    {
        EmailCategory = category;
        Notify();
    }

    public void SetInvoiceRisk(bool risk)
    {
        HasInvoiceRisk = risk;
        Notify();
    }

    public void SetTaskPriority(string priority)
    {
        TaskPriority = priority;
        Notify();
    }

    public void IncrementActivity()
    {
        ActivityCount++;
        Notify();
    }

    public void IncrementTask()
    {
        TaskCount++;
        Notify();
    }

    // ======================================================
    // EMAIL DOMAIN DISPATCHERS
    // ======================================================
    public void SetCurrentEmail(EmailInsightDto email)
    {
        CurrentEmail = email;
        Notify();
    }

    public void ClearCurrentEmail()
    {
        CurrentEmail = null;
        Notify();
    }

    // ======================================================
    // DATA MATRIX SETTERS
    // ======================================================
    public void SetMatchedClient(BusinessPartnerSnapshotDto? client)
    {
        MatchedClient = client;
        Notify();
    }

    public void SetLinkedOrders(List<OpenOrderDto>? orders)
    {
        LinkedOrders = orders ?? new List<OpenOrderDto>();
        Notify();
    }

    public void SetLinkedTasks(List<TaskDto>? tasks)
    {
        LinkedTasks = tasks ?? new List<TaskDto>();
        Notify();
    }

    public void SetSalesOrders(List<SalesOrderDto> orders)
    {
        SalesOrders = orders ?? new List<SalesOrderDto>();
        Notify();
    }

    public void SetInvoices(List<InvoiceDto> invoices)
    {
        Invoices = invoices ?? new List<InvoiceDto>();
        Notify();
    }

    public void SetActivities(List<ActivityDto> activities)
    {
        Activities = activities ?? new List<ActivityDto>();
        Notify();
    }

    // ======================================================
    // UI CANVAS MUTATORS
    // ======================================================
    public void SetBusy(bool busy)
    {
        IsBusy = busy;
        Notify();
    }

    // ======================================================
    // CONTEXT STRIPPING SCHEMES
    // ======================================================
    public void ClearEmailContext()
    {
        CurrentEmail = null;
        MatchedClient = null;
        LinkedOrders.Clear();
        LinkedTasks.Clear();
        SalesOrders.Clear();
        Invoices.Clear();
        Activities.Clear();
        EmailCategory = "";
        HasInvoiceRisk = false;
        Notify();
    }

    // ======================================================
    // THREAD-SAFE STATE SYNCHRONIZATION 
    // ======================================================
    private void Notify()
    {
        OnChange?.Invoke();
    }

    /// <summary>
    /// Automatically cuts off trapped event allocation listeners during multi-tenant transitions.
    /// </summary>
    public void Dispose()
    {
        OnChange = null;
    }
}
