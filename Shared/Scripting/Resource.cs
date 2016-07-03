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
using Lidgren.Network;

namespace Shared.Scripting
{
    public class Resource
    {
        //public Server Server;
        public string Name;
        public bool IsStarted;
#if SERVER
        public List<ivmp_server_core.Scripting.ServerScript> Scripts;
#endif
#if CLIENT
        public List<ivmp_client_core.Scripting.ClientScript> Scripts;
#endif

        public Resource()
        {
#if SERVER
            Scripts = new List<ivmp_server_core.Scripting.ServerScript>();
#endif
#if CLIENT
            Scripts = new List<ivmp_client_core.Scripting.ClientScript>();
#endif
        }

#if SERVER
        public void AddScript(string ScriptName, string ScriptCode, string Type)
#endif
#if CLIENT
        public void AddScript(string ScriptName, string ScriptCode)
#endif
        {
#if SERVER
            ivmp_server_core.Scripting.ServerScript Script = new ivmp_server_core.Scripting.ServerScript();
#endif
#if CLIENT
            ivmp_client_core.Scripting.ClientScript Script = new ivmp_client_core.Scripting.ClientScript();
#endif
            Script.Name = ScriptName;
            Script.Set(ScriptCode);
#if SERVER
            Script.Type = Type;
#endif
            Scripts.Add(Script);
        }

#if SERVER
        public void SendToClient(NetConnection Connection)
        {
            NetOutgoingMessage msg = ivmp_server_core.Server.NetServer.CreateMessage();
            msg.Write((int)NetworkMessageType.LoadResource);
            msg.Write(Name);
            ivmp_server_core.Server.NetServer.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered);
            foreach (var Script in Scripts)
            {
                if (Script.Type != "client")
                    continue;
                msg = ivmp_server_core.Server.NetServer.CreateMessage();
                msg.Write((int)NetworkMessageType.ResourceFile);
                msg.Write(Name);
                msg.Write(Script.Name);
                msg.Write(Script.Code);
                ivmp_server_core.Server.NetServer.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered);
            }
            if(IsStarted)
            {
                msg = ivmp_server_core.Server.NetServer.CreateMessage();
                msg.Write((int)NetworkMessageType.StartResource);
                msg.Write(Name);
                ivmp_server_core.Server.NetServer.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered);
            }
        }
#endif

        public void Load()
        {
#if SERVER
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
                string ScriptType = ScriptToLoad.Attributes["type"].InnerText;
                if(!System.IO.File.Exists(Directory + "/" + ScriptName))
                {
                    Console.WriteLine("Cannot find " + ScriptName + "in resource " + Name);
                    throw new Exception("Cannot find script.");
                }
                string ScriptCode = System.IO.File.ReadAllText(Directory + "/" + ScriptName);
                AddScript(ScriptName, ScriptCode, ScriptType);
            }
#endif
        }

        public void Start()
        {
            foreach(var Script in Scripts)
            {
#if SERVER
                if (Script.Type != "server")
                    continue;
#endif
                Script.Execute();
            }
#if SERVER
            ivmp_server_core.Server.EventsManager.GetEvent("OnResourceStart").Trigger();
#endif
            IsStarted = true;
        }
    }
}
