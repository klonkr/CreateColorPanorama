using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CreateColorPanorama
{
    public class Panorama
    {
        private List<string> Svg { get; }
        private string[] Pictures { get; }
        private string FolderName { get; }
        private string FileName { get; }
        private double WidthPerPic { get; }
        private int Height { get; }
        private bool Gradient { get; }
        private double Width { get; }

        public Panorama(Video video, double widthPerPic, int height, FileInfo outputfile, bool gradient)
        {
            Gradient = gradient;
            FolderName = Path.GetFileNameWithoutExtension(video.FileLocation);
            Pictures = Directory.GetFiles($"output/{FolderName}");
            WidthPerPic = widthPerPic;
            Height = height;
            //Pictures = video.Pics;
            FileName = outputfile != null ? Path.GetFileNameWithoutExtension(outputfile.ToString()) : FolderName;

            Width = Pictures.Length * WidthPerPic;
            Svg = new List<string>
                {@"<?xml version=""1.0"" standalone=""no""?>", $@"<svg width=""{Width}"" height=""¨{Height}"" xmlns=""http://www.w3.org/2000/svg"" version=""1.1"">"};
            if (Gradient)
                Svg.Add(@"  <defs>
      <linearGradient id=""Gradient2"" x1=""0"" x2=""0"" y1=""0"" y2=""1"">
        <stop offset=""0%"" stop-color=""black""/>
        <stop offset=""50%"" stop-color=""black"" stop-opacity=""0""/>
        <stop offset=""100%"" stop-color=""black""/>
      </linearGradient>
  </defs>");
            GenerateSvg();
        }

        private System.Drawing.Color CalculateAverageColor(Bitmap bm)
        {
            int width = bm.Width;
            int height = bm.Height;
            int minDiversion = 15; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
            int dropped = 0; // keep track of dropped pixels
            long[] totals = new long[] { 0, 0, 0 };
            int bppModifier = bm.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb ? 3 : 4; // cutting corners, will fail on anything else but 32 and 24 bit images

            BitmapData srcData = bm.LockBits(new System.Drawing.Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, bm.PixelFormat);
            int stride = srcData.Stride;
            IntPtr scan0 = srcData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = (y * stride) + x * bppModifier;

                        int red = p[idx + 2];
                        int green = p[idx + 1];
                        int blue = p[idx];

                        if (Math.Abs(red - green) > minDiversion || Math.Abs(red - blue) > minDiversion || Math.Abs(green - blue) > minDiversion)
                        {
                            totals[2] += red;
                            totals[1] += green;
                            totals[0] += blue;
                        }
                        else
                        {
                            dropped++;
                        }
                    }
                }
            }


            int count = width * height - dropped;

            if (count > 0)
            {
                int avgR = (int)(totals[2] / count);
                int avgG = (int)(totals[1] / count);
                int avgB = (int)(totals[0] / count);

                return System.Drawing.Color.FromArgb(avgR, avgG, avgB);

            }


            return System.Drawing.Color.Black;
        }

        public void GenerateSvg()
        {
            double x = 1;
            System.IO.Directory.CreateDirectory($"panoramas/");
            Console.WriteLine("Starting");
            using (ProgressBar progressBar = new ProgressBar())
            {
                for (int i = 0; i < Pictures.Length; i++)
                {
                    Bitmap bm = new Bitmap($"output/{FolderName}/{i + 1}.png");
                    Color co = CalculateAverageColor(bm);
                    Svg.Add(
                        $@"  <rect x=""{x}"" y=""0"" width=""{WidthPerPic}"" height=""{Height}"" fill=""rgb({co.R},{co.G},{co.B})"" z-index=""-1""/>");
                    x += WidthPerPic;
                    progressBar.Report((double)i / Pictures.Length);
                }
                if(Gradient)
                    Svg.Add($@"  <rect id=""rect1"" x=""0"" y=""0"" z-index=""1"" width=""{Width}"" height=""{Height}"" fill=""url(#Gradient2)""/>");
                Svg.Add(@"</svg>");
            }
            using (StreamWriter file = new StreamWriter($"panoramas/{FileName}.svg"))
            {
                foreach (var line in Svg)
                {
                    file.WriteLine(line);
                }
            }

        }
    }
}
