using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteStatus
{
    public class Server
    {


        public Server(string comp, string cpu, string ram, string freeram, string raminuse, string uptime, string updated, string isvm, string OS,string drives)
        {

            this.Computer = comp;
            this.CPU = cpu;
            this.RAM = ram;
            this.FreeRAM = freeram;
            this.RAMInUse = raminuse;
            this.Uptime = uptime;
            this.Updated = updated;
            this.IsVM = isvm;
            this.OS = OS;
            this.Drives = drives;

        }
        public string Computer { get; set; }
        public string CPU { get; set; }
        public string RAM { get; set; }
        public string FreeRAM { get; set; }
        public string RAMInUse { get; set; }
        public string Uptime { get; set; }
        public string Updated { get; set; }
        public string IsVM { get; set; }
        public string OS { get; set; }
        public string Drives { get; set; }
    }
}
