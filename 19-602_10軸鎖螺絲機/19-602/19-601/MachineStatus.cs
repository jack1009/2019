using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _19_602
{
    public class MachineStatus
    {
        public string PlcIP { get; set; }
        public int PlcPort { get; set; }
        public int Channel { get; set; }
        public bool AreaSensor { get; set; }
        public string ComPort { get; set; }
        public int Language { get; set; }
    }
}
