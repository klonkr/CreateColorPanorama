using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;

namespace CreateColorPanorama
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLineParser.CommandLineParser parser = new CommandLineParser.CommandLineParser();
            ApplicationArguments aa = new ApplicationArguments();

            try
            {
                parser.ExtractArgumentAttributes(aa);
                parser.ParseCommandLine(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                parser.ShowUsage();
                Environment.Exit(1);
            }

            Video v = new Video(aa.InputFile.ToString(), aa.Spp, aa.Verbose);
            if (v.ConvertToJpegs())
            {
                Panorama panorama = new Panorama(v, aa.Width, aa.Height, aa.OutPutFile);
            }
        }
    }
}
