using System;
using System.Collections.Generic;
using System.Text;

namespace Zork.Core
{
    public static class PuzzleHandler
    {
        public static int[] cpdr { get; } = { 1024, -8, 2048, -7, 3072, 1, 4096, 9, 5120, 8, 6144, 7, 7168, -1, 8192, -9 };
        public static int[] cpwl { get; } = { 205, -8, 206, 8, 207, 1, 208, -1 };
        public static int[] cpvec { get; } =
            { 1,  1,  1,  1,  1,  1, 1, 1,
              1,  0, -1,  0,  0, -1, 0, 1,
              1, -1,  0,  1,  0, -2, 0, 1,
              1,  0,  0,  0,  0,  1, 0, 1,
              1, -3,  0,  0, -1, -1, 0, 1,
              1,  0,  0, -1,  0,  0, 0, 1,
              1,  1,  1,  0,  0,  0, 1, 1,
              1,  1,  1,  1,  1,  1, 1, 1 };
    }
}
