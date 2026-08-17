using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Helper
{
    public interface IHelper
    {
        public void LogError(string message, Exception ex = null);
    }
    public class Helper : IHelper
    {
        public void LogError(string message, Exception ex = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "An error occurred while processing the request.";
            }

            try
            {
                // Get project root (one level up from bin folder)
                string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));

                // Create Logs folder inside project root
                string folderPath = Path.Combine(projectRoot, "Logs");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Daily log file name
                string filePath = Path.Combine(folderPath, $"error_{DateTime.Now:yyyyMMdd}.txt");

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    writer.WriteLine("Error Message: " + message);

                    if (ex != null)
                    {
                        writer.WriteLine("Exception: " + ex.Message);
                        writer.WriteLine("Stack Trace: " + ex.StackTrace);
                    }

                    writer.WriteLine(new string('-', 50));
                }
            }
            catch (Exception loggingEx)
            {
                Console.WriteLine($"Logging failed: {loggingEx.Message}");
            }
        }
    }
}