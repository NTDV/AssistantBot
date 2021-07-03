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
        
        public async Task<Event> GetEvent(long id)
        {
            return await _db.Events.FindAsync(id);
        }

        public async Task<int> EditEvent(long id, Event @new)
        {
            var old = await GetEvent(id);

            if (old?.IsContentEquals(@new) ?? true) 
                return 0;
            
            _db.Entry(old).CurrentValues.SetValues(@new);
            return await _db.SaveChangesAsync();
        }
        
        public async Task<int> EditEvent(Event old, Event @new)
        {
            if (old?.IsContentEquals(@new) ?? true) 
                return 0;

            old.Title = @new.Title;
            old.Place = @new.Place;
            old.Info = @new.Info;
            old.Time = @new.Time;
            
            return await _db.SaveChangesAsync();
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

        public async Task<Note> GetNote(long id)
        {
            return await _db.Notes.FindAsync(id);
        }

        public async Task<int> EditNote(Note old, Note @new)
        {
            if (old?.IsContentEquals(@new) ?? true) 
                return 0;
            
            old.Title = @new.Title;
            old.Description = @new.Description;
            
            return await _db.SaveChangesAsync();
        }
        
        public async Task<int> RemoveNote(Note note)
        {
            _db.Notes.Remove(note);
            return await _db.SaveChangesAsync();
        }
    }
}