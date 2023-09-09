using Newtonsoft.Json;

// Path of the JSON file that contains tasks
const string path = "data/data.json";
List<TodoTask> tasks;
var currentSortingMode = SortingMode.Date;
await ShowTasks();

return;

// Save the serialized tasks to a data.json file
async Task SaveTasks()
{
    var tasksJson = JsonConvert.SerializeObject(tasks, Formatting.Indented);
    await File.WriteAllTextAsync(path, tasksJson);
}

// Load all tasks from a json file
async Task<List<TodoTask>> LoadTasks()
{
    if (!File.Exists(path))
    {
        Console.WriteLine("data.json cannot be loaded");
        return new List<TodoTask>();
    }
    var jsonResult = await File.ReadAllTextAsync(path);
    return JsonConvert.DeserializeObject<List<TodoTask>>(jsonResult) ?? throw new InvalidOperationException();
}

//Make a sorted list based on the selected mode
IEnumerable<TodoTask> GetSortedTasks()
{
    return currentSortingMode switch
    {
        SortingMode.Unordered => tasks,
        SortingMode.Project => tasks.OrderBy(task => task.Project),
        SortingMode.Date => tasks.OrderBy(task => task.DueDate),
        SortingMode.Status => tasks.OrderBy(task => task.Status),
        _ => throw new ArgumentOutOfRangeException(nameof(currentSortingMode), currentSortingMode, null)
    };
}

void PrintTasks()
{
    //Print task parameters in nice columns
    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.WriteLine("{0,-20} {1,-20} {2, -20} {3, -20}", "NAME", "PROJECT", "DUE DATE", "STATUS");
    foreach (var task in GetSortedTasks())
    {
        Console.ForegroundColor = task.Status ? ConsoleColor.DarkCyan : ConsoleColor.Magenta;
        var status = task.Status ? "Completed" : "Pending";
        Console.WriteLine("{0,-20} {1,-20} {2, -20} {3, -20}", task.Name, task.Project, task.DueDate.ToString(), status);
    }
}

// This method displays tasks with specified sorting mode
async Task ShowTasks()
{  
    var options = new List<string> { "Sort by date", "Sort by project", "Sort by status", "Add new task", "Edit task", "Save and quit" };
    var index = 0;

    while (true)
    {
        Console.Clear(); 

        tasks = await LoadTasks();
        if (tasks is {Count: 0})
        {
            Console.WriteLine("No tasks have been added");
            return;
        }
        
        PrintTasks();

        Console.ForegroundColor = ConsoleColor.White;
        for (var i = 0; i < options.Count; i++)
        {
            if (i == index)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine(options[i]);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(options[i]);
            }
        }

        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.UpArrow:
                index = (index > 0) ? --index : options.Count - 1;
                break;
            case ConsoleKey.DownArrow:
                index = (index < options.Count - 1) ? ++index : 0;
                break;
            case ConsoleKey.Enter:
                switch (index)
                {
                    case 0:
                        currentSortingMode = SortingMode.Date;
                        break;
                    case 1:
                        currentSortingMode = SortingMode.Project;
                        break;
                    case 2:
                        currentSortingMode = SortingMode.Status;
                        break;
                    case 3:
                        await AddTask();
                        return;
                    case 4:
                        EditTask();
                        return;
                    case 5:
                        return;
                }
                break;
        }
    }
}

// This method adds a new task to the list
async Task AddTask()
{
    string? name;
    while (true)
    {
        Console.WriteLine("Enter task name: ");
        name = Console.ReadLine();
        if (!string.IsNullOrEmpty(name))
            break;
        Console.WriteLine("Invalid input. Please enter a valid task name.");
    }

    string? project;
    while (true)
    {
        Console.WriteLine("Enter project name: ");
        project = Console.ReadLine();
        if (!string.IsNullOrEmpty(project))
            break;
        Console.WriteLine("Invalid input. Please enter a valid project name.");
    }

    DateOnly dueDate;
    while (true)
    {
        Console.WriteLine("Enter due date (yyyy-MM-dd): ");
        var input = Console.ReadLine();
      
        if (DateOnly.TryParse(input, out dueDate))
            break;
        
        Console.WriteLine("Invalid input. Please enter a valid due date.");
    }
    
    Console.WriteLine("Is the task completed? (yes/no)");
    var statusInput = Console.ReadLine()?.ToLower();
    var status = statusInput == "yes";

    var task = new TodoTask(name, project, DateOnly.FromDateTime(DateTime.Now), dueDate, status);
    tasks.Add(task);
    await SaveTasks();
    await ShowTasks();
}

void EditTask()
{
    var taskIndex = 0;
    TodoTask selectedTask;
    
    while (true)
    {
        Console.Clear();
        for (var i = 0; i < tasks.Count; i++)
        {
            if (i == taskIndex)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine(tasks[i].Name);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(tasks[i].Name);
            }
        }

        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.UpArrow:
                taskIndex = (taskIndex > 0) ? --taskIndex : tasks.Count - 1;
                break;
            case ConsoleKey.DownArrow:
                taskIndex = (taskIndex < tasks.Count - 1) ? ++taskIndex : 0;
                break;
            case ConsoleKey.Enter:
                // Here you can place the logic for editing the selected task.
                break;
            case ConsoleKey.Escape:
                // Break the loop and return to main menu
                return;
        }
    }
}

//Class for todo tasks
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

// sorting mode enumerator
internal enum SortingMode
{
    Unordered,
    Project,
    Date,
    Status
}