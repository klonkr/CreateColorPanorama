using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CreateColorPanorama
{
    public class Video
    {
        public string FileLocation { get; }
        public int Time { get; set; }
        public List<int> Pics { get; set; }
        public string FileName { get; set; }
        public double SPP { get; set; }
        public bool Verbose { get; set; }

        public Video(string fileLocation, double spp, bool verbose)
        {
            Verbose = verbose;
            SPP = spp;
            Pics = new List<int>();
            this.FileLocation = fileLocation;
            this.FileName = Path.GetFileNameWithoutExtension(FileLocation);
            GetDuration();
        }

        public void GetDuration()
        {
            Process process = new Process();

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.FileName =
                Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) +
                @"\bin\ffprobe.exe";

            process.StartInfo.Arguments =
            $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 {FileLocation}";

            string duration = null;
            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                duration = process.StandardOutput.ReadLine();
                
            }
            DateTime dt = new DateTime();
            if (duration != null)
            {
                Time = (int)double.Parse(duration);
                // Duration = dt;
            }
            


            process.Close();
        }
        public bool ConvertToJpegs()
        {
            Console.WriteLine($"Starning conversion of video {FileLocation}...");
            DateTime dt = new DateTime();
            int minutemark = 0;
            System.IO.Directory.CreateDirectory($"output/{FileName}");

            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < Time / SPP; i++)
                {
                    Process process = new Process();

                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.StartInfo.FileName =
                        Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) +
                        @"\bin\ffmpeg.exe";
                    if (Verbose)
                        Console.WriteLine($"Started processing frame at {dt.ToString("ss")}, with arguments: -ss { dt.ToString("HH:mm:ss")} -i {FileLocation} -frames:v 1 output/{dt.ToString("ss")}.jpg");
                    process.StartInfo.Arguments =
                    $"-y -ss { dt.ToString("HH:mm:ss")} -i {FileLocation} -frames:v 1 output/{FileName}/{i}.jpg";

                    if (Verbose)
                        Console.WriteLine("FFMPEG staring...");
                    process.Start();
                    if (Verbose)
                        Console.WriteLine("FFMPEG started");

                    process.BeginOutputReadLine();
                    string output = process.StandardError.ReadToEnd();
                    if (Verbose)
                        Console.WriteLine("Waiting for FFMPEG to finish...");
                    process.WaitForExit(2000);

                    if (Verbose)
                        Console.WriteLine("FFMPEG done.");
                    process.Close();
                    if (Verbose)
                        Console.WriteLine("Closed FFMPEG");

                    progress.Report((double) i / (Time/SPP));
                    Thread.Sleep(20);
                    dt = dt.AddSeconds(SPP);
                    Pics.Add(i);
                }
            }
            return true;
        }
    }
}
