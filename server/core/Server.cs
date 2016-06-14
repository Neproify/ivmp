/*
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE', which is part of this source code package.
 * Copyright (c) 2016 Neproify
*/

using System;
using System.Collections.Generic;
using System.Xml;
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
        public PlayersController PlayersController;
        public VehiclesController VehiclesController;
        public Scripting.ResourcesManager ResourcesManager;

        public Server()
        {
            Instance = this;
            TickRate = Shared.Settings.TickRate;
            if(!System.IO.File.Exists("serverconfig.xml"))
            {
                Console.WriteLine("Config file not found...");
                System.Threading.Thread.Sleep(5000);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            XmlDocument Config = new XmlDocument();
            Config.Load("serverconfig.xml");
            Port = int.Parse(Config.DocumentElement.SelectSingleNode("/Config/ServerPort").InnerText);
            MaxPlayers = int.Parse(Config.DocumentElement.SelectSingleNode("/Config/MaxPlayers").InnerText);
            XmlNodeList Resources = Config.DocumentElement.SelectNodes("/Config/Resource");

            NetPeerConfiguration NetConfig = new NetPeerConfiguration("ivmp");
            NetConfig.MaximumConnections = MaxPlayers;
            NetConfig.Port = Port;
            NetConfig.ConnectionTimeout = 50;
            NetConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            NetConfig.EnableMessageType(NetIncomingMessageType.StatusChanged);
            NetServer = new NetServer(NetConfig);
            NetServer.Start();
            PlayersController = new PlayersController();
            VehiclesController = new VehiclesController();
            ResourcesManager = new Scripting.ResourcesManager();
            ResourcesManager.Server = Instance;

            // load resources
            foreach(XmlNode Resource in Resources)
            {
                ResourcesManager.Load(Resource.Attributes["Name"].InnerText);
                ResourcesManager.Start(Resource.Attributes["Name"].InnerText);
            }

            Timer tick = new Timer();
            tick.Elapsed += OnTick;
            tick.Interval = TickRate;
            tick.Enabled = true;
            tick.Start();
            Console.WriteLine("Started game server on Port " + Port);
            Console.WriteLine("Max Players: " + MaxPlayers);
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
                                    Player Player = new Player();
                                    Player.Server = this;
                                    Player.NetConnection = Msg.SenderConnection;
                                    PlayersController.Add(Player);
                                    Console.WriteLine("Client connected. ID: " + Player.ID);
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
                                    Player Player = PlayersController.GetByNetConnection(Msg.SenderConnection);
                                    Console.WriteLine("Client disconnected. ID: " + Player.ID);
                                    PlayersController.Remove(Player);
                                    Player = null;
                                    NetOutgoingMessage OutMsg = NetServer.CreateMessage();
                                    OutMsg.Write((int)Shared.NetworkMessageType.PlayerDisconnected);
                                    OutMsg.Write(Msg.SenderConnection.RemoteUniqueIdentifier);
                                    NetServer.SendToAll(OutMsg, Msg.SenderConnection, NetDeliveryMethod.ReliableSequenced, 1);
                                    break;
                                }
                        }
                        break;
                    case NetIncomingMessageType.DebugMessage:
                        break;
                    case NetIncomingMessageType.WarningMessage:
                        break;
                    case NetIncomingMessageType.Data:
                        int MsgType = Msg.ReadInt32();
                        switch(MsgType)
                        {
                            case (int)Shared.NetworkMessageType.UpdatePlayer:
                                PlayerUpdateStruct PlayerData = new PlayerUpdateStruct();
                                Msg.ReadAllFields(PlayerData);
                                Player Player = PlayersController.GetByNetConnection(Msg.SenderConnection);
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
                                    Vehicle Vehicle = VehiclesController.GetByID(PlayerData.CurrentVehicle);
                                    Vehicle.Position = Position;
                                    Vehicle.Rotation = Rotation;
                                    Vehicle.Driver = Player;
                                    Player.CurrentVehicle = PlayerData.CurrentVehicle;
                                }
                                else
                                {
                                    if(Player.CurrentVehicle > 0)
                                    {
                                        Vehicle Vehicle = VehiclesController.GetByID(Player.CurrentVehicle);
                                        Vehicle.Driver = null;
                                        Player.CurrentVehicle = 0;
                                    }
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
            List<Player> Players = PlayersController.GetAll();
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

                Msg.WriteAllFields(PlayerData);

                NetServer.SendToAll(Msg, Player.NetConnection, NetDeliveryMethod.Unreliable, 1);
            }
        }

        public void UpdateAllVehicles()
        {
            List<Vehicle> Vehicles = VehiclesController.GetAll();
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
                Msg.WriteAllFields(VehicleData);

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
