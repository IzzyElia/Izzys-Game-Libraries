using Izzy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IzzysGameLibrary
{
    public static class Adapters
    {
        public static Microsoft.Xna.Framework.Color ToXNA(this FloatColor color)
        {
            return new Microsoft.Xna.Framework.Color(color.red, color.green, color.blue, color.alpha);
        }
    }
}
