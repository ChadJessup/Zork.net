using System;
using System.Collections.Generic;
using System.Text;
using Zork.Core.Object;
using Zork.Core.Room;

namespace Zork.Core
{
    public static class dverb2
    {
        /* SAVE- SAVE GAME STATE */
        public static void savegm_(Game game)
        {
            /* Local variables */
            int i;
            //FILE* e;

            game.ParserVectors.prswon = false;
            /* 						!DISABLE GAME. */
            /* Note: save file format is different for PDP vs. non-PDP versions */

            //if ((e = fopen("dsave.dat", BINWRITE)) == NULL)
            //    goto L100;

            //gttime_(&i);
            /* 						!GET TIME. */

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
            //do_uio(220, &game.Objects.odesc1[0], sizeof(int));
            //do_uio(220, &game.Objects.odesc2[0], sizeof(int));
            //do_uio(220, &game.Objects.oflag1[0], sizeof(int));
            //do_uio(220, &game.Objects.oflag2[0], sizeof(int));
            //do_uio(220, &game.Objects.ofval[0], sizeof(int));
            //do_uio(220, &game.Objects.otval[0], sizeof(int));
            //do_uio(220, &game.Objects.osize[0], sizeof(int));
            //do_uio(220, &game.Objects.ocapac[0], sizeof(int));
            //do_uio(220, &game.Objects.oroom[0], sizeof(int));
            //do_uio(220, &game.Objects.oadv[0], sizeof(int));
            //do_uio(220, &game.Objects.ocan[0], sizeof(int));
            //
            //do_uio(200, &game.Rooms.rval[0], sizeof(int));
            //do_uio(200, &game.Rooms.RoomFlags[0], sizeof(int));
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
            /* 						!CANT DO IT. */
        } /* savegm_ */

        /* RESTORE- RESTORE GAME STATE */
        public static void rstrgm_(Game game)
        {
            /* Local variables */
            int i, j, k;
//            FILE* e;

            game.ParserVectors.prswon = false;
            /* 						!DISABLE GAME. */
            /* Note: save file format is different for PDP vs. non-PDP versions */

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
            //do_uio(220, game.Objects.odesc1[0], sizeof(int));
            //do_uio(220, game.Objects.odesc2[0], sizeof(int));
            //do_uio(220, game.Objects.oflag1[0], sizeof(int));
            //do_uio(220, game.Objects.oflag2[0], sizeof(int));
            //do_uio(220, game.Objects.ofval[0], sizeof(int));
            //do_uio(220, game.Objects.otval[0], sizeof(int));
            //do_uio(220, game.Objects.osize[0], sizeof(int));
            //do_uio(220, game.Objects.ocapac[0], sizeof(int));
            //do_uio(220, game.Objects.oroom[0], sizeof(int));
            //do_uio(220, game.Objects.oadv[0], sizeof(int));
            //do_uio(220, game.Objects.ocan[0], sizeof(int));
            //
            //do_uio(200, game.Rooms.rval[0], sizeof(int));
            //do_uio(200, game.Rooms.RoomFlags[0], sizeof(int));

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
            /* 						!CANT DO IT. */
            return;

            L200:
            MessageHandler.rspeak_(game, 600);
            /* 						!OBSOLETE VERSION */
            //(void)fclose(e);
        }

