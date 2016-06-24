using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ivmp_server_core.Scripting.Natives
{
    public class Player : EntityNatives
    {
        public string Name
        {
            get
            {
                return Entity.Name;
            }
        }

        public bool IsSpawned
        {
            get
            {
                return Entity.IsSpawned;
            }
        }

        public Scripting.Natives.Vehicle Vehicle
        {
            get
            {
                return new Scripting.Natives.Vehicle(Entity.CurrentVehicle);
            }
        }

        public Player(ivmp_server_core.Player ServerPlayer)
        {
            Entity = ServerPlayer;
        }

        public void Spawn(SharpDX.Vector3 Position, float Heading)
        {
            Entity.Spawn(Position, Heading);
        }

        public void FadeScreenIn(int Duration)
        {
            Entity.FadeScreenIn(Duration);
        }

        public void FadeScreenOut(int Duration)
        {
            Entity.FadeScreenOut(Duration);
        }
    }
}
