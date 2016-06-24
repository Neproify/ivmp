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
        public Vector3 StartVelocity;
        public Vector3 EndVelocity;
        public float StartHeading;
        public float EndHeading;

        public bool IsWalking;
        public bool IsRunning;
        public bool IsJumping;
        public bool IsCrouching;
        public bool IsGettingIntoVehicle;
        public bool IsGettingOutOfVehicle;

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
            StartPosition = Ped.Position;
            if (Instant == true)
            {
                StartPosition = Position;
            }
            EndPosition = Position;
        }

        public void SetPosition(Vector3 Position)
        {
            SetPosition(Position, false);
        }

        public void SetVelocity(Vector3 Velocity)
        {
            StartVelocity = Ped.Velocity;
            EndVelocity = Velocity;
        }

        public void SetHeading(float Heading)
        {
            StartHeading = Ped.Heading;
            EndHeading = Heading;
        }

        public void SetHealth(int Health)
        {
            if (Ped.Exists() == true)
            {
                Ped.Health = Health;
            }
        }

        public void SetArmor(int Armor)
        {
            if (Ped.Exists() == true)
            {
                Ped.Armor = Armor;
            }
        }

        public Vector3 GetPosition()
        {
            if (Ped.Exists() == true)
            {
                return Ped.Position;
            }
            return Vector3.Zero;
        }

        public void UpdateInterpolation()
        {
            if (CurrentVehicle != null)
            {
                return;
            }
            if (Ped.Exists())
            {
                // interpolate position
                float Progress = ((float)DateTime.Now.Subtract(Interpolation_Start).TotalMilliseconds) / ((float)Interpolation_End.Subtract(Interpolation_Start).TotalMilliseconds);

                Vector3 GamePosition = Ped.Position;
                GamePosition.Z -= 1.0f;

                if (GamePosition.DistanceTo(EndPosition) > 5.0f)
                {
                    Ped.Position = EndPosition;
                }

                //Vector3 CurrentPosition;
                //CurrentPosition = Vector3.Lerp(StartPosition, EndPosition, Progress);

                //Ped.Position = CurrentPosition;
                // Interpolate velocity

                Vector3 CurrentVelocity;
                CurrentVelocity = Vector3.Lerp(StartVelocity, EndVelocity, Progress);

                Ped.Velocity = CurrentVelocity;

                // Interpolate heading

                float CurrentHeading;
                Vector2 StartHeading = new Vector2(this.StartHeading, 0);
                Vector2 EndHeading = new Vector2(this.EndHeading, 0);
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
                if (CurrentVehicle != null && !Ped.isInVehicle(CurrentVehicle.Vehicle))
                {
                    Ped.WarpIntoVehicle(CurrentVehicle.Vehicle, VehicleSeat.Driver);
                }
                else if (CurrentVehicle == null)
                {
                    if (Ped.isInVehicle())
                    {
                        Ped.LeaveVehicle();
                    }
                }

                bool AnimationPlayed = false;
                if (!Ped.isInVehicle())
                {
                    if (IsCrouching == true && !AnimationPlayed)
                    {
                        AnimationManager.PlayAnimation(Shared.RemotePlayerAnimations.Crouch);
                        AnimationPlayed = true;
                    }
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
