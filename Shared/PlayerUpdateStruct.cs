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
        public long ID; // only when sending to client

        public string Name;

        public string Model; // only when sending to client
        public int Health;
        public int Armor;

        public float Pos_X;
        public float Pos_Y;
        public float Pos_Z;

        public float Heading;

        public bool IsWalking;
        public bool IsRunning;
        public bool IsJumping;
    }
}
