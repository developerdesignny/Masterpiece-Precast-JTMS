using JTMS.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Windows;

namespace JTMS.Data
{
    public class JTMSContext : DbContext
    {
        public DbSet<MoldModel> Molds { get; set; }
        public DbSet<ProjectNotifications> Notifications { get; set; }
        public DbSet<MoldDetailsModel> SubMolds { get; set; }
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<CompanyModel> Companies { get; set; }
        public DbSet<PalletModel> Pallets { get; set; }
        public DbSet<ProgressReportModel> ProgressReports { get; set; }
        public DbSet<ProjectNotesModels> ProjectNotes { get; set; }
        public JTMSContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                var dbVersion = string.IsNullOrEmpty(Properties.Settings.Default.DatabaseVersion) ? "8.4.0" : Properties.Settings.Default.DatabaseVersion;
                var connectionStr = string.IsNullOrEmpty(Properties.Settings.Default.ConnectionString) ? "Server=127.0.0.1;Database=jtms;Uid=root;Pwd=pass;SslMode=None;AllowPublicKeyRetrieval=True" : Properties.Settings.Default.ConnectionString;

                optionsBuilder.UseMySql(connectionStr, ServerVersion.Parse(dbVersion, ServerType.MySql));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
