using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ivmp_server_core.Scripting.Natives
{
    public class Player : ElementNatives
    {
        public string Name
        {
            get
            {
                return Element.Name;
            }
        }

        public bool IsSpawned
        {
            get
            {
                return Element.IsSpawned;
            }
        }

        public Scripting.Natives.Vehicle Vehicle
        {
            get
            {
                return new Scripting.Natives.Vehicle(Element.CurrentVehicle);
            }
        }

        public Player(ivmp_server_core.Player ServerPlayer)
        {
            Element = ServerPlayer;
        }

        public void Spawn(SharpDX.Vector3 Position, float Heading)
        {
            Element.Spawn(Position, Heading);
        }

        public void FadeScreenIn(int Duration)
        {
            Element.FadeScreenIn(Duration);
        }

        public void FadeScreenOut(int Duration)
        {
            Element.FadeScreenOut(Duration);
        }
    }
}
