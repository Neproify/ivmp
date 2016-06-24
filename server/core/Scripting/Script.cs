/*
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE', which is part of this source code package.
 * Copyright (c) 2016 Neproify
*/

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
        public string Name;

        public Scripting.Natives.Console ConsoleNatives;

        public Script()
        {
            ConsoleNatives = new Scripting.Natives.Console();
            Engine = new Engine(cfg => cfg.AllowClr(typeof(Scripting.Natives.Vehicle).Assembly,
                typeof(SharpDX.Vector2).Assembly,
                typeof(SharpDX.Vector3).Assembly,
                typeof(SharpDX.Vector4).Assembly,
                typeof(SharpDX.Quaternion).Assembly));
            Engine.SetValue("Vector2", new Func<float, float, SharpDX.Vector2>((X, Y) => { return new SharpDX.Vector2(X, Y); }));
            Engine.SetValue("Vector3", new Func<float, float, float, SharpDX.Vector3>((X, Y, Z) => { return new SharpDX.Vector3(X, Y, Z); }));
            Engine.SetValue("Vector4", new Func<float, float, float, float, SharpDX.Vector4>((X, Y, Z, W) => { return new SharpDX.Vector4(X, Y, Z, W); }));
            Engine.SetValue("Quaternion", new Func<float, float, float, float, SharpDX.Quaternion>((X, Y, Z, W) => { return new SharpDX.Quaternion(X, Y, Z, W); }));
            Engine.SetValue("Console", ConsoleNatives);
            Engine.SetValue("Event", new Func<string, Scripting.Natives.Event>((Name) => { return new Scripting.Natives.Event(Name, Server.EventsManager); }));
            Engine.SetValue("Vehicle", new Func<string, SharpDX.Vector3, Scripting.Natives.Vehicle>((Model, Position) => { return new Scripting.Natives.Vehicle(Model, Position, Server.VehiclesController); }));
        }

        public void Set(string Code)
        {
            this.Code = Code;
        }

        public void Execute()
        {
            try
            {
                Engine.Execute(Code);
            }
            catch(Exception e)
            {
                Console.WriteLine(e + " in script " + Name);
            }
        }
    }
}
