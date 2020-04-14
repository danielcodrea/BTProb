using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BTProb.Models.Settings
{
    public class SerilogSettings
    {
        public List<FileSettings> WriteTo { get; set; }
    }

    public class FileSettings
    {
        public string Name { get; set; }
        public Args Args { get; set; }
    }

    public class Args
    {
        public string path { get; set; }
        public RollingInterval rollingInterval { get; set; }
        public bool shared { get; set; }
        public string outputTemplate { get; set; }
    }
}
