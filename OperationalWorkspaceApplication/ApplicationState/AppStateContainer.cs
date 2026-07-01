using System;
using System.Collections.Generic;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceApplication.ApplicationState;

// 1. Added IDisposable to enforce cleanup rules
public class AppStateContainer : IDisposable
{
    // ======================================================
    // AUTH & ENVIRONMENT CONTEXT GOVERNANCE
    // ======================================================
    public bool IsAuthenticated { get; private set; }
    public string? AccessToken { get; private set; }

    // Captures the explicit assigned target environment folder (e.g., "SEED", "PROD")
    public string ActiveSageEndpoint { get; private set; } = "SEED";

    // ======================================================
    // AUTOMATION ENGINE (STEPS 1 - 5 MERGED)
    // ======================================================
    public List<string> AutomationLog { get; private set; } = new();

    public string EmailCategory { get; private set; } = "";

    public bool HasInvoiceRisk { get; private set; }

    public string TaskPriority { get; private set; } = "Normal";

    public int ActivityCount { get; private set; }

    public int TaskCount { get; private set; }

    // Unified helper matching step 1 and your existing implementation style
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
    // EMAIL
    // ======================================================
    public EmailInsightDto? CurrentEmail { get; private set; }
    public string? CurrentEmailId => CurrentEmail?.Id.ToString();
    public string? CurrentSubject => CurrentEmail?.Subject;

    // ======================================================
    // CRM
    // ======================================================
    public BusinessPartnerSnapshotDto? MatchedClient { get; private set; }
    public List<OpenOrderDto> LinkedOrders { get; private set; } = new();
    public List<TaskDto> LinkedTasks { get; private set; } = new();
    public List<SalesOrderDto> SalesOrders { get; private set; } = new();

    public List<InvoiceDto> Invoices { get; private set; } = new();

    public List<ActivityDto> Activities { get; private set; } = new();

    // ======================================================
    // UI
    // ======================================================
    public bool IsBusy { get; private set; }

    // ======================================================
    // EVENTS
    // ======================================================
    public event Action? OnChange;

    // ======================================================
    // AUTH ACTIONS & SITE GOVERNANCE SETTERS
    // ======================================================
    public void SetAuthentication(string token)
    {
        IsAuthenticated = true;
        AccessToken = token;
        Notify();
    }

    // Direct mutator allowing the login workflow to map environment folders dynamically
    public void SetActiveSageEndpoint(string folder)
    {
        ActiveSageEndpoint = folder;
        Notify();
    }

    public void ClearAuthentication()
    {
        IsAuthenticated = false;
        AccessToken = null;
        ActiveSageEndpoint = "SEED"; // Safely defaults context back to seed configurations
        Notify();
    }

    // ======================================================
    // EMAIL ACTIONS
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
    // CRM ACTIONS
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

    // ======================================================
    // UI ACTIONS
    // ======================================================
    public void SetBusy(bool busy)
    {
        IsBusy = busy;
        Notify();
    }

    // ======================================================
    // FULL CONTEXT RESET
    // ======================================================
    public void ClearEmailContext()
    {
        CurrentEmail = null;
        MatchedClient = null;
        LinkedOrders.Clear();
        LinkedTasks.Clear();
        Notify();
    }

    // ======================================================
    // INTERNAL NOTIFICATION FLOW
    // ======================================================
    private void Notify()
    {
        OnChange?.Invoke();
    }


    public void SetSalesOrders(List<SalesOrderDto> orders)
    {
        SalesOrders = orders;
        Notify();
    }

    public void SetInvoices(List<InvoiceDto> invoices)
    {
        Invoices = invoices;
        Notify();
    }

    public void SetActivities(List<ActivityDto> activities)
    {
        Activities = activities;
        Notify();
    }
    // Existing fallback automation hooks kept for backwards service orchestration compatibility



    // 2. CRITICAL ADDITION: Wipes out trapped listeners automatically 
    // when a Blazor user session or Outlook Add-In context changes.
    public void Dispose()
    {
        OnChange = null;
    }
}
