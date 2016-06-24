using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ivmp_server_core.Scripting.Natives
{
    public class EntityNatives
    {
        public dynamic Entity;

        public SharpDX.Vector3 Position
        {
            get
            {
                return Entity.Position;
            }
            set
            {
                Entity.Position = value;
            }
        }

        public SharpDX.Vector3 Velocity
        {
            get
            {
                return Entity.Velocity;
            }
        }

        public SharpDX.Quaternion Rotation
        {
            get
            {
                return Entity.Rotation;
            }
            set
            {
                Entity.Rotation = value;
            }
        }
    }
}
