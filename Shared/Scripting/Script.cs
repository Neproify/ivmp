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
using SharpDX;

namespace Shared.Scripting
{
    public class Script
    {
        public Jint.Engine Engine;
        public string Code;
        public string Name;

        public Scripting.Natives.Console ConsoleNatives;

        public Script()
        {
            ConsoleNatives = new Scripting.Natives.Console();
            Engine = new Engine(cfg => cfg.AllowClr(typeof(Vector2).Assembly,
                typeof(Vector3).Assembly,
                typeof(Vector4).Assembly,
                typeof(Quaternion).Assembly));
#if SERVER
            Engine.SetValue("Vector2", new Func<float, float, Vector2>((X, Y) => { return new Vector2(X, Y); }));
            Engine.SetValue("Vector3", new Func<float, float, float, Vector3>((X, Y, Z) => { return new Vector3(X, Y, Z); }));
            Engine.SetValue("Vector4", new Func<float, float, float, float, Vector4>((X, Y, Z, W) => { return new Vector4(X, Y, Z, W); }));
            Engine.SetValue("Quaternion", new Func<float, float, float, float, Quaternion>((X, Y, Z, W) => { return new Quaternion(X, Y, Z, W); }));
#endif
#if CLIENT
            Engine.SetValue("Vector2", new Func<float, float, GTA.Vector2>((X, Y) => { return new GTA.Vector2(X, Y); }));
            Engine.SetValue("Vector3", new Func<float, float, float, GTA.Vector3>((X, Y, Z) => { return new GTA.Vector3(X, Y, Z); }));
            Engine.SetValue("Vector4", new Func<float, float, float, float, GTA.Vector4>((X, Y, Z, W) => { return new GTA.Vector4(X, Y, Z, W); }));
            Engine.SetValue("Quaternion", new Func<float, float, float, float, GTA.Quaternion>((X, Y, Z, W) => { return new GTA.Quaternion(X, Y, Z, W); }));
#endif
            Engine.SetValue("Console", ConsoleNatives);
            Engine.SetValue("Event", new Func<string, Scripting.Natives.EventNatives>((Name) => { return new Scripting.Natives.EventNatives(Name); }));
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
