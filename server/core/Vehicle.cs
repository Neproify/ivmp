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

namespace ivmp_server_core
{
    public class Vehicle
    {
        public int ID;
        public string Model;

        public float Pos_X;
        public float Pos_Y;
        public float Pos_Z;

        public float Rot_X;
        public float Rot_Y;
        public float Rot_Z;

        public Vehicle(string Model, float PosX, float PosY, float PosZ)
        {
            this.Model = Model;
            Pos_X = PosX;
            Pos_Y = PosY;
            Pos_Z = PosZ;

            Rot_X = 0;
            Rot_Y = 0;
            Rot_Z = 0;
        }
    }
}
