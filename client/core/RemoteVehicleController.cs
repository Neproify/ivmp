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
        public List<RemoteVehicle> vehicles;

        public RemoteVehicleController()
        {
            vehicles = new List<RemoteVehicle>();
        }

        public void add(RemoteVehicle vehicle)
        {
            vehicles.Add(vehicle);
        }

        public void remove(RemoteVehicle vehicle)
        {
            vehicles.Remove(vehicle);
        }

        public RemoteVehicle findByID(int ID)
        {
            RemoteVehicle vehicle;
            if (!vehicles.Any(x => x.ID == ID))
            {
                vehicle = new RemoteVehicle("Feltzer");
                vehicle.ID = ID;
                add(vehicle);
                return vehicle;
            }
            vehicle = vehicles.Find(x => x.ID == ID);
            return vehicle;
        }

        public RemoteVehicle findByGame(Vehicle vehicle)
        {
            return vehicles.Find(x => x.vehicle == vehicle);
        }
    }
}
