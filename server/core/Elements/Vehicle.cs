﻿/*
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE', which is part of this source code package.
 * Copyright (c) 2016 Neproify
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace ivmp_server_core
{
    public enum VehicleSeat
    {
        None = -3,
        Driver = -1,
        RightFront = 0,
        LeftRear = 1,
        RightRear = 2
    }

    public class Vehicle : Element
    {
        public Player Driver = null;

        public float Speed;

        public Vehicle(string Model, Vector3 Position)
        {
            this.Model = Model;
            this.Position = Position;

            this.Rotation = SharpDX.Quaternion.Zero;
            this.Velocity = SharpDX.Vector3.Zero;
            this.Heading = 0f;
            this.Speed = 0f;
            Type = "Vehicle";
            Server.EventsManager.GetEvent("OnElementCreated").Trigger(Jint.Native.JsValue.FromObject(Server.Engine, new Scripting.Natives.Vehicle(this)));
        }

        public new void Destroy()
        {
            Server.EventsManager.GetEvent("OnElementDestroyed").Trigger(Jint.Native.JsValue.FromObject(Server.Engine, new Scripting.Natives.Vehicle(this)));
        }
    }
}
