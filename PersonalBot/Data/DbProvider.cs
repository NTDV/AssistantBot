using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using PersonalBot.Data.Models;
using PersonalBot.Resources.Providers.Declarations;

namespace PersonalBot.Data
{
    public class DbProvider
    {
        private readonly MainDbContext _db;

        public DbProvider(ISettingsProvider settingsProvider)
        {
            _db = new MainDbContext(settingsProvider);
        }
        
        public Event[] GetAllEvents(long ownerId)
        {
            return _db.Events.Where(e => e.ChatId == ownerId).ToArray();
        }

        public Event[] GetNotifyingEvents()
        {
            TimeZoneInfo moscowTimezone;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                moscowTimezone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                moscowTimezone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");
            else
                throw new PlatformNotSupportedException("Because of different timezones declarations we supports onli Linux and Windows now.");
            
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowTimezone);
            
            var events = _db.Events.ToArray().Where(e => e.Time <= now).ToArray();
            _db.Events.RemoveRange(events);
            _db.SaveChangesAsync();
            return events;
        }
        public async Task<int> AddEventAsync(Event @event)
        { 
            await _db.Events.AddAsync(@event);
            return await _db.SaveChangesAsync();
        }
        
        public Event GetEvent(long id)
        {
            return _db.Events.Find(id);
        }
        
        public async Task<int> RemoveEvent(Event @event)
        {
            _db.Events.Remove(@event);
            return await _db.SaveChangesAsync();
        }
        
        public async Task<int> AddNoteAsync(Note note)
        { 
            await _db.Notes.AddAsync(note);
            return await _db.SaveChangesAsync();
        }
        
        public Note[] GetAllNotes(long ownerId)
        {
            return _db.Notes.Where(e => e.ChatId == ownerId).ToArray();
        }

        public Note GetNote(long id)
        {
            return _db.Notes.Find(id);
        }

        public async Task<int> RemoveNote(Note note)
        {
            _db.Notes.Remove(note);
            return await _db.SaveChangesAsync();
        }
    }
}