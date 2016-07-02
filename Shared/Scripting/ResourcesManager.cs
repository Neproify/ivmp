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

namespace Shared.Scripting
{
    public class ResourcesManager
    {
        public List<Resource> Resources;

        public ResourcesManager()
        {
            Resources = new List<Resource>();
        }

        public void Load(string Name)
        {
            Resource Resource = new Resource();
            Resource.Name = Name;
            try
            {
                Resource.Load();
            }
            catch(Exception)
            {
                throw new Exception("Error when loading resource");
            }
            Resources.Add(Resource);
            Console.WriteLine("Loading resource " + Name);
        }

        public Resource GetByName(string Name)
        {
            return Resources.Find(x => x.Name == Name);
        }

        public void Start(string Name)
        {
            Resource Resource = GetByName(Name);
            if (Resource == null)
                return;
            Resource.Start();
            Console.WriteLine("Starting resource " + Name);
        }
    }
}
