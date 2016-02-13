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
    public class Player
    {
        public Server server;

        public long ID;
        public NetConnection netConnection;

        public string Name;

        public bool isSpawned;

        public string Model;
        public int Health;
        public int Armor;

        public float Pos_X;
        public float Pos_Y;
        public float Pos_Z;

        public float Heading;

        public bool isWalking;
        public bool isRunning;
        public bool isJumping;
        
        public void Spawn(float Pos_X, float Pos_Y, float Pos_Z, float Heading)
        {
            NetOutgoingMessage msg = server.server.CreateMessage();
            msg.Write((int)Shared.NetworkMessageTypes.MessageType.SpawnPlayer);
            msg.Write(Pos_X);
            msg.Write(Pos_Y);
            msg.Write(Pos_Z);
            msg.Write(Heading);
            server.server.SendMessage(msg, netConnection, NetDeliveryMethod.ReliableOrdered);
            isSpawned = true;
        }

        public void FadeScreenIn(int duration)
        {
            NetOutgoingMessage msg = server.server.CreateMessage();
            msg.Write((int)Shared.NetworkMessageTypes.MessageType.FadeScreenIn);
            msg.Write(duration);
            server.server.SendMessage(msg, netConnection, NetDeliveryMethod.ReliableOrdered);
        }

        public void FadeScreenOut(int duration)
        {
            NetOutgoingMessage msg = server.server.CreateMessage();
            msg.Write((int)Shared.NetworkMessageTypes.MessageType.FadeScreenOut);
            msg.Write(duration);
            server.server.SendMessage(msg, netConnection, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
