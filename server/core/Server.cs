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
using System.Net;
using System.Net.Sockets;
using System.Timers;
using Lidgren.Network;
using Shared;

namespace ivmp_server_core
{
    public class Server
    {

        public Server instance;
        public int port;
        public int tickRate;
        public int maxPlayers;

        public NetServer server;
        public PlayerController playerController;
        public VehicleController vehicleController;

        public Server()
        {
            instance = this;
            port = 7777;
            tickRate = Shared.Settings.TickRate;
            maxPlayers = 32;
            NetPeerConfiguration config = new NetPeerConfiguration("ivmp");
            config.MaximumConnections = maxPlayers;
            config.Port = port;
            config.ConnectionTimeout = 50;
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            server = new NetServer(config);
            server.Start();
            playerController = new PlayerController();
            vehicleController = new VehicleController();
            Timer tick = new Timer();
            tick.Elapsed += OnTick;
            tick.Interval = tickRate;
            tick.Enabled = true;
            tick.Start();
            Console.WriteLine("Started game server on port " + port);
            Console.WriteLine("Max players: " + maxPlayers);

            // test vehicles
            //2783.87f, 426.42f, 5.82f
            Vehicle vehicle1 = new Vehicle("Infernus", 2785.87f, 426.42f, 5.82f);
            Vehicle vehicle2 = new Vehicle("Infernus", 2787.87f, 426.42f, 5.82f);
            Vehicle vehicle3 = new Vehicle("Infernus", 2789.87f, 426.42f, 5.82f);
            vehicleController.add(vehicle1);
            vehicleController.add(vehicle2);
            vehicleController.add(vehicle3);
        }

