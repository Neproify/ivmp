﻿/*
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE', which is part of this source code package.
 * Copyright (c) 2016 Neproify
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using Lidgren.Network;
using Shared;

namespace ivmp_server_core
{
    public class Server
    {
        public Server Instance;
        public int Port;
        public int TickRate;
        public int MaxPlayers;

        public NetServer NetServer;
        public PlayerController PlayerController;
        public VehicleController VehicleController;

        public Server()
        {
            Instance = this;
            Port = 7777;
            TickRate = Shared.Settings.TickRate;
            MaxPlayers = 32;
            NetPeerConfiguration Config = new NetPeerConfiguration("ivmp");
            Config.MaximumConnections = MaxPlayers;
            Config.Port = Port;
            Config.ConnectionTimeout = 50;
            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            NetServer = new NetServer(Config);
            NetServer.Start();
            PlayerController = new PlayerController();
            VehicleController = new VehicleController();
            Timer tick = new Timer();
            tick.Elapsed += OnTick;
            tick.Interval = TickRate;
            tick.Enabled = true;
            tick.Start();
            Console.WriteLine("Started game server on Port " + Port);
            Console.WriteLine("Max Players: " + MaxPlayers);

            // test vehicles
            //2783.87f, 426.42f, 5.82f
            Vehicle Vehicle1 = new Vehicle("Infernus", 2785.87f, 426.42f, 5.82f);
            Vehicle Vehicle2 = new Vehicle("Infernus", 2787.87f, 426.42f, 5.82f);
            Vehicle Vehicle3 = new Vehicle("Infernus", 2789.87f, 426.42f, 5.82f);
            VehicleController.Add(Vehicle1);
            VehicleController.Add(Vehicle2);
            VehicleController.Add(Vehicle3);
        }

        public void OnTick(object sender, ElapsedEventArgs e)
        {
            NetIncomingMessage Msg;
            while ((Msg = NetServer.ReadMessage()) != null)
            {
                switch (Msg.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        int Version = Msg.ReadInt32();
                        if(Version == Shared.Settings.NetworkVersion)
                        {
                            Msg.SenderConnection.Approve();
                        }
                        else
                        {
                            Msg.SenderConnection.Deny();
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)Msg.ReadByte();
                        switch (status)
                        {
                            case NetConnectionStatus.Connected:
                                {
                                    Console.WriteLine("Client connected. ID: " + Msg.SenderConnection.RemoteUniqueIdentifier);
                                    Player Player = new Player();
                                    Player.Server = this;
                                    Player.ID = Msg.SenderConnection.RemoteUniqueIdentifier;
                                    Player.NetConnection = Msg.SenderConnection;
                                    PlayerController.Add(Player);
                                    NetOutgoingMessage OutMsg = NetServer.CreateMessage();
                                    OutMsg.Write((int)Shared.NetworkMessageType.PlayerConnected);
                                    OutMsg.Write(Player.ID);
                                    NetServer.SendToAll(OutMsg, Msg.SenderConnection, NetDeliveryMethod.ReliableUnordered, 1);
                                    Player.Spawn(2783.87f, 426.42f, 5.82f, 45.0f);
                                    Player.FadeScreenIn(1000);
                                    break;
                                }
                            case NetConnectionStatus.Disconnected:
                                {
                                    Console.WriteLine("Client disconnected. ID: " + Msg.SenderConnection.RemoteUniqueIdentifier);
                                    Player Player = PlayerController.FindByID(Msg.SenderConnection.RemoteUniqueIdentifier);
                                    PlayerController.Remove(Player);
                                    Player = null;
                                    NetOutgoingMessage OutMsg = NetServer.CreateMessage();
                                    OutMsg.Write((int)Shared.NetworkMessageType.PlayerDisconnected);
                                    OutMsg.Write(Msg.SenderConnection.RemoteUniqueIdentifier);
                                    NetServer.SendToAll(OutMsg, Msg.SenderConnection, NetDeliveryMethod.ReliableSequenced, 1);
                                    break;
                                }
                        }
                        break;
                    case NetIncomingMessageType.WarningMessage:
                        break;
                    case NetIncomingMessageType.Data:
                        int MsgType = Msg.ReadInt32();
                        switch(MsgType)
                        {
                            case (int)Shared.NetworkMessageType.UpdatePlayer:
                                PlayerUpdateStruct PlayerData = new PlayerUpdateStruct();
                                PlayerData.Name = Msg.ReadString();
                                PlayerData.Health = Msg.ReadInt32();
                                PlayerData.Armor = Msg.ReadInt32();
                                PlayerData.Pos_X = Msg.ReadFloat();
                                PlayerData.Pos_Y = Msg.ReadFloat();
                                PlayerData.Pos_Z = Msg.ReadFloat();
                                PlayerData.Heading = Msg.ReadFloat();
                                PlayerData.IsWalking = Msg.ReadBoolean();
                                PlayerData.IsRunning = Msg.ReadBoolean();
                                PlayerData.IsJumping = Msg.ReadBoolean();
                                Player Player = PlayerController.FindByID(Msg.SenderConnection.RemoteUniqueIdentifier);
                                Player.Name = PlayerData.Name;
                                Player.Health = PlayerData.Health;
                                Player.Armor = PlayerData.Armor;
                                Player.Pos_X = PlayerData.Pos_X;
                                Player.Pos_Y = PlayerData.Pos_Y;
                                Player.Pos_Z = PlayerData.Pos_Z;
                                Player.Heading = PlayerData.Heading;
                                Player.IsWalking = PlayerData.IsWalking;
                                Player.IsRunning = PlayerData.IsRunning;
                                Player.IsJumping = PlayerData.IsJumping;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        Console.WriteLine("Unhandled message type: " + Msg.MessageType);
                        break;
                }
                NetServer.Recycle(Msg);
            }

            UpdateAllPlayers();
            UpdateAllVehicles();
        }

        public void UpdateAllPlayers()
        {
            List<Player> Players = PlayerController.GetAll();
            foreach (var Player in Players)
            {
                NetOutgoingMessage Msg = NetServer.CreateMessage();
                Msg.Write((int)Shared.NetworkMessageType.UpdatePlayer);
                PlayerUpdateStruct PlayerData = new PlayerUpdateStruct();
                PlayerData.ID = Player.ID;
                PlayerData.Name = Player.Name;
                PlayerData.Model = Player.Model;
                PlayerData.Health = Player.Health;
                PlayerData.Armor = Player.Armor;
                PlayerData.Pos_X = Player.Pos_X;
                PlayerData.Pos_Y = Player.Pos_Y;
                PlayerData.Pos_Z = Player.Pos_Z;
                PlayerData.Heading = Player.Heading;
                PlayerData.IsWalking = Player.IsWalking;
                PlayerData.IsRunning = Player.IsRunning;
                PlayerData.IsJumping = Player.IsJumping;

                Msg.Write(PlayerData.ID);
                Msg.Write(PlayerData.Name);
                Msg.Write(PlayerData.Model);
                Msg.Write(PlayerData.Health);
                Msg.Write(PlayerData.Armor);
                Msg.Write(PlayerData.Pos_X);
                Msg.Write(PlayerData.Pos_Y);
                Msg.Write(PlayerData.Pos_Z);
                Msg.Write(PlayerData.Heading);
                Msg.Write(PlayerData.IsWalking);
                Msg.Write(PlayerData.IsRunning);
                Msg.Write(PlayerData.IsJumping);

                NetServer.SendToAll(Msg, Player.NetConnection, NetDeliveryMethod.Unreliable, 1);
            }
        }

        public void UpdateAllVehicles()
        {
            List<Vehicle> Vehicles = VehicleController.GetAll();
            foreach(var Vehicle in Vehicles)
            {
                NetOutgoingMessage Msg = NetServer.CreateMessage();
                VehicleUpdateStruct VehicleData = new VehicleUpdateStruct();
                VehicleData.ID = Vehicle.ID;
                VehicleData.Model = Vehicle.Model;
                VehicleData.Pos_X = Vehicle.Pos_X;
                VehicleData.Pos_Y = Vehicle.Pos_Y;
                VehicleData.Pos_Z = Vehicle.Pos_Z;
                VehicleData.Rot_X = Vehicle.Rot_X;
                VehicleData.Rot_Y = Vehicle.Rot_Y;
                VehicleData.Rot_Z = Vehicle.Rot_Z;

                Msg.Write((int)Shared.NetworkMessageType.UpdateVehicle);
                Msg.Write(VehicleData.ID);
                Msg.Write(VehicleData.Model);
                Msg.Write(VehicleData.Pos_X);
                Msg.Write(VehicleData.Pos_Y);
                Msg.Write(VehicleData.Pos_Z);
                Msg.Write(VehicleData.Rot_X);
                Msg.Write(VehicleData.Rot_Y);
                Msg.Write(VehicleData.Rot_Z);

                NetServer.SendToAll(Msg, NetDeliveryMethod.Unreliable);
            }
        }

        public void Shutdown()
        {
            NetServer.Shutdown("Shutdown");
        }
    }
}
