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
            Players.Add(Player);
        }

        public void Remove(Player Player)
        {
            Players.Remove(Player);
        }

        public Player GetByID(long ID)
        {
            return Players.Find(Player => Player.ID == ID);
        }

        public List<Player> GetAll()
        {
            return Players;
        }
    }
}
