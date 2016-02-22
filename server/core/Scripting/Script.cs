using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;

namespace ivmp_server_core.Scripting
{
    public class Script
    {
        public Server Server;

        public Engine Engine;
        public string Code;

        public Scripting.Natives.Console ConsoleNatives;

        public Script()
        {
            ConsoleNatives = new Scripting.Natives.Console();
            Engine = new Engine(cfg => cfg.AllowClr(typeof(Scripting.Natives.Vehicle).Assembly,
                 typeof(SharpDX.Vector3).Assembly));
            Engine.SetValue("Console", ConsoleNatives);
            Engine.SetValue("Vector3", new Func<float, float, float, SharpDX.Vector3>((X, Y, Z) => { return new SharpDX.Vector3(X, Y, Z); }));
            Engine.SetValue("Vehicle", new Func<string, SharpDX.Vector3, Scripting.Natives.Vehicle>((Model, Position) => { return new Scripting.Natives.Vehicle(Model, Position, Server.VehiclesController); }));
        }

        public void Set(string Code)
        {
            this.Code = Code;
        }

        public void Execute()
        {
            Engine.Execute(Code);
        }
    }
}
