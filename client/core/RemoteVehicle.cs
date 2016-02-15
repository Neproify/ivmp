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
using GTA;

namespace ivmp_client_core
{
    public class RemoteVehicle
    {
        public Vehicle Vehicle;

        public int ID;
        public string Model;

        public Vector3 Position;
        public Quaternion Rotation;

        public RemoteVehicle(string Model)
        {
            this.Model = Model;
            Vehicle = World.CreateVehicle(Model, Vector3.Zero);
        }
    }
}
