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

namespace ivmp_server_core
{
    public class Vehicle : Element
    {
        public Player Driver = null;

        public float Speed;

        public Vehicle(string Model, SharpDX.Vector3 Position, ivmp_server_core.Server ServerInstance)
        {
            this.Model = Model;
            this.Position = Position;

            this.Rotation = SharpDX.Quaternion.Zero;
            this.Velocity = SharpDX.Vector3.Zero;
            this.Heading = 0f;
            this.Speed = 0f;
            Type = "Vehicle";
            Server = ServerInstance;
            Server.EventsManager.GetEvent("OnElementCreated").Trigger(Jint.Native.JsValue.FromObject(Server.Engine, new Scripting.Natives.Vehicle(this)));
        }

        public new void Destroy()
        {
            Server.EventsManager.GetEvent("OnElementDestroyed").Trigger(Jint.Native.JsValue.FromObject(Server.Engine, new Scripting.Natives.Vehicle(this)));
        }
    }
}
