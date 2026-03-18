namespace TaskManagerServerCS.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Priority { get; set; } = "בינונית";
    public string DueDate { get; set; } = string.Empty;
    public string Status { get; set; } = "ממתינה";
}