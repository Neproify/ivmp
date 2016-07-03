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
using System.Windows.Forms;
using GTA;
using Lidgren.Network;
using Shared;

namespace ivmp_client_core
{
    public class Core : Script
    {
        public Core()
        {
            Client.Core = this;
            Client.Initialize();
        }

        public void SetInterval(int Interval)
        {
            this.Interval = Interval;
        }

        public void AddConsoleCommand(string Command, ConsoleCommandDelegate MethodToBindTo)
        {
            BindConsoleCommand(Command, MethodToBindTo);
        }
    }
}
