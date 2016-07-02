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

namespace Shared.Scripting.Natives
{
    public class Console
    {
        public Console()
        {
        }

        public void Print(string Text)
        {
            System.Console.WriteLine(Text);
        }
    }
}
