using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace ivmp_server_core.Scripting
{
    public class ServerScript : Shared.Scripting.Script
    {
        public string Type;

        public ServerScript()
        {
            Engine.SetValue("Vehicle", new Func<string, Vector3, Scripting.Natives.Vehicle>((Model, Position) => { return new Scripting.Natives.Vehicle(Model, Position, Server.VehiclesController); }));
        }
    }
}