        /* WALK- MOVE IN SPECIFIED DIRECTION */
        public static bool walk_(Game game)
        {
            /* System generated locals */
            bool ret_val;

            ret_val = true;
            /* 						!ASSUME WINS. */
            if (game.Player.Winner != (int)AIndices.player || RoomHandler.IsRoomLit(game.Player.Here, game) || RoomHandler.prob_(game, 25, 25))
            {
                goto L500;
            }

            if (!dso3.findxt_(game, game.ParserVectors.prso, game.Player.Here))
            {
                goto L450;
            }
            /* 						!INVALID EXIT? GRUE */
            /* 						! */
            switch (game.curxt_.xtype)
            {
                case 1: goto L400;
                case 2: goto L200;
                case 3: goto L100;
                case 4: goto L300;
            }
            /* 						!DECODE EXIT TYPE. */
            throw new InvalidOperationException();
            //bug_(9, game.curxt_.xtype);

            L100:
            if (cxappl_(game, game.curxt_.xactio) != 0)
            {
                goto L400;
            }
            /* 						!CEXIT... RETURNED ROOM? */
            // TODO: chadj: figure out how to do this
            //if (game.Flags[game.curxt_.xobj - 1])
            {
              //  goto L400;
            }
            /* 						!NO, FLAG ON? */
            L200:
            AdventurerHandler.jigsup_(game, 523);
            /* 						!BAD EXIT, GRUE */
            /* 						! */
            return ret_val;

            L300:
            if (cxappl_(game, game.curxt_.xactio) != 0)
            {
                goto L400;
            }
            /* 						!DOOR... RETURNED ROOM? */
            if ((game.Objects.oflag2[game.curxt_.xobj - 1] & ObjectFlags2.OPENBT) != 0)
            {
                goto L400;
            }
            /* 						!NO, DOOR OPEN? */
            AdventurerHandler.jigsup_(game, 523);
            /* 						!BAD EXIT, GRUE */
            /* 						! */
            return ret_val;

            L400:
            if (RoomHandler.IsRoomLit(game.curxt_.xroom1, game))
            {
                goto L900;
            }
            /* 						!VALID ROOM, IS IT LIT? */
            L450:
            AdventurerHandler.jigsup_(game, 522);
            /* 						!NO, GRUE */
            /* 						! */
            return ret_val;

            /* ROOM IS LIT, OR WINNER IS NOT PLAYER (NO GRUE). */

            L500:
            if (dso3.findxt_(game, game.ParserVectors.prso, game.Player.Here))
            {
                goto L550;
            }
            /* 						!EXIT EXIST? */
            L525:
            game.curxt_.xstrng = 678;
            /* 						!ASSUME WALL. */
            if (game.ParserVectors.prso == (int)XSearch.xup)
            {
                game.curxt_.xstrng = 679;
            }
            /* 						!IF UP, CANT. */
            if (game.ParserVectors.prso == (int)XSearch.xdown)
            {
                game.curxt_.xstrng = 680;
            }
            /* 						!IF DOWN, CANT. */
            if ((game.Rooms.RoomFlags[game.Player.Here - 1] & RoomFlags.RNWALL) != 0)
            {
                game.curxt_.xstrng = 524;
            }

            MessageHandler.rspeak_(game, game.curxt_.xstrng);
            game.ParserVectors.prscon = 1;
            /* 						!STOP CMD STREAM. */
            return ret_val;

            L550:
            switch (game.curxt_.xtype)
            {
                case 1: goto L900;
                case 2: goto L600;
                case 3: goto L700;
                case 4: goto L800;
            }
            /* 						!BRANCH ON EXIT TYPE. */
            throw new InvalidOperationException();
            //bug_(9, curxt_.xtype);

            L700:
            if (cxappl_(game, game.curxt_.xactio) != 0)
            {
                goto L900;
            }
            /* 						!CEXIT... RETURNED ROOM? */
            // TODO: chadj figure this out
            //if (game.Flags[game.curxt_.xobj - 1])
            {
              //  goto L900;
            }
            /* 						!NO, FLAG ON? */
            L600:
            if (game.curxt_.xstrng == 0)
            {
                goto L525;
            }
            /* 						!IF NO REASON, USE STD. */
            MessageHandler.rspeak_(game, game.curxt_.xstrng);
            /* 						!DENY EXIT. */
            game.ParserVectors.prscon = 1;
            /* 						!STOP CMD STREAM. */
            return ret_val;

            L800:
            if (cxappl_(game, game.curxt_.xactio) != 0)
            {
                goto L900;
            }
            /* 						!DOOR... RETURNED ROOM? */
            if ((game.Objects.oflag2[game.curxt_.xobj - 1] & ObjectFlags2.OPENBT) != 0)
            {
                goto L900;
            }
            /* 						!NO, DOOR OPEN? */
            if (game.curxt_.xstrng == 0)
            {
                game.curxt_.xstrng = 525;
            }
            /* 						!IF NO REASON, USE STD. */
            MessageHandler.rspsub_(game.curxt_.xstrng, game.Objects.odesc2[game.curxt_.xobj - 1], game);
            game.ParserVectors.prscon = 1;
            /* 						!STOP CMD STREAM. */
            return ret_val;

            L900:
            ret_val = AdventurerHandler.moveto_(game, game.curxt_.xroom1, game.Player.Winner);
            /* 						!MOVE TO ROOM. */
            if (ret_val)
            {
                ret_val = RoomHandler.RoomDescription(0, game);
            }
            /* 						!DESCRIBE ROOM. */
            return ret_val;
        } /* walk_ */

