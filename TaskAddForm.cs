using System;
using System.Windows.Forms;
using Npgsql;

namespace TaskManagerApp
{
    public partial class TaskAddForm : Form
    {
        private string username;

        private TextBox txtTitle;
        private TextBox txtDescription;
        private ComboBox cmbStatus;
        private Button btnSave;

        public TaskAddForm(string username)
        {
            this.username = username;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Add Task";
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
            btnSave = new Button() { Text = "Save", Location = new System.Drawing.Point(100, 220) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {

                string host = "10.10.10.132";
                string dbUsername = "taskuser";
                string dbPassword = "P@ssw0rd";
                string database = "taskmanagerdb";





                string connectionString = $"Host={host};Username={dbUsername};Password={dbPassword};Database={database}";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO tasks (user_id, title, description, status) VALUES ((SELECT user_id FROM users WHERE username = @username), @title, @description, @status)";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("username", username);
                        cmd.Parameters.AddWithValue("title", txtTitle.Text);
                        cmd.Parameters.AddWithValue("description", txtDescription.Text);
                        cmd.Parameters.AddWithValue("status", cmbStatus.SelectedItem.ToString());

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Task added successfully.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the task: {ex.Message}");
            }
        }
    }
}
