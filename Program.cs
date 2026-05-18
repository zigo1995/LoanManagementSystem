using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;
using System.IO;
using LoanManagementSystem.Presentation;

namespace LoanManagementSystem
{
    static class Program
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// The main entry point for the application
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Enable visual styles
                Application.EnableVisualStyles();
                Application.DefaultFont = new System.Drawing.Font("Segoe UI", 10F);
                Application.SetHighDpiMode(HighDpiMode.SystemAware);

                // Configure services
                var services = new ServiceCollection();

                // Load configuration
                var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                Configuration = configBuilder.Build();

                // Add configuration to services
                services.AddSingleton(Configuration);

                // Add application services
                var connectionString = Configuration.GetConnectionString("DefaultConnection");
                services.AddApplicationServices(connectionString);

                ServiceProvider = services.BuildServiceProvider();

                // Run application
                Application.Run(new SplashScreen());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"خطأ في بدء التطبيق:\n\n{ex.Message}\n\n{ex.StackTrace}",
                    "خطأ حرج",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }
    }

    /// <summary>
    /// Splash Screen shown while application initializes
    /// </summary>
    public class SplashScreen : Form
    {
        public SplashScreen()
        {
            this.BackColor = System.Drawing.Color.FromArgb(15, 32, 65);
            this.Size = new System.Drawing.Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ControlBox = false;

            var label = new Label();
            label.Text = "نظام إدارة القروض\nجاري التحميل...";
            label.ForeColor = System.Drawing.Color.White;
            label.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            label.RightToLeft = RightToLeft.Yes;
            label.Dock = DockStyle.Fill;

            this.Controls.Add(label);

            var timer = new Timer();
            timer.Interval = 3000;
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                var loginForm = new LoginForm(Program.ServiceProvider.GetService(typeof(IAuthenticationService)) as IAuthenticationService);
                loginForm.Show();
                this.Hide();
            };
            timer.Start();
        }
    }
}