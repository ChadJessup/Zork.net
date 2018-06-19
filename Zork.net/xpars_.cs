using System;
using System.Collections.Generic;
using System.Text;

namespace Zork.Core
{
    public static class xpars_
    {
        public static int xrmask { get; set; } = 255;
        public static int xdmask { get; set; } = 31744;
        public static int xfmask { get; set; } = 3;
        public static int xfshft { get; set; } = 256;
        public static int xashft { get; set; } = 256;
        public static int[] xelnt = { 1, 2, 3, 3 };
        public static int xnorm { get; set; } = 1;
        public static int xno { get; set; } = 2;
        public static int xcond { get; set; } = 3;
        public static int xdoor { get; set; } = 4;
        public static int xlflag { get; set; } = 32768;
    }
}
