using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracing.DataStructures
{
    public struct Resolution
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Pixels
        {
            get
            {
                return X * Y;
            }
        }
        // TODO: Aspect Ratio

        public Resolution(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
