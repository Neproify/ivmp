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

namespace ivmp_server_core.Scripting.Natives
{
    public class Vehicle : EntityNatives
    {
        public VehiclesController VehiclesController;

        public ivmp_server_core.Player Driver
        {
            get
            {
                return Entity.Driver;
            }
        }

        public Vehicle(string Model, SharpDX.Vector3 Position, VehiclesController VehiclesController)
        {
            Entity = new ivmp_server_core.Vehicle(Model, Position);
            VehiclesController.Add(Entity);
        }

        public Vehicle(int ID)
        {
            Entity = VehiclesController.GetByID(ID);
        }
    }
}
