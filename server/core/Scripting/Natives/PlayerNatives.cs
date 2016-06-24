using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ivmp_server_core.Scripting.Natives
{
    public class Player
    {
        public ivmp_server_core.Player ServerPlayer;

        public Player(ivmp_server_core.Player ServerPlayer)
        {
            this.ServerPlayer = ServerPlayer;
        }

        public void Spawn(SharpDX.Vector3 Position, float Heading)
        {
            System.Console.WriteLine("SpawnScripting");
            ServerPlayer.Spawn(Position, Heading);
        }

        public void FadeScreenIn(int Duration)
        {
            System.Console.WriteLine("FadeScreenInScripting");
            ServerPlayer.FadeScreenIn(Duration);
        }

        public void FadeScreenOut(int Duration)
        {
            ServerPlayer.FadeScreenOut(Duration);
        }
    }
}
