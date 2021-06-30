using Microsoft.EntityFrameworkCore;
using PersonalBot.Data.Models;
using PersonalBot.Resources.Providers.Declarations;

namespace PersonalBot.Data
{
    public sealed class MainDbContext : DbContext
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<Note> Notes { get; set; }
        
        private ISettingsProvider _settingsProvider;

        public MainDbContext(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_settingsProvider["connection_string"]);
        }
    }
}