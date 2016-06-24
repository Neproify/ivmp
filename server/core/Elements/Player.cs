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
    public class Player : Element
    {
        public NetConnection NetConnection;

        public int CurrentVehicle;

        public string Name;

        public bool IsSpawned;

        public int Health;
        public int Armor;

        public bool IsWalking;
        public bool IsRunning;
        public bool IsJumping;
        public bool IsCrouching;
        public bool IsGettingIntoVehicle;
        public bool IsGettingOutOfVehicle;

        public void Spawn(SharpDX.Vector3 Position, float Heading)
        {
            NetOutgoingMessage Msg = Server.NetServer.CreateMessage();
            Msg.Write((int)Shared.NetworkMessageType.SpawnPlayer);
            Msg.Write(Position.X);
            Msg.Write(Position.Y);
            Msg.Write(Position.Z);
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
