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
    public class VehicleController
    {
        List<Vehicle> vehicles;

        public VehicleController()
        {
            vehicles = new List<Vehicle>();
        }

        public void add(Vehicle vehicle)
        {
            vehicles.OrderBy(x => x.ID);
            int freeID = 1;
            foreach (var veh in vehicles)
            {
                if (veh.ID > freeID)
                {
                    break;
                }
                freeID = veh.ID + 1;
            }
            vehicle.ID = freeID;
            vehicles.Add(vehicle);
        }

        public Vehicle getByID(int ID)
        {
            return vehicles.Find(x => x.ID == ID);
        }

        public void remove(Vehicle vehicle)
        {
            vehicles.Remove(vehicle);
        }

        public List<Vehicle> getAll()
        {
            return vehicles;
        }
    }
}