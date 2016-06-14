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
using Lidgren.Network;

namespace ivmp_server_core
{
    public class PlayersController
    {
        public List<Player> Players;

        public PlayersController()
        {
            Players = new List<Player>();
        }

        public void Add(Player Player)
        {
            Players.OrderBy(x => x.ID);
            int FreeID = 1;
            foreach (var Plr in Players)
            {
                if (Plr.ID > FreeID)
                {
                    break;
                }
                FreeID = Plr.ID + 1;
            }
            Player.ID = FreeID;
            Players.Add(Player);
        }

        public void Remove(Player Player)
        {
            Players.Remove(Player);
        }

        public Player GetByID(int ID)
        {
            return Players.Find(Player => Player.ID == ID);
        }

        public Player GetByNetConnection(NetConnection NetConnection)
        {
            return Players.Find(Player => Player.NetConnection == NetConnection);
        }

        public List<Player> GetAll()
        {
            return Players;
        }
    }
}
