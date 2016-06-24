using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;

namespace ivmp_server_core.Scripting.Natives
{
    public class Event
    {
        public ivmp_server_core.Scripting.Event ServerEvent;
        public EventsManager EventsManager;

        public string Name;

        public Event(string Name, EventsManager EventsManager)
        {
            this.EventsManager = EventsManager;
            EventsManager.AddEvent(Name);
            ServerEvent = EventsManager.GetEvent(Name);
            this.Name = Name;
        }

        public void AddHandler(Jint.Native.JsValue Function)
        {
            EventHandler EventHandler = new EventHandler();
            EventHandler.Function = Function;
            ServerEvent.AddHandler(EventHandler);
        }

        public void Trigger(params Jint.Native.JsValue[] Arguments)
        {
            ServerEvent.Trigger(Arguments);
        }
    }
}
