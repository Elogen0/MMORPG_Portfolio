using InflearnServer.Game.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace InflearnServer.DB
{
    public class AppDbContext : DbContext
    {
        public DbSet<AccountDb> Accounts { get; set; }
        public DbSet<PlayerDb> Players { get; set; }
        public DbSet<ItemDb> Items { get; set; }
        public DbSet<QuestDb> Quests { get; set; }

        static readonly ILoggerFactory _logger = LoggerFactory.Create(builder => { builder.AddConsole(); });
        string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GameDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options
                .UseLoggerFactory(_logger)
                .UseSqlServer(ConfigManager.Config == null ? _connectionString : ConfigManager.Config.connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AccountDb>()
                .HasIndex(a => a.AccountName)
                .IsUnique();

            builder.Entity<PlayerDb>()
                .HasIndex(a => a.PlayerName)
                .IsUnique();
        }
    }
}
