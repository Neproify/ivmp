using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Scripting
{
    public class EventHandler
    {
        public Jint.Native.JsValue Function;

        public EventHandler()
        {
        }

        public void Trigger(Jint.Native.JsValue[] Arguments)
        {
            Function.Invoke(Arguments);
        }
    }
}
