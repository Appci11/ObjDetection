using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetection
{
    public class Illustration
    {
        //target
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        //source
        public int Page { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }

        public Illustration()
        {
        }
        public Illustration(int number, string name, string type, int page, double left, double top, double right, double bottom)
        {
            Number = number;
            Name = name;
            Type = type;
            Page = page;
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
