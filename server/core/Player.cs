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
        public Server Server;

        public long ID;
        public NetConnection NetConnection;

        public string Name;

        public bool IsSpawned;

        public string Model;
        public int Health;
        public int Armor;

        public float Pos_X;
        public float Pos_Y;
        public float Pos_Z;

        public float Heading;

        public bool IsWalking;
        public bool IsRunning;
        public bool IsJumping;
        
        public void Spawn(float Pos_X, float Pos_Y, float Pos_Z, float Heading)
        {
            NetOutgoingMessage Msg = Server.NetServer.CreateMessage();
            Msg.Write((int)Shared.NetworkMessageType.SpawnPlayer);
            Msg.Write(Pos_X);
            Msg.Write(Pos_Y);
            Msg.Write(Pos_Z);
            Msg.Write(Heading);
            Server.NetServer.SendMessage(Msg, NetConnection, NetDeliveryMethod.ReliableOrdered);
            IsSpawned = true;
        }

        public void FadeScreenIn(int Duration)
        {
            NetOutgoingMessage Msg = Server.NetServer.CreateMessage();
            Msg.Write((int)Shared.NetworkMessageType.FadeScreenIn);
            Msg.Write(Duration);
            Server.NetServer.SendMessage(Msg, NetConnection, NetDeliveryMethod.ReliableOrdered);
        }

        public void FadeScreenOut(int Duration)
        {
            NetOutgoingMessage Msg = Server.NetServer.CreateMessage();
            Msg.Write((int)Shared.NetworkMessageType.FadeScreenOut);
            Msg.Write(Duration);
            Server.NetServer.SendMessage(Msg, NetConnection, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
