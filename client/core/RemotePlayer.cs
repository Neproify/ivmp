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

        /*public float Pos_X;
        public float Pos_Y;
        public float Pos_Z;
        public float Heading;*/
        public DateTime Interpolation_Start;
        public DateTime Interpolation_End;
        public float Start_Pos_X;
        public float Start_Pos_Y;
        public float Start_Pos_Z;
        public float Start_Heading;
        public float End_Pos_X;
        public float End_Pos_Y;
        public float End_Pos_Z;
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
            Ped.Delete();
        }

        public void SetPosition(Vector3 Position, bool instant)
        {
            /*if(ped.Exists() == true)
            {
                Pos_X = Position.X;
                Pos_Y = Position.Y;
                Pos_Z = Position.Z;
            }*/
            if (instant == true)
            {
                Start_Pos_X = Position.X;
                Start_Pos_Y = Position.Y;
                Start_Pos_Z = Position.Z;
            }
            else
            {
                Start_Pos_X = End_Pos_X;
                Start_Pos_Y = End_Pos_Y;
                Start_Pos_Z = End_Pos_Z;
            }
            End_Pos_X = Position.X;
            End_Pos_Y = Position.Y;
            End_Pos_Z = Position.Z;
        }

        public void SetPosition(Vector3 Position)
        {
            SetPosition(Position, false);
        }

        public void SetHeading(float Heading)
        {
            /*if(ped.Exists() == true)
            {
                this.Heading = Heading;
            }*/
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
            /*if (ped.Exists() == true)
            {
                Vector3 position = new Vector3();
                position.X = Pos_X;
                position.Y = Pos_Y;
                position.Z = Pos_Z;
                return position;
            }*/
            return Vector3.Zero;
        }

        public void UpdateInterpolation()
        {

            // interpolate position
            /*if (!bCancelPositionUpdate)
            {
                ped.Position = GetPosition();
            }*/

            if (true)
            {
                float Progress = ((float)DateTime.Now.Subtract(Interpolation_Start).TotalMilliseconds) / ((float)Interpolation_End.Subtract(Interpolation_Start).TotalMilliseconds);
                Vector3 StartPosition = new Vector3();
                StartPosition.X = Start_Pos_X;
                StartPosition.Y = Start_Pos_Y;
                StartPosition.Z = Start_Pos_Z;
                Vector3 EndPosition = new Vector3();
                EndPosition.X = End_Pos_X;
                EndPosition.Y = End_Pos_Y;
                EndPosition.Z = End_Pos_Z;

                if (StartPosition.DistanceTo(EndPosition) > 5.0f)
                {
                    SetPosition(EndPosition, true);
                }

                Vector3 CurrentPosition;
                CurrentPosition = Vector3.Lerp(StartPosition, EndPosition, Progress);

                Ped.Position = CurrentPosition;
            }
            // interpolate heading
            /*ped.Heading = Heading;*/
            if (true)
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
            /*bool bCancelPositionUpdate = false;
            bool bCancelRotationUpdate = false;*/

            bool AnimationPlayed = false;
            if (IsJumping == true && !AnimationPlayed)
            {
                AnimationManager.PlayAnimation(Shared.RemotePlayerAnimations.Jump);
                AnimationPlayed = true;
                //bCancelPositionUpdate = true;
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
