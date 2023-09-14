using Newtonsoft.Json;

const string jsonPath = "data/data.json";
var tasks = await LoadTasks();
var currentSortingMode = SortingMode.Date;
MainMenu();

return;

// Save the serialized tasks to a data.json file
async Task SaveTasks()
{
    var tasksJson = JsonConvert.SerializeObject(tasks, Formatting.Indented);
    await File.WriteAllTextAsync(jsonPath, tasksJson);
}

// Load all tasks from a json file
async Task<List<TodoTask>> LoadTasks()
{
    if (!File.Exists(jsonPath))
    {
        Console.WriteLine("data.json cannot be loaded");
        return new List<TodoTask>();
    }
    var jsonResult = await File.ReadAllTextAsync(jsonPath);
    return JsonConvert.DeserializeObject<List<TodoTask>>(jsonResult) ?? throw new InvalidOperationException();
}

//Make a sorted list based on the selected sorting mode
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

// Show all tasks and a basic menu
void MainMenu()
{  
    var options = new List<string> { "Sort by date", "Sort by project", "Sort by status", "Add new task", "Edit task", "Save and quit" };
    var index = 0;

    while (true)
    {
        Console.Clear(); 
        
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks have been added");
            return;
        }
        
        PrintTasksWithOptionalSelection();

        //Print all menu options
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

        // Arrow key navigation for menu
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
                        AddTask();
                        break;
                    case 4:
                        SelectTask();
                        break;
                    case 5:
                        return;
                }
                break;
        }
    }
}

// Create a new task
async Task AddTask()
{
    // Get the name
    string? name;
    while (true)
    {
        Console.WriteLine("Enter task name: ");
        name = Console.ReadLine();
        if (!string.IsNullOrEmpty(name))
            break;
        Console.WriteLine("Invalid input. Please enter a valid task name.");
    }

    // Get the project name
    string? project;
    while (true)
    {
        Console.WriteLine("Enter project name: ");
        project = Console.ReadLine();
        if (!string.IsNullOrEmpty(project))
            break;
        Console.WriteLine("Invalid input. Please enter a valid project name.");
    }

    // Get due date
    DateOnly dueDate;
    while (true)
    {
        Console.WriteLine("Enter due date (yyyy-MM-dd): ");
        var input = Console.ReadLine();
      
        if (DateOnly.TryParse(input, out dueDate))
            break;
        
        Console.WriteLine("Invalid input. Please enter a valid due date.");
    }
    
    //Get if the task is finished or not
    Console.WriteLine("Is the task completed? (yes/no)");
    var statusInput = Console.ReadLine()?.ToLower();
    var status = statusInput == "yes";

    var task = new TodoTask(name, project, DateOnly.FromDateTime(DateTime.Now), dueDate, status);
    tasks.Add(task);
    await SaveTasks();
    MainMenu();
}

// Use arrow keys to navigate between tasks
void SelectTask()
{
    var taskIndex = 0;
    
    while (true)
    {
        Console.Clear();
        PrintTasksWithOptionalSelection(taskIndex);
        
        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.UpArrow:
                taskIndex = (taskIndex > 0) ? --taskIndex : tasks.Count - 1;
                break;
            case ConsoleKey.DownArrow:
                taskIndex = (taskIndex < tasks.Count - 1) ? ++taskIndex : 0;
                break;
            case ConsoleKey.Enter:
                EditSelectedTask(taskIndex);
                return;
            case ConsoleKey.B:
                return;
        }
    }
}

// Print task parameters in nice columns with the selected task highlighted
void PrintTasksWithOptionalSelection(int? selectedTaskIndex = null)
{
    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.WriteLine("{0,-20} {1,-20} {2, -20} {3, -20}", "NAME", "PROJECT", "DUE DATE", "STATUS");

    for(var i = 0; i < tasks.Count; i++)
    {
        Console.ResetColor();
        var task = GetSortedTasks().ToList()[i];
        if(i == selectedTaskIndex)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
        }
        else
        {
            Console.ForegroundColor = task.Status ? ConsoleColor.Cyan : ConsoleColor.Magenta;
        }

        var status = task.Status ? "Completed" : "Pending";
        Console.WriteLine("{0,-20} {1,-20} {2, -20} {3, -20}", task.Name, task.Project, task.DueDate.ToString(), status);
    }
    
    Console.ResetColor();
    if (selectedTaskIndex == tasks.Count)
    {
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
    }
    
    Console.ResetColor();   
}

// Overwrite parameters of a given task
void EditSelectedTask(int taskIndex)
{
    var selectedTask = tasks[taskIndex];
    var options = new List<string> { "Edit Name", "Edit Project", "Edit Due Date", "Toggle Status (Completed/Pending)", "Save Changes", "Cancel" };
    var index = 0;

    while (true)
    {
        Console.Clear();
        Console.WriteLine("Editing Task: " + selectedTask.Name);

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
                        // Replace task name
                        Console.Write("Enter new name: ");
                        selectedTask.Name = Console.ReadLine();
                        break;
                    case 1:
                        // Replace project name
                        Console.Write("Enter new project name: ");
                        selectedTask.Project = Console.ReadLine();
                        break;
                    case 2:
                        // Replace due date
                        Console.Write("Enter new due date (yyyy-MM-dd): ");
                        if (DateOnly.TryParse(Console.ReadLine(), out var newDueDate))
                        {
                            selectedTask.DueDate = newDueDate;
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format. Please enter a valid date (yyyy-MM-dd).");
                        }
                        break;
                    case 3:
                        // Flip flop the status
                        selectedTask.Status = !selectedTask.Status;
                        break;
                    case 4:
                        // Overwrite old task and return to the main menu
                        tasks[taskIndex] = selectedTask;
                        SaveTasks().Wait();
                        return;
                    case 5:
                        // Cancel editing and return to the main menu
                        return;
                }
                break;
        }
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