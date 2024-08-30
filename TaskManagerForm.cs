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
            // Bouton Ajouter une tâche
            btnAddTask = new Button();
            btnAddTask.Text = "Add Task";
            btnAddTask.Location = new System.Drawing.Point(10, 10);
            btnAddTask.Click += new EventHandler(BtnAddTask_Click);
            this.Controls.Add(btnAddTask);

            // DataGridView pour afficher les tâches
            dgvTasks = new DataGridView();
            dgvTasks.Location = new System.Drawing.Point(10, 50);
            dgvTasks.Size = new System.Drawing.Size(760, 400);
            dgvTasks.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvTasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTasks.AllowUserToAddRows = false;
            dgvTasks.AllowUserToDeleteRows = false;
            dgvTasks.ReadOnly = true;
            dgvTasks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Ajouter une colonne de bouton pour la mise à jour
            DataGridViewButtonColumn btnUpdateColumn = new DataGridViewButtonColumn();
            btnUpdateColumn.Name = "Update";
            btnUpdateColumn.Text = "Update";
            btnUpdateColumn.UseColumnTextForButtonValue = true; // Affiche "Update" dans tous les boutons
            btnUpdateColumn.Width = 60; // Réduire la largeur de la colonne Update
            dgvTasks.Columns.Add(btnUpdateColumn);

            // Ajouter une colonne de bouton pour la suppression
            DataGridViewButtonColumn btnDeleteColumn = new DataGridViewButtonColumn();
            btnDeleteColumn.Name = "Delete";
            btnDeleteColumn.Text = "Delete";
            btnDeleteColumn.UseColumnTextForButtonValue = true; // Affiche "Delete" dans tous les boutons
            btnDeleteColumn.Width = 60; // Réduire la largeur de la colonne Delete
            dgvTasks.Columns.Add(btnDeleteColumn);

            // Ajouter les composants au formulaire
            this.Controls.Add(dgvTasks);

            // Associer l'événement CellClick pour gérer les clics sur les boutons dans le DataGridView
            dgvTasks.CellClick += DgvTasks_CellClick;
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

        private void DgvTasks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Vérifie que la cellule cliquée est bien une cellule de bouton
            if (e.RowIndex >= 0 && (e.ColumnIndex == dgvTasks.Columns["Update"].Index || e.ColumnIndex == dgvTasks.Columns["Delete"].Index))
            {
                int taskId = Convert.ToInt32(dgvTasks.Rows[e.RowIndex].Cells["id"].Value);

                if (e.ColumnIndex == dgvTasks.Columns["Update"].Index)
                {
                    // Ouvrir un formulaire de mise à jour avec les informations de la tâche sélectionnée
                    TaskUpdateForm updateForm = new TaskUpdateForm(taskId, username);
                    updateForm.ShowDialog();
                    LoadTasks(); // Recharger les tâches après la mise à jour
                }
                else if (e.ColumnIndex == dgvTasks.Columns["Delete"].Index)
                {
                    // Demande de confirmation de suppression
                    var result = MessageBox.Show("Are you sure you want to delete this task?", "Delete Task", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        DeleteTask(taskId);
                        LoadTasks(); // Recharger les tâches après la suppression
                    }
                }
            }
        }

        private void DeleteTask(int taskId)
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

                    string query = "DELETE FROM tasks WHERE id = @taskId";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("taskId", taskId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the task: {ex.Message}");
            }
        }

        // Handlers pour les boutons (à implémenter)
        private void BtnAddTask_Click(object sender, EventArgs e)
        {
            TaskAddForm addForm = new TaskAddForm(username);
            addForm.ShowDialog();
            LoadTasks(); // Recharger les tâches après l'ajout
        }

    }
}
