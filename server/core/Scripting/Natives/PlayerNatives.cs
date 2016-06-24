using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ivmp_server_core.Scripting.Natives
{
    public class Player : EntityNatives
    {
        public Player(ivmp_server_core.Player ServerPlayer)
        {
            Entity = ServerPlayer;
        }

        public void Spawn(SharpDX.Vector3 Position, float Heading)
        {
            System.Console.WriteLine("SpawnScripting");
            Entity.Spawn(Position, Heading);
        }

        public void FadeScreenIn(int Duration)
        {
            System.Console.WriteLine("FadeScreenInScripting");
            Entity.FadeScreenIn(Duration);
        }

        public void FadeScreenOut(int Duration)
        {
            Entity.FadeScreenOut(Duration);
        }
    }
}
