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
        public List<RemotePlayer> Players;

        public RemotePlayerController()
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

        public RemotePlayer FindByID(long ID)
        {
            RemotePlayer Player;
            if(!Players.Any(x => x.ID == ID))
            {
                Player = new RemotePlayer();
                Player.ID = ID;
                Add(Player);
                return Player;
            }
            Player = Players.Find(x => x.ID == ID);
            return Player;
        }

        public void UpdateAll()
        {
            Players.ForEach(x => x.Update());
        }
    }
}
