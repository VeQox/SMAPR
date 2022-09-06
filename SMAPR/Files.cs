using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAPR
{
    class Files
    {
        public int Successful { get; set; }
        public int Failed { get; set; }
        public int Count { get { return Successful + Failed; } }
    }
}
