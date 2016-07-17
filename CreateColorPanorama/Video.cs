using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CreateColorPanorama
{
    public class Video
    {
        public string FileLocation { get; }
        private int Time { get; set; }
        private string FileName { get; }
        private double Spp { get; set; }
        private ProgressBar ProgressBar { get; set; }
        private List<int> Progress { get; set; }

        public Video(string fileLocation, double spp)
        {
            Progress = new List<int>();
            Spp = spp;
            FileLocation = fileLocation;
            FileName = Path.GetFileNameWithoutExtension(FileLocation);
            GetDuration();
        }

        public void GetDuration()
        {
            using (Process process = new Process())
            {
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
                Time = (int)double.Parse(duration);

                process.WaitForExit();
            }
        }
        public bool ConvertToJpegs()
        {
            Console.WriteLine($"Starning conversion of video {FileLocation}...");
            System.IO.Directory.CreateDirectory($"output/{FileName}");

            using (Process process = new Process())
            {
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                //process.OutputDataReceived += new DataReceivedEventHandler(OutPutHandler);
                process.ErrorDataReceived += new DataReceivedEventHandler(OutPutHandler);

                process.StartInfo.FileName =
                    Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) +
                    @"\bin\ffmpeg.exe";
                process.StartInfo.Arguments =
                    $"-y -i {FileLocation} -vf fps=1 output/{FileName}/%d.png";

                ProgressBar = new ProgressBar();
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                //string output = process.StandardError.ReadToEnd();
                process.WaitForExit();

            }
            return true;
        }

        private void OutPutHandler(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data) || !e.Data.Contains("frame")) return;
            string output = e.Data;
            int result = int.Parse(Regex.Match(output, @"\+?\d+").ToString());
            ProgressBar.Report((double)result / Time);
        }
    }
}
