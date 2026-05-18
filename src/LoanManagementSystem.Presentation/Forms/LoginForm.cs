using System;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using LoanManagementSystem.Application.Services;

namespace LoanManagementSystem.Presentation.Forms
{
    public partial class LoginForm : Form
    {
        private readonly IAuthenticationService _authService;
        private int failedAttempts = 0;
        private const int MAX_FAILED_ATTEMPTS = 5;
        private DateTime lockoutTime = DateTime.MinValue;
        private const int LOCKOUT_DURATION_MINUTES = 15;

        private Guna2TextBox usernameTextBox;
        private Guna2TextBox passwordTextBox;
        private Guna2Button loginButton;
        private CheckBox rememberCheckBox;

        public LoginForm(IAuthenticationService authService)
        {
            InitializeComponent();
            _authService = authService;
            ConfigureArabicUI();
        }

        private void ConfigureArabicUI()
        {
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Text = "نظام إدارة القروض - تسجيل الدخول";
            this.BackColor = System.Drawing.Color.FromArgb(15, 32, 65);
            this.ClientSize = new System.Drawing.Size(450, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Main Panel
            var mainPanel = new Guna2Panel();
            mainPanel.FillColor = System.Drawing.Color.FromArgb(15, 32, 65);
            mainPanel.Dock = DockStyle.Fill;

            // Title Label
            var titleLabel = new Label();
            titleLabel.Text = "نظام إدارة القروض";
            titleLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.White;
            titleLabel.RightToLeft = RightToLeft.Yes;
            titleLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            titleLabel.Location = new System.Drawing.Point(20, 40);
            titleLabel.Size = new System.Drawing.Size(400, 50);

            // Username Label
            var usernameLabel = new Label();
            usernameLabel.Text = "اسم المستخدم:";
            usernameLabel.ForeColor = System.Drawing.Color.White;
            usernameLabel.RightToLeft = RightToLeft.Yes;
            usernameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            usernameLabel.Location = new System.Drawing.Point(220, 120);
            usernameLabel.Size = new System.Drawing.Size(150, 30);

            // Username TextBox
            usernameTextBox = new Guna2TextBox();
            usernameTextBox.PlaceholderText = "أدخل اسم المستخدم";
            usernameTextBox.RightToLeft = RightToLeft.Yes;
            usernameTextBox.Location = new System.Drawing.Point(50, 155);
            usernameTextBox.Size = new System.Drawing.Size(320, 50);
            usernameTextBox.BorderColor = System.Drawing.Color.FromArgb(0, 122, 204);
            usernameTextBox.FocusedState.BorderColor = System.Drawing.Color.FromArgb(0, 180, 255);

            // Password Label
            var passwordLabel = new Label();
            passwordLabel.Text = "كلمة المرور:";
            passwordLabel.ForeColor = System.Drawing.Color.White;
            passwordLabel.RightToLeft = RightToLeft.Yes;
            passwordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            passwordLabel.Location = new System.Drawing.Point(220, 230);
            passwordLabel.Size = new System.Drawing.Size(150, 30);

            // Password TextBox
            passwordTextBox = new Guna2TextBox();
            passwordTextBox.PlaceholderText = "أدخل كلمة المرور";
            passwordTextBox.RightToLeft = RightToLeft.Yes;
            passwordTextBox.UseSystemPasswordChar = true;
            passwordTextBox.Location = new System.Drawing.Point(50, 265);
            passwordTextBox.Size = new System.Drawing.Size(320, 50);
            passwordTextBox.BorderColor = System.Drawing.Color.FromArgb(0, 122, 204);
            passwordTextBox.FocusedState.BorderColor = System.Drawing.Color.FromArgb(0, 180, 255);

            // Login Button
            loginButton = new Guna2Button();
            loginButton.Text = "تسجيل الدخول";
            loginButton.RightToLeft = RightToLeft.Yes;
            loginButton.FillColor = System.Drawing.Color.FromArgb(0, 122, 204);
            loginButton.ForeColor = System.Drawing.Color.White;
            loginButton.Location = new System.Drawing.Point(50, 340);
            loginButton.Size = new System.Drawing.Size(320, 50);
            loginButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            loginButton.Click += LoginButton_Click;

            // Remember Me Checkbox
            rememberCheckBox = new CheckBox();
            rememberCheckBox.Text = "تذكر بيانات الدخول";
            rememberCheckBox.ForeColor = System.Drawing.Color.White;
            rememberCheckBox.RightToLeft = RightToLeft.Yes;
            rememberCheckBox.Location = new System.Drawing.Point(60, 410);
            rememberCheckBox.Size = new System.Drawing.Size(300, 25);

            // Add controls to panel
            mainPanel.Controls.Add(titleLabel);
            mainPanel.Controls.Add(usernameLabel);
            mainPanel.Controls.Add(usernameTextBox);
            mainPanel.Controls.Add(passwordLabel);
            mainPanel.Controls.Add(passwordTextBox);
            mainPanel.Controls.Add(loginButton);
            mainPanel.Controls.Add(rememberCheckBox);

            this.Controls.Add(mainPanel);
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            if (DateTime.Now < lockoutTime)
            {
                MessageBox.Show(
                    $"الحساب مغلق مؤقتًا. حاول لاحقًا في {lockoutTime:HH:mm:ss}",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("يرجى إدخال اسم المستخدم وكلمة المرور", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var result = await _authService.AuthenticateAsync(username, password);

                if (result.IsSuccessful)
                {
                    failedAttempts = 0;
                    SessionManager.SetCurrentUser(result.UserId.Value, result.Username, result.FullName, result.RoleId.Value);

                    MessageBox.Show("تم تسجيل الدخول بنجاح", "نجح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();
                    // Open main form here
                }
                else
                {
                    failedAttempts++;

                    if (failedAttempts >= MAX_FAILED_ATTEMPTS)
                    {
                        lockoutTime = DateTime.Now.AddMinutes(LOCKOUT_DURATION_MINUTES);
                        MessageBox.Show(
                            $"تم تجاوز عدد محاولات الدخول. يتم قفل الحساب لمدة {LOCKOUT_DURATION_MINUTES} دقائق",
                            "خطأ",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(result.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }

    public static class SessionManager
    {
        public static int CurrentUserId { get; set; }
        public static string CurrentUsername { get; set; }
        public static string CurrentUserFullName { get; set; }
        public static int CurrentUserRoleId { get; set; }

        public static void SetCurrentUser(int userId, string username, string fullName, int roleId)
        {
            CurrentUserId = userId;
            CurrentUsername = username;
            CurrentUserFullName = fullName;
            CurrentUserRoleId = roleId;
        }

        public static void ClearSession()
        {
            CurrentUserId = 0;
            CurrentUsername = null;
            CurrentUserFullName = null;
            CurrentUserRoleId = 0;
        }
    }
}