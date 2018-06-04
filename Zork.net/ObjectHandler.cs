using System;
using System.Collections.Generic;
using System.Text;

namespace Zork.Core
{
    public static class ObjectHandler
    {
        public static bool IsInRoom(int roomId, int obj, Game game)
        {
            // System generated locals
            int i__1;
            bool ret_val;

            // Local variables
            int i;

            ret_val = true;
            if (game.Objects.oroom[obj - 1] == roomId)
            {
                return ret_val;
            }

            // !IN ROOM?
            i__1 = game.Rooms2.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // !NO, SCH ROOM2.
                if (game.Rooms2.Rooms[i - 1] == obj && game.Rooms2.RRoom[i - 1] == roomId)
                {

                    return ret_val;
                }
                /* L100: */
            }
            ret_val = false;

            // !NOT PRESENT.
            return ret_val;
        }

        /// <summary>
        /// princo_ - PRINT CONTENTS OF OBJECT
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="descriptionId"></param>
        /// <param name="game"></param>
        public static void PrintDescription(int objectId, int descriptionId, Game game)
        {
            int i__1;

            // Local variables
            int i;

            MessageHandler.rspsub_(descriptionId, game.Objects.odesc2[objectId - 1], game);
            // !PRINT HEADER.
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // !LOOP THRU.
                if (game.Objects.ocan[i - 1] == objectId)
                {
                    MessageHandler.rspsub_(502, game.Objects.odesc2[i - 1], game);
                }

                // L100:
            }
        }

        /// <summary>
        /// oactor_ - Get Actor associated with object.
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public static int GetActor(int objId, Game game)
        {
            int ret_val = 1;

            for (int i = 1; i <= game.Adventurers.Count; ++i)
            {
                // !LOOP THRU ACTORS.
                ret_val = i;
                // !ASSUME FOUND.
                if (game.Adventurers.Objects[i - 1] == objId)
                {
                    return ret_val;
                }

                // !FOUND IT?
                // L100:
            }

            throw new InvalidOperationException($"PROGRAM ERROR 40, PARAMETER={objId}");
            //bug_(40, obj);
            // !NO, DIE.
            return ret_val;
        }

        /// <summary>
        /// qempty_ Test for object empty
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public static bool IsObjectEmpty(int objId, Game game)
        {
            bool ret_val = false;

            // !ASSUME LOSE.
            for (int i = 1; i <= game.Objects.Count; ++i)
            {
                if (game.Objects.ocan[i - 1] == objId)
                {
                    return ret_val;
                }
            }

            ret_val = true;
            return ret_val;
        }

        /// <summary>
        /// qhere_ - Test for object in room
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="rm"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public static bool qhere_(int obj, int rm, Game game)
        {
            int i__1;
            bool ret_val;

            int i;

            ret_val = true;
            if (game.Objects.oroom[obj - 1] == rm)
            {
                return ret_val;
            }
            /* 						!IN ROOM? */
            i__1 = game.Rooms2.Count;
            for (i = 1; i <= i__1; ++i)
            {
                /* 						!NO, SCH ROOM2. */
                if (game.Rooms2.Rooms[i - 1] == obj && game.Rooms2.RRoom[i - 1] == rm)
                {
                    return ret_val;
                }
                /* L100: */
            }
            ret_val = false;
            /* 						!NOT PRESENT. */
            return ret_val;
        }

        /* WEIGHT- RETURNS SUM OF WEIGHT OF QUALIFYING OBJECTS */
        public static int weight_(int rm, int cn, int ad, Game game)
        {
            /* System generated locals */
            int ret_val, i__1;

            /* Local variables */
            int i, j;

            ret_val = 0;
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                /* 						!OMIT BIG FIXED ITEMS. */
                if (game.Objects.osize[i - 1] >= 10000)
                {
                    goto L100;
                }
                /* 						!IF FIXED, FORGET IT. */
                if (ObjectHandler.qhere_(i, rm, game) && rm != 0 || game.Objects.oadv[i - 1] == ad && ad != 0)
                {
                    goto L50;
                }

                j = i;
                /* 						!SEE IF CONTAINED. */
                L25:
                j = game.Objects.ocan[j - 1];
                /* 						!GET NEXT LEVEL UP. */
                if (j == 0)
                {
                    goto L100;
                }
                /* 						!END OF LIST? */
                if (j != cn)
                {
                    goto L25;
                }
                L50:
                ret_val += game.Objects.osize[i - 1];
                L100:
                ;
            }
            return ret_val;
        }
    }
}