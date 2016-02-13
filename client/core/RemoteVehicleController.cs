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
    public class RemoteVehicleController
    {
        public List<RemoteVehicle> Vehicles;

        public RemoteVehicleController()
        {
            Vehicles = new List<RemoteVehicle>();
        }

        public void Add(RemoteVehicle Vehicle)
        {
            Vehicles.Add(Vehicle);
        }

        public void Remove(RemoteVehicle Vehicle)
        {
            Vehicles.Remove(Vehicle);
        }

        public RemoteVehicle FindByID(int ID)
        {
            RemoteVehicle Vehicle;
            if (!Vehicles.Any(x => x.ID == ID))
            {
                Vehicle = new RemoteVehicle("Feltzer");
                Vehicle.ID = ID;
                Add(Vehicle);
                return Vehicle;
            }
            Vehicle = Vehicles.Find(x => x.ID == ID);
            return Vehicle;
        }

        public RemoteVehicle FindByGame(Vehicle Vehicle)
        {
            return Vehicles.Find(x => x.Vehicle == Vehicle);
        }
    }
}
