﻿using System;

namespace Zork.Core
{
    public static class GlobalHandler
    {
        public static bool ghere_(int obj, int rm, Game game)
        {
            bool ret_val = true;

            // !ASSUME WINS.
            switch (obj - game.Star.strbit)
            {
                case 1: goto L1000;
                case 2: goto L1000;
                case 3: goto L1000;
                case 4: goto L1000;
                case 5: goto L1000;
                case 6: goto L1000;
                case 7: goto L1000;
                case 8: goto L1000;
                case 9: goto L1000;
                case 10: goto L1000;
                case 11: goto L1000;
                case 12: goto L2000;
                case 13: goto L3000;
                case 14: goto L4000;
                case 15: goto L5000;
                case 16: goto L5000;
                case 17: goto L5000;
                case 18: goto L6000;
                case 19: goto L7000;
                case 20: goto L8000;
                case 21: goto L9000;
                case 22: goto L9100;
                case 23: goto L8000;
                case 24: goto L10000;
                case 25: goto L11000;
            }

            throw new InvalidOperationException("60");

            // 1000--	STARS ARE ALWAYS HERE

            L1000:
            return ret_val;

            // 2000--	BIRD

            L2000:
            ret_val = rm >= (int)RoomIds.Forest1 && rm < (int)RoomIds.ForestClearing || rm == (int)RoomIds.mtree;
            return ret_val;

            // 3000--	TREE

            L3000:
            ret_val = rm >= (int)RoomIds.Forest1 && rm < (int)RoomIds.ForestClearing && rm != (int)RoomIds.Forest3;
            return ret_val;

            // 4000--	NORTH WALL

            L4000:
            ret_val = rm >= (int)RoomIds.bkvw && rm <= (int)RoomIds.bkbox || rm == (int)RoomIds.cpuzz;
            return ret_val;

            // 5000--	EAST, SOUTH, WEST WALLS

            L5000:
            ret_val = rm >= (int)RoomIds.bkvw && rm < (int)RoomIds.bkbox || rm == (int)RoomIds.cpuzz;
            return ret_val;

            // 6000--	GLOBAL WATER

            L6000:
            ret_val = (game.Rooms[rm - 1].Flags & (int)RoomFlags.WATER + RoomFlags.RFILL) != 0;
            return ret_val;

            // 7000--	GLOBAL GUARDIANS

            L7000:
            ret_val = rm >= (int)RoomIds.mrc && rm <= (int)RoomIds.mrd || rm >= (int)RoomIds.mrce && rm <= (int)RoomIds.mrdw || rm == (int)RoomIds.inmir;
            return ret_val;

            // 8000--	ROSE/CHANNEL

            L8000:
            ret_val = rm >= (int)RoomIds.mra && rm <= (int)RoomIds.mrd || rm == (int)RoomIds.inmir;
            return ret_val;

            // 9000--	MIRROR
            // 9100		PANEL

            L9100:
            if (rm == (int)RoomIds.fdoor)
            {
                return ret_val;
            }

            // !PANEL AT FDOOR.
            L9000:
            ret_val = rm >= (int)RoomIds.mra && rm <= (int)RoomIds.mrc || rm >= (int)RoomIds.mrae && rm <= (int)RoomIds.mrcw;
            return ret_val;

            // 10000--	MASTER

            L10000:
            ret_val = rm == (int)RoomIds.fdoor || rm == (int)RoomIds.ncorr || rm == (int)RoomIds.parap || rm == (int)RoomIds.cell;
            return ret_val;

            // 11000--	LADDER

            L11000:
            ret_val = rm == (int)RoomIds.cpuzz;
            return ret_val;
        }
    }
}
