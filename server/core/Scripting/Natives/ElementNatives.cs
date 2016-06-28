using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ivmp_server_core.Scripting.Natives
{
    public abstract class ElementNatives<T> where T : Element
    {
        public T Element { get; set; }

        public SharpDX.Vector3 Position
        {
            get
            {
                return Element.Position;
            }
            set
            {
                Element.Position = value;
            }
        }

        public SharpDX.Vector3 Velocity
        {
            get
            {
                return Element.Velocity;
            }
        }

        public SharpDX.Quaternion Rotation
        {
            get
            {
                return Element.Rotation;
            }
            set
            {
                Element.Rotation = value;
            }
        }

        public string Type
        {
            get
            {
                return Element.Type;
            }
        }

        public void Destroy()
        {
            Element.Destroy();
        }
    }
}
