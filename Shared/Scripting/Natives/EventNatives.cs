using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;

namespace Shared.Scripting.Natives
{
    public class EventNatives
    {
        public Shared.Scripting.Event Event;
        public Shared.Scripting.EventsManager EventsManager;

        public string Name
        {
            get
            {
                return Event.Name;
            }
        }

        public EventNatives(string Name)
        {
#if SERVER
            EventsManager = ivmp_server_core.Server.EventsManager;
#endif
            EventsManager.AddEvent(Name);
            Event = EventsManager.GetEvent(Name);
        }

        public void AddHandler(Jint.Native.JsValue Function)
        {
            Shared.Scripting.EventHandler EventHandler = new Shared.Scripting.EventHandler();
            EventHandler.Function = Function;
            Event.AddHandler(EventHandler);
        }

        public void Trigger(params Jint.Native.JsValue[] Arguments)
        {
            Event.Trigger(Arguments);
        }
    }
}
