using System;
using System.Linq;
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
        
        public Event[] GetAllEvents()
        {
            return _db.Events.ToArray();
        }

        public Event[] GetNotifyingEvents()
        {
            var now = DateTime.Now;
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
        
        public Note[] GetAllNotes()
        {
            return _db.Notes.ToArray();
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