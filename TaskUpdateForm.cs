using System;
using System.Windows.Forms;
using Npgsql;

namespace TaskManagerApp
{
    public partial class TaskUpdateForm : Form
    {
        private int taskId;
        private string username;

        private TextBox txtTitle;
        private TextBox txtDescription;
        private ComboBox cmbStatus;
        private Button btnSave;

        public TaskUpdateForm(int taskId, string username)
        {
            this.taskId = taskId;
            this.username = username;

            InitializeComponents();
            LoadTaskDetails();
        }

        private void InitializeComponents()
        {
            this.Text = "Update Task";
            this.Size = new System.Drawing.Size(420, 300);

            // Title
            Label lblTitle = new Label() { Text = "Title", Location = new System.Drawing.Point(10, 10) };
            this.Controls.Add(lblTitle);

            txtTitle = new TextBox() { Location = new System.Drawing.Point(120, 10), Width = 250 };
            this.Controls.Add(txtTitle);

            // Description
            Label lblDescription = new Label() { Text = "Description", Location = new System.Drawing.Point(10, 50) };
            this.Controls.Add(lblDescription);

            txtDescription = new TextBox() { Location = new System.Drawing.Point(120, 50), Width = 250, Height = 100, Multiline = true };
            this.Controls.Add(txtDescription);

            // Status
            Label lblStatus = new Label() { Text = "Status", Location = new System.Drawing.Point(10, 170) };
            this.Controls.Add(lblStatus);

            cmbStatus = new ComboBox() { Location = new System.Drawing.Point(120, 170), Width = 250 };
            cmbStatus.Items.AddRange(new string[] { "Pending", "Completed", "In Progress" });
            this.Controls.Add(cmbStatus);

            // Save Button
            btnSave = new Button() { Text = "Save", Location = new System.Drawing.Point(120, 220) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }

        private void LoadTaskDetails()
        {
            // Charger les détails de la tâche sélectionnée
            try
            {
                string host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
                string dbUsername = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
                string dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "password";
                string database = Environment.GetEnvironmentVariable("DB_NAME") ?? "postgres";

                string connectionString = $"Host={host};Username={dbUsername};Password={dbPassword};Database={database}";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT title, description, status FROM tasks WHERE id = @taskId";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("taskId", taskId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtTitle.Text = reader.GetString(0);
                                txtDescription.Text = reader.GetString(1);
                                cmbStatus.SelectedItem = reader.GetString(2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading the task details: {ex.Message}");
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Enregistrer les modifications de la tâche
            try
            {
                string host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
                string dbUsername = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
                string dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "password";
                string database = Environment.GetEnvironmentVariable("DB_NAME") ?? "postgres";

                string connectionString = $"Host={host};Username={dbUsername};Password={dbPassword};Database={database}";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE tasks SET title = @title, description = @description, status = @status WHERE id = @taskId";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("title", txtTitle.Text);
                        cmd.Parameters.AddWithValue("description", txtDescription.Text);
                        cmd.Parameters.AddWithValue("status", cmbStatus.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("taskId", taskId);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Task updated successfully.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the task: {ex.Message}");
            }
        }
    }
}
