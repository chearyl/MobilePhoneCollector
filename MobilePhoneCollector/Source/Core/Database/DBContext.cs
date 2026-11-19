using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace MobilePhoneCollector.Source.Core.Database
{
    public class DBContext : DbContext
    {
        private string _defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PhoneCollector", $"phone.db");
        private string _connectionString = "";
        private int _version = 1;

        public DbSet<Phone> Phone { get; set; }
        public DbSet<Specs> Specs { get; set; }
        public DbSet<FileData> FileData { get; set; }
        public DbSet<PhoneImage> PhoneImage { get; set; }
        public DbSet<PhoneReview> PhoneReview { get; set; }
        public DbSet<Brands> Brands { get; set; }
        public int Version => _version;

        public DBContext()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_defaultPath) ?? "");
            _connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = _defaultPath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                ForeignKeys = true,
            }.ToString();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}
