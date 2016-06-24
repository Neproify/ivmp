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
    public class Program
    {
        public static bool Running = true;
        public static string Input;

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting IV:MP server... Type \"shutdown\" to stop.");
            var Server = new Server();
            while(Running == true)
            {
                Input = Console.ReadLine();
                if(Input == "shutdown")
                {
                    Server.Shutdown();
                    Running = false;
                }
                if(Input == "testplayer")
                {
                    Server.CreateTestPlayer();
                }
                if(Input == "removetestplayer")
                {
                    Server.RemoveTestPlayer();
                }
            }
        }
    }
}
