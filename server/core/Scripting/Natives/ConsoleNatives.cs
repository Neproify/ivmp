using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ivmp_server_core.Scripting.Natives
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
