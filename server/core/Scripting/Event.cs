using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ivmp_server_core.Scripting
{
    public class Event
    {
        public string Name;
        public List<EventHandler> Handlers;

        public Event(string Name)
        {
            this.Name = Name;
            Handlers = new List<EventHandler>();
        }

        public bool AddHandler(EventHandler Handler)
        {
            Handlers.Add(Handler);
            return true;
        }

        public void Trigger(params Jint.Native.JsValue[] Arguments)
        {
            Handlers.ForEach(Handler => Handler.Trigger(Arguments));
        }
    }
}
