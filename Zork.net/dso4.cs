using System;

namespace Zork.Core
{
    public static class dso4
    {
        /// <summary>
        /// robadv_ - Steal Winner's Valuables
        /// </summary>
        /// <param name="game"></param>
        /// <param name="actorId"></param>
        /// <param name="nr"></param>
        /// <param name="nc"></param>
        /// <param name="na"></param>
        /// <returns></returns>
        public static int RobAdventurer(Game game, ActorIds actorId, RoomIds nr, ObjectIds nc, ActorIds na)
        {
            int ret_val;
            ObjectIds i;

            ret_val = 0;
            // !COUNT OBJECTS
            for (i = (ObjectIds)1; i <= (ObjectIds)game.Objects.Count; ++i)
            {
                if (game.Objects[i].Adventurer != actorId ||
                    game.Objects[i].otval <= 0 ||
                    game.Objects[i].Flag2.HasFlag(ObjectFlags2.SCRDBT))
                {
                    goto L100;
                }

                // !STEAL OBJECT
                ObjectHandler.SetNewObjectStatus(i, 0, game.Rooms[nr], nc, na, game);
                ++ret_val;
                L100:
                ;
            }
            return ret_val;
        }

        /// <summary>
        /// robrm_ - Steal Room's Valuables
        /// </summary>
        /// <param name="game"></param>
        /// <param name="rm"></param>
        /// <param name="pr"></param>
        /// <param name="nr"></param>
        /// <param name="nc"></param>
        /// <param name="na"></param>
        /// <returns></returns>
        public static int RobRoom(Game game, RoomIds rm, int pr, RoomIds nr, ObjectIds nc, int na)
        {
            int ret_val, i__2;
            ObjectIds i;

            // OBJECTS
            ret_val = 0;
            for (i = (ObjectIds)1; i <= (ObjectIds)game.Objects.Count; ++i)
            {
                // !LOOP ON OBJECTS.
                if (!ObjectHandler.IsObjectInRoom(i, rm, game))
                {
                    goto L100;
                }

                if (game.Objects[i].otval <= 0 ||
                    game.Objects[i].Flag2.HasFlag(ObjectFlags2.SCRDBT) ||
                    (game.Objects[i].Flag1 & ObjectFlags.IsVisible) == 0 || !RoomHandler.prob_(game, pr, pr))
                {
                    goto L50;
                }

                ObjectHandler.SetNewObjectStatus(i, 0, nr, nc, (ActorIds)na, game);
                ++ret_val;
                game.Objects[i].Flag2 |= ObjectFlags2.WasTouched;
                goto L100;

                L50:
                if (game.Objects[i].Flag2.HasFlag(ObjectFlags2.IsActor))
                {
                    i__2 = (int)ObjectHandler.GetActor(i, game);
                    ret_val += RobAdventurer(game, (ActorIds)i__2, nr, nc, (ActorIds)na);
                }
                L100:
                ;
            }

            return ret_val;
        }

        /// <summary>
        /// winnin_ - See if Villian is winning
        /// </summary>
        /// <param name="game"></param>
        /// <param name="vl"></param>
        /// <param name="hr"></param>
        /// <returns></returns>
        public static bool IsVillianWinning(Game game, ObjectIds vl, ActorIds hr)
        {
            bool ret_val;
            int ps;

            // OBJECTS
            int villianStrength = game.Objects[vl].Capacity;

            // !VILLAIN STRENGTH
            ps = villianStrength - ComputeFightStrength(game, hr, true);

            // !HIS MARGIN OVER HERO
            ret_val = RoomHandler.prob_(game, 90, 100);
            if (ps > 3)
            {
                return ret_val;
            }

            // !+3... 90% WINNING
            ret_val = RoomHandler.prob_(game, 75, 85);
            if (ps > 0)
            {
                return ret_val;
            }

            // !>0... 75% WINNING
            ret_val = RoomHandler.prob_(game, 50, 30);
            if (ps == 0)
            {
                return ret_val;
            }

            // !=0... 50% WINNING
            ret_val = RoomHandler.prob_(game, 25, 25);
            if (villianStrength > 1)
            {
                return ret_val;
            }

            // !ANY VILLAIN STRENGTH.
            ret_val = RoomHandler.prob_(game, 10, 0);
            return ret_val;
        }

        /// <summary>
        /// fights_ - Compute Fight Strength
        /// </summary>
        /// <param name="game"></param>
        /// <param name="actorId"></param>
        /// <param name="flg"></param>
        /// <returns></returns>
        public static int ComputeFightStrength(Game game, ActorIds actorId, bool flg)
        {
            const int smin = 2;
            const int smax = 7;

            // System generated locals
            int ret_val;

            ret_val = smin + ((smax - smin) * game.Adventurers[actorId].Score + game.State.MaxScore / 2) / game.State.MaxScore;
            if (flg)
            {
                ret_val += game.Adventurers[actorId].Strength;
            }

            return ret_val;
        }

        /// <summary>
        /// vilstr_ - Compute Villain Strength
        /// </summary>
        /// <param name="game"></param>
        /// <param name="villianId"></param>
        /// <returns></returns>
        public static int ComputeVillianStrength(Game game, ObjectIds villianId)
        {
            int ret_val = game.Objects[villianId].Capacity;
            if (ret_val <= 0)
            {
                return ret_val;
            }

            if (villianId != ObjectIds.Thief || !game.Flags.IsThiefEngrossed)
            {
                goto L100;
            }

            // !THIEF UNENGROSSED.
            game.Flags.IsThiefEngrossed = false;

            // !NO BETTER THAN 2.
            ret_val = Math.Min(ret_val, 2);

            L100:
            foreach (var villian in game.Villians.Values)
            {
                // !SEE IF  BEST WEAPON.
                if (villian.Id == villianId && game.ParserVectors.IndirectObject == villian.BestWeapon)
                {
                    // Computing MAX
                    ret_val = Math.Max(1, ret_val - 1);
                }
                // L200:
            }

            return ret_val;
        }
    }
}