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
            string host = Environment.GetEnvironmentVariable("DB_HOST") ?? throw new InvalidOperationException("DB_HOST is not set");
            string username = Environment.GetEnvironmentVariable("DB_USER") ?? throw new InvalidOperationException("DB_USER is not set");
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? throw new InvalidOperationException("DB_PASSWORD is not set");
            string database = Environment.GetEnvironmentVariable("DB_NAME") ?? throw new InvalidOperationException("DB_NAME is not set");

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
