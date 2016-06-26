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
    public class RemoteVehicle : Element
    {
        public Vehicle GameReference;

        public Quaternion StartRotation;
        public Quaternion EndRotation;

        public float StartSpeed;
        public float EndSpeed;

        public RemoteVehicle(string Model)
        {
            this.Model = Model;
            GameReference = World.CreateVehicle(Model, Vector3.Zero);
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
            StartVelocity = EndVelocity;
            EndVelocity = Velocity;
        }

        public void SetRotation(Quaternion Rotation)
        {
            StartRotation = EndRotation;
            EndRotation = Rotation;
        }

        public override void SetHeading(float Heading)
        {
            StartHeading = EndHeading;
            EndHeading = Heading;
        }

        public void SetSpeed(float Speed)
        {
            StartSpeed = EndSpeed;
            EndSpeed = Speed;
        }

        public void UpdateInterpolation()
        {
            // Interpolate position
            if (GameReference.Exists())
            {
                float Progress = ((float)DateTime.Now.Subtract(Interpolation_Start).TotalMilliseconds) / ((float)Interpolation_End.Subtract(Interpolation_Start).TotalMilliseconds);

                Vector3 GamePosition = GameReference.Position;

                if (GamePosition.DistanceTo(EndPosition) > 5.0f)
                {
                    GameReference.Position = EndPosition;
                }

                // Interpolate position

                Vector3 CurrentPosition;
                CurrentPosition = Vector3.Lerp(StartPosition, EndPosition, Progress);
                GameReference.Position = CurrentPosition;

                // Interpolate velocity

                Vector3 CurrentVelocity;
                CurrentVelocity = Vector3.Lerp(StartVelocity, EndVelocity, Progress);
                GameReference.Velocity = CurrentVelocity;

                // Interpolate Rotation

                Quaternion CurrentRotation;
                CurrentRotation = Quaternion.Lerp(StartRotation, EndRotation, Progress);

                GameReference.RotationQuaternion = CurrentRotation;

                // Interpolate heading

                float CurrentHeading;
                Vector2 StartHeading = new Vector2(this.StartHeading, 0);
                Vector2 EndHeading = new Vector2(this.EndHeading, 0);
                CurrentHeading = Vector2.Lerp(StartHeading, EndHeading, Progress).X;
                GameReference.Heading = CurrentHeading;

                // Interpolate speed

                float CurrentSpeed;
                Vector2 StartSpeed = new Vector2(this.StartSpeed, 0);
                Vector2 EndSpeed = new Vector2(this.EndSpeed, 0);
                CurrentSpeed = Vector2.Lerp(StartSpeed, EndSpeed, Progress).X;
                GameReference.Speed = CurrentSpeed;
            }
        }
    }
}
