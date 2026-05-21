
namespace OperationalWorkspaceApplication.DTOs;

public class EmailContextDto
{
    public EmailInsightDto? Email { get; set; }

    public List<LinkedOpenOrderDto> LinkedOrders { get; set; }
        = new();

    public List<TaskDto> LinkedTasks { get; set; }
        = new();
}