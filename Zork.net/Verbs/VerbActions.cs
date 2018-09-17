using System;
using System.Collections.Generic;
using System.Text;
using Zork.Core.Attributes;

namespace Zork.Core.Verbs
{
    public class VerbActions
    {
        [VerbAction(VerbId.Read)]
        public static bool Read(Game game)
        {
            if (!RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                // !ROOM LIT?
                MessageHandler.Speak(356, game);
                // !NO, CANT READ.
                return true;
            }

            if (game.ParserVectors.IndirectObject == ObjectIds.Nothing)
            {
                goto L18200;
            }

            // !READ THROUGH OBJ?
            if (game.Objects[game.ParserVectors.IndirectObject].Flag1.HasFlag(ObjectFlags.IsTransparent))
            {
                goto L18200;
            }

            // !NOT TRANSPARENT.
            MessageHandler.Speak(357, game.Objects[game.ParserVectors.IndirectObject].ShortDescription, game);

            return true;

        L18200:
            if (game.Objects[game.ParserVectors.DirectObject].Flag1.HasFlag(ObjectFlags.IsReadable))
            {
                goto L18300;
            }

            // !NOT READABLE.
            MessageHandler.Speak(358, game.Objects[game.ParserVectors.DirectObject].ShortDescription, game);

            return true;

        L18300:
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(game.Objects[game.ParserVectors.DirectObject].WrittenText, game);
            }

            return true;
        }

        [VerbAction(VerbId.Look)]
        public static bool Look(Game game)
        {
            bool ret_val = true;

            if (game.ParserVectors.DirectObject != 0)
            {
                goto L41500;
            }

            // !SOMETHING TO LOOK AT?
            ret_val = RoomHandler.RoomDescription(Verbosity.Full, game);
            // !HANDLED BY RMDESC.
            return ret_val;

        L41500:
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }


            ObjectIds i = (ObjectIds)game.Objects[game.ParserVectors.DirectObject].oreadId;
            // !GET READING MATERIAL.
            if (i != 0)
            {
                MessageHandler.Speak(i, game);
            }

            // !OUTPUT IF THERE,
            if (i == 0)
            {
                MessageHandler.Speak(429, game.Objects[game.ParserVectors.DirectObject].ShortDescription, game);
            }

            // !OTHERWISE DEFAULT.
            game.ParserVectors.prsa = VerbId.foow;
            // !DEFUSE ROOM PROCESSORS.
            return ret_val;
        }

        [VerbAction(VerbId.SaveGame)]
        public static bool SaveGame(Game game)
        {
            if (game.Rooms[RoomIds.tstrs].Flags.HasFlag(RoomFlags.SEEN))
            {
                // !NO SAVES IN ENDGAME.
                MessageHandler.Speak(828, game);
                return true;
            }

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
            //do_uio(25, &game.Clock[0].Flags, sizeof(bool));
            //do_uio(25, &game.Clock[0].Ticks, sizeof(int));

            //if (fclose(e) == EOF)
            {
                //    goto L100;
            }

            MessageHandler.Speak(597, game);
            return true;

        L100:
            MessageHandler.Speak(598, game);
            // !CANT DO IT.

            return true;
        }

        [VerbAction(VerbId.Walk)]
        public static bool Walk(Game game) => dverb2.walk_(game);

        [VerbAction(VerbId.RestoreGame)]
        public static bool RestoreGame(Game game)
        {
            if (game.Rooms[RoomIds.tstrs].Flags.HasFlag(RoomFlags.SEEN))
            {
                // !NO RESTORES IN ENDGAME.
                MessageHandler.Speak(829, game);
                return true;
            }

            // RESTORE- RESTORE GAME STATE
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
            //do_uio(25, game.Clock[0].Flags, sizeof(bool));
            //do_uio(25, game.Clock[0].Ticks, sizeof(int));

            //(void)fclose(e);

            MessageHandler.Speak(599, game);
            return true;

        L100:
            MessageHandler.Speak(598, game);
            // !CANT DO IT.
            return true;

        L200:
            MessageHandler.Speak(600, game);
            // !OBSOLETE VERSION
            //(void)fclose(e);
        }
    }
}
