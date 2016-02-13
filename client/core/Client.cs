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
using System.Windows.Forms;
using GTA;
using Lidgren.Network;
using Shared;

namespace ivmp_client_core
{
    public class Client : Script
    {
        public Client Instance;
        public NetClient NetClient;

        public string name;
        public RemotePlayerController RemotePlayerController;
        public RemoteVehicleController RemoteVehicleController;

        public bool IsSpawned;

        public bool Initialized = false;

        public Client()
        {
            Instance = this;
            Interval = Shared.Settings.TickRate;
            KeyDown += new GTA.KeyEventHandler(OnKeyDown);
            Tick += new EventHandler(OnTick);
            PerFrameDrawing += new GraphicsEventHandler(OnFrameRender);
            name = "Player";
            NetPeerConfiguration config = new NetPeerConfiguration("ivmp");
            config.AutoFlushSendQueue = true;
            config.ConnectionTimeout = 30;
            NetClient = new NetClient(config);
            NetClient.Start();
            Connect("25.152.94.206", 7777);
        }

        public void Connect(string IP, int port)
        {
            if(NetClient.ConnectionStatus == NetConnectionStatus.Connected)
            {
                Disconnect();
            }
            NetOutgoingMessage msg = NetClient.CreateMessage();
            msg.Write(Shared.Settings.NetworkVersion);
            NetClient.Connect(IP, port, msg);
        }

        public void Disconnect()
        {
            NetClient.Disconnect("Disconnect");
        }

