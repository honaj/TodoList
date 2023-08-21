using Newtonsoft.Json;

const string path = "data/data.json";
var tasks = await LoadTasks();

MainMenu();
return;

async void MainMenu()
{
    Console.Clear();
    Console.WriteLine("Welcome to ToDoLy!");
    Console.WriteLine();
    Console.WriteLine("Pick an option:");
    Console.WriteLine("(1) Show Task List");
    Console.WriteLine("(2) Add New Task");
    Console.WriteLine("(3) Edit Task (update, mark as completed, remove");
    Console.WriteLine("(4) Save and Quit");
    while (true)
    {
        switch (Console.ReadKey(true).KeyChar)
        {
            case '1':
                ShowTasks();
                break;
            case '2':
                AddTask();
                break;
            case '3':
                // Edit task logic
                break;
            case '4':
                await SaveTasks();
                Quit();
                break;
        }   
    }
}

async Task SaveTasks()
{
    var tasksJson = JsonConvert.SerializeObject(tasks, Formatting.Indented);
    // Save the serialized tasks to a data.json file
    await File.WriteAllTextAsync(path, tasksJson);
}

async Task<List<TodoTask>> LoadTasks()
{
    if (!File.Exists(path))
    {
        Console.WriteLine("data.json cannot be loaded");
        return new List<TodoTask>();
    }
    var jsonResult = await File.ReadAllTextAsync(path);
    return JsonConvert.DeserializeObject<List<TodoTask>>(jsonResult);
}

async void ShowTasks()
{
    tasks = await LoadTasks();
    if (tasks is {Count: 0})
    {
        Console.WriteLine("No tasks have been added");
        return;
    }
    
    Console.WriteLine("{0,-10} {1,-10} {2, -20} {3, -10}", "Name", "Project", "Due Date", "Status");
    foreach (var task in tasks)
    {
        Console.WriteLine("{0,-10} {1,-10} {2, -20} {3, -10}", task.Name, task.Project, task.DueDate.ToString(),
            task.Status);
    }
    
    Console.WriteLine("(1) Sort by date");
    Console.WriteLine("(2) Sort by project");
    Console.WriteLine("(3) Back to menu");
    while (true)
    {
        switch (Console.ReadKey(true).KeyChar)
        {
            case '1':
                break;
            case '2':
                break;
            case '3':
                MainMenu();
                break;
        }
    }
}

async void AddTask()
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

    var task = new TodoTask(name, project, dueDate, status);
    tasks.Add(task);
    await SaveTasks();

    Console.WriteLine("Task added successfully!");
    MainMenu();
}

void Quit()
{
    Environment.Exit(0);
}

public class TodoTask
{
    public string? Name { get; set; }
    public string? Project { get; set; }
    public DateOnly DueDate { get; set; }
    public bool Status { get; set; }

    public TodoTask(string? name, string? project, DateOnly dueDate, bool status)
    {
        Name = name;
        Project = project;
        DueDate = dueDate;
        Status = status;
    }
}