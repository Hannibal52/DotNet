using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Npgsql;

namespace TaskManagerApp
{
    public partial class Form1 : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;

        public Form1()
        {
            this.Text = "Login";

            // Username Label
            Label lblUsername = new Label();
            lblUsername.Text = "Username:";
            lblUsername.Location = new System.Drawing.Point(10, 20);
            this.Controls.Add(lblUsername);

            // Username TextBox
            txtUsername = new TextBox();
            txtUsername.Location = new System.Drawing.Point(120, 20);
            txtUsername.Width = 200;
            this.Controls.Add(txtUsername);

            // Password Label
            Label lblPassword = new Label();
            lblPassword.Text = "Password:";
            lblPassword.Location = new System.Drawing.Point(10, 60);
            this.Controls.Add(lblPassword);

            // Password TextBox
            txtPassword = new TextBox();
            txtPassword.Location = new System.Drawing.Point(120, 60);
            txtPassword.Width = 200;
            txtPassword.PasswordChar = '*';
            this.Controls.Add(txtPassword);

            // Login Button
            btnLogin = new Button();
            btnLogin.Text = "Login";
            btnLogin.Location = new System.Drawing.Point(100, 100);
            btnLogin.Click += new EventHandler(BtnLogin_Click);
            this.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (TestDatabaseConnection(username, password))
            {
                MessageBox.Show("Login successful!");
            }
            else
            {
                MessageBox.Show("Login failed. Please check your username and password.");
            }
        }

        private bool TestDatabaseConnection(string username, string password)
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

                    // Query to check if the username and password match an existing user
                    string query = "SELECT COUNT(1) FROM users WHERE username = @username AND password_hash = @password";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("username", username);
                        cmd.Parameters.AddWithValue("password", password); // Comparaison du mot de passe brut
                                                                           // Hash the password before checking

                        object? result = cmd.ExecuteScalar();
                        if (result != null && long.TryParse(result.ToString(), out long userCount))
                        {
                            return userCount > 0; // Returns true if a matching user is found
                        }
                        else
                        {
                            return false; // No matching user found
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return false;
            }
        }

        // Method to hash passwords using SHA-256
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
