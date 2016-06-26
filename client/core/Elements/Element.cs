using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;

namespace ivmp_client_core
{
    public abstract class Element
    {
        public int ID;

        public string Model;

        public DateTime Interpolation_Start;
        public DateTime Interpolation_End;
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public Vector3 StartVelocity;
        public Vector3 EndVelocity;
        public float StartHeading;
        public float EndHeading;

        public abstract void Destroy();
        public abstract void SetPosition(Vector3 Position, bool Instant);
        public abstract void SetPosition(Vector3 Position);
        public abstract void SetVelocity(Vector3 Velocity);
        public abstract void SetHeading(float Heading);
    }
}
