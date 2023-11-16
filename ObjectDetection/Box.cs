using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetection
{
    public class Box
    {
        public string Label { get; set; } = string.Empty;
        public double Score { get; set; }
        public double XTop { get; set; }
        public double YTop { get; set; }
        public double XBottom { get; set; }
        public double YBottom { get; set; }

        public Box()
        {
        }
        public Box(string label, double score, double xTop, double yTop, double xBottom, double yBottom)
        {
            Label = label;
            Score = score;
            XTop = xTop;
            YTop = yTop;
            XBottom = xBottom;
            YBottom = yBottom;
        }
    }
}
