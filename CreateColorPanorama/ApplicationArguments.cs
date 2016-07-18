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

        [ValueArgument(typeof(double), 'w', "width", Description = "Set desired width-per-picture", ValueOptional = false, DefaultValue = 1)]
        public double Width;

        [ValueArgument(typeof(int), 'h', "height", Description = "Set desired height of resulting picture", ValueOptional = false, DefaultValue = 1000)]
        public int Height;

        [ValueArgument(typeof(double), 'f', "fps", Description = "Set desired PicsPerSec", ValueOptional = false, DefaultValue = 1)]
        public double Spp;

        [FileArgument('o', "output", Description = "Output file", FileMustExist = false, Optional = true)]
        public FileInfo OutPutFile;

        [SwitchArgument('g', defaultValue: true, Description = "Set gradient, default is true", Optional = true)] public
            bool Gradient;
    }
}
