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
    public class RemotePlayer : Element
    {
        public Ped GameReference;

        public bool Initialized = false;

        RemotePlayerAnimationManager AnimationManager;

        public string Name;
        public RemoteVehicle CurrentVehicle;

        public bool IsWalking;
        public bool IsRunning;
        public bool IsJumping;
        public bool IsCrouching;
        public bool IsGettingIntoVehicle;
        public bool IsGettingOutOfVehicle;

        public bool TaskJumpAdded;

        public RemotePlayer(string Model)
        {
            GameReference = World.CreatePed(GTA.Model.FromString(Model), Vector3.Zero);
            GameReference.BlockGestures = true;
            GameReference.BlockPermanentEvents = true;
            GameReference.BlockWeaponSwitching = true;
            GameReference.PreventRagdoll = true;
            AnimationManager = new RemotePlayerAnimationManager(this);
            TaskJumpAdded = false;
            Initialized = true;
        }

        public override void Destroy()
        {
            if (GameReference.Exists())
            {
                GameReference.Delete();
            }
        }

        public override void SetPosition(Vector3 Position, bool Instant)
        {
            StartPosition = GameReference.Position;
            if (Instant == true)
            {
                StartPosition = Position;
            }
            EndPosition = Position;
        }

        public override void SetPosition(Vector3 Position)
        {
            SetPosition(Position, false);
        }

        public override void SetVelocity(Vector3 Velocity)
        {
            StartVelocity = GameReference.Velocity;
            EndVelocity = Velocity;
        }

        public override void SetHeading(float Heading)
        {
            StartHeading = GameReference.Heading;
            EndHeading = Heading;
        }

        public void SetHealth(int Health)
        {
            if (GameReference.Exists() == true)
            {
                GameReference.Health = Health;
            }
        }

        public void SetArmor(int Armor)
        {
            if (GameReference.Exists() == true)
            {
                GameReference.Armor = Armor;
            }
        }

        public Vector3 GetPosition()
        {
            if (GameReference.Exists() == true)
            {
                return GameReference.Position;
            }
            return Vector3.Zero;
        }

        public void UpdateInterpolation()
        {
            if (CurrentVehicle != null)
            {
                return;
            }

            if (GameReference.Exists())
            {
                // interpolate position
                float Progress = ((float)DateTime.Now.Subtract(Interpolation_Start).TotalMilliseconds) / ((float)Interpolation_End.Subtract(Interpolation_Start).TotalMilliseconds);

                Vector3 GamePosition = GameReference.Position;
                GamePosition.Z -= 1.0f;

                if (GamePosition.DistanceTo(EndPosition) > 5.0f)
                {
                    GameReference.Position = EndPosition;
                }

                // Interpolate velocity

                Vector3 CurrentVelocity;
                CurrentVelocity = Vector3.Lerp(StartVelocity, EndVelocity, Progress);

                GameReference.Velocity = CurrentVelocity;

                // Interpolate heading

                float CurrentHeading;
                Vector2 StartHeading = new Vector2(this.StartHeading, 0);
                Vector2 EndHeading = new Vector2(this.EndHeading, 0);
                CurrentHeading = Vector2.Lerp(StartHeading, EndHeading, Progress).X;

                GameReference.Heading = CurrentHeading;
            }
        }

        public void Update()
        {
            if (!Initialized)
                return;

            if (GameReference.Exists())
            {
                if (CurrentVehicle != null && !GameReference.isInVehicle(CurrentVehicle.GameReference))
                {
                    GameReference.WarpIntoVehicle(CurrentVehicle.GameReference, VehicleSeat.Driver);
                }
                else if (CurrentVehicle == null)
                {
                    if (GameReference.isInVehicle())
                    {
                        GameReference.LeaveVehicle();
                    }
                }

                bool AnimationPlayed = false;
                if (!GameReference.isInVehicle())
                {
                    if (IsCrouching == true)
                    {
                        if (!GTA.Native.Function.Call<bool>("IS_CHAR_DUCKING", GameReference))
                        {
                            GameReference.Task.ClearAllImmediately();
                            GTA.Native.Function.Call("SET_CHAR_DUCKING", GameReference, true);
                        }
                        AnimationPlayed = true;
                    }
                    else
                    {
                        if (GTA.Native.Function.Call<bool>("IS_CHAR_DUCKING", GameReference))
                        {
                            GTA.Native.Function.Call("SET_CHAR_DUCKING", GameReference, false);
                        }
                    }

                    if(IsJumping == true && !TaskJumpAdded)
                    {
                        TaskJumpAdded = true;
                        GTA.Native.Function.Call("TASK_JUMP", GameReference, 1);
                        AnimationPlayed = true;
                    }

                    if (!AnimationPlayed)
                    {
                        if (IsWalking == true)
                        {
                            if (IsRunning == true)
                            {
                                AnimationManager.PlayAnimation(Shared.RemotePlayerAnimations.Run);
                            }
                            else
                            {
                                AnimationManager.PlayAnimation(Shared.RemotePlayerAnimations.Walk);
                            }
                        }
                        else
                        {
                            AnimationManager.PlayAnimation(Shared.RemotePlayerAnimations.StandStill);
                        }
                    }
                    /*if (IsJumping == true && !AnimationPlayed)
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
                    }*/
                }
            }
        }
    }
}
