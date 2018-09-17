using System;

namespace Zork.Core
{
    public static class dverb2
    {
        // WALK- MOVE IN SPECIFIED DIRECTION
        public static bool walk_(Game game)
        {
            bool ret_val = true;

            // !ASSUME WINS.
            if (game.Player.Winner != ActorIds.Player ||
                RoomHandler.IsRoomLit(game.Player.Here, game) ||
                RoomHandler.prob_(game, 25, 25))
            {
                goto L500;
            }

            if (!dso3.FindExit(game, (int)game.ParserVectors.DirectObject, game.Player.Here))
            {
                goto L450;
            }
            // !INVALID EXIT? GRUE
            // !
            switch (game.CurrentExit.ExitType)
            {
                case 1: goto L400;
                case 2: goto L200;
                case 3: goto L100;
                case 4: goto L300;
            }
            // !DECODE EXIT TYPE.
            throw new InvalidOperationException();
            //bug_(9, game.curxt_.xtype);

            L100:
            if (HandleConditionalExit(game, game.CurrentExit.Action) != 0)
            {
                goto L400;
            }
            // !CEXIT... RETURNED ROOM?
            // TODO: chadj: figure out how to do this
            //if (game.Flags[game.curxt_.xobj - 1])
            {
              //  goto L400;
            }
            // !NO, FLAG ON?
            L200:
            // !BAD EXIT, GRUE
            AdventurerHandler.jigsup_(game, 523);
            // !
            return ret_val;

            L300:
            // !DOOR... RETURNED ROOM?
            if (HandleConditionalExit(game, game.CurrentExit.Action) != 0)
            {
                goto L400;
            }
            // !NO, DOOR OPEN?
            if ((game.Objects[game.CurrentExit.Object].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L400;
            }

            // !BAD EXIT, GRUE
            AdventurerHandler.jigsup_(game, 523);
            // !
            return ret_val;

            L400:
            // !VALID ROOM, IS IT LIT?
            if (RoomHandler.IsRoomLit(game.CurrentExit.xroom1, game))
            {
                goto MOVETOROOM;
            }
            L450:
            // !NO, GRUE
            // !
            AdventurerHandler.jigsup_(game, 522);
            return ret_val;

            // ROOM IS LIT, OR WINNER IS NOT PLAYER (NO GRUE).

            L500:
            // !EXIT EXIST?
            if (dso3.FindExit(game, (int)game.ParserVectors.DirectObject, game.Player.Here))
            {
                goto L550;
            }

            L525:
            game.CurrentExit.xstrng = 678;
            // !ASSUME WALL.
            if (game.ParserVectors.DirectObject == (ObjectIds)XSearch.xup)
            {
                game.CurrentExit.xstrng = 679;
            }
            // !IF UP, CANT.
            if (game.ParserVectors.DirectObject == (ObjectIds)XSearch.xdown)
            {
                game.CurrentExit.xstrng = 680;
            }
            // !IF DOWN, CANT.
            if ((game.Rooms[game.Player.Here].Flags & RoomFlags.NOWALL) != 0)
            {
                game.CurrentExit.xstrng = 524;
            }

            MessageHandler.Speak(game.CurrentExit.xstrng, game);
            game.ParserVectors.prscon = 1;
            // !STOP CMD STREAM.
            return ret_val;

            L550:
            switch (game.CurrentExit.ExitType)
            {
                case 1: goto MOVETOROOM;
                case 2: goto L600;
                case 3: goto L700;
                case 4: goto CONDITIONALEXIT;
            }

            // !BRANCH ON EXIT TYPE.
            throw new InvalidOperationException();
            //bug_(9, curxt_.xtype);

            L700:
            if (HandleConditionalExit(game, game.CurrentExit.Action) != 0)
            {
                goto MOVETOROOM;
            }
            // !CEXIT... RETURNED ROOM?
            // TODO: chadj figure this out

            //if (game.Flags[game.curxt_.xobj - 1])
            {
              //  goto L900;
            }

            // !NO, FLAG ON?
            L600:
            if (game.CurrentExit.xstrng == 0)
            {
                goto L525;
            }

            // !IF NO REASON, USE STD.
            MessageHandler.Speak(game.CurrentExit.xstrng, game);
            // !DENY EXIT.
            game.ParserVectors.prscon = 1;
            // !STOP CMD STREAM.
            return ret_val;

            CONDITIONALEXIT:
            if (HandleConditionalExit(game, game.CurrentExit.Action) != 0)
            {
                goto MOVETOROOM;
            }

            // !DOOR... RETURNED ROOM?
            if ((game.Objects[game.CurrentExit.Object].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto MOVETOROOM;
            }

            // !NO, DOOR OPEN?
            if (game.CurrentExit.xstrng == 0)
            {
                game.CurrentExit.xstrng = 525;
            }

            // !IF NO REASON, USE STD.
            MessageHandler.Speak(game.CurrentExit.xstrng, game.Objects[game.CurrentExit.Object].ShortDescription, game);
            game.ParserVectors.prscon = 1;
            // !STOP CMD STREAM.
            return ret_val;

            MOVETOROOM:
            ret_val = AdventurerHandler.MoveToNewRoom(game, game.CurrentExit.xroom1, game.Player.Winner);
            // !MOVE TO ROOM.
            if (ret_val)
            {
                ret_val = RoomHandler.RoomDescription(Verbosity.RoomAndContents, game);
            }

            return ret_val;
        }

        // CXAPPL- CONDITIONAL EXIT PROCESSORS
        public static int HandleConditionalExit(Game game, int ri)
        {
            int ret_val = 0;
            ObjectIds i, k;
            int nxt, j;
            int ldir;

            // !NO RETURN.
            if (ri == 0)
            {
                return ret_val;
            }

            // !IF NO ACTION, DONE.
            switch (ri)
            {
                case 1: goto COFFIN;
                case 2: goto CAROUSEL;
                case 3: goto CHIMNEY;
                case 4: goto MAGNETROOM;
                case 5: goto RANDOMEXIT;
                case 6: goto CAROUSELOFFEXIT;
                case 7: goto BANKALARM;
                case 8: goto FROBOZZFLAG;
                case 9: goto FROBOZZFLAGMIRIN;
                case 10: goto FROBOZZFLAGGMIRROREXIT;
                case 11: goto MAYBEDOOR;
                case 12: goto PUZZLEROOMMAINENTRANCE;
                case 13: goto PUZZLEROOMSIZEENTRANCE;
                case 14: goto PUZZLEROOMTRANSITIONS;
            }

            throw new InvalidOperationException();
            //bug_(5, ri);

            // C1- COFFIN-CURE

            COFFIN:
            game.Flags.egyptf = game.Objects[ObjectIds.Coffin].Adventurer != game.Player.Winner;
            // !T IF NO COFFIN.
            return ret_val;

            // C2- CAROUSEL EXIT
            // C5- CAROUSEL OUT

            CAROUSEL:
            if (game.Flags.IsCarouselOff)
            {
                return ret_val;
            }

            // !IF FLIPPED, NOTHING.
            L2500:
            // !SPIN THE COMPASS.
            MessageHandler.Speak(121, game);

            RANDOMEXIT:
            // !CHOOSE RANDOM EXIT.
            i = (ObjectIds)(xpars_.xelnt[xpars_.xcond - 1] * game.rnd_(8));
            game.CurrentExit.xroom1 = (RoomIds)(int)(game.Exits.Travel[game.Rooms[game.Player.Here].Exit + (int)i - 1] & xpars_.ExitRoomMask);
            // !RETURN EXIT.
            ret_val = (int)game.CurrentExit.xroom1;
            return ret_val;

            // C3- CHIMNEY FUNCTION

            CHIMNEY:
            game.Flags.litldf = false;
            // !ASSUME HEAVY LOAD.
            j = 0;
            for (i = (ObjectIds)1; i <= (ObjectIds)game.Objects.Count; ++i)
            {
                // !COUNT OBJECTS.
                if (game.Objects[i].Adventurer == game.Player.Winner)
                {
                    ++j;
                }
                // L3100:
            }

            if (j > 2)
            {
                return ret_val;
            }

            // !CARRYING TOO MUCH?
            game.CurrentExit.xstrng = 446;
            // !ASSUME NO LAMP.
            if (game.Objects[ObjectIds.Lamp].Adventurer != game.Player.Winner)
            {
                return ret_val;
            }

            // !NO LAMP?
            game.Flags.litldf = true;
            // !HE CAN DO IT.
            if (!game.Objects[ObjectIds.TrapDoor].Flag2.HasFlag(ObjectFlags2.IsOpen))
            {
                game.Objects[ObjectIds.TrapDoor].Flag2 &= ~ObjectFlags2.WasTouched;
            }

            return ret_val;

            // C4-	FROBOZZ FLAG (MAGNET ROOM, FAKE EXIT)
            // C6-	FROBOZZ FLAG (MAGNET ROOM, REAL EXIT)

            MAGNETROOM:
            if (game.Flags.IsCarouselOff)
            {
                goto L2500;
            }

            // !IF FLIPPED, GO SPIN.
            game.Flags.frobzf = false;
            // !OTHERWISE, NOT AN EXIT.
            return ret_val;

            CAROUSELOFFEXIT:
            if (game.Flags.IsCarouselOff)
            {
                goto L2500;
            }

            // !IF FLIPPED, GO SPIN.
            game.Flags.frobzf = true;
            // !OTHERWISE, AN EXIT.
            return ret_val;

            // C7-	FROBOZZ FLAG (BANK ALARM)

            BANKALARM:
            game.Flags.frobzf = RoomHandler.GetRoomThatContainsObject(ObjectIds.bills, game).Id != 0 & RoomHandler.GetRoomThatContainsObject(ObjectIds.portr, game).Id != 0;

            return ret_val;
            // CXAPPL, PAGE 3

            // C8-	FROBOZZ FLAG (MRGO)

            FROBOZZFLAG:
            game.Flags.frobzf = false;
            // !ASSUME CANT MOVE.
            if (game.Switches.mloc != game.CurrentExit.xroom1) {
                goto L8100;
            }
            // !MIRROR IN WAY?
            if (game.ParserVectors.DirectObject == (ObjectIds)XSearch.xnorth || game.ParserVectors.DirectObject == (ObjectIds)XSearch.xsouth)
            {
                goto L8200;
            }

            if (game.Switches.mdir % 180 != 0)
            {
                goto L8300;
            }

            // !MIRROR MUST BE N-S.
            game.CurrentExit.xroom1 = (game.CurrentExit.xroom1 - RoomIds.mra << 1) + RoomIds.mrae;
            // !CALC EAST ROOM.
            if (game.ParserVectors.DirectObject > (ObjectIds)XSearch.xsouth)
            {
                ++game.CurrentExit.xroom1;
            }

            // !IF SW/NW, CALC WEST.
            L8100:
            return (int)game.CurrentExit.xroom1;

            L8200:
            game.CurrentExit.xstrng = 814;
            // !ASSUME STRUC BLOCKS.
            if (game.Switches.mdir % 180 == 0)
            {
                return ret_val;
            }

            // !IF MIRROR N-S, DONE.
            L8300:
            ldir = game.Switches.mdir;
            // !SEE WHICH MIRROR.
            if (game.ParserVectors.DirectObject == (ObjectIds)XSearch.xsouth)
            {
                ldir = 180;
            }

            game.CurrentExit.xstrng = 815;
            // !MIRROR BLOCKS.
            if (ldir > 180 && !game.Flags.mr1f || ldir < 180 && !game.Flags.mr2f)
            {
                game.CurrentExit.xstrng = 816;
            }

            return ret_val;

            // C9-	FROBOZZ FLAG (MIRIN)

            FROBOZZFLAGMIRIN:
            if (RoomHandler.IsMirrorHere(game, game.Player.Here) != 1)
            {
                goto L9100;
            }

            // !MIRROR 1 HERE?
            if (game.Flags.mr1f)
            {
                game.CurrentExit.xstrng = 805;
            }

            // !SEE IF BROKEN.
            game.Flags.frobzf = game.Flags.mropnf;
            // !ENTER IF OPEN.
            return ret_val;

            L9100:
            game.Flags.frobzf = false;
            // !NOT HERE,
            game.CurrentExit.xstrng = 817;
            // !LOSE.
            return ret_val;
            // CXAPPL, PAGE 4

            // C10-	FROBOZZ FLAG (MIRROR EXIT)

            FROBOZZFLAGGMIRROREXIT:
            game.Flags.frobzf = false;
            // !ASSUME CANT.
            ldir = game.ParserVectors.DirectObject - (ObjectIds)((int)XSearch.xnorth / (int)XSearch.xnorth * 45);
            // !XLATE DIR TO DEGREES.
            if (!game.Flags.mropnf || (game.Switches.mdir + 270) % 360 != ldir && game.ParserVectors.DirectObject != (ObjectIds)XSearch.xexit)
            {
                goto L10200;
            }

            game.CurrentExit.xroom1 = (game.Switches.mloc - RoomIds.mra << 1) + RoomIds.mrae + 1 - game.Switches.mdir / 180;

            // !ASSUME E-W EXIT.
            if (game.Switches.mdir % 180 == 0)
            {
                goto L10100;
            }

            // !IF N-S, OK.
            game.CurrentExit.xroom1 = game.Switches.mloc + 1;
            // !ASSUME N EXIT.
            if (game.Switches.mdir > 180)
            {
                game.CurrentExit.xroom1 = game.Switches.mloc - 1;
            }

            // !IF SOUTH.
            L10100:
            return (int)game.CurrentExit.xroom1;

            L10200:
            if (!game.Flags.wdopnf || (game.Switches.mdir + 180) % 360 != ldir && game.ParserVectors.DirectObject != (ObjectIds)XSearch.xexit)
            {
                return ret_val;
            }

            game.CurrentExit.xroom1 = game.Switches.mloc + 1;
            // !ASSUME N.
            if (game.Switches.mdir == 0)
            {
                game.CurrentExit.xroom1 = game.Switches.mloc - 1;
            }

            // !IF S.
            MessageHandler.Speak(818, game);
            // !CLOSE DOOR.
            game.Flags.wdopnf = false;
            return (int)game.CurrentExit.xroom1;

            // C11-	MAYBE DOOR.  NORMAL MESSAGE IS THAT DOOR IS CLOSED.
            // 	BUT IF LCELL.NE.4, DOOR ISNT THERE.

            MAYBEDOOR:
            if (game.Switches.LeftCell != 4)
            {
                game.CurrentExit.xstrng = 678;
            }

            // !SET UP MSG.
            return ret_val;

            // C12-	FROBZF (PUZZLE ROOM MAIN ENTRANCE)

            PUZZLEROOMMAINENTRANCE:
            game.Flags.frobzf = true;
            // !ALWAYS ENTER.
            game.Switches.cphere = 10;
            // !SET SUBSTATE.
            return ret_val;

            // C13-	CPOUTF (PUZZLE ROOM SIZE ENTRANCE)

            PUZZLEROOMSIZEENTRANCE:
            game.Switches.cphere = 52;
            // !SET SUBSTATE.
            return ret_val;
            // CXAPPL, PAGE 5

            // C14-	FROBZF (PUZZLE ROOM TRANSITIONS)

            PUZZLEROOMTRANSITIONS:
            game.Flags.frobzf = false;
            // !ASSSUME LOSE.
            if (game.ParserVectors.DirectObject != (ObjectIds)XSearch.xup)
            {
                goto L14100;
            }

            // !UP?
            if (game.Switches.cphere != 10)
            {
                return ret_val;
            }

            // !AT EXIT?
            game.CurrentExit.xstrng = 881;
            // !ASSUME NO LADDER.
            if (PuzzleHandler.cpvec[game.Switches.cphere] != -2)
            {
                return ret_val;
            }

            // !LADDER HERE?
            MessageHandler.Speak(882, game);
            // !YOU WIN.
            game.Flags.frobzf = true;
            // !LET HIM OUT.
            return ret_val;

            L14100:
            if (game.Switches.cphere != 52 || game.ParserVectors.DirectObject != (ObjectIds)XSearch.xwest || !game.Flags.cpoutf)
            {
                goto L14200;
            }

            game.Flags.frobzf = true;
            // !YES, LET HIM OUT.
            return ret_val;

            L14200:
            for (i = (ObjectIds)1; i <= (ObjectIds)16; i += 2)
            {
                // !LOCATE EXIT.
                if (game.ParserVectors.DirectObject == (ObjectIds)PuzzleHandler.cpdr[(int)i - 1])
                {
                    goto L14400;
                }
                // L14300:
            }

            return ret_val;
            // !NO SUCH EXIT.

            L14400:
            j = PuzzleHandler.cpdr[(int)i];
            // !GET DIRECTIONAL OFFSET.
            nxt = game.Switches.cphere + j;
            // !GET NEXT STATE.
            k = (ObjectIds)8;
            // !GET ORTHOGONAL DIR.
            if (j < 0)
            {
                k = (ObjectIds)(-8);
            }

            if ((Math.Abs(j) == 1 || Math.Abs(j) == 8 || (PuzzleHandler.cpvec[game.Switches.cphere + (int)k - 1] == 0 || PuzzleHandler.cpvec[nxt - (int)k - 1] == 0)) && PuzzleHandler.cpvec[nxt - 1] == 0)
            {
                goto L14500;
            }

            return ret_val;

            L14500:
            dso7.cpgoto_(game, nxt);
            // !MOVE TO STATE.
            game.CurrentExit.xroom1 = RoomIds.cpuzz;
            // !STAY IN ROOM.
            return (int)game.CurrentExit.xroom1;
        }
    }
}