        /* CXAPPL- CONDITIONAL EXIT PROCESSORS */
        public static int cxappl_(Game game, int ri)
        {
            /* System generated locals */
            int ret_val, i__1;

            /* Local variables */
            int i, j, k;
            int nxt;
            int ldir;

            ret_val = 0;
            /* 						!NO RETURN. */
            if (ri == 0) {
                return ret_val;
            }
            /* 						!IF NO ACTION, DONE. */
            switch (ri) {
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

            /* C1- COFFIN-CURE */

            L1000:
            game.Flags.egyptf = game.Objects.oadv[(int)ObjectIndices.coffi - 1] != game.Player.Winner;
            /* 						!T IF NO COFFIN. */
            return ret_val;

            /* C2- CAROUSEL EXIT */
            /* C5- CAROUSEL OUT */

            L2000:
            if (game.Flags.caroff) {
                return ret_val;
            }
            /* 						!IF FLIPPED, NOTHING. */
            L2500:
            MessageHandler.rspeak_(game, 121);
            /* 						!SPIN THE COMPASS. */
            L5000:
            i = xpars_.xelnt[xpars_.xcond - 1] * game.rnd_(8);
            /* 						!CHOOSE RANDOM EXIT. */
            game.curxt_.xroom1 = game.Exits.Travel[game.Rooms.RoomExits[game.Player.Here - 1] + i - 1] & xpars_.xrmask;
            ret_val = game.curxt_.xroom1;
            /* 						!RETURN EXIT. */
            return ret_val;

            /* C3- CHIMNEY FUNCTION */

            L3000:
            game.Flags.litldf = false;
            /* 						!ASSUME HEAVY LOAD. */
            j = 0;
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i) {
                /* 						!COUNT OBJECTS. */
                if (game.Objects.oadv[i - 1] == game.Player.Winner) {
                    ++j;
                }
                /* L3100: */
            }

            if (j > 2) {
                return ret_val;
            }
            /* 						!CARRYING TOO MUCH? */
            game.curxt_.xstrng = 446;
            /* 						!ASSUME NO LAMP. */
            if (game.Objects.oadv[(int)ObjectIndices.lamp - 1] != game.Player.Winner) {
                return ret_val;
            }
            /* 						!NO LAMP? */
            game.Flags.litldf = true;
            /* 						!HE CAN DO IT. */
            if ((game.Objects.oflag2[(int)ObjectIndices.door - 1] & ObjectFlags2.OPENBT) == 0) {
                game.Objects.oflag2[(int)ObjectIndices.door - 1] &= ~ObjectFlags2.TCHBT;
            }
            return ret_val;

            /* C4-	FROBOZZ FLAG (MAGNET ROOM, FAKE EXIT) */
            /* C6-	FROBOZZ FLAG (MAGNET ROOM, REAL EXIT) */

            L4000:
            if (game.Flags.caroff) {
                goto L2500;
            }
            /* 						!IF FLIPPED, GO SPIN. */
            game.Flags.frobzf = false;
            /* 						!OTHERWISE, NOT AN EXIT. */
            return ret_val;

            L6000:
            if (game.Flags.caroff) {
                goto L2500;
            }
            /* 						!IF FLIPPED, GO SPIN. */
            game.Flags.frobzf = true;
            /* 						!OTHERWISE, AN EXIT. */
            return ret_val;

            /* C7-	FROBOZZ FLAG (BANK ALARM) */

            L7000:
            game.Flags.frobzf = game.Objects.oroom[(int)ObjectIndices.bills - 1] != 0 &
                game.Objects.oroom[(int)ObjectIndices.portr - 1] != 0;
            return ret_val;
            /* CXAPPL, PAGE 3 */

            /* C8-	FROBOZZ FLAG (MRGO) */

            L8000:
            game.Flags.frobzf = false;
            /* 						!ASSUME CANT MOVE. */
            if (game.Switch.mloc != game.curxt_.xroom1) {
                goto L8100;
            }
            /* 						!MIRROR IN WAY? */
            if (game.ParserVectors.prso == (int)XSearch.xnorth || game.ParserVectors.prso == (int)XSearch.xsouth) {
                goto L8200;
            }
            if (game.Switch.mdir % 180 != 0)
            {
                goto L8300;
            }

            /* 						!MIRROR MUST BE N-S. */
            game.curxt_.xroom1 = (game.curxt_.xroom1 - (int)RoomIndices.mra << 1) + (int)RoomIndices.mrae;
            /* 						!CALC EAST ROOM. */
            if (game.ParserVectors.prso > (int)XSearch.xsouth)
            {
                ++game.curxt_.xroom1;
            }
            /* 						!IF SW/NW, CALC WEST. */
            L8100:
            ret_val = game.curxt_.xroom1;
            return ret_val;

            L8200:
            game.curxt_.xstrng = 814;
            /* 						!ASSUME STRUC BLOCKS. */
            if (game.Switch.mdir % 180 == 0) {
                return ret_val;
            }
            /* 						!IF MIRROR N-S, DONE. */
            L8300:
            ldir = game.Switch.mdir;
            /* 						!SEE WHICH MIRROR. */
            if (game.ParserVectors.prso == (int)XSearch.xsouth)
            {
                ldir = 180;
            }

            game.curxt_.xstrng = 815;
            /* 						!MIRROR BLOCKS. */
            if (ldir > 180 && !game.Flags.mr1f || ldir < 180 && !game.Flags.mr2f)
            {
                game.curxt_.xstrng = 816;
            }

            return ret_val;

            /* C9-	FROBOZZ FLAG (MIRIN) */

            L9000:
            if (RoomHandler.mrhere_(game, game.Player.Here) != 1)
            {
                goto L9100;
            }

            /* 						!MIRROR 1 HERE? */
            if (game.Flags.mr1f)
            {
                game.curxt_.xstrng = 805;
            }

            /* 						!SEE IF BROKEN. */
            game.Flags.frobzf = game.Flags.mropnf;
            /* 						!ENTER IF OPEN. */
            return ret_val;

            L9100:
            game.Flags.frobzf = false;
            /* 						!NOT HERE, */
            game.curxt_.xstrng = 817;
            /* 						!LOSE. */
            return ret_val;
            /* CXAPPL, PAGE 4 */

            /* C10-	FROBOZZ FLAG (MIRROR EXIT) */

            L10000:
            game.Flags.frobzf = false;
            /* 						!ASSUME CANT. */
            ldir = (game.ParserVectors.prso - (int)XSearch.xnorth) / (int)XSearch.xnorth * 45;
            /* 						!XLATE DIR TO DEGREES. */
            if (!game.Flags.mropnf || (game.Switch.mdir + 270) % 360 != ldir && game.ParserVectors.prso != (int)XSearch.xexit)
            {
                goto L10200;
            }

            game.curxt_.xroom1 = (game.Switch.mloc - (int)RoomIndices.mra << 1) + (int)RoomIndices.mrae + 1
                - game.Switch.mdir / 180;
            /* 						!ASSUME E-W EXIT. */
            if (game.Switch.mdir % 180 == 0)
            {
                goto L10100;
            }

            /* 						!IF N-S, OK. */
            game.curxt_.xroom1 = game.Switch.mloc + 1;
            /* 						!ASSUME N EXIT. */
            if (game.Switch.mdir > 180)
            {
                game.curxt_.xroom1 = game.Switch.mloc - 1;
            }
            /* 						!IF SOUTH. */
            L10100:
            ret_val = game.curxt_.xroom1;
            return ret_val;

            L10200:
            if (!game.Flags.wdopnf || (game.Switch.mdir + 180) % 360 != ldir &&
                game.ParserVectors.prso != (int)XSearch.xexit) {
                return ret_val;
            }
            game.curxt_.xroom1 = game.Switch.mloc + 1;
            /* 						!ASSUME N. */
            if (game.Switch.mdir == 0)
            {
                game.curxt_.xroom1 = game.Switch.mloc - 1;
            }
            /* 						!IF S. */
            MessageHandler.rspeak_(game, 818);
            /* 						!CLOSE DOOR. */
            game.Flags.wdopnf = false;
            ret_val = game.curxt_.xroom1;
            return ret_val;

            /* C11-	MAYBE DOOR.  NORMAL MESSAGE IS THAT DOOR IS CLOSED. */
            /* 	BUT IF LCELL.NE.4, DOOR ISNT THERE. */

            L11000:
            if (game.Switch.lcell != 4)
            {
                game.curxt_.xstrng = 678;
            }
            /* 						!SET UP MSG. */
            return ret_val;

            /* C12-	FROBZF (PUZZLE ROOM MAIN ENTRANCE) */

            L12000:
            game.Flags.frobzf = true;
            /* 						!ALWAYS ENTER. */
            game.Switch.cphere = 10;
            /* 						!SET SUBSTATE. */
            return ret_val;

            /* C13-	CPOUTF (PUZZLE ROOM SIZE ENTRANCE) */

            L13000:
            game.Switch.cphere = 52;
            /* 						!SET SUBSTATE. */
            return ret_val;
            /* CXAPPL, PAGE 5 */

            /* C14-	FROBZF (PUZZLE ROOM TRANSITIONS) */

            L14000:
            game.Flags.frobzf = false;
            /* 						!ASSSUME LOSE. */
            if (game.ParserVectors.prso != (int)XSearch.xup) {
                goto L14100;
            }
            /* 						!UP? */
            if (game.Switch.cphere != 10) {
                return ret_val;
            }
            /* 						!AT EXIT? */
            game.curxt_.xstrng = 881;
            /* 						!ASSUME NO LADDER. */
            if (PuzzleHandler.cpvec[game.Switch.cphere] != -2) {
                return ret_val;
            }
            /* 						!LADDER HERE? */
            MessageHandler.rspeak_(game, 882);
            /* 						!YOU WIN. */
            game.Flags.frobzf = true;
            /* 						!LET HIM OUT. */
            return ret_val;

            L14100:
            if (game.Switch.cphere != 52 || game.ParserVectors.prso != (int)XSearch.xwest || !
                game.Flags.cpoutf) {
                goto L14200;
            }
            game.Flags.frobzf = true;
            /* 						!YES, LET HIM OUT. */
            return ret_val;

            L14200:
            for (i = 1; i <= 16; i += 2) {
                /* 						!LOCATE EXIT. */
                if (game.ParserVectors.prso == PuzzleHandler.cpdr[i - 1]) {
                    goto L14400;
                }
                /* L14300: */
            }
            return ret_val;
            /* 						!NO SUCH EXIT. */

            L14400:
            j = PuzzleHandler.cpdr[i];
            /* 						!GET DIRECTIONAL OFFSET. */
            nxt = game.Switch.cphere + j;
            /* 						!GET NEXT STATE. */
            k = 8;
            /* 						!GET ORTHOGONAL DIR. */
            if (j < 0) {
                k = -8;
            }
            if ((Math.Abs(j) == 1 || Math.Abs(j) == 8 || (PuzzleHandler.cpvec[game.Switch.cphere + k - 1] == 0 || PuzzleHandler.cpvec[nxt - k - 1] == 0)) && PuzzleHandler.cpvec[nxt - 1] == 0) {
                goto L14500;
            }
            return ret_val;

            L14500:
            dso7.cpgoto_(game, nxt);
            /* 						!MOVE TO STATE. */
            game.curxt_.xroom1 = (int)RoomIndices.cpuzz;
            /* 						!STAY IN ROOM. */
            ret_val = game.curxt_.xroom1;
            return ret_val;

        }
    }
}