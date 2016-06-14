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
using GTA;

namespace ivmp_client_core
{
    public class RemotePlayer
    {
        public bool Initialized = false;

        RemotePlayerAnimationManager AnimationManager;

        public long ID;
        public string Name;
        public Ped Ped;
        public RemoteVehicle CurrentVehicle;

        public DateTime Interpolation_Start;
        public DateTime Interpolation_End;
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public float Start_Heading;
        public float End_Heading;

        public bool IsWalking;
        public bool IsRunning;
        public bool IsJumping;

        public RemotePlayer()
        {
            Ped = World.CreatePed(Vector3.Zero);
            Ped.BlockGestures = true;
            Ped.BlockPermanentEvents = true;
            Ped.BlockWeaponSwitching = true;
            Ped.PreventRagdoll = true;
            AnimationManager = new RemotePlayerAnimationManager(this);
            Initialized = true;
        }

        public void Destroy()
        {
            if (Ped.Exists())
            {
                Ped.Delete();
            }
        }

        public void SetPosition(Vector3 Position, bool Instant)
        {
            if (Instant == true)
            {
                StartPosition = Position;
            }
            else
            {
                StartPosition = EndPosition;
            }
            EndPosition = Position;
        }

        public void SetPosition(Vector3 Position)
        {
            SetPosition(Position, false);
        }

        public void SetHeading(float Heading)
        {
            Start_Heading = End_Heading;
            End_Heading = Heading;
        }

        public void SetHealth(int Health)
        {
            if(Ped.Exists() == true)
            {
                Ped.Health = Health;
            }
        }

        public void SetArmor(int Armor)
        {
            if(Ped.Exists() == true)
            {
                Ped.Armor = Armor;
            }
        }

        public Vector3 GetPosition()
        {
            if(Ped.Exists() == true)
            {
                return Ped.Position;
            }
            return Vector3.Zero;
        }

        public void UpdateInterpolation()
        {
            if(CurrentVehicle != null)
            {
                return;
            }
            // interpolate position
            if (Ped.Exists())
            {
                float Progress = ((float)DateTime.Now.Subtract(Interpolation_Start).TotalMilliseconds) / ((float)Interpolation_End.Subtract(Interpolation_Start).TotalMilliseconds);

                if (StartPosition.DistanceTo(EndPosition) > 5.0f)
                {
                    SetPosition(EndPosition, true);
                }

                Vector3 CurrentPosition;
                CurrentPosition = Vector3.Lerp(StartPosition, EndPosition, Progress);

                Ped.Position = CurrentPosition;
            }
            // interpolate heading
            if (Ped.Exists())
            {
                float Progress = ((float)DateTime.Now.Subtract(Interpolation_Start).TotalMilliseconds) / ((float)Interpolation_End.Subtract(Interpolation_Start).TotalMilliseconds);
                float CurrentHeading;
                Vector2 StartHeading = new Vector2(Start_Heading, 0);
                Vector2 EndHeading = new Vector2(End_Heading, 0);
                CurrentHeading = Vector2.Lerp(StartHeading, EndHeading, Progress).X;

                Ped.Heading = CurrentHeading;
            }
        }

        public void Update()
        {
            if (!Initialized)
                return;

            if (Ped.Exists())
            {
                if(CurrentVehicle != null && !Ped.isInVehicle(CurrentVehicle.Vehicle))
                {
                    Ped.WarpIntoVehicle(CurrentVehicle.Vehicle, VehicleSeat.Driver);
                }
                else if(CurrentVehicle == null)
                {
                    if(Ped.isInVehicle())
                    {
                        Ped.LeaveVehicle();
                    }
                }

                bool AnimationPlayed = false;
                if (!Ped.isInVehicle())
                {
                    if (IsJumping == true && !AnimationPlayed)
                    {
                        AnimationManager.PlayAnimation(Shared.RemotePlayerAnimations.Jump);
                        AnimationPlayed = true;
                    }
                    if (IsRunning == true && !AnimationPlayed)
                    {
                        AnimationManager.PlayAnimation(Shared.RemotePlayerAnimations.Run);
                        AnimationPlayed = true;
                    }
                    if (IsWalking == true && !AnimationPlayed)
                    {
                        AnimationManager.PlayAnimation(Shared.RemotePlayerAnimations.Walk);
                        AnimationPlayed = true;
                    }
                    if (!AnimationPlayed)
                    {
                        AnimationManager.PlayAnimation(Shared.RemotePlayerAnimations.StandStill);
                        AnimationPlayed = true;
                    }
                }
            }
        }
    }
}
