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
    public class RemotePlayersController
    {
        public List<RemotePlayer> Players;

        public RemotePlayersController()
        {
            Players = new List<RemotePlayer>();
        }

        public void Add(RemotePlayer Player)
        {
            Players.Add(Player);
        }

        public void Remove(RemotePlayer Player)
        {
            Players.Remove(Player);
        }

        public RemotePlayer GetByID(long ID)
        {
            RemotePlayer Player;
            Player = Players.Find(x => x.ID == ID);
            return Player;
        }

        public void UpdateAll()
        {
            Players.ForEach(x => x.Update());
        }
    }
}
