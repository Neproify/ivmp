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
    class NetworkMessageTypes
    {
        public enum MessageType
        {
            PlayerConnected,
            PlayerDisconnected,
            UpdatePlayer,
            SpawnPlayer,
            FadeScreenIn,
            FadeScreenOut,
            UpdateVehicle
        }
    }
}
