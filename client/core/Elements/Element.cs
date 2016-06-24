using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;

namespace ivmp_client_core
{
    public class Element
    {
        public dynamic GameReference;

        public int ID;

        public string Model;

        public DateTime Interpolation_Start;
        public DateTime Interpolation_End;
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public Vector3 StartVelocity;
        public Vector3 EndVelocity;
        public Quaternion StartRotation;
        public Quaternion EndRotation;
        public float StartHeading;
        public float EndHeading;

        public void Destroy()
        {
            if (GameReference.Exists())
            {
                GameReference.Delete();
            }
        }

        public void SetPosition(Vector3 Position, bool Instant)
        {
            StartPosition = GameReference.Position;
            if (Instant == true)
            {
                StartPosition = Position;
            }
            EndPosition = Position;
        }

        public void SetPosition(Vector3 Position)
        {
            SetPosition(Position, false);
        }

        public void SetVelocity(Vector3 Velocity)
        {
            StartVelocity = GameReference.Velocity;
            EndVelocity = Velocity;
        }

        public void SetRotation(Quaternion Rotation)
        {
            StartRotation = GameReference.RotationQuaternion;
            EndRotation = Rotation;
        }

        public void SetHeading(float Heading)
        {
            StartHeading = GameReference.Heading;
            EndHeading = Heading;
        }
    }
}
