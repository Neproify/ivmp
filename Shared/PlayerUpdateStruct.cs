/*
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE', which is part of this source code package.
 * Copyright (c) 2016 Neproify
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class PlayerUpdateStruct
    {
        public long ID = 0; // only when sending to client

        public string Name = "Player";

        public string Model = "F_Y_SWAT"; // only when sending to client
        public int Health = 0;
        public int Armor = 0;

        public int CurrentVehicle = 0;

        public float Pos_X = 0f;
        public float Pos_Y = 0f;
        public float Pos_Z = 0f;

        public float Vel_X = 0f;
        public float Vel_Y = 0f;
        public float Vel_Z = 0f;

        public float Rot_X = 0f;
        public float Rot_Y = 0f;
        public float Rot_Z = 0f;
        public float Rot_A = 0f;

        public float Heading = 0f;

        public bool IsWalking = false;
        public bool IsRunning = false;
        public bool IsJumping = false;
        public bool IsCrouching = false;
        public bool IsGettingIntoVehicle = false;
        public bool IsGettingOutOfVehicle = false;
    }
}
