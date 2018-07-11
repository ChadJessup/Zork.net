
namespace Zork.Core
{
    public static class dverb1
    {
        /// <summary>
        /// take_ - Take Sequence.
        /// Triggered for verbs: take, put, drop, read, etc.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="tellUser">Tells user that something was taken.</param>
        /// <returns>true is taken, false otherwise</returns>
        public static bool TakeParsedObject(Game game, bool tellUser)
        {
            // System generated locals
            int i__1;
            bool ret_val;

            ObjectIds x;

            // !ASSUME LOSES.
            ret_val = false;

            // !GET OBJECT ACTION.
            Object oa = game.Objects[game.ParserVectors.DirectObject];

            // !STAR?
            if (game.ParserVectors.DirectObject <= (ObjectIds)game.Star.strbit)
            {
                goto L100;
            }

            // !YES, LET IT HANDLE.
            ret_val = ObjectHandler.ApplyObjectsFromParseVector(game);
            return ret_val;

            L100:
            // !INSIDE?
            x = game.Objects[game.ParserVectors.DirectObject].Container;

            // !HIS VEHICLE?
            if (game.ParserVectors.DirectObject != (ObjectIds)game.Adventurers[game.Player.Winner].VehicleId)
            {
                goto L400;
            }

            // !DUMMY.
            MessageHandler.rspeak_(game, 672);
            return ret_val;

            L400:
            if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.IsTakeable) != 0)
            {
                goto L500;
            }

            if (!ObjectHandler.DoObjectSpecialAction(oa, 0, game))
            {
                i__1 = game.rnd_(5) + 552;
                MessageHandler.rspeak_(game, i__1);
            }

            return ret_val;

            // OBJECT IS TAKEABLE AND IN POSITION TO BE TAKEN.

            L500:
            if (x != 0 || ObjectHandler.IsObjectInRoom(game.ParserVectors.DirectObject, game.Player.Here, game))
            {
                goto L600;
            }

            // !ALREADY GOT IT?
            if (game.Objects[game.ParserVectors.DirectObject].Adventurer == game.Player.Winner)
            {
                MessageHandler.rspeak_(game, 557);
            }

            return ret_val;

            L600:
            // !TOO MUCH WEIGHT.
            if (x != 0 && game.Objects[x].Adventurer == game.Player.Winner || ObjectHandler.GetWeight(0, game.ParserVectors.DirectObject, game.Player.Winner, game) + game.Objects[game.ParserVectors.DirectObject].Size <= game.State.MaxLoad)
            {
                goto L700;
            }

            MessageHandler.rspeak_(game, 558);
            return ret_val;

            L700:
            // !AT LAST.
            ret_val = true;
            // !DID IT HANDLE?
            if (ObjectHandler.DoObjectSpecialAction(oa, 0, game))
            {
                return ret_val;
            }

            // !TAKE OBJECT FOR WINNER.
            ObjectHandler.SetNewObjectStatus(game.ParserVectors.DirectObject, 0, 0, 0, game.Player.Winner, game);
            game.Objects[game.ParserVectors.DirectObject].Flag2 |= ObjectFlags2.WasTouched;

            // !UPDATE SCORE.
            AdventurerHandler.ScoreUpdate(game, game.Objects[game.ParserVectors.DirectObject].Value);

            // !CANT BE SCORED AGAIN.
            game.Objects[game.ParserVectors.DirectObject].Value = 0;

            // !TELL TAKEN.
            if (tellUser)
            {
                MessageHandler.rspeak_(game, 559);
            }

