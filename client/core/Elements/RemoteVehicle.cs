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
        public RemoteVehicle(string Model)
        {
            this.Model = Model;
            GameReference = World.CreateVehicle(Model, Vector3.Zero);
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

                // Interpolate velocity

                Vector3 CurrentVelocity;
                CurrentVelocity = Vector3.Lerp(StartVelocity, EndVelocity, Progress);
                GameReference.Velocity = EndVelocity;

                // Interpolate Rotation

                Quaternion CurrentRotation;
                CurrentRotation = Quaternion.Lerp(StartRotation, EndRotation, Progress);

                GameReference.RotationQuaternion = CurrentRotation;

                // Interpolate heading

                float CurrentHeading;
                Vector2 StartHeading = new Vector2(this.StartHeading, 0);
                Vector2 EndHeading = new Vector2(this.EndHeading, 0);
                CurrentHeading = Vector2.Lerp(StartHeading, EndHeading, Progress).X;
                GameReference.Heading = this.EndHeading;
            }
        }
    }
}
