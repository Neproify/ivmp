using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;

namespace ivmp_server_core
{
    public class Element
    {
        public Server Server;

        public int ID;

        public string Model;

        public SharpDX.Vector3 Position;

        public SharpDX.Vector3 Velocity;

        public SharpDX.Quaternion Rotation;

        public float Heading;

        public Element()
        {
            //Server.EventsManager.GetEvent("OnElementCreated").Trigger(Jint.Native.JsValue.FromObject(Server.Engine, new Scripting.Natives.ElementNatives().Element = this));
        }

        public void Destroy()
        {
            //Server.EventsManager.GetEvent("OnElementDestroyed").Trigger(Jint.Native.JsValue.FromObject(Server.Engine, new Scripting.Natives.ElementNatives().Element = this));
        }
    }
}
