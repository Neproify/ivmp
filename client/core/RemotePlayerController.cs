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

namespace ivmp_client_core
{
    public class RemotePlayerController
    {
        public List<RemotePlayer> players;

        public RemotePlayerController()
        {
            players = new List<RemotePlayer>();
        }

        public void add(RemotePlayer player)
        {
            players.Add(player);
        }

        public void remove(RemotePlayer player)
        {
            players.Remove(player);
        }

        public RemotePlayer findByID(long ID)
        {
            RemotePlayer player;
            if(!players.Any(x => x.ID == ID))
            {
                player = new RemotePlayer();
                player.ID = ID;
                add(player);
                return player;
            }
            player = players.Find(x => x.ID == ID);
            return player;
        }

        public void UpdateAll()
        {
            players.ForEach(x => x.Update());
        }
    }
}
