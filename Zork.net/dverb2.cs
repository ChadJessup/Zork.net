using System;

namespace Zork.Core
{
    public static class dverb2
    {
        // SAVE- SAVE GAME STATE
        public static void savegm_(Game game)
        {
            game.ParserVectors.prswon = false;
            // !DISABLE GAME.
            // Note: save file format is different for PDP vs. non-PDP versions

            //if ((e = fopen("dsave.dat", BINWRITE)) == NULL)
            //    goto L100;

            //gttime_(&i);
            // !GET TIME.

//#define do_uio(i, zbuf, cbytes) \
            //(void)fwrite((const char*)(zbuf), (cbytes), (i), e)

            //do_uio(1, &vers_1.vmaj, sizeof(int));
            //do_uio(1, &vers_1.vmin, sizeof(int));
            //do_uio(1, &vers_1.vedit, sizeof(int));
            //
            //do_uio(1, &game.Player.Winner, sizeof(int));
            //do_uio(1, &game.Player.Here, sizeof(int));
            //do_uio(1, &game.Hack.thfpos, sizeof(int));
            //do_uio(1, &game.Player.TelFlag, sizeof(bool));
            //do_uio(1, &game.Hack.thfflg, sizeof(bool));
            //do_uio(1, &game.Hack.thfact, sizeof(bool));
            //do_uio(1, &game.Hack.swdact, sizeof(bool));
            //do_uio(1, &game.Hack.swdsta, sizeof(int));
            //do_uio(64, &PuzzleHandler.cpvec[0], sizeof(int));
            //
            //do_uio(1, &i, sizeof(int));
            //do_uio(1, &game.State.moves, sizeof(int));
            //do_uio(1, &game.State.Deaths, sizeof(int));
            //do_uio(1, &game.State.rwscor, sizeof(int));
            //do_uio(1, &game.State.egscor, sizeof(int));
            //do_uio(1, &game.State.mxload, sizeof(int));
            //do_uio(1, &game.State.ltshft, sizeof(int));
            //do_uio(1, &game.State.bloc, sizeof(int));
            //do_uio(1, &game.State.mungrm, sizeof(int));
            //do_uio(1, &game.State.hs, sizeof(int));
            //do_uio(1, &game.Screen.fromdr, sizeof(int));
            //do_uio(1, &game.Screen.scolrm, sizeof(int));
            //do_uio(1, &game.Screen.scolac, sizeof(int));
            //
            //do_uio(220, &game.Objects[0].odesc1, sizeof(int));
            //do_uio(220, &game.Objects[0].odesc2, sizeof(int));
            //do_uio(220, &game.Objects[0].oflag1, sizeof(int));
            //do_uio(220, &game.Objects[0].oflag2, sizeof(int));
            //do_uio(220, &game.Objects[0].ofval, sizeof(int));
            //do_uio(220, &game.Objects[0].otval, sizeof(int));
            //do_uio(220, &game.Objects[0].osize, sizeof(int));
            //do_uio(220, &game.Objects[0].ocapac, sizeof(int));
            //do_uio(220, &game.Objects[0].oroom, sizeof(int));
            //do_uio(220, &game.Objects[0].oadv, sizeof(int));
            //do_uio(220, &game.Objects[0].ocan, sizeof(int));
            //
            //do_uio(200, &game.NewRooms.rval[0], sizeof(int));
            //do_uio(200, &game.NewRooms.RoomFlags[0], sizeof(int));
            //
            //do_uio(4, &game.Adventurers.aroom[0], sizeof(int));
            //do_uio(4, &game.Adventurers.ascore[0], sizeof(int));
            //do_uio(4, &game.Adventurers.avehic[0], sizeof(int));
            //do_uio(4, &game.Adventurers.astren[0], sizeof(int));
            //do_uio(4, &game.Adventurers.aflag[0], sizeof(int));
            //
            //do_uio(46, &game.Flags[0], sizeof(bool));
            //do_uio(22, &switch_[0], sizeof(int));
            //do_uio(4, &game.Villians.vprob[0], sizeof(int));
            //do_uio(25, &game.Clock.Flags[0], sizeof(bool));
            //do_uio(25, &game.Clock.Ticks[0], sizeof(int));

            //if (fclose(e) == EOF)
            {
            //    goto L100;
            }

            MessageHandler.rspeak_(game, 597);
            return;

            L100:
            MessageHandler.rspeak_(game, 598);
            // !CANT DO IT.
        } // savegm_

