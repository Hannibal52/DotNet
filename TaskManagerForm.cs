using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace TaskManagerApp
{
    public partial class TaskManagerForm : Form
    {
        private DataGridView dgvTasks;
        private Button btnAddTask;
        private Button btnEditTask;
        private Button btnDeleteTask;
        private string username;  // Champ pour stocker le nom d'utilisateur

        public TaskManagerForm(string username)
        {
            this.username = username;  // Initialiser le nom d'utilisateur
            this.Text = "Task Manager";
            this.WindowState = FormWindowState.Maximized;

            InitializeComponents();
            LoadTasks();  // Charger les tâches à l'initialisation du formulaire
        }

        private void InitializeComponents()
        {
            // DataGridView pour afficher les tâches
            dgvTasks = new DataGridView();
            dgvTasks.Location = new System.Drawing.Point(10, 10);
            dgvTasks.Size = new System.Drawing.Size(760, 400);
            dgvTasks.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvTasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTasks.AllowUserToAddRows = false;
            dgvTasks.AllowUserToDeleteRows = false;
            dgvTasks.ReadOnly = true;
            dgvTasks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Bouton Ajouter une tâche
            btnAddTask = new Button();
            btnAddTask.Text = "Add Task";
            btnAddTask.Location = new System.Drawing.Point(10, 420);
            btnAddTask.Click += new EventHandler(BtnAddTask_Click);

            // Bouton Modifier une tâche
            btnEditTask = new Button();
            btnEditTask.Text = "Edit Task";
            btnEditTask.Location = new System.Drawing.Point(110, 420);
            btnEditTask.Click += new EventHandler(BtnEditTask_Click);

            // Bouton Supprimer une tâche
            btnDeleteTask = new Button();
            btnDeleteTask.Text = "Delete Task";
            btnDeleteTask.Location = new System.Drawing.Point(210, 420);
            btnDeleteTask.Click += new EventHandler(BtnDeleteTask_Click);

            // Ajouter les composants au formulaire
            this.Controls.Add(dgvTasks);
            this.Controls.Add(btnAddTask);
            this.Controls.Add(btnEditTask);
            this.Controls.Add(btnDeleteTask);
        }

        private void LoadTasks()
        {
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

                    // Requête pour récupérer toutes les tâches de l'utilisateur connecté
                    string query = "SELECT id, title, description, status, created_at FROM tasks WHERE user_id = (SELECT user_id FROM users WHERE username = @username)";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("username", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);

                            dgvTasks.DataSource = dataTable;  // Afficher les tâches dans le DataGridView
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading tasks: {ex.Message}");
            }
        }

        // Handlers pour les boutons (à implémenter)
        private void BtnAddTask_Click(object sender, EventArgs e)
        {
            // Logique pour ajouter une tâche
            MessageBox.Show("Add Task button clicked.");
        }

        private void BtnEditTask_Click(object sender, EventArgs e)
        {
            // Logique pour modifier une tâche
            MessageBox.Show("Edit Task button clicked.");
        }

        private void BtnDeleteTask_Click(object sender, EventArgs e)
        {
            // Logique pour supprimer une tâche
            MessageBox.Show("Delete Task button clicked.");
        }
    }
}
