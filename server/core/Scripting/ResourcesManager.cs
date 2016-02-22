using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ivmp_server_core.Scripting
{
    public class ResourcesManager
    {
        public Server Server;
        public List<Resource> Resources;

        public ResourcesManager()
        {
            Resources = new List<Resource>();
        }

        public void Load(string Name)
        {
            Resource Resource = new Resource();
            Resource.Name = Name;
            Resource.Server = Server;
            Resource.Load();
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
