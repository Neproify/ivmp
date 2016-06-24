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
    public class RemoteVehicle
    {
        public Vehicle Vehicle;

        public int ID;
        public string Model;

        public DateTime Interpolation_Start;
        public DateTime Interpolation_End;
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public Vector3 StartVelocity;
        public Vector3 EndVelocity;
        public Quaternion StartRotation;
        public Quaternion EndRotation;

        public RemoteVehicle(string Model)
        {
            this.Model = Model;
            Vehicle = World.CreateVehicle(Model, Vector3.Zero);
        }

        public void SetPosition(Vector3 Position, bool Instant)
        {
            StartPosition = Vehicle.Position;
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
            StartVelocity = Vehicle.Velocity;
            EndVelocity = Velocity;
        }

        public void SetRotation(Quaternion Rotation)
        {
            StartRotation = Vehicle.RotationQuaternion;
            EndRotation = Rotation;
        }

        public void UpdateInterpolation()
        {
            // Interpolate position
            if (Vehicle.Exists())
            {
                float Progress = ((float)DateTime.Now.Subtract(Interpolation_Start).TotalMilliseconds) / ((float)Interpolation_End.Subtract(Interpolation_Start).TotalMilliseconds);

                if (Vehicle.Position.DistanceTo(EndPosition) > 5.0f)
                {
                    Vehicle.Position = EndPosition;
                }

                Vector3 CurrentPosition;
                CurrentPosition = Vector3.Lerp(StartPosition, EndPosition, Progress);

                Vehicle.Position = CurrentPosition;
                // Interpolate velocity - Fix it later

                //Vector3 CurrentVelocity;
                //CurrentVelocity = Vector3.Lerp(StartVelocity, EndVelocity, Progress);

                //Vehicle.Velocity = EndVelocity;
                // Interpolate Rotation

                Quaternion CurrentRotation;
                CurrentRotation = Quaternion.Lerp(StartRotation, EndRotation, Progress);

                Vehicle.RotationQuaternion = CurrentRotation;
            }
        }
    }
}
