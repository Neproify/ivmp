using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ivmp_server_core
{
    public class Entity
    {
        public Server Server;

        public int ID;

        public string Model;

        public SharpDX.Vector3 Position;

        public SharpDX.Vector3 Velocity;

        public SharpDX.Quaternion Rotation;
    }
}
