using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateColorPanorama
{
    public class Panorama
    {
        public List<string> Svg { get; set; }
        public List<int> Pictures { get; }
        public string FolderName { get; }
        public string FileName { get; set; }
        public int WidthPerPic { get; set; }
        public int Height { get; set; }

        public Panorama(Video video, int widthPerPic, int height, FileInfo outputfile)
        {
            WidthPerPic = widthPerPic;
            Height = height;
            Pictures = video.Pics;
            FolderName = Path.GetFileNameWithoutExtension(video.FileLocation);
            if (outputfile != null)
                FileName = Path.GetFileNameWithoutExtension(outputfile.ToString());
            else
                FileName = FolderName;
                
            int width = Pictures.Count * WidthPerPic;
            Svg = new List<string>
                {@"<?xml version=""1.0"" standalone=""no""?>", $@"<svg width=""{width}"" height=""¨{Height}"" xmlns=""http://www.w3.org/2000/svg"" version=""1.1"">"};
            GenerateSVG();
        }
        public System.Drawing.Color CalculateAverageColor(Bitmap bm)
        {
            int width = bm.Width;
            int height = bm.Height;
            int red = 0;
            int green = 0;
            int blue = 0;
            int minDiversion = 15; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
            int dropped = 0; // keep track of dropped pixels
            long[] totals = new long[] { 0, 0, 0 };
            int bppModifier = bm.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb ? 3 : 4; // cutting corners, will fail on anything else but 32 and 24 bit images

            BitmapData srcData = bm.LockBits(new System.Drawing.Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, bm.PixelFormat);
            int stride = srcData.Stride;
            IntPtr Scan0 = srcData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = (y * stride) + x * bppModifier;
                        red = p[idx + 2];
                        green = p[idx + 1];
                        blue = p[idx];
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

        public void GenerateSVG()
        {
            int x = 0;
            foreach (var picture in Pictures)
            {
                Bitmap bm = new Bitmap($@"output\{FolderName}\{picture}.jpg");
                Color co = CalculateAverageColor(bm);
                Svg.Add(
                    $@" <rect x=""{x}"" y=""0"" width=""{WidthPerPic}"" height=""{Height}"" fill=""rgb({co.R},{co.G},{co.B})""/>");
                x += WidthPerPic;
            }
            Svg.Add(@"</svg>");

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