        // RESTORE- RESTORE GAME STATE
        public static void rstrgm_(Game game)
        {
            game.ParserVectors.prswon = false;
            // !DISABLE GAME.
            // Note: save file format is different for PDP vs. non-PDP versions

//            if ((e = fopen("dsave.dat", BINREAD)) == NULL)
            {
//                goto L100;
            }

//#define do_uio(i, zbuf, cbytes) \
            //(void)fread((char*)(zbuf), (cbytes), (i), e)

            //do_uio(1, &i, sizeof(int));
            //do_uio(1, &j, sizeof(int));
            //do_uio(1, &k, sizeof(int));

            //if (i != vers_1.vmaj | j != vers_1.vmin)
            {
            //    goto L200;
            }

            //do_uio(1, game.Player.Winner, sizeof(int));
            //do_uio(1, game.Player.Here, sizeof(int));
            //do_uio(1, game.Hack.thfpos, sizeof(int));
            //do_uio(1, game.Player.TelFlag, sizeof(bool));
            //do_uio(1, game.Hack.thfflg, sizeof(bool));
            //do_uio(1, game.Hack.thfact, sizeof(bool));
            //do_uio(1, game.Hack.swdact, sizeof(bool));
            //do_uio(1, game.Hack.swdsta, sizeof(int));
            //do_uio(64, PuzzleHandler.cpvec[0], sizeof(int));
            //
            //do_uio(1, time_1.pltime, sizeof(int));
            //do_uio(1, game.State.moves, sizeof(int));
            //do_uio(1, game.State.Deaths, sizeof(int));
            //do_uio(1, game.State.rwscor, sizeof(int));
            //do_uio(1, game.State.egscor, sizeof(int));
            //do_uio(1, game.State.mxload, sizeof(int));
            //do_uio(1, game.State.ltshft, sizeof(int));
            //do_uio(1, game.State.bloc, sizeof(int));
            //do_uio(1, game.State.mungrm, sizeof(int));
            //do_uio(1, game.State.hs, sizeof(int));
            //do_uio(1, game.Screen.fromdr, sizeof(int));
            //do_uio(1, game.Screen.scolrm, sizeof(int));
            //do_uio(1, game.Screen.scolac, sizeof(int));
            //
            //do_uio(220, game.Objects[0].odesc1, sizeof(int));
            //do_uio(220, game.Objects[0].odesc2, sizeof(int));
            //do_uio(220, game.Objects[0].oflag1, sizeof(int));
            //do_uio(220, game.Objects[0].oflag2, sizeof(int));
            //do_uio(220, game.Objects[0].ofval, sizeof(int));
            //do_uio(220, game.Objects[0].otval, sizeof(int));
            //do_uio(220, game.Objects[0].osize, sizeof(int));
            //do_uio(220, game.Objects[0].ocapac, sizeof(int));
            //do_uio(220, game.Objects[0].oroom, sizeof(int));
            //do_uio(220, game.Objects[0].oadv, sizeof(int));
            //do_uio(220, game.Objects[0].ocan, sizeof(int));
            //
            //do_uio(200, game.NewRooms.rval[0], sizeof(int));
            //do_uio(200, game.NewRooms.RoomFlags[0], sizeof(int));

            //do_uio(4, game.Adventurers.aroom[0], sizeof(int));
            //do_uio(4, game.Adventurers.ascore[0], sizeof(int));
            //do_uio(4, game.Adventurers.avehic[0], sizeof(int));
            //do_uio(4, game.Adventurers.astren[0], sizeof(int));
            //do_uio(4, game.Adventurers.aflag[0], sizeof(int));
            //
            //do_uio(46, game.Flags[0], sizeof(bool));
            //do_uio(22, switch_[0], sizeof(int));
            //do_uio(4, game.Villians.vprob[0], sizeof(int));
            //do_uio(25, game.Clock.Flags[0], sizeof(bool));
            //do_uio(25, game.Clock.Ticks[0], sizeof(int));

            //(void)fclose(e);

            MessageHandler.rspeak_(game, 599);
            return;

            L100:
            MessageHandler.rspeak_(game, 598);
            // !CANT DO IT.
            return;

            L200:
            MessageHandler.rspeak_(game, 600);
            // !OBSOLETE VERSION
            //(void)fclose(e);
        }

