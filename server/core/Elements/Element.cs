using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;
using SharpDX;

namespace ivmp_server_core
{
    public class Element
    {
        public int ID;

        public string Model;

        public Vector3 Position;

        public Vector3 Velocity;

        public Quaternion Rotation;

        public float Heading;

        public string Type;

        public Element()
        {
            Type = "Element";
            //Server.EventsManager.GetEvent("OnElementCreated").Trigger(Jint.Native.JsValue.FromObject(Server.Engine, new Scripting.Natives.ElementNatives().Element = this));
        }

        public void Destroy()
        {
            //Server.EventsManager.GetEvent("OnElementDestroyed").Trigger(Jint.Native.JsValue.FromObject(Server.Engine, new Scripting.Natives.ElementNatives().Element = this));
        }
    }
}
