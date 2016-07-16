using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLineParser.Arguments;

namespace CreateColorPanorama
{
    public class ApplicationArguments
    {
        [FileArgument('i', "input", Description = "Input file", FileMustExist = true, Optional = false)]
        public FileInfo InputFile;
        [ValueArgument(typeof(int), 'w', "width", Description = "Set desired width-per-picture", ValueOptional = false, DefaultValue = 1)]
        public int Width;
        [ValueArgument(typeof(int), 'h', "height", Description = "Set desired height of resulting picture", ValueOptional = false, DefaultValue = 1000)]
        public int Height;
        [ValueArgument(typeof(double), 's', "spp", Description = "Set desired SecondsPerPic", ValueOptional = false, DefaultValue = 10)]
        public double Spp;
        [FileArgument('o', "output", Description = "Output file", FileMustExist = false, Optional = true)]
        public FileInfo OutPutFile;
        [SwitchArgument('v', "verbose", true, Description = "Shows whats going on", Optional = false)]
        public bool Verbose;

    }
}
