using System;
using Npgsql;
using System.Windows.Forms;

namespace TaskManagerApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Console.WriteLine("Program started.");

            // Retrieve connection information from environment variables
            string host = "10.10.10.132";  
            string username = "taskuser";
            string password = "P@ssw0rd";
            string database = "taskmanagerdb";

            Console.WriteLine("Environment variables loaded.");

            // Build the connection string using the environment variables
            string connectionString = $"Host={host};Username={username};Password={password};Database={database}";

            Console.WriteLine("Connection string built.");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Connected to PostgreSQL successfully!");

                // Here you can add code to interact with the database (e.g., fetch users or tasks)

                connection.Close();
            }

            Console.WriteLine("Connection closed. Starting the application...");

            // This runs the Windows Forms application
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

            Console.WriteLine("Application is running.");
        }
    }
}
