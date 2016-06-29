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

        public string PlayerName;
        public RemotePlayersController RemotePlayersController;
        public RemoteVehiclesController RemoteVehiclesController;

        public bool IsSpawned;

        public bool Initialized = false;

        public Client()
        {
            Instance = this;
            Interval = Shared.Settings.TickRate;
            KeyDown += new GTA.KeyEventHandler(OnKeyDown);
            Tick += new EventHandler(OnTick);
            PerFrameDrawing += new GraphicsEventHandler(OnFrameRender);
            PlayerName = "Player";
            NetPeerConfiguration Config = new NetPeerConfiguration("ivmp");
            Config.AutoFlushSendQueue = true;
            Config.ConnectionTimeout = 30;
            NetClient = new NetClient(Config);
            NetClient.Start();
            BindConsoleCommand("connect", ConnectCommand);
            BindConsoleCommand("disconnect", DisconnectCommand);
        }

        public void ConnectCommand(ParameterCollection Parameter)
        {
            Connect(Parameter[0], Parameter.ToInteger(1));
        }

        public void DisconnectCommand(ParameterCollection Parameter)
        {
            Disconnect();
        }

        public void Connect(string IP, int port)
        {
            if(NetClient.ConnectionStatus == NetConnectionStatus.Connected)
            {
                Disconnect();
            }
            NetOutgoingMessage Msg = NetClient.CreateMessage();
            Msg.Write(Shared.Settings.NetworkVersion);
            NetClient.Connect(IP, port, Msg);
        }

        public void Disconnect()
        {
            NetClient.Disconnect("Quit");
            RemotePlayersController = null;
            RemoteVehiclesController = null;
            Initialized = false;
        }

        public void OnTick(object sender, EventArgs e)
        {
            if(NetClient == null)
            {
                return;
            }

            NetIncomingMessage Msg;

            while((Msg = NetClient.ReadMessage()) != null)
            {
                switch(Msg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)Msg.ReadByte();
                        switch(status)
                        {
                            case NetConnectionStatus.InitiatedConnect:
                                Game.Console.Print("Connecting to server.");
                                break;
                            case NetConnectionStatus.Connected:
                                Game.Console.Print("Connected to server.");
                                World.CarDensity = 0;
                                World.PedDensity = 0;
                                Game.WantedMultiplier = 0;
                                Game.LocalPlayer.WantedLevel = 0;
                                GTA.Native.Function.Call("CLEAR_AREA", 0.0f, 0.0f, 0.0f, 4000.0f, true);
                                Game.LocalPlayer.Character.BlockGestures = true;
                                Game.LocalPlayer.Character.BlockPermanentEvents = true;
                                Game.LocalPlayer.Character.PreventRagdoll = true;
                                Game.LocalPlayer.Character.WillFlyThroughWindscreen = false;
                                Ped[] Peds = World.GetAllPeds();
                                foreach(var Ped in Peds)
                                {
                                    if (Ped.Exists())
                                    {
                                        if (Ped != Game.LocalPlayer.Character)
                                        {
                                            Ped.Delete();
                                        }
                                    }
                                }
                                Vehicle[] Vehicles = World.GetAllVehicles();
                                foreach(var Vehicle in Vehicles)
                                {
                                    if (Vehicle.Exists())
                                    {
                                        Vehicle.Delete();
                                    }
                                }
                                RemotePlayersController = new RemotePlayersController();
                                RemoteVehiclesController = new RemoteVehiclesController();
                                IsSpawned = false;
                                Game.FadeScreenOut(1);
                                Game.LocalPlayer.Model = "F_Y_SWAT";
                                Initialized = true;
                                break;
                            case NetConnectionStatus.Disconnected:
                                Game.Console.Print("Disconnected from server.");
                                Game.FadeScreenIn(500);
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        if (!Initialized)
                            continue;
                        int MsgType = Msg.ReadInt32();
                        switch (MsgType)
                        {
                            case (int)Shared.NetworkMessageType.PlayerConnected:
                                {
                                    break;
                                }
                            case (int)Shared.NetworkMessageType.PlayerDisconnected:
                                {
                                    int ID = Msg.ReadInt32();
                                    RemotePlayer Player = RemotePlayersController.GetByID(ID);
                                    if (Player != null)
                                    {
                                        RemotePlayersController.Remove(Player);
                                        Player.Destroy();
                                        Player = null;
                                    }
                                    break;
                                }
                            case (int)Shared.NetworkMessageType.UpdatePlayer:
                                {
                                    PlayerUpdateStruct PlayerData = new PlayerUpdateStruct();
                                    Msg.ReadAllFields(PlayerData);
                                    RemotePlayer Player = RemotePlayersController.GetByID(PlayerData.ID);
                                    if(Player == null)
                                    {
                                        Player = new RemotePlayer(PlayerData.Model);
                                        Player.ID = PlayerData.ID;
                                        RemotePlayersController.Add(Player);
                                    }
                                    Player.Name = PlayerData.Name;
                                    Player.SetHealth(PlayerData.Health);
                                    Player.SetArmor(PlayerData.Armor);
                                    if (PlayerData.CurrentVehicle > 0)
                                    {
                                        Player.CurrentVehicle = RemoteVehiclesController.GetByID(PlayerData.CurrentVehicle);
                                        Player.CurrentSeat = (VehicleSeat)PlayerData.VehicleSeat;
                                        Player.Update();
                                    }
                                    else
                                    {
                                        if(Player.CurrentVehicle != null)
                                        {
                                            Player.CurrentVehicle = null;
                                        }
                                        Vector3 Position = new Vector3();
                                        Position.X = PlayerData.Pos_X;
                                        Position.Y = PlayerData.Pos_Y;
                                        Position.Z = PlayerData.Pos_Z - 1.0f;
                                        Vector3 Velocity = new Vector3();
                                        Velocity.X = PlayerData.Vel_X;
                                        Velocity.Y = PlayerData.Vel_Y;
                                        Velocity.Z = PlayerData.Vel_Z;
                                        Player.SetPosition(Position);
                                        Player.SetVelocity(Velocity);
                                        Player.SetHeading(PlayerData.Heading);
                                        Player.IsWalking = PlayerData.IsWalking;
                                        Player.IsRunning = PlayerData.IsRunning;
                                        if (!Player.IsJumping && PlayerData.IsJumping == true)
                                        {
                                            Player.TaskJumpAdded = false;
                                        }
                                        Player.IsJumping = PlayerData.IsJumping;
                                        Player.IsCrouching = PlayerData.IsCrouching;

                                        Player.Interpolation_Start = DateTime.Now;
                                        Player.Interpolation_End = DateTime.Now.AddMilliseconds((double)Shared.Settings.TickRate).AddMilliseconds(NetClient.ServerConnection.AverageRoundtripTime / 1000);
                                        Player.Update();
                                    }
                                    break;
                                }
                            case (int)Shared.NetworkMessageType.SpawnPlayer:
                                {
                                    Vector3 Position = new Vector3();
                                    Position.X = Msg.ReadFloat();
                                    Position.Y = Msg.ReadFloat();
                                    Position.Z = Msg.ReadFloat();
                                    float Heading = Msg.ReadFloat();
                                    IsSpawned = true;
                                    Game.LocalPlayer.Character.Position = Position;
                                    Game.LocalPlayer.Character.Heading = Heading;
                                    break;
                                }
                            case (int)Shared.NetworkMessageType.FadeScreenIn:
                                {
                                    int Duration = Msg.ReadInt32();
                                    Game.FadeScreenIn(Duration);
                                    break;
                                }
                            case (int)Shared.NetworkMessageType.FadeScreenOut:
                                {
                                    int Duration = Msg.ReadInt32();
                                    Game.FadeScreenOut(Duration);
                                    break;
                                }
                            case (int)Shared.NetworkMessageType.UpdateVehicle:
                                {
                                    VehicleUpdateStruct VehicleData = new VehicleUpdateStruct();
                                    Msg.ReadAllFields(VehicleData);

                                    RemoteVehicle Vehicle = RemoteVehiclesController.GetByID(VehicleData.ID);
                                    if(Vehicle == null)
                                    {
                                        Vehicle = new RemoteVehicle(VehicleData.Model);
                                        Vehicle.ID = VehicleData.ID;
                                        RemoteVehiclesController.Add(Vehicle);
                                    }
                                    Vehicle.Model = VehicleData.Model;
                                    Vector3 Position = new Vector3();
                                    Position.X = VehicleData.Pos_X;
                                    Position.Y = VehicleData.Pos_Y;
                                    Position.Z = VehicleData.Pos_Z;
                                    Vehicle.SetPosition(Position);
                                    Vector3 Velocity = new Vector3();
                                    Velocity.X = VehicleData.Vel_X;
                                    Velocity.Y = VehicleData.Vel_Y;
                                    Velocity.Z = VehicleData.Vel_Z;
                                    Vehicle.SetVelocity(Velocity);
                                    Vehicle.SetHeading(VehicleData.Heading);
                                    Quaternion Rotation = new Quaternion();
                                    Rotation.X = VehicleData.Rot_X;
                                    Rotation.Y = VehicleData.Rot_Y;
                                    Rotation.Z = VehicleData.Rot_Z;
                                    Rotation.W = VehicleData.Rot_A;
                                    Vehicle.SetRotation(Rotation);
                                    Vehicle.Interpolation_Start = DateTime.Now;
                                    Vehicle.Interpolation_End = DateTime.Now.AddMilliseconds((double)Shared.Settings.TickRate).AddMilliseconds(NetClient.ServerConnection.AverageRoundtripTime / 1000);
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        break;
                    default:
                        Game.Console.Print("Unhandled message type: " + Msg.MessageType);
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
                PlayerData.Name = Name;
                PlayerData.Health = Game.LocalPlayer.Character.Health;
                PlayerData.Armor = Game.LocalPlayer.Character.Armor;
                if (Game.LocalPlayer.Character.isInVehicle())
                {
                    Vehicle CurrentVehicle = Game.LocalPlayer.Character.CurrentVehicle;
                    PlayerData.CurrentVehicle = RemoteVehiclesController.GetByGame(CurrentVehicle).ID;
                    VehicleSeat CurrentSeat = VehicleSeat.None;
                    if (CurrentVehicle.GetPedOnSeat(VehicleSeat.Driver) == Game.LocalPlayer.Character)
                    {
                        CurrentSeat = VehicleSeat.Driver;
                    }
                    else if (CurrentVehicle.GetPedOnSeat(VehicleSeat.RightFront) == Game.LocalPlayer.Character)
                    {
                        CurrentSeat = VehicleSeat.RightFront;
                    }
                    else if (CurrentVehicle.GetPedOnSeat(VehicleSeat.LeftRear) == Game.LocalPlayer.Character)
                    {
                        CurrentSeat = VehicleSeat.LeftRear;
                    }
                    else if (CurrentVehicle.GetPedOnSeat(VehicleSeat.RightRear) == Game.LocalPlayer.Character)
                    {
                        CurrentSeat = VehicleSeat.RightRear;
                    }
                    PlayerData.VehicleSeat = (int)CurrentSeat;
                    PlayerData.Pos_X = CurrentVehicle.Position.X;
                    PlayerData.Pos_Y = CurrentVehicle.Position.Y;
                    PlayerData.Pos_Z = CurrentVehicle.Position.Z;
                    Vector3 Velocity = CurrentVehicle.Velocity;
                    Velocity.Normalize();
                    PlayerData.Vel_X = Velocity.X;
                    PlayerData.Vel_Y = Velocity.Y;
                    PlayerData.Vel_Z = Velocity.Z;
                    PlayerData.Rot_X = CurrentVehicle.RotationQuaternion.X;
                    PlayerData.Rot_Y = CurrentVehicle.RotationQuaternion.Y;
                    PlayerData.Rot_Z = CurrentVehicle.RotationQuaternion.Z;
                    PlayerData.Rot_A = CurrentVehicle.RotationQuaternion.W;
                    PlayerData.Heading = CurrentVehicle.Heading;
                    PlayerData.Speed = CurrentVehicle.Speed;
                }
                else
                {
                    Vector3 PlayerVel = Game.LocalPlayer.Character.Velocity;
                    PlayerData.Pos_X = PlayerPos.X;
                    PlayerData.Pos_Y = PlayerPos.Y;
                    PlayerData.Pos_Z = PlayerPos.Z;
                    PlayerData.Vel_X = PlayerVel.X;
                    PlayerData.Vel_Y = PlayerVel.Y;
                    PlayerData.Vel_Z = PlayerVel.Z;
                    PlayerData.Heading = PlayerHeading;
                    PlayerData.IsWalking = Game.isGameKeyPressed(GameKey.MoveBackward) ||
                        Game.isGameKeyPressed(GameKey.MoveForward) ||
                        Game.isGameKeyPressed(GameKey.MoveLeft) ||
                        Game.isGameKeyPressed(GameKey.MoveRight);
                    PlayerData.IsRunning = Game.isGameKeyPressed(GameKey.Sprint);
                    PlayerData.IsJumping = Game.isGameKeyPressed(GameKey.Jump);
                    PlayerData.IsCrouching = GTA.Native.Function.Call<bool>("IS_CHAR_DUCKING", Game.LocalPlayer.Character);
                }
                OutMsg.Write((int)NetworkMessageType.UpdatePlayer);
                OutMsg.WriteAllFields(PlayerData);
                NetClient.SendMessage(OutMsg, NetDeliveryMethod.UnreliableSequenced);
            }
        }

        public void OnKeyDown(object sender, GTA.KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Keys.G:
                    Vehicle NearestVehicle = World.GetClosestVehicle(Game.LocalPlayer.Character.Position, 10.0f);
                    if(NearestVehicle != null && NearestVehicle.Exists())
                    {
                        VehicleSeat FreeSeat = NearestVehicle.GetFreePassengerSeat();
                        if(FreeSeat != VehicleSeat.None)
                        {
                            Game.LocalPlayer.Character.Task.EnterVehicle(NearestVehicle, FreeSeat);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void OnFrameRender(object sender, GraphicsEventArgs e)
        {
            if(Initialized && NetClient.ConnectionStatus == NetConnectionStatus.Connected)
            {
                List<RemotePlayer> Players = RemotePlayersController.Players;
                foreach(var Player in Players)
                {
                    Player.UpdateInterpolation();
                }

                List<RemoteVehicle> Vehicles = RemoteVehiclesController.Vehicles;
                foreach(var Vehicle in Vehicles)
                {
                    bool CancelThisVehicleUpdate = false;
                    if (Game.LocalPlayer.Character.isInVehicle())
                    {
                        if (Game.LocalPlayer.Character.CurrentVehicle == Vehicle.GameReference)
                        {
                            CancelThisVehicleUpdate = true;
                        }
                    }

                    if (CancelThisVehicleUpdate == false)
                    {
                        Vehicle.UpdateInterpolation();
                    }
                }
            }
        }
    }
}
