using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ivmp_server_core.Scripting
{
    public class EventsManager
    {
        public List<Event> Events;

        public EventsManager()
        {
            Events = new List<Event>();
        }

        public bool AddEvent(string Name)
        {
            if(Events.Any(Event => Event.Name == Name))
            {
                return false;
            }
            Event CreatedEvent = new Event(Name);
            Events.Add(CreatedEvent);
            return true;
        }

        public Event GetEvent(string Name)
        {
            return Events.Find(Event => Event.Name == Name);
        }
    }
}
