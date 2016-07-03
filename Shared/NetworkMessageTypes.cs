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
    public enum NetworkMessageType
    {
        PlayerConnected,
        PlayerDisconnected,
        LoadResource,
        StartResource,
        ResourceFile,
        UpdatePlayer,
        SpawnPlayer,
        FadeScreenIn,
        FadeScreenOut,
        UpdateVehicle
    }
}
