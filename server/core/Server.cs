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
            Vehicle Vehicle1 = new Vehicle("Infernus", new SharpDX.Vector3(2785.87f, 426.42f, 5.82f));
            Vehicle Vehicle2 = new Vehicle("Infernus", new SharpDX.Vector3(2787.87f, 426.42f, 5.82f));
            Vehicle Vehicle3 = new Vehicle("Infernus", new SharpDX.Vector3(2789.87f, 426.42f, 5.82f));
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
                                    Player.Spawn(new SharpDX.Vector3(2783.87f, 426.42f, 5.82f), 45.0f);
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
                                PlayerData.CurrentVehicle = Msg.ReadInt32();
                                PlayerData.Pos_X = Msg.ReadFloat();
                                PlayerData.Pos_Y = Msg.ReadFloat();
                                PlayerData.Pos_Z = Msg.ReadFloat();
                                PlayerData.Rot_X = Msg.ReadFloat();
                                PlayerData.Rot_Y = Msg.ReadFloat();
                                PlayerData.Rot_Z = Msg.ReadFloat();
                                PlayerData.Rot_A = Msg.ReadFloat();
                                PlayerData.Heading = Msg.ReadFloat();
                                PlayerData.IsWalking = Msg.ReadBoolean();
                                PlayerData.IsRunning = Msg.ReadBoolean();
                                PlayerData.IsJumping = Msg.ReadBoolean();
                                Player Player = PlayerController.FindByID(Msg.SenderConnection.RemoteUniqueIdentifier);
                                Player.Name = PlayerData.Name;
                                Player.Health = PlayerData.Health;
                                Player.Armor = PlayerData.Armor;
                                SharpDX.Vector3 Position = new SharpDX.Vector3();
                                Position.X = PlayerData.Pos_X;
                                Position.Y = PlayerData.Pos_Y;
                                Position.Z = PlayerData.Pos_Z;
                                SharpDX.Quaternion Rotation = new SharpDX.Quaternion();
                                Rotation.X = PlayerData.Rot_X;
                                Rotation.Y = PlayerData.Rot_Y;
                                Rotation.Z = PlayerData.Rot_Z;
                                Rotation.W = PlayerData.Rot_A;
                                if (PlayerData.CurrentVehicle > 0)
                                {
                                    Vehicle Vehicle = VehicleController.GetByID(PlayerData.CurrentVehicle);
                                    Vehicle.Position = Position;
                                    Vehicle.Rotation = Rotation;
                                    Vehicle.Driver = Player;
                                    Player.CurrentVehicle = PlayerData.CurrentVehicle;
                                }
                                else
                                {
                                    Player.Position = Position;
                                    Player.Heading = PlayerData.Heading;
                                    Player.IsWalking = PlayerData.IsWalking;
                                    Player.IsRunning = PlayerData.IsRunning;
                                    Player.IsJumping = PlayerData.IsJumping;
                                }
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
                PlayerData.CurrentVehicle = Player.CurrentVehicle;
                PlayerData.Pos_X = Player.Position.X;
                PlayerData.Pos_Y = Player.Position.Y;
                PlayerData.Pos_Z = Player.Position.Z;
                PlayerData.Heading = Player.Heading;
                PlayerData.IsWalking = Player.IsWalking;
                PlayerData.IsRunning = Player.IsRunning;
                PlayerData.IsJumping = Player.IsJumping;

                Msg.Write(PlayerData.ID);
                Msg.Write(PlayerData.Name);
                Msg.Write(PlayerData.Model);
                Msg.Write(PlayerData.Health);
                Msg.Write(PlayerData.Armor);
                Msg.Write(PlayerData.CurrentVehicle);
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
                VehicleData.Pos_X = Vehicle.Position.X;
                VehicleData.Pos_Y = Vehicle.Position.Y;
                VehicleData.Pos_Z = Vehicle.Position.Z;
                VehicleData.Rot_X = Vehicle.Rotation.X;
                VehicleData.Rot_Y = Vehicle.Rotation.Y;
                VehicleData.Rot_Z = Vehicle.Rotation.Z;
                VehicleData.Rot_A = Vehicle.Rotation.W;

                Msg.Write((int)Shared.NetworkMessageType.UpdateVehicle);
                Msg.Write(VehicleData.ID);
                Msg.Write(VehicleData.Model);
                Msg.Write(VehicleData.Pos_X);
                Msg.Write(VehicleData.Pos_Y);
                Msg.Write(VehicleData.Pos_Z);
                Msg.Write(VehicleData.Rot_X);
                Msg.Write(VehicleData.Rot_Y);
                Msg.Write(VehicleData.Rot_Z);
                Msg.Write(VehicleData.Rot_A);

                if (Vehicle.Driver == null)
                {
                    NetServer.SendToAll(Msg, NetDeliveryMethod.Unreliable);
                }
                else
                {
                    NetServer.SendToAll(Msg, Vehicle.Driver.NetConnection, NetDeliveryMethod.Unreliable, 2);
                }
            }
        }

        public void Shutdown()
        {
            NetServer.Shutdown("Shutdown");
        }
    }
}