            return ret_val;
        }

        // DROP- DROP VERB PROCESSOR
        public static bool drop_(Game game, bool z)
        {
            // System generated locals
            bool ret_val;

            // Local variables
            bool f;
            ObjectIds i, x;

            ret_val = true;
            // !ASSUME WINS.
            x = game.Objects[game.ParserVectors.DirectObject].Container;
            // !GET CONTAINER.
            if (x == 0)
            {
                goto L200;
            }
            // !IS IT INSIDE?
            if (game.Objects[x].Adventurer != game.Player.Winner)
            {
                goto L1000;
            }
            // !IS HE CARRYING CON?
            if ((game.Objects[x].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L300;
            }
            MessageHandler.rspsub_(game, 525, game.Objects[x].Description2Id);
            // !CANT REACH.
            return ret_val;

            L200:
            if (game.Objects[game.ParserVectors.DirectObject].Adventurer != game.Player.Winner)
            {
                goto L1000;
            }
            // !IS HE CARRYING OBJ?
            L300:
            if (game.Adventurers[game.Player.Winner].VehicleId == 0)
            {
                goto L400;
            }
            // !IS HE IN VEHICLE?
            game.ParserVectors.IndirectObject = (ObjectIds)game.Adventurers[game.Player.Winner].VehicleId;
            // !YES,
            f = put_(game, true);
            // !DROP INTO VEHICLE.
            game.ParserVectors.IndirectObject = 0;
            // !DISARM PARSER.
            return ret_val;
            // !DONE.

            L400:
            ObjectHandler.SetNewObjectStatus(game.ParserVectors.DirectObject, 0, game.Player.Here, 0, 0, game);
            // !DROP INTO ROOM.

            if (game.Player.Here == RoomIds.mtree)
            {
                ObjectHandler.SetNewObjectStatus(game.ParserVectors.DirectObject, 0, RoomIds.Forest3, 0, 0, game);
            }

            // !SCORE OBJECT.
            AdventurerHandler.ScoreUpdate(game, game.Objects[game.ParserVectors.DirectObject].Value);
            // !CANT BE SCORED AGAIN.
            game.Objects[game.ParserVectors.DirectObject].Value = 0;
            game.Objects[game.ParserVectors.DirectObject].Flag2 |= ObjectFlags2.WasTouched;

            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }
            // !DID IT HANDLE?
            i = 0;
            // !ASSUME NOTHING TO SAY.
            if (game.ParserVectors.prsa == VerbIds.Drop)
            {
                i = (ObjectIds)528;
            }

            if (game.ParserVectors.prsa == VerbIds.Throw)
            {
                i = (ObjectIds)529;
            }
            if (i != 0 && game.Player.Here == RoomIds.mtree)
            {
                i = (ObjectIds)659;
            }
            MessageHandler.rspsub_(game, (int)i, game.Objects[game.ParserVectors.DirectObject].Description2Id);
            return ret_val;

            L1000:
            MessageHandler.rspeak_(game, 527);
            // !DONT HAVE IT.
            return ret_val;

        } // drop_

        // PUT- PUT VERB PROCESSOR
        public static bool put_(Game game, bool flg)
        {
            // System generated locals
            bool ret_val;

            // Local variables
            ObjectIds j;
            ObjectIds svi, svo;

            ret_val = false;
            if (game.ParserVectors.DirectObject <= (ObjectIds)game.Star.strbit &&
                game.ParserVectors.IndirectObject <= (ObjectIds)game.Star.strbit)
            {
                goto L200;
            }
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.rspeak_(game, 560);
            }
            // !STAR
            ret_val = true;
            return ret_val;

            L200:
            if ((game.Objects[game.ParserVectors.IndirectObject].Flag2 & ObjectFlags2.IsOpen) != 0 ||
                (game.Objects[game.ParserVectors.IndirectObject].Flag1 & (int)ObjectFlags.IsDoor + ObjectFlags.CONTBT) != 0 ||
                (game.Objects[game.ParserVectors.IndirectObject].Flag2 & ObjectFlags2.IsVehicle) != 0)
            {
                goto L300;
            }

            MessageHandler.rspeak_(game, 561);
            // !CANT PUT IN THAT.
            return ret_val;

            L300:
            if ((game.Objects[game.ParserVectors.IndirectObject].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L400;
            }
            // !IS IT OPEN?
            MessageHandler.rspeak_(game, 562);
            // !NO, JOKE
            return ret_val;

            L400:
            if (game.ParserVectors.DirectObject != game.ParserVectors.IndirectObject)
            {
                goto L500;
            }
            // !INTO ITSELF?
            MessageHandler.rspeak_(game, 563);
            // !YES, JOKE.
            return ret_val;

            L500:
            if (game.Objects[game.ParserVectors.DirectObject].Container != game.ParserVectors.IndirectObject)
            {
                goto L600;
            }
            // !ALREADY INSIDE.
            MessageHandler.rspsb2_(game, 564, game.Objects[game.ParserVectors.DirectObject].Description2Id, game.Objects[game.ParserVectors.IndirectObject].Description2Id);
            ret_val = true;
            return ret_val;

            L600:
            // TODO: chadj - commenting these out, not sure if needed.
            // if (ObjectHandler.GetWeight(0, game.ParserVectors.DirectObject, 0, game)
            //    + ObjectHandler.GetWeight(0, game.ParserVectors.IndirectObject, 0, game)
            if (game.Objects[game.ParserVectors.DirectObject].Size <= game.Objects[game.ParserVectors.IndirectObject].Capacity)
            {
                goto L700;
            }

            MessageHandler.rspeak_(game, 565);
            // !THEN CANT DO IT.
            return ret_val;

            // NOW SEE IF OBJECT (OR ITS CONTAINER) IS IN ROOM

            L700:
            j = game.ParserVectors.DirectObject;
            // !START SEARCH.
            L725:
            if (ObjectHandler.IsObjectInRoom(j, game.Player.Here, game))
            {
                goto L750;
            }

            // !IS IT HERE?
            j = game.Objects[j].Container;
            if (j != 0)
            {
                goto L725;
            }
            // !MORE TO DO?
            goto L800;
            // !NO, SCH FAILS.

            L750:
            // !SAVE PARSER.
            svo = game.ParserVectors.DirectObject;
            svi = game.ParserVectors.IndirectObject;

            game.ParserVectors.prsa = VerbIds.Take;
            game.ParserVectors.IndirectObject = 0;

            // !TAKE OBJECT.
            if (!TakeParsedObject(game, false))
            {
                return ret_val;
            }

            game.ParserVectors.prsa = VerbIds.Put;
            game.ParserVectors.DirectObject = svo;
            game.ParserVectors.IndirectObject = svi;
            goto L1000;

            // NOW SEE IF OBJECT IS ON PERSON.

            L800:
            if (game.Objects[game.ParserVectors.DirectObject].Container == 0)
            {
                goto L1000;
            }

            // !INSIDE?
            if ((game.Objects[game.Objects[game.ParserVectors.DirectObject].Container].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L900;
            }

            // !OPEN?
            MessageHandler.rspsub_(game, 566, game.Objects[game.ParserVectors.DirectObject].Description2Id);
            // !LOSE.
            return ret_val;

            L900:
            AdventurerHandler.ScoreUpdate(game, game.Objects[game.ParserVectors.DirectObject].Value);
            // !SCORE OBJECT.
            game.Objects[game.ParserVectors.DirectObject].Value = 0;
            game.Objects[game.ParserVectors.DirectObject].Flag2 |= ObjectFlags2.WasTouched;
            ObjectHandler.SetNewObjectStatus(game.ParserVectors.DirectObject, 0, 0, 0, game.Player.Winner, game);
            // !TEMPORARILY ON WINNER.

            L1000:
            // !NO, GIVE OBJECT A SHOT.
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }

            // !CONTAINED INSIDE.
            ObjectHandler.SetNewObjectStatus(game.ParserVectors.DirectObject, 2, 0, game.ParserVectors.IndirectObject, 0, game);

            ret_val = true;
            return ret_val;
        }

        // VALUAC- HANDLES VALUABLES/EVERYTHING
        public static void valuac_(Game game, int v)
        {
            // System generated locals
            int i__1;

            // Local variables
            bool f;
            int i;
            bool f1;
            int savep, saveh;

            // !ASSUME NO ACTIONS.
            f = true;
            // !ASSUME NOT LIT.
            i = 579;
            // !IF NOT LIT, PUNT.
            if (!RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                goto L4000;
            }
            // !ASSUME WRONG VERB.
            i = 677;
            // !SAVE PRSO.
            savep = (int)game.ParserVectors.DirectObject;
            // !SAVE HERE.
            saveh = (int)game.Player.Here;

            // L100:
            // !TAKE EVERY/VALUA?
            if (game.ParserVectors.prsa != VerbIds.Take)
            {
                goto L1000;
            }

            i__1 = game.Objects.Count;
            for (game.ParserVectors.DirectObject = (ObjectIds)1; game.ParserVectors.DirectObject <= (ObjectIds)i__1; ++game.ParserVectors.DirectObject)
            {
                // !LOOP THRU OBJECTS.
                if (!ObjectHandler.IsObjectInRoom(game.ParserVectors.DirectObject, game.Player.Here, game)
                    || (game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.IsVisible) == 0
                    || (game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsActor) != 0
                    || savep == v && game.Objects[game.ParserVectors.DirectObject].otval <= 0)
                {
                    goto L500;
                }

                if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.IsTakeable) == 0 &&
                    (game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.CanTry) == 0)
                {
                    goto L500;
                }

                f = false;
                MessageHandler.rspsub_(game, 580, game.Objects[game.ParserVectors.DirectObject].Description2Id);

                f1 = TakeParsedObject(game, true);

                if (saveh != (int)game.Player.Here)
                {
                    return;
                }

                L500:
                ;
            }
            goto L3000;

            L1000:
            // !DROP EVERY/VALUA?
            if (game.ParserVectors.prsa != VerbIds.Drop)
            {
                goto L2000;
            }

            i__1 = game.Objects.Count;
            for (game.ParserVectors.DirectObject = (ObjectIds)1; game.ParserVectors.DirectObject <= (ObjectIds)i__1; ++game.ParserVectors.DirectObject)
            {
                if (game.Objects[game.ParserVectors.DirectObject].Adventurer != game.Player.Winner || savep == v
                    && game.Objects[game.ParserVectors.DirectObject].otval <= 0)
                {
                    goto L1500;
                }

                f = false;
                MessageHandler.rspsub_(game, 580, game.Objects[game.ParserVectors.DirectObject].Description2Id);
                f1 = drop_(game, true);

                if (saveh != (int)game.Player.Here)
                {
                    return;
                }
                L1500:
                ;
            }
            goto L3000;

            L2000:
            if (game.ParserVectors.prsa != VerbIds.Put)
            {
                goto L3000;
            }
            // !PUT EVERY/VALUA?
            i__1 = game.Objects.Count;
            for (game.ParserVectors.DirectObject = (ObjectIds)1; game.ParserVectors.DirectObject <= (ObjectIds)i__1; ++game.ParserVectors.DirectObject)
            {
                // !LOOP THRU OBJECTS.
                if (game.Objects[game.ParserVectors.DirectObject].Adventurer != game.Player.Winner
                    || game.ParserVectors.DirectObject == game.ParserVectors.IndirectObject
                    || savep == v
                    && game.Objects[game.ParserVectors.DirectObject].otval <= 0
                    || (game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.IsVisible) == 0)
                {
                    goto L2500;
                }

                f = false;
                MessageHandler.rspsub_(game, 580, game.Objects[game.ParserVectors.DirectObject].Description2Id);
                f1 = put_(game, true);
                if (saveh != (int)game.Player.Here)
                {
                    return;
                }
                L2500:
                ;
            }

            L3000:
            i = 581;
            if (savep == v)
            {
                i = 582;
            }

            // !CHOOSE MESSAGE.
            L4000:
            if (f)
            {
                MessageHandler.rspeak_(game, i);
            }
            // !IF NOTHING, REPORT.
        }
    }
}