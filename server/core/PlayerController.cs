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
    public class PlayerController
    {
        public List<Player> players;

        public PlayerController()
        {
            players = new List<Player>();
        }

        public void add(Player player)
        {
            players.Add(player);
        }

        public void remove(Player player)
        {
            players.Remove(player);
        }

        public Player findByID(long ID)
        {
            return players.Find(player => player.ID == ID);
        }

        public List<Player> getAll()
        {
            return players;
        }
    }
}
