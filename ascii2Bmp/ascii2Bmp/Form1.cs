using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ascii2Bmp
{
    public partial class Ascii2Bmp : Form
    {
        public Ascii2Bmp()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            string path = "c:\\testimage\\";
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] fileInfo = directoryInfo.GetFiles();

            if (null != fileInfo && fileInfo.Count() > 0)
            {
                //if(true==createHistogram(fileInfo))
                    createBmp(fileInfo);
            }
        }

        protected bool createHistogram(FileInfo[] fileInfo)
        {
            try
            {
                int[] histogram = new int[4096];

                // build a histogram
                for (int x = 0; x < fileInfo.Count(); x++)
                {
                    // convert 1 line of ascii to bytes
                    string[] pixels = loadAscii(fileInfo[x]);
                    for (int y = 0; y < pixels.Length; y++)
                    {
                        int value = int.Parse(pixels[y]);
                        histogram[value]++;
                    }
                }

                // get statistics
                int min=4096;
                int max=0;
                int numOfShades=0;
                for (int i=0; i<histogram.Length; i++)
                {
                    if (histogram[i] > 0)
                    {
                        numOfShades++;

                        if (i < min)
                            min = i;

                        if (i > max)
                            max = i;
                    }

                }
                return writeHistogram(histogram, min, max, numOfShades);

            }
            catch(Exception ex)
            {
                return false;
            }
        }

        protected bool writeHistogram(int[] histogram, int min, int max, int numOfShades)
        {
            try
            {

                using (StreamWriter outfile = new StreamWriter(@"C:\\dev\histogram.txt"))
                {
                    for(int i=0; i<histogram.Length; i++)
                    {
                        string line = i.ToString() + "\t" + histogram[i].ToString();
                        outfile.WriteLine(line);
                    }

                    string minStr = "min\t" + min.ToString();
                    outfile.WriteLine(minStr);

                    string maxStr = "max\t" + max.ToString();
                    outfile.WriteLine(maxStr);

                    string numShadesStr = "num of shades\t" + numOfShades.ToString();
                    outfile.WriteLine(numShadesStr);
                }
                return true;
            }
                catch(Exception ex)
            {
                return false;
            }
        }

        protected void createBmp(FileInfo[] fileInfo)
        {
            // create a bmp instance -- consider 8bpp only -- will requantize 12bpp -> 8 bpp
            Bitmap bmp = new Bitmap(fileInfo.Count(), 2048);

            for (int x = 0; x < fileInfo.Count(); x++)
            {
                // convert 1 line of ascii to bytes
                string[] pixels = loadAscii(fileInfo[x]);
                for (int y = 0; y < 2048; y++)
                {
                    int p = requantize(pixels[y]);  // 12bpp -> 8bpp linear scaling for now
                    Color color = Color.FromArgb(p, p, p);
                    bmp.SetPixel(x, y, color);
                }
            }
            bmp.Save("c:\\testImage\\test.bmp");
        }

        protected int requantize(string value)
        {
            double pixel = double.Parse(value);
            double p = pixel / 4096 * 256;
            return (int)(p);
        }

        protected string[] loadAscii(FileInfo fileInfo)
        {
            try
            {
                string[] lines = new string[2048];
                int index = 0;
                System.IO.StreamReader file = new System.IO.StreamReader(fileInfo.FullName);
                while (index < 2048 && (lines[index] = file.ReadLine()) != null )
                {
                    index++;
                }
                return lines;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
