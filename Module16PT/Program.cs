using System;
using System.IO;

class Program
{
    private static string watchedDirectory;
    private static string logFilePath;
    private static NotifyFilters filters;
    private static bool isRecursive;

    static void Main(string[] args)
    {
        Console.WriteLine("File Change Tracker");
        Console.WriteLine("===================");

        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Start File Tracking");
            Console.WriteLine("2. Configure Settings");
            Console.WriteLine("3. Exit");

            int choice = GetIntegerInput("Enter your choice: ");

            switch (choice)
            {
                case 1:
                    StartFileTracking();
                    break;
                case 2:
                    ConfigureSettings();
                    break;
                case 3:
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private static void StartFileTracking()
    {
        if (string.IsNullOrEmpty(watchedDirectory) || string.IsNullOrEmpty(logFilePath))
        {
            Console.WriteLine("Please configure settings first.");
            return;
        }

        Console.WriteLine($"File tracking started for directory: {watchedDirectory}");
        Console.WriteLine($"Logging changes to: {logFilePath}");
        Console.WriteLine("Press 'Q' to stop tracking and return to the main menu.");

        using (var watcher = new FileSystemWatcher(watchedDirectory))
        {
            watcher.IncludeSubdirectories = isRecursive;
            watcher.NotifyFilter = filters;
            watcher.Changed += OnFileChanged;
            watcher.Created += OnFileCreated;
            watcher.Deleted += OnFileDeleted;
            watcher.Renamed += OnFileRenamed;

            watcher.EnableRaisingEvents = true;

            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    Console.WriteLine("File tracking stopped.");
                    break;
                }
            }
        }
    }

    private static void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        LogChange("File Changed", e.FullPath);
    }

    private static void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        LogChange("File Created", e.FullPath);
    }

    private static void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        LogChange("File Deleted", e.FullPath);
    }

    private static void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        LogChange("File Renamed", $"{e.OldFullPath} to {e.FullPath}");
    }

    private static void LogChange(string changeType, string path)
    {
        string logMessage = $"{DateTime.Now} - {changeType}: {path}";
        Console.WriteLine(logMessage);
        File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
    }

    private static void ConfigureSettings()
    {
        Console.WriteLine("\nConfigure Settings:");
        watchedDirectory = GetStringInput("Enter the directory to watch: ");
        logFilePath = GetStringInput("Enter the log file path: ");

        Console.WriteLine("Filtering Options:");
        filters = GetNotifyFiltersFromUser();

        isRecursive = GetRecursiveOptionFromUser();
    }

    private static string GetStringInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    private static int GetIntegerInput(string prompt)
    {
        Console.Write(prompt);
        int value;
        while (!int.TryParse(Console.ReadLine(), out value))
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
            Console.Write(prompt);
        }
        return value;
    }

    private static NotifyFilters GetNotifyFiltersFromUser()
    {
        NotifyFilters result = NotifyFilters.FileName;
        Console.WriteLine("Select filtering options:");
        Console.WriteLine("1. FileName");
        Console.WriteLine("2. DirectoryName");
        int choice = GetIntegerInput("Enter your choice (1/2): ");

        if (choice == 1)
        {
            result |= NotifyFilters.DirectoryName;
        }

        return result;
    }

    private static bool GetRecursiveOptionFromUser()
    {
        Console.Write("Enable recursive tracking? (Y/N): ");
        string input = Console.ReadLine();
        return input.Equals("Y", StringComparison.OrdinalIgnoreCase);
    }
}
