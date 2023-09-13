// Simple class for todo tasks

public class TodoTask
{
    public string? Name { get; set; }
    public string? Project { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly CreationDate { get; set; }
    public bool Status { get; set; }
    
    public TodoTask(string? name, string? project, DateOnly creationDate, DateOnly dueDate, bool status)
    {
        Name = name;
        Project = project;
        CreationDate = creationDate;
        DueDate = dueDate;
        Status = status;
    }
}