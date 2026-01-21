using EventCalendar.Repositories;
using EventCalendar.Services;

namespace EventCalendar;

static class Program
{
    private static string LogFilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "EventCalendar",
        "error.log"
    );

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        // Setup global error handlers (Requirement 6.3)
        Application.ThreadException += Application_ThreadException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        ApplicationConfiguration.Initialize();
        
        try
        {
            // Initialize services (Requirements 4.4, 5.1, 6.1)
            var repository = new EventRepository();
            var eventManager = new EventManager(repository);
            var notificationService = new EventNotificationService(eventManager);
            
            // Start notification service (Requirement 4.4)
            notificationService.Start();
            
            // Create and run main form
            var mainForm = new MainForm(eventManager, notificationService);
            
            // Check if started minimized (e.g., from Windows startup)
            bool startMinimized = args.Length > 0 && args[0] == "--minimized";
            if (startMinimized)
            {
                mainForm.WindowState = FormWindowState.Minimized;
                mainForm.ShowInTaskbar = false;
            }
            
            Application.Run(mainForm);
            
            // Stop notification service when application closes
            notificationService.Stop();
        }
        catch (Exception ex)
        {
            HandleUnhandledException(ex);
        }
    }

    private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
        HandleUnhandledException(e.Exception);
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            HandleUnhandledException(ex);
        }
    }

    private static void HandleUnhandledException(Exception ex)
    {
        // Log error to file (Requirement 6.3)
        LogError(ex);

        // Show user-friendly message (Requirement 6.3)
        string message = "Ocorreu um erro inesperado no aplicativo.\n\n" +
                        $"Detalhes: {ex.Message}\n\n" +
                        $"O erro foi registrado em: {LogFilePath}";

        MessageBox.Show(
            message,
            "Erro - Event Calendar",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error
        );
    }

    private static void LogError(Exception ex)
    {
        try
        {
            // Ensure log directory exists
            string logDirectory = Path.GetDirectoryName(LogFilePath)!;
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Write error to log file
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex.GetType().Name}: {ex.Message}\n" +
                            $"Stack Trace:\n{ex.StackTrace}\n" +
                            $"----------------------------------------\n";

            File.AppendAllText(LogFilePath, logEntry);
        }
        catch
        {
            // If logging fails, silently continue - don't throw another exception
        }
    }
}