        public void OnTick(object sender, ElapsedEventArgs e)
        {
            NetIncomingMessage msg;
            while ((msg = server.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        int version = msg.ReadInt32();
                        if(version == Shared.Settings.NetworkVersion)
                        {
                            msg.SenderConnection.Approve();
                        }
                        else
                        {
                            msg.SenderConnection.Deny();
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        switch (status)
                        {
                            case NetConnectionStatus.Connected:
                                {
                                    Console.WriteLine("Client connected. ID: " + msg.SenderConnection.RemoteUniqueIdentifier);
                                    Player player = new Player();
                                    player.server = this;
                                    player.ID = msg.SenderConnection.RemoteUniqueIdentifier;
                                    player.netConnection = msg.SenderConnection;
                                    playerController.add(player);
                                    NetOutgoingMessage OutMsg = server.CreateMessage();
                                    OutMsg.Write((int)Shared.NetworkMessageTypes.MessageType.PlayerConnected);
                                    OutMsg.Write(player.ID);
                                    server.SendToAll(OutMsg, msg.SenderConnection, NetDeliveryMethod.ReliableUnordered, 1);
                                    player.Spawn(2783.87f, 426.42f, 5.82f, 45.0f);
                                    player.FadeScreenIn(1000);
                                    break;
                                }
                            case NetConnectionStatus.Disconnected:
                                {
                                    Console.WriteLine("Client disconnected. ID: " + msg.SenderConnection.RemoteUniqueIdentifier);
                                    Player player = playerController.findByID(msg.SenderConnection.RemoteUniqueIdentifier);
                                    playerController.remove(player);
                                    player = null;
                                    NetOutgoingMessage OutMsg = server.CreateMessage();
                                    OutMsg.Write((int)Shared.NetworkMessageTypes.MessageType.PlayerDisconnected);
                                    OutMsg.Write(msg.SenderConnection.RemoteUniqueIdentifier);
                                    server.SendToAll(OutMsg, msg.SenderConnection, NetDeliveryMethod.ReliableSequenced, 1);
                                    break;
                                }
                        }
                        break;
                    case NetIncomingMessageType.WarningMessage:
                        break;
                    case NetIncomingMessageType.Data:
                        int MsgType = msg.ReadInt32();
                        switch(MsgType)
                        {
                            case (int)Shared.NetworkMessageTypes.MessageType.UpdatePlayer:
                                PlayerUpdateStruct PlayerData = new PlayerUpdateStruct();
                                PlayerData.Name = msg.ReadString();
                                PlayerData.Health = msg.ReadInt32();
                                PlayerData.Armor = msg.ReadInt32();
                                PlayerData.Pos_X = msg.ReadFloat();
                                PlayerData.Pos_Y = msg.ReadFloat();
                                PlayerData.Pos_Z = msg.ReadFloat();
                                PlayerData.Heading = msg.ReadFloat();
                                PlayerData.isWalking = msg.ReadBoolean();
                                PlayerData.isRunning = msg.ReadBoolean();
                                PlayerData.isJumping = msg.ReadBoolean();
                                Player player = playerController.findByID(msg.SenderConnection.RemoteUniqueIdentifier);
                                player.Name = PlayerData.Name;
                                player.Health = PlayerData.Health;
                                player.Armor = PlayerData.Armor;
                                player.Pos_X = PlayerData.Pos_X;
                                player.Pos_Y = PlayerData.Pos_Y;
                                player.Pos_Z = PlayerData.Pos_Z;
                                player.Heading = PlayerData.Heading;
                                player.isWalking = PlayerData.isWalking;
                                player.isRunning = PlayerData.isRunning;
                                player.isJumping = PlayerData.isJumping;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        Console.WriteLine("Unhandled message type: " + msg.MessageType);
                        break;
                }
                server.Recycle(msg);
            }

            UpdateAllPlayers();
            UpdateAllVehicles();
        }

        public void UpdateAllPlayers()
        {
            List<Player> players = playerController.getAll();
            foreach (var player in players)
            {
                NetOutgoingMessage msg = server.CreateMessage();
                msg.Write((int)Shared.NetworkMessageTypes.MessageType.UpdatePlayer);
                PlayerUpdateStruct PlayerData = new PlayerUpdateStruct();
                PlayerData.ID = player.ID;
                PlayerData.Name = player.Name;
                PlayerData.Model = player.Model;
                PlayerData.Health = player.Health;
                PlayerData.Armor = player.Armor;
                PlayerData.Pos_X = player.Pos_X;
                PlayerData.Pos_Y = player.Pos_Y;
                PlayerData.Pos_Z = player.Pos_Z;
                PlayerData.Heading = player.Heading;
                PlayerData.isWalking = player.isWalking;
                PlayerData.isRunning = player.isRunning;
                PlayerData.isJumping = player.isJumping;

                msg.Write(PlayerData.ID);
                msg.Write(PlayerData.Name);
                msg.Write(PlayerData.Model);
                msg.Write(PlayerData.Health);
                msg.Write(PlayerData.Armor);
                msg.Write(PlayerData.Pos_X);
                msg.Write(PlayerData.Pos_Y);
                msg.Write(PlayerData.Pos_Z);
                msg.Write(PlayerData.Heading);
                msg.Write(PlayerData.isWalking);
                msg.Write(PlayerData.isRunning);
                msg.Write(PlayerData.isJumping);

                server.SendToAll(msg, player.netConnection, NetDeliveryMethod.Unreliable, 1);
            }
        }

        public void UpdateAllVehicles()
        {
            List<Vehicle> vehicles = vehicleController.getAll();
            foreach(var vehicle in vehicles)
            {
                NetOutgoingMessage msg = server.CreateMessage();
                VehicleUpdateStruct VehicleData = new VehicleUpdateStruct();
                VehicleData.ID = vehicle.ID;
                VehicleData.Model = vehicle.Model;
                VehicleData.Pos_X = vehicle.Pos_X;
                VehicleData.Pos_Y = vehicle.Pos_Y;
                VehicleData.Pos_Z = vehicle.Pos_Z;
                VehicleData.Rot_X = vehicle.Rot_X;
                VehicleData.Rot_Y = vehicle.Rot_Y;
                VehicleData.Rot_Z = vehicle.Rot_Z;

                msg.Write((int)Shared.NetworkMessageTypes.MessageType.UpdateVehicle);
                msg.Write(VehicleData.ID);
                msg.Write(VehicleData.Model);
                msg.Write(VehicleData.Pos_X);
                msg.Write(VehicleData.Pos_Y);
                msg.Write(VehicleData.Pos_Z);
                msg.Write(VehicleData.Rot_X);
                msg.Write(VehicleData.Rot_Y);
                msg.Write(VehicleData.Rot_Z);

                server.SendToAll(msg, NetDeliveryMethod.Unreliable);
            }
        }

        public void Shutdown()
        {
            server.Shutdown("Shutdown");
        }
    }
}