        public void OnTick(object sender, EventArgs e)
        {
            if(NetClient == null)
            {
                return;
            }

            NetIncomingMessage msg;

            while((msg = NetClient.ReadMessage()) != null)
            {
                switch(msg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        switch(status)
                        {
                            case NetConnectionStatus.InitiatedConnect:
                                Game.Console.Print("Connecting to server.");
                                break;
                            case NetConnectionStatus.Connected:
                                Game.Console.Print("Connected to server. ID: " + NetClient.UniqueIdentifier);
                                World.CarDensity = 0;
                                World.PedDensity = 0;
                                GTA.Native.Function.Call("CLEAR_AREA", 0.0f, 0.0f, 0.0f, 4000.0f, true);
                                Game.LocalPlayer.Character.BlockGestures = true;
                                Game.LocalPlayer.Character.BlockPermanentEvents = true;
                                Game.LocalPlayer.Character.PreventRagdoll = true;
                                Game.LocalPlayer.Character.WillFlyThroughWindscreen = false;
                                Ped[] peds = World.GetAllPeds();
                                foreach(var ped in peds)
                                {
                                    if(ped != Game.LocalPlayer.Character)
                                    {
                                        ped.Delete();
                                    }
                                }
                                Vehicle[] vehicles = World.GetAllVehicles();
                                foreach(var vehicle in vehicles)
                                {
                                    vehicle.Delete();
                                }
                                RemotePlayerController = new RemotePlayerController();
                                RemoteVehicleController = new RemoteVehicleController();
                                IsSpawned = false;
                                Game.FadeScreenOut(1);
                                Initialized = true;
                                break;
                            case NetConnectionStatus.Disconnected:
                                Game.Console.Print("Disconnected from server.");
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        int MsgType = msg.ReadInt32();
                        switch (MsgType)
                        {
                            case (int)Shared.NetworkMessageType.PlayerConnected:
                                {
                                    break;
                                }
                            case (int)Shared.NetworkMessageType.PlayerDisconnected:
                                {
                                    long ID = msg.ReadInt64();
                                    RemotePlayer player = RemotePlayerController.FindByID(ID);
                                    RemotePlayerController.Remove(player);
                                    player.Destroy();
                                    player = null;
                                    break;
                                }
                            case (int)Shared.NetworkMessageType.UpdatePlayer:
                                {
                                    PlayerUpdateStruct PlayerData = new PlayerUpdateStruct();
                                    PlayerData.ID = msg.ReadInt64();
                                    PlayerData.Name = msg.ReadString();
                                    PlayerData.Model = msg.ReadString();
                                    PlayerData.Health = msg.ReadInt32();
                                    PlayerData.Armor = msg.ReadInt32();
                                    PlayerData.Pos_X = msg.ReadFloat();
                                    PlayerData.Pos_Y = msg.ReadFloat();
                                    PlayerData.Pos_Z = msg.ReadFloat();
                                    PlayerData.Heading = msg.ReadFloat();
                                    PlayerData.isWalking = msg.ReadBoolean();
                                    PlayerData.isRunning = msg.ReadBoolean();
                                    PlayerData.isJumping = msg.ReadBoolean();
                                    RemotePlayer player = RemotePlayerController.FindByID(PlayerData.ID);
                                    player.Name = PlayerData.Name;
                                    player.SetHealth(PlayerData.Health);
                                    player.SetArmor(PlayerData.Armor);
                                    Vector3 position = new Vector3();
                                    position.X = PlayerData.Pos_X;
                                    position.Y = PlayerData.Pos_Y;
                                    position.Z = PlayerData.Pos_Z - 1.0f;
                                    player.SetPosition(position);
                                    player.SetHeading(PlayerData.Heading);
                                    player.IsWalking = PlayerData.isWalking;
                                    player.IsRunning = PlayerData.isRunning;
                                    player.IsJumping = PlayerData.isJumping;

                                    player.Interpolation_Start = DateTime.Now;
                                    player.Interpolation_End = DateTime.Now.AddMilliseconds((double)Shared.Settings.TickRate).AddMilliseconds(NetClient.ServerConnection.AverageRoundtripTime / 1000);
                                    player.Update();
                                    break;
                                }
                            case (int)Shared.NetworkMessageType.SpawnPlayer:
                                {
                                    Vector3 position = new Vector3();
                                    position.X = msg.ReadFloat();
                                    position.Y = msg.ReadFloat();
                                    position.Z = msg.ReadFloat();
                                    float Heading = msg.ReadFloat();
                                    IsSpawned = true;
                                    Game.LocalPlayer.Character.Position = position;
                                    Game.LocalPlayer.Character.Heading = Heading;
                                    break;
                                }
                            case (int)Shared.NetworkMessageType.FadeScreenIn:
                                {
                                    int duration = msg.ReadInt32();
                                    Game.FadeScreenIn(duration);
                                    break;
                                }
                            case (int)Shared.NetworkMessageType.FadeScreenOut:
                                {
                                    int duration = msg.ReadInt32();
                                    Game.FadeScreenOut(duration);
                                    break;
                                }
                            case (int)Shared.NetworkMessageType.UpdateVehicle:
                                {
                                    VehicleUpdateStruct VehicleData = new VehicleUpdateStruct();
                                    VehicleData.ID = msg.ReadInt32();
                                    VehicleData.Model = msg.ReadString();
                                    VehicleData.Pos_X = msg.ReadFloat();
                                    VehicleData.Pos_Y = msg.ReadFloat();
                                    VehicleData.Pos_Z = msg.ReadFloat();
                                    VehicleData.Rot_X = msg.ReadFloat();
                                    VehicleData.Rot_Y = msg.ReadFloat();
                                    VehicleData.Rot_Z = msg.ReadFloat();

                                    RemoteVehicle Vehicle = RemoteVehicleController.FindByID(VehicleData.ID);
                                    Vehicle.Model = VehicleData.Model;
                                    Vector3 position = new Vector3();
                                    position.X = VehicleData.Pos_X;
                                    position.Y = VehicleData.Pos_Y;
                                    position.Z = VehicleData.Pos_Z;
                                    Vehicle.Vehicle.Position = position;
                                    Quaternion rotation = new Quaternion();
                                    rotation.X = VehicleData.Rot_X;
                                    rotation.Y = VehicleData.Rot_Y;
                                    rotation.Z = VehicleData.Rot_Z;
                                    Vehicle.Vehicle.RotationQuaternion = rotation;
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        break;
                    default:
                        Game.Console.Print("Unhandled message type: " + msg.MessageType);
                        break;
                }
            }

            if (NetClient.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage OutMsg = NetClient.CreateMessage();
                PlayerUpdateStruct PlayerData = new PlayerUpdateStruct();
                Vector3 PlayerPos = Game.LocalPlayer.Character.Position;
                Vector3 PlayerRot = Game.LocalPlayer.Character.Direction;
                float PlayerHeading = Game.LocalPlayer.Character.Heading;
                PlayerData.Name = name;
                PlayerData.Health = Game.LocalPlayer.Character.Health;
                PlayerData.Armor = Game.LocalPlayer.Character.Armor;
                PlayerData.Pos_X = PlayerPos.X;
                PlayerData.Pos_Y = PlayerPos.Y;
                PlayerData.Pos_Z = PlayerPos.Z;
                PlayerData.Heading = PlayerHeading;
                PlayerData.isWalking = Game.isGameKeyPressed(GameKey.MoveBackward) ||
                    Game.isGameKeyPressed(GameKey.MoveForward) ||
                    Game.isGameKeyPressed(GameKey.MoveLeft) ||
                    Game.isGameKeyPressed(GameKey.MoveRight);
                PlayerData.isRunning = Game.isGameKeyPressed(GameKey.Sprint);
                PlayerData.isJumping = Game.isGameKeyPressed(GameKey.Jump);
                OutMsg.Write((int)NetworkMessageType.UpdatePlayer);
                OutMsg.Write(PlayerData.Name);
                OutMsg.Write(PlayerData.Health);
                OutMsg.Write(PlayerData.Armor);
                OutMsg.Write(PlayerData.Pos_X);
                OutMsg.Write(PlayerData.Pos_Y);
                OutMsg.Write(PlayerData.Pos_Z);
                OutMsg.Write(PlayerData.Heading);
                OutMsg.Write(PlayerData.isWalking);
                OutMsg.Write(PlayerData.isRunning);
                OutMsg.Write(PlayerData.isJumping);
                NetClient.SendMessage(OutMsg, NetDeliveryMethod.UnreliableSequenced);
            }
        }

        public void OnKeyDown(object sender, GTA.KeyEventArgs e)
        {
            switch(e.Key)
            {
                default:
                    break;
            }
        }

        public void OnFrameRender(object sender, GraphicsEventArgs e)
        {
            if(Initialized && NetClient.ConnectionStatus == NetConnectionStatus.Connected)
            {
                List<RemotePlayer> players = RemotePlayerController.Players;
                foreach(var player in players)
                {
                    player.UpdateInterpolation();
                }
            }
        }
    }
}
