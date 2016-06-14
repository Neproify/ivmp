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
        public Quaternion StartRotation;
        public Quaternion EndRotation;

        public RemoteVehicle(string Model)
        {
            this.Model = Model;
            Vehicle = World.CreateVehicle(Model, Vector3.Zero);
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

        public void SetRotation(Quaternion Rotation)
        {
            StartRotation = EndRotation;
            EndRotation = Rotation;
        }

        public void UpdateInterpolation()
        {
            // interpolate position
            if (Vehicle.Exists())
            {
                bool CancelInterpolation = false;
                float Progress = ((float)DateTime.Now.Subtract(Interpolation_Start).TotalMilliseconds) / ((float)Interpolation_End.Subtract(Interpolation_Start).TotalMilliseconds);

                if (StartPosition.DistanceTo(EndPosition) > 5.0f)
                {
                    SetPosition(EndPosition, true);
                }

                Vector3 CurrentPosition;
                CurrentPosition = Vector3.Lerp(StartPosition, EndPosition, Progress);

                if (EndPosition.DistanceTo(Vehicle.Position) <= 0.01f)
                {
                    CancelInterpolation = true;
                }

                if (!CancelInterpolation)
                {
                    Vehicle.Position = CurrentPosition;
                }
            }

            // interpolate rotation
            if (Vehicle.Exists())
            {
                float Progress = ((float)DateTime.Now.Subtract(Interpolation_Start).TotalMilliseconds) / ((float)Interpolation_End.Subtract(Interpolation_Start).TotalMilliseconds);

                Quaternion CurrentRotation;
                CurrentRotation = Quaternion.Lerp(StartRotation, EndRotation, Progress);

                Vehicle.RotationQuaternion = CurrentRotation;
            }
        }
    }
}
