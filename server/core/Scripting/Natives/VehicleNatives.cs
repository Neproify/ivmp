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

        public Vehicle(string Model, SharpDX.Vector3 Position, VehiclesController VehiclesController)
        {
            ServerVehicle = new ivmp_server_core.Vehicle(Model, Position);
            VehiclesController.Add(ServerVehicle);
        }

        public void SetPosition(SharpDX.Vector3 Position)
        {
            ServerVehicle.Position = Position;
        }
    }
}
