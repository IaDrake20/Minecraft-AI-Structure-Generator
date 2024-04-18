using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertProject
{
    class Prepare
    {
        public List<Block> LoadJson()
        {
            List<Block> blocks;
            using (StreamReader r = new StreamReader("file.json"))
            {
                string json = r.ReadToEnd();
                blocks = JsonConvert.DeserializeObject<List<Block>>(json);
            }
            return blocks;
        }

        public int[] rgbToHsl(int r, int g, int b)
        {
            r /= 255;
            g /= 255;
            b /= 255;

            //consts needed
            //const int c_r = r;

            var max = Math.Max(r, g);
            max = Math.Max(max, b);

            var min = Math.Min(r, g);
            min = Math.Min(max, b);

            var h = (max + min) / 2;
            var s = (max + min) / 2;
            var l = (max + min) / 2;

            if(max == min)
            {
                h = s = 0;
            } else
            {
                var d = max - min;

                s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
                
                if(max == r)
                {
                    h = (g - b) / d + (g < b ? 6 : 0);
                } else if (max == g)
                {
                    h = (b - r) / d + 2;
                } else if(max == b)
                {
                    h = (r - g) / d + 4;
                }
                h /= 6;
            }
            h *= 360;
            s *= 100;
            l *= 100;

            Math.Round((Decimal)h);
            Math.Round((Decimal)s);
            Math.Round((Decimal)l);

            return new int[] { h, s, l };
        }

        public void unkown1()
        {

        }
    }
}
