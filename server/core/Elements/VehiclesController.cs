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
    public class VehiclesController
    {
        List<Vehicle> Vehicles;

        public Server Server;

        public VehiclesController(Server Server)
        {
            Vehicles = new List<Vehicle>();
            this.Server = Server;
        }

        public void Add(Vehicle Vehicle)
        {
            Vehicles.OrderBy(x => x.ID);
            int FreeID = 1;
            foreach (var Veh in Vehicles)
            {
                if (Veh.ID > FreeID)
                {
                    break;
                }
                FreeID = Veh.ID + 1;
            }
            Vehicle.ID = FreeID;
            Vehicle.Server = Server;
            Vehicles.Add(Vehicle);
        }

        public Vehicle GetByID(int ID)
        {
            return Vehicles.Find(x => x.ID == ID);
        }

        public void Remove(Vehicle Vehicle)
        {
            Vehicles.Remove(Vehicle);
        }

        public List<Vehicle> GetAll()
        {
            return Vehicles;
        }
    }
}