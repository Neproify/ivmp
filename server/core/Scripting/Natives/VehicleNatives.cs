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
    public class Vehicle
    {
        public ivmp_server_core.Vehicle ServerVehicle;
        public VehiclesController VehiclesController;

        public SharpDX.Vector3 Position
        {
            get
            {
                return ServerVehicle.Position;
            }
            set
            {
                ServerVehicle.Position = value;
            }
        }

        public SharpDX.Quaternion Rotation
        {
            get
            {
                return ServerVehicle.Rotation;
            }
            set
            {
                ServerVehicle.Rotation = value;
            }
        }

        public Vehicle(string Model, SharpDX.Vector3 Position, VehiclesController VehiclesController)
        {
            ServerVehicle = new ivmp_server_core.Vehicle(Model, Position);
            VehiclesController.Add(ServerVehicle);
        }

        public Vehicle(int ID)
        {
            ServerVehicle = VehiclesController.GetByID(ID);
        }
    }
}
