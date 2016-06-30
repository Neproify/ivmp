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
using System.Xml;

namespace ivmp_server_core.Scripting
{
    public class Resource
    {
        public Server Server;
        public string Name;
        public List<Script> Scripts;

        public Resource()
        {
            Scripts = new List<Script>();
        }

        public void Load()
        {
            string Directory = "resources/" + Name;
            if (!System.IO.Directory.Exists(Directory))
            {
                Console.WriteLine("Resource directory(" + Name + ") not found.");
                throw new Exception("Resource directory not found.");
            }
            if (!System.IO.File.Exists(Directory + "/meta.xml"))
            {
                Console.WriteLine("Cannot find meta.xml in resource " + Name);
                throw new Exception("Cannot find meta.xml");
            }
            XmlDocument Meta = new XmlDocument();
            Meta.Load(Directory + "/meta.xml");
            XmlNodeList ScriptsToLoad = Meta.DocumentElement.SelectNodes("/meta/script");
            foreach(XmlNode ScriptToLoad in ScriptsToLoad)
            {
                string ScriptName = ScriptToLoad.Attributes["src"].InnerText;
                if(!System.IO.File.Exists(Directory + "/" + ScriptName))
                {
                    Console.WriteLine("Cannot find " + ScriptName + "in resource " + Name);
                    throw new Exception("Cannot find script.");
                }
                Script Script = new Script();
                Script.Server = Server;
                Script.Name = ScriptName;
                Script.Set(System.IO.File.ReadAllText(Directory + "/" + ScriptName));
                Scripts.Add(Script);
            }
        }

        public void Start()
        {
            foreach(var Script in Scripts)
            {
                Script.Execute();
                Server.EventsManager.GetEvent("OnResourceStart").Trigger();
            }
        }
    }
}
