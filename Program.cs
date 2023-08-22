using Newtonsoft.Json;

// Path of the JSON file that contains tasks
const string path = "data/data.json";
var tasks = await LoadTasks();

await MainMenu();

return;

// This method shows the main menu of the application
async Task MainMenu()
{
    var options = new List<string> { "Show Task List", "Add New Task", "Edit Task (update, mark as completed, remove)", "Save and Quit" };
    int index = 0;

    while (true)
    {
        Console.Clear();
        Console.WriteLine("Welcome to ToDoLy!");
        Console.WriteLine();
        Console.WriteLine("Pick an option:");

        for (int i = 0; i < options.Count; i++)
        {
            if (i == index)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
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
                        await ShowTasks(SortingMode.Unordered);
                        break;
                    case 1:
                        await AddTask();
                        break;
                    case 2:
                        EditTask();
                        break;
                    case 3:
                        await SaveTasks();
                        Quit();
                        break;
                }
                break;
        }
    }
}

// Save the serialized tasks to a data.json file
async Task SaveTasks()
{
    var tasksJson = JsonConvert.SerializeObject(tasks, Formatting.Indented);
    await File.WriteAllTextAsync(path, tasksJson);
}

// This method loads tasks from a json file 
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

// This method displays tasks with specified sorting mode
async Task ShowTasks(SortingMode mode)
{
    Console.Clear();
    tasks = await LoadTasks();
    if (tasks is {Count: 0})
    {
        Console.WriteLine("No tasks have been added");
        return;
    }

    IEnumerable<TodoTask> tasksToView = mode switch
    {
        SortingMode.Unordered => tasks,
        SortingMode.Project => tasks.OrderBy(task => task.Project),
        SortingMode.Date => tasks.OrderBy(task => task.DueDate),
        SortingMode.Status => tasks.OrderByDescending(task => task.Status),
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
    };

    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.WriteLine("{0,-20} {1,-20} {2, -20} {3, -20}", "NAME", "PROJECT", "DUE DATE", "STATUS");
    foreach (var task in tasksToView)
    {
        Console.ForegroundColor = task.Status ? ConsoleColor.DarkCyan : ConsoleColor.Magenta;
        var status = task.Status ? "Completed" : "Pending";
        Console.WriteLine("{0,-20} {1,-20} {2, -20} {3, -20}", task.Name, task.Project, task.DueDate.ToString(),
            status);
    }

    var options = new List<string> { "Sort by date", "Sort by project", "Sort by status", "Back to menu" };
    int index = 0;

    while (true)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        for (int i = 0; i < options.Count; i++)
        {
            if (i == index)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
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
                        await ShowTasks(SortingMode.Date);
                        break;
                    case 1:
                        await ShowTasks(SortingMode.Project);
                        break;
                    case 2:
                        await ShowTasks(SortingMode.Status);
                        break;
                    case 3:
                        await MainMenu();
                        break;
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

    Console.WriteLine("Task added successfully!");
    await MainMenu();
}

void EditTask()
{
    
}

// This method terminates the application
void Quit()
{
    Environment.Exit(0);
}

public class TodoTask
{
    // properties of the task
    public string? Name { get; set; }
    public string? Project { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly CreationDate { get; set; }
    public bool Status { get; set; }

    // constructor of the task
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