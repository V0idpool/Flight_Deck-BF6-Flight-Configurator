namespace FlightDeck_BF6_Flight_Configurator.Helpers
{
    internal class ExceptionHandler
    {
        public static string GetLogFilePath()
        {
            string baseDir = Application.StartupPath;
            string subDir = Path.Combine(baseDir, "Logs");

            if (!Directory.Exists(subDir))
            {
                Directory.CreateDirectory(subDir);
            }

            return Path.Combine(subDir, "FD_Logs.txt");
        }
        public static void LogError(Exception ex)
        {
            try
            {
                string logFilePath = GetLogFilePath();

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"[{DateTime.Now}] - Unhandled Exception Type: {ex.GetType().FullName}");
                    writer.WriteLine($"Message: {ex.Message}");
                    writer.WriteLine($"StackTrace: {ex.StackTrace}");
                    writer.WriteLine();
                }

                System.Diagnostics.Debug.WriteLine($"Error details logged to: {logFilePath}");
                ExceptionHandler.LogError(ex);
            }
            catch (Exception logEx)
            {
                System.Diagnostics.Debug.WriteLine("Error logging failed: " + logEx.Message);
                ExceptionHandler.LogError(logEx);
            }
        }
        public static void LogMessage(string message)
        {
            try
            {
                string logFilePath = GetLogFilePath();
                string logEntry = $"[{DateTime.Now}] - INFO: {message}";

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(logEntry);
                }

                System.Diagnostics.Debug.WriteLine(logEntry);
            }
            catch (Exception logEx)
            {
                System.Diagnostics.Debug.WriteLine("Message logging failed: " + logEx.Message);
                ExceptionHandler.LogError(logEx);
            }
        }
        public static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;

            if (exception != null)
            {
                System.Diagnostics.Debug.WriteLine($"Unhandled Exception Type: {exception.GetType().FullName}");
                System.Diagnostics.Debug.WriteLine($"Message: {exception.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {exception.StackTrace}");

                LogError(exception);
            }
        }

    }
}
