using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;

namespace OperationalWorkspaceApplication.Responses;

// 1. Base Result/Response class for Task operations
public class TaskResponse
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public Guid? TaskId { get; init; } // Added to support CreateAsync
    public Guid Id { get; internal set; }
    public string Title { get; internal set; } = string.Empty;

    public static TaskResponse Success(Guid? taskId = null) =>
        new() { IsSuccess = true, TaskId = taskId };

    public static TaskResponse Fail(string message) =>
        new() { IsSuccess = false, Message = message };
}

// 2. Specific Response for GetAsync
public sealed record TaskListResponse(IReadOnlyList<TaskDto> Tasks);

// 3. Specific Response for CompleteAsync
public sealed record CompleteTaskResponse(bool IsSuccess, string Message = "");

// REMOVED: The "public sealed record TaskResponse" that was at the bottom
