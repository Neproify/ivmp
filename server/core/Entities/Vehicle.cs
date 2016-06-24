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
    public class Vehicle
    {
        public int ID;
        public string Model;

        public Player Driver = null;

        public SharpDX.Vector3 Position;

        public SharpDX.Vector3 Velocity;

        public SharpDX.Quaternion Rotation;

        public Vehicle(string Model, SharpDX.Vector3 Position)
        {
            this.Model = Model;
            this.Position = Position;

            this.Rotation = SharpDX.Quaternion.Zero;
            this.Velocity = SharpDX.Vector3.Zero;
        }
    }
}