        // WALK- MOVE IN SPECIFIED DIRECTION
        public static bool walk_(Game game)
        {
            // System generated locals
            bool ret_val;

            ret_val = true;
            // !ASSUME WINS.
            if (game.Player.Winner != ActorIds.Player || RoomHandler.IsRoomLit(game.Player.Here, game) || RoomHandler.prob_(game, 25, 25))
            {
                goto L500;
            }

            if (!dso3.FindExit(game, (int)game.ParserVectors.DirectObject, game.Player.Here))
            {
                goto L450;
            }
            // !INVALID EXIT? GRUE
            // !
            switch (game.curxt_.xtype)
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
            if (cxappl_(game, game.curxt_.xactio) != 0)
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
            AdventurerHandler.jigsup_(game, 523);
            // !BAD EXIT, GRUE
            // !
            return ret_val;

            L300:
            if (cxappl_(game, game.curxt_.xactio) != 0)
            {
                goto L400;
            }
            // !DOOR... RETURNED ROOM?
            if ((game.Objects[game.curxt_.xobj].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L400;
            }
            // !NO, DOOR OPEN?
            AdventurerHandler.jigsup_(game, 523);
            // !BAD EXIT, GRUE
            // !
            return ret_val;

            L400:
            if (RoomHandler.IsRoomLit(game.curxt_.xroom1, game))
            {
                goto L900;
            }
            // !VALID ROOM, IS IT LIT?
            L450:
            AdventurerHandler.jigsup_(game, 522);
            // !NO, GRUE
            // !
            return ret_val;

            // ROOM IS LIT, OR WINNER IS NOT PLAYER (NO GRUE).

            L500:
            if (dso3.FindExit(game, (int)game.ParserVectors.DirectObject, game.Player.Here))
            {
                goto L550;
            }
            // !EXIT EXIST?
            L525:
            game.curxt_.xstrng = 678;
            // !ASSUME WALL.
            if (game.ParserVectors.DirectObject == (ObjectIds)XSearch.xup)
            {
                game.curxt_.xstrng = 679;
            }
            // !IF UP, CANT.
            if (game.ParserVectors.DirectObject == (ObjectIds)XSearch.xdown)
            {
                game.curxt_.xstrng = 680;
            }
            // !IF DOWN, CANT.
            if ((game.Rooms[game.Player.Here].Flags & RoomFlags.NOWALL) != 0)
            {
                game.curxt_.xstrng = 524;
            }

            MessageHandler.rspeak_(game, game.curxt_.xstrng);
            game.ParserVectors.prscon = 1;
            // !STOP CMD STREAM.
            return ret_val;

            L550:
            switch (game.curxt_.xtype)
            {
                case 1: goto L900;
                case 2: goto L600;
                case 3: goto L700;
                case 4: goto L800;
            }
            // !BRANCH ON EXIT TYPE.
            throw new InvalidOperationException();
            //bug_(9, curxt_.xtype);

            L700:
            if (cxappl_(game, game.curxt_.xactio) != 0)
            {
                goto L900;
            }
            // !CEXIT... RETURNED ROOM?
            // TODO: chadj figure this out

            //if (game.Flags[game.curxt_.xobj - 1])
            {
              //  goto L900;
            }

            // !NO, FLAG ON?
            L600:
            if (game.curxt_.xstrng == 0)
            {
                goto L525;
            }

            // !IF NO REASON, USE STD.
            MessageHandler.rspeak_(game, game.curxt_.xstrng);
            // !DENY EXIT.
            game.ParserVectors.prscon = 1;
            // !STOP CMD STREAM.
            return ret_val;

            L800:
            if (cxappl_(game, game.curxt_.xactio) != 0)
            {
                goto L900;
            }

            // !DOOR... RETURNED ROOM?
            if ((game.Objects[game.curxt_.xobj].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L900;
            }

            // !NO, DOOR OPEN?
            if (game.curxt_.xstrng == 0)
            {
                game.curxt_.xstrng = 525;
            }

            // !IF NO REASON, USE STD.
            MessageHandler.rspsub_(game.curxt_.xstrng, game.Objects[game.curxt_.xobj].Description2, game);
            game.ParserVectors.prscon = 1;
            // !STOP CMD STREAM.
            return ret_val;

            L900:
            ret_val = AdventurerHandler.moveto_(game, game.curxt_.xroom1, game.Player.Winner);
            // !MOVE TO ROOM.
            if (ret_val)
            {
                ret_val = RoomHandler.RoomDescription(0, game);
            }

            return ret_val;
        }

        // CXAPPL- CONDITIONAL EXIT PROCESSORS
        public static int cxappl_(Game game, int ri)
        {
            int ret_val, i__1;
            ObjectIds i, k;
            int nxt, j;
            int ldir;

            ret_val = 0;
            // !NO RETURN.
            if (ri == 0)
            {
                return ret_val;
            }

            // !IF NO ACTION, DONE.
            switch (ri)
            {
                case 1: goto L1000;
                case 2: goto L2000;
                case 3: goto L3000;
                case 4: goto L4000;
                case 5: goto L5000;
                case 6: goto L6000;
                case 7: goto L7000;
                case 8: goto L8000;
                case 9: goto L9000;
                case 10: goto L10000;
                case 11: goto L11000;
                case 12: goto L12000;
                case 13: goto L13000;
                case 14: goto L14000;
            }

            throw new InvalidOperationException();
            //bug_(5, ri);

            // C1- COFFIN-CURE

            L1000:
            game.Flags.egyptf = game.Objects[ObjectIds.Coffin].Adventurer != game.Player.Winner;
            // !T IF NO COFFIN.
            return ret_val;

            // C2- CAROUSEL EXIT
            // C5- CAROUSEL OUT

            L2000:
            if (game.Flags.IsCarouselOff)
            {
                return ret_val;
            }

            // !IF FLIPPED, NOTHING.
            L2500:
            // !SPIN THE COMPASS.
            MessageHandler.rspeak_(game, 121);

            L5000:
            // !CHOOSE RANDOM EXIT.
            i = (ObjectIds)(xpars_.xelnt[xpars_.xcond - 1] * game.rnd_(8));
            game.curxt_.xroom1 = (RoomIds)(int)(game.Exits.Travel[game.Rooms[game.Player.Here].Exit + (int)i - 1] & xpars_.xrmask);
            // !RETURN EXIT.
            ret_val = (int)game.curxt_.xroom1;
            return ret_val;

            // C3- CHIMNEY FUNCTION

            L3000:
            game.Flags.litldf = false;
            // !ASSUME HEAVY LOAD.
            j = 0;
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
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
            game.curxt_.xstrng = 446;
            // !ASSUME NO LAMP.
            if (game.Objects[ObjectIds.Lamp].Adventurer != game.Player.Winner)
            {
                return ret_val;
            }

            // !NO LAMP?
            game.Flags.litldf = true;
            // !HE CAN DO IT.
            if ((game.Objects[ObjectIds.TrapDoor].Flag2 & ObjectFlags2.IsOpen) == 0)
            {
                game.Objects[ObjectIds.TrapDoor].Flag2 &= ~ObjectFlags2.WasTouched;
            }

            return ret_val;

            // C4-	FROBOZZ FLAG (MAGNET ROOM, FAKE EXIT)
            // C6-	FROBOZZ FLAG (MAGNET ROOM, REAL EXIT)

            L4000:
            if (game.Flags.IsCarouselOff)
            {
                goto L2500;
            }

            // !IF FLIPPED, GO SPIN.
            game.Flags.frobzf = false;
            // !OTHERWISE, NOT AN EXIT.
            return ret_val;

            L6000:
            if (game.Flags.IsCarouselOff)
            {
                goto L2500;
            }

            // !IF FLIPPED, GO SPIN.
            game.Flags.frobzf = true;
            // !OTHERWISE, AN EXIT.
            return ret_val;

            // C7-	FROBOZZ FLAG (BANK ALARM)

            L7000:
            game.Flags.frobzf = RoomHandler.GetRoomThatContainsObject(ObjectIds.bills, game).Id != 0 & RoomHandler.GetRoomThatContainsObject(ObjectIds.portr, game).Id != 0;

            return ret_val;
            // CXAPPL, PAGE 3

            // C8-	FROBOZZ FLAG (MRGO)

            L8000:
            game.Flags.frobzf = false;
            // !ASSUME CANT MOVE.
            if (game.Switches.mloc != game.curxt_.xroom1) {
                goto L8100;
            }
            // !MIRROR IN WAY?
            if (game.ParserVectors.DirectObject == (ObjectIds)XSearch.xnorth || game.ParserVectors.DirectObject == (ObjectIds)XSearch.xsouth) {
                goto L8200;
            }
            if (game.Switches.mdir % 180 != 0)
            {
                goto L8300;
            }

            // !MIRROR MUST BE N-S.
            game.curxt_.xroom1 = (game.curxt_.xroom1 - RoomIds.mra << 1) + RoomIds.mrae;
            // !CALC EAST ROOM.
            if (game.ParserVectors.DirectObject > (ObjectIds)XSearch.xsouth)
            {
                ++game.curxt_.xroom1;
            }
            // !IF SW/NW, CALC WEST.
            L8100:
            ret_val = (int)game.curxt_.xroom1;
            return ret_val;

            L8200:
            game.curxt_.xstrng = 814;
            // !ASSUME STRUC BLOCKS.
            if (game.Switches.mdir % 180 == 0) {
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

            game.curxt_.xstrng = 815;
            // !MIRROR BLOCKS.
            if (ldir > 180 && !game.Flags.mr1f || ldir < 180 && !game.Flags.mr2f)
            {
                game.curxt_.xstrng = 816;
            }

            return ret_val;

            // C9-	FROBOZZ FLAG (MIRIN)

            L9000:
            if (RoomHandler.IsMirrorHere(game, game.Player.Here) != 1)
            {
                goto L9100;
            }

            // !MIRROR 1 HERE?
            if (game.Flags.mr1f)
            {
                game.curxt_.xstrng = 805;
            }

            // !SEE IF BROKEN.
            game.Flags.frobzf = game.Flags.mropnf;
            // !ENTER IF OPEN.
            return ret_val;

            L9100:
            game.Flags.frobzf = false;
            // !NOT HERE,
            game.curxt_.xstrng = 817;
            // !LOSE.
            return ret_val;
            // CXAPPL, PAGE 4

            // C10-	FROBOZZ FLAG (MIRROR EXIT)

            L10000:
            game.Flags.frobzf = false;
            // !ASSUME CANT.
            ldir = game.ParserVectors.DirectObject - (ObjectIds)((int)XSearch.xnorth / (int)XSearch.xnorth * 45);
            // !XLATE DIR TO DEGREES.
            if (!game.Flags.mropnf || (game.Switches.mdir + 270) % 360 != ldir && game.ParserVectors.DirectObject != (ObjectIds)XSearch.xexit)
            {
                goto L10200;
            }

            game.curxt_.xroom1 = (game.Switches.mloc - RoomIds.mra << 1) + RoomIds.mrae + 1 - game.Switches.mdir / 180;

            // !ASSUME E-W EXIT.
            if (game.Switches.mdir % 180 == 0)
            {
                goto L10100;
            }

            // !IF N-S, OK.
            game.curxt_.xroom1 = game.Switches.mloc + 1;
            // !ASSUME N EXIT.
            if (game.Switches.mdir > 180)
            {
                game.curxt_.xroom1 = game.Switches.mloc - 1;
            }
            // !IF SOUTH.
            L10100:
            ret_val = (int)game.curxt_.xroom1;
            return ret_val;

            L10200:
            if (!game.Flags.wdopnf || (game.Switches.mdir + 180) % 360 != ldir && game.ParserVectors.DirectObject != (ObjectIds)XSearch.xexit)
            {
                return ret_val;
            }

            game.curxt_.xroom1 = game.Switches.mloc + 1;
            // !ASSUME N.
            if (game.Switches.mdir == 0)
            {
                game.curxt_.xroom1 = game.Switches.mloc - 1;
            }
            // !IF S.
            MessageHandler.rspeak_(game, 818);
            // !CLOSE DOOR.
            game.Flags.wdopnf = false;
            ret_val = (int)game.curxt_.xroom1;
            return ret_val;

            // C11-	MAYBE DOOR.  NORMAL MESSAGE IS THAT DOOR IS CLOSED.
            // 	BUT IF LCELL.NE.4, DOOR ISNT THERE.

            L11000:
            if (game.Switches.LeftCell != 4)
            {
                game.curxt_.xstrng = 678;
            }
            // !SET UP MSG.
            return ret_val;

            // C12-	FROBZF (PUZZLE ROOM MAIN ENTRANCE)

            L12000:
            game.Flags.frobzf = true;
            // !ALWAYS ENTER.
            game.Switches.cphere = 10;
            // !SET SUBSTATE.
            return ret_val;

            // C13-	CPOUTF (PUZZLE ROOM SIZE ENTRANCE)

            L13000:
            game.Switches.cphere = 52;
            // !SET SUBSTATE.
            return ret_val;
            // CXAPPL, PAGE 5

            // C14-	FROBZF (PUZZLE ROOM TRANSITIONS)

            L14000:
            game.Flags.frobzf = false;
            // !ASSSUME LOSE.
            if (game.ParserVectors.DirectObject != (ObjectIds)XSearch.xup)
            {
                goto L14100;
            }
            // !UP?
            if (game.Switches.cphere != 10) {
                return ret_val;
            }
            // !AT EXIT?
            game.curxt_.xstrng = 881;
            // !ASSUME NO LADDER.
            if (PuzzleHandler.cpvec[game.Switches.cphere] != -2) {
                return ret_val;
            }
            // !LADDER HERE?
            MessageHandler.rspeak_(game, 882);
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
                if (game.ParserVectors.DirectObject == (ObjectIds)PuzzleHandler.cpdr[(int)i - 1]) {
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
            if (j < 0) {
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
            game.curxt_.xroom1 = RoomIds.cpuzz;
            // !STAY IN ROOM.
            ret_val = (int)game.curxt_.xroom1;
            return ret_val;

        }
    }
}