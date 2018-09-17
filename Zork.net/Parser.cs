using System;
using System.Collections.Generic;
using System.Linq;
using Zork.Core.Attributes;

namespace Zork.Core
{
    public class Parser
    {
        public Dictionary<VerbId, (VerbActionAttribute attrib, Func<Game, bool> action)> Verbs { get; internal set; }

        public string ReadLine(Game game, int who)
        {
            string buffer;

            switch (who + 1)
            {
                case 1: goto L90;
                case 2: goto L10;
            }

            L10:
            game.WriteOutput(">");

            L90:
            if (!game.QueuedCommands.Any())
            {
                buffer = game.ReadInput();

                foreach (var command in buffer.Split(new[] { ';' }))
                {
                    game.QueuedCommands.Enqueue(command);
                }
            }

            buffer = game.QueuedCommands.Dequeue();

            if (string.IsNullOrWhiteSpace(buffer))
            {
                return "";
            }

            if (buffer[0] == '!')
            {
                buffer = buffer.TrimStart(new[] { '!' });
            }

            return buffer.ToUpper() + '\0';
        }

        /// <summary>
        /// parse_ - Top level parse routine
        /// </summary>
        /// <param name="input"></param>
        /// <param name="vbflag"></param>
        public bool Parse(string input, bool vbflag, Game game)
        {
            int i__1;
            // !ASSUME FAILS.
            bool ret_val = false;

            int[] outbuf = new int[40];

            game.ParserVectors.prsa = 0;

            // !ZERO OUTPUTS.
            game.ParserVectors.IndirectObject = ObjectIds.Nothing;
            game.ParserVectors.DirectObject = ObjectIds.Nothing;

            if (!this.Lex(input, outbuf, out int outlnt, vbflag, game))
            {
                goto L100;
            }

            if ((i__1 = ParseStart(outbuf, outlnt, vbflag, game)) < 0)
            {
                goto L100;
            }
            else if (i__1 == 0)
            {
                goto L200;
            }
            else
            {
                goto L300;
            }

            // !DO SYN SCAN.

            // PARSE REQUIRES VALIDATION

            L200:
            // !ECHO MODE, FORCE FAIL.
            if (!vbflag)
            {
                goto L350;
            }

            // !DO SYN MATCH.
            if (!MatchSyntax(game))
            {
                goto L100;
            }

            if (game.ParserVectors.DirectObject > 0 & game.ParserVectors.DirectObject < (ObjectIds)XSearch.xmin)
            {
                game.Last.lastit = game.ParserVectors.DirectObject;
            }

            L300:
            // SUCCESSFUL PARSE OR SUCCESSFUL VALIDATION
            ret_val = true;

            L350:
            // !CLEAR ORPHANS.
            Orphans.Orphan(0, 0, 0, 0, 0, game);

            // PARSE FAILS, DISALLOW CONTINUATION
            return ret_val;

            L100:
            game.ParserVectors.prscon = 1;

            return ret_val;
        }

        /// <summary>
        /// Lex_ - lexical analyzer
        /// </summary>
        /// <param name="inbuf"></param>
        /// <param name="outbuf"></param>
        /// <param name="outputPointer"></param>
        /// <param name="vbflag"></param>
        /// <returns></returns>
        public bool Lex(string inbuf, int[] outbuf, out int outputPointer, bool vbflag, Game game)
        {
            char[] dlimit = new char[9] { 'A', 'Z', (char)('A' - 1), '1', '9', (char)('1' - 31), '-', '-', (char)('-' - 27) };
            bool ret_val = false;

            int i;
            char character;
            int wordIndex = 0, offset1, offset2, characterPointer;

            // !ASSUME LEX FAILS.
            outputPointer = -1;

            ADVANCEPOINTER:
            outputPointer += 2;

            // !CHAR PTR=0.
            characterPointer = 0;

            GETCHAR:
            character = inbuf[game.ParserVectors.prscon - 1];

            // !END OF INPUT?
            if (character == '\0')
            {
                goto ENDOFCOMMAND;
            }

            // !ADVANCE PTR.
            ++game.ParserVectors.prscon;

            // !END OF COMMAND?
            if (character == '.')
            {
                goto ENDOFCOMMAND;
            }

            // !END OF COMMAND?
            if (character == ',')
            {
                goto ENDOFCOMMAND;
            }

            // !SPACE?
            if (character == ' ')
            {
                goto HANDLESPACE;
            }

            // !SCH FOR CHAR.
            for (i = 1; i <= dlimit.Length; i += 3)
            {
                if (character >= dlimit[i - 1] & character <= dlimit[i])
                {
                    goto CHARACTERINRANGE;
                }
                // L500:
            }

            if (vbflag)
            {
                // !GREEK TO ME, FAIL.
                MessageHandler.Speak(601, game);
            }

            // END OF INPUT, SEE IF PARTIAL WORD AVAILABLE.
            return ret_val;

            ENDOFCOMMAND:
            if (inbuf[game.ParserVectors.prscon - 1] == '\0')
            {
                game.ParserVectors.prscon = 1;
            }

            // !FORCE PARSE RESTART.
            if (characterPointer == 0 & outputPointer == 1)
            {
                return ret_val;
            }

            // !ANY LAST WORD?
            if (characterPointer == 0)
            {
                outputPointer += -2;
            }

            // LEGITIMATE CHARACTERS: LETTER, DIGIT, OR HYPHEN.
            return true;

            CHARACTERINRANGE:
            offset1 = character - dlimit[i + 1];
            // !IGNORE IF TOO MANY CHAR.
            if (characterPointer >= 6)
            {
                goto GETCHAR;
            }

            // !COMPUTE WORD INDEX.
            wordIndex = outputPointer + characterPointer / 3;

            switch (characterPointer % 3 + 1)
            {
                case 1: goto CHAR1;
                case 2: goto CHAR2;
                case 3: goto CHAR3;
            }

            CHAR1:
            // !CHAR 1... *780
            offset2 = offset1 * 780;

            // !*1560 (40 ADDED BELOW).
            outbuf[wordIndex] = outbuf[wordIndex] + offset2 + offset2;

            CHAR2:
            // !*39 (1 ADDED BELOW).
            outbuf[wordIndex] += offset1 * 39;

            CHAR3:
            // !*1.
            outbuf[wordIndex] += offset1;

            // !GET NEXT CHAR.
            ++characterPointer;
            goto GETCHAR;

            HANDLESPACE:
            // !ANY WORD YET?
            if (characterPointer == 0)
            {
                goto GETCHAR;
            }

            // !YES, ADV OP.
            goto ADVANCEPOINTER;
        }

        /// <summary>
        /// THIS ROUTINE DETAILS ON BIT 4 OF PRSFLG
        /// </summary>
        private bool MatchSyntax(Game game)
        {
            //  THE FOLLOWING DATA STATEMENT WAS ORIGINALLY:
            // 	DATA R50MIN/1RA/

            const int r50min = 1600;

            int i__1;
            bool ret_val = false;
            ObjectIds j;
            int newj;
            ObjectIds drive, limit, qprep, sprep;
            int dforce;

            // !SET UP PTR TO SYNTAX.
            j = (ObjectIds)game.pv_1.act;
            // !NO DEFAULT.
            drive = 0;
            // !NO FORCED DEFAULT.
            dforce = 0;
            qprep = (ObjectIds)(game.Orphans.Flag & game.Orphans.oprep);
            L100:
            j += 2;

            // !FIND START OF SYNTAX.
            if (ParserConstants.Verbs[(int)j - 1] <= 0 || ParserConstants.Verbs[(int)j - 1] >= r50min)
            {
                goto L100;
            }

            // !COMPUTE LIMIT.
            limit = j + ParserConstants.Verbs[(int)j - 1] + 1;
            // !ADVANCE TO NEXT.
            ++j;

            UNPACKSYNTAX:
            UnpackSyntax((int)j, out newj, game);

            sprep = (ObjectIds)(game.Syntax.DirectObject & SyntaxObjectFlags.Mask);
            if (!IsSyntaxEqual(game.prpvec.p1, game.ObjectVector.o1, game.Syntax.DirectObject, game.Syntax.DirectObjectFlag1, game.Syntax.DirectObjectFlag2, game))
            {
                goto L1000;
            }

            sprep = (ObjectIds)(game.Syntax.IndirectObject & SyntaxObjectFlags.Mask);
            if (IsSyntaxEqual(game.prpvec.p2, game.ObjectVector.o2, game.Syntax.IndirectObject, game.Syntax.IndirectObjectFlag1, game.Syntax.IndirectObjectFlag2, game))
            {
                goto L6000;
            }

            // SYNTAX MATCH FAILS, TRY NEXT ONE.
            if (game.ObjectVector.o2 != 0)
            {
                goto L3000;
            }
            else
            {
                goto L500;
            }

            // !IF O2=0, SET DFLT.
            L1000:
            if (game.ObjectVector.o1 != 0)
            {
                goto L3000;
            }
            else
            {
                goto L500;
            }

            // !IF O1=0, SET DFLT.
            L500:
            if (qprep == 0 || qprep == sprep)
            {
                dforce = (int)j;
            }

            // !IF PREP MCH.
            if (game.Syntax.Flags.HasFlag(SyntaxFlags.SDRIV))
            {
                drive = j;
            }

            L3000:
            j = (ObjectIds)newj;
            if (j < limit)
            {
                goto UNPACKSYNTAX;
            }

            // !MORE TO DO?

            // MATCH HAS FAILED.  IF DEFAULT SYNTAX EXISTS, TRY TO SNARF
            // ORPHANS OR GWIMS, OR MAKE NEW ORPHANS.

            if (drive == 0)
            {
                drive = (ObjectIds)dforce;
            }

            // !NO DRIVER? USE FORCE.
            if (drive == 0)
            {
                goto L10000;
            }

            // !ANY DRIVER?
            UnpackSyntax((int)drive, out dforce, game);
            // !UNPACK DFLT SYNTAX.

            // TRY TO FILL DIRECT OBJECT SLOT IF THAT WAS THE PROBLEM.
            if (game.Syntax.Flags.HasFlag(SyntaxFlags.DirectObject) || game.ObjectVector.o1 != 0)
            {
                goto L4000;
            }

            // FIRST TRY TO SNARF ORPHAN OBJECT.
            game.ObjectVector.o1 = (ObjectIds)game.Orphans.Flag & game.Orphans.Slot;
            if (game.ObjectVector.o1 == 0)
            {
                goto L3500;
            }

            // !ANY ORPHAN?
            if (IsSyntaxEqual(game.prpvec.p1, game.ObjectVector.o1, game.Syntax.DirectObject, game.Syntax.DirectObjectFlag1, game.Syntax.DirectObjectFlag2, game))
            {
                goto L4000;
            }

            // ORPHAN FAILS, TRY GWIM.
            L3500:
            game.ObjectVector.o1 = (ObjectIds)GetWhatIMean(game.Syntax.DirectObject, game.Syntax.DirectObjectWord1, game.Syntax.DirectObjectWord2, game);
            // !GET GWIM.
            if (game.ObjectVector.o1 > 0)
            {
                goto L4000;
            }

            // !TEST RESULT.
            i__1 = (int)(game.Syntax.DirectObject & SyntaxObjectFlags.Mask);
            Orphans.Orphan(-1, game.pv_1.act, 0, i__1, 0, game);
            MessageHandler.Speak(623, game);

            return ret_val;

            // TRY TO FILL INDIRECT OBJECT SLOT IF THAT WAS THE PROBLEM.

            L4000:
            if (!game.Syntax.Flags.HasFlag(SyntaxFlags.IndirectObject) || game.ObjectVector.o2 != 0)
            {
                goto L6000;
            }

            // !GWIM.
            game.ObjectVector.o2 = (ObjectIds)GetWhatIMean(game.Syntax.IndirectObject, game.Syntax.IndirectObjectWord1, game.Syntax.IndirectObjectWord2, game);

            if (game.ObjectVector.o2 > 0)
            {
                goto L6000;
            }

            if (game.ObjectVector.o1 == 0)
            {
                game.ObjectVector.o1 = (ObjectIds)(game.Orphans.Flag & (int)game.Orphans.Slot);
            }

            i__1 = (int)(game.Syntax.DirectObject & SyntaxObjectFlags.Mask);
            Orphans.Orphan(-1, game.pv_1.act, game.ObjectVector.o1, i__1, 0, game);
            MessageHandler.Speak(624, game);

            return ret_val;

            // TOTAL CHOMP

            L10000:
            // !CANT DO ANYTHING.
            MessageHandler.Speak(601, game);
            return ret_val;

            // NOW TRY TO TAKE INDIVIDUAL OBJECTS AND
            // IN GENERAL CLEAN UP THE PARSE VECTOR.

            L6000:
            if (!game.Syntax.Flags.HasFlag(SyntaxFlags.SFLIP))
            {
                goto L5000;
            }

            j = game.ObjectVector.o1;

            // !YES.
            game.ObjectVector.o1 = game.ObjectVector.o2;
            game.ObjectVector.o2 = j;

            L5000:
            game.ParserVectors.prsa = (VerbId)(game.Syntax.Flags & SyntaxFlags.VectorMask);

            // !GET DIR OBJ.
            game.ParserVectors.DirectObject = game.ObjectVector.o1;

            // !GET IND OBJ.
            game.ParserVectors.IndirectObject = game.ObjectVector.o2;

            // !TRY TAKE.
            if (!TakeObject(game.ParserVectors.DirectObject, game.Syntax.DirectObject, game))
            {
                return ret_val;
            }

            // !TRY TAKE.
            if (!TakeObject(game.ParserVectors.IndirectObject, game.Syntax.IndirectObject, game))
            {
                return ret_val;
            }

            return true;
        }

        /// <summary>
        /// gwim_ - Get what I mean
        /// </summary>
        /// <param name="sflag"></param>
        /// <param name="sfw1"></param>
        /// <param name="sfw2"></param>
        /// <returns></returns>
        private int GetWhatIMean(SyntaxObjectFlags sflag, int sfw1, int sfw2, Game game)
        {
            // System generated locals
            int ret_val;

            // Local variables
            ObjectIds av;
            int nobj;
            ObjectIds robj;
            bool nocare;

            // GWIM, PAGE 2

            // !ASSUME LOSE.
            ret_val = -1;
            av = (ObjectIds)game.Adventurers[game.Player.Winner].VehicleId;
            nobj = 0;
            nocare = (sflag & SyntaxObjectFlags.AdventurerMustHave) == 0;

            // FIRST SEARCH ADVENTURER

            if ((sflag & SyntaxObjectFlags.SearchAdventurer) != 0)
            {
                nobj = FindWhatIMean(sfw1, (ObjectFlags2)sfw2, 0, 0, game.Player.Winner, nocare, game);
            }

            if ((sflag & SyntaxObjectFlags.SearchRoom) != 0)
            {
                goto L100;
            }

            L50:
            ret_val = nobj;
            return ret_val;

            // ALSO SEARCH ROOM

            L100:
            robj = (ObjectIds)FindWhatIMean(sfw1, (ObjectFlags2)sfw2, game.Player.Here, 0, 0, nocare, game);
            if (robj < 0)
            {
                goto L500;
            }
            else if (robj == 0)
            {
                goto L50;
            }
            else
            {
                goto L200;
            }
            // !TEST RESULT.

            // ROBJ > 0

            L200:
            if (av == 0 || robj == av || (game.Objects[robj].Flag2 & ObjectFlags2.FINDBT)
                != 0)
            {
                goto L300;
            }

            if (game.Objects[robj].Container != av)
            {
                goto L50;
            }

            // !UNREACHABLE? TRY NOBJ
            L300:
            if (nobj != 0)
            {
                return ret_val;
            }

            // !IF AMBIGUOUS, RETURN.
            if (!TakeObject(robj, sflag, game))
            {
                return ret_val;
            }

            // !IF UNTAKEABLE, RETURN
            ret_val = (int)robj;
            L500:
            return ret_val;
        }

        /// <summary>
        /// fwim_ - Find what I mean
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="rm"></param>
        /// <param name="con"></param>
        /// <param name="actorId"></param>
        /// <param name="nocare"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public int FindWhatIMean(int f1, ObjectFlags2 f2, RoomIds rm, ObjectIds con, ActorIds actorId, bool nocare, Game game)
        {
            int ret_val, i__2;

            ObjectIds j;
            ObjectIds i;

            // OBJECTS
            ret_val = 0;
            // !ASSUME NOTHING.
            for (i = (ObjectIds)1; i < (ObjectIds)game.Objects.Count; ++i)
            {
                // !LOOP
                if ((rm == 0 || RoomHandler.GetRoomThatContainsObject(i, game)?.Id != rm)
                    && (actorId == 0 || game.Objects[i].Adventurer != actorId) && (con == 0 || game.Objects[i].Container != con))
                {
                    goto L1000;
                }

                // OBJECT IS ON LIST... IS IT A MATCH?

                if ((game.Objects[i].Flag1 & ObjectFlags.IsVisible) == 0)
                {
                    goto L1000;
                }

                // double check: was ~(nocare)
                if (!nocare & (game.Objects[i].Flag1 & ObjectFlags.IsTakeable) == 0
                    || ((int)game.Objects[i].Flag1 & f1) == 0 && (game.Objects[i].Flag2 & f2) == 0)
                {
                    goto L500;
                }
                if (ret_val == 0)
                {
                    goto L400;
                }
                // !ALREADY GOT SOMETHING?
                ret_val = -ret_val;
                // !YES, AMBIGUOUS.
                return ret_val;

                L400:
                ret_val = (int)i;
                // !NOTE MATCH.

                // DOES OBJECT CONTAIN A MATCH?

                L500:
                if ((game.Objects[i].Flag2 & ObjectFlags2.IsOpen) == 0)
                {
                    goto L1000;
                }

                i__2 = game.Objects.Count;
                for (j = (ObjectIds)1; j <= (ObjectIds)i__2; ++j)
                {
                    // !NO, SEARCH CONTENTS.
                    if (game.Objects[j].Container != i
                        || (game.Objects[j].Flag1 & ObjectFlags.IsVisible) == 0
                        || ((int)game.Objects[j].Flag1 & f1) == 0
                        && (game.Objects[j].Flag2 & f2) == 0)
                    {
                        goto L700;
                    }
                    if (ret_val == 0)
                    {
                        goto L600;
                    }
                    ret_val = -ret_val;
                    return ret_val;

                    L600:
                    ret_val = (int)j;
                    L700:
                    ;
                }
                L1000:
                ;
            }

            return ret_val;
        }

        /// <summary>
        /// takeit_ - PARSER BASED TAKE OBJECT
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        private bool TakeObject(ObjectIds obj, SyntaxObjectFlags sflag, Game game)
        {
            // !ASSUME LOSES.
            bool ret_val = false;

            ObjectIds x;
            string objDescription2;

            // !NULL/STARS WIN.
            if (obj == 0 || obj > (ObjectIds)game.Star.strbit)
            {
                goto L4000;
            }

            // !GET DESC.
            objDescription2 = game.Objects[obj].ShortDescription;
            // !GET CONTAINER.
            x = game.Objects[obj].Container;

            if (x == 0 || (sflag & SyntaxObjectFlags.MustBeReachable) == 0)
            {
                goto L500;
            }

            if (game.Objects[x].Flag2.HasFlag(ObjectFlags2.IsOpen))
            {
                goto L500;
            }

            // !CANT REACH.
            MessageHandler.Speak(MessageConstants.CANTREACH, objDescription2, game);
            return ret_val;

            L500:
            if ((sflag & SyntaxObjectFlags.SearchRoom) == 0)
            {
                goto SEARCHROOM;
            }

            if ((sflag & SyntaxObjectFlags.TryTake) == 0)
            {
                goto L2000;
            }

            // SHOULD BE IN ROOM (VRBIT NE 0) AND CAN BE TAKEN (VTBIT NE 0)
            if (SearchForObject(0, 0, game.Player.Here, 0, 0, obj, game)?.Id <= 0)
            {
                goto L4000;
            }
            // !IF NOT, OK.

            // ITS IN THE ROOM AND CAN BE TAKEN.
            if (game.Objects[obj].Flag1.HasFlag(ObjectFlags.IsTakeable) &&
               !game.Objects[obj].Flag2.HasFlag(ObjectFlags2.CanTry))
            {
                goto L3000;
            }

            // NOT TAKEABLE.  IF WE CARE, FAIL.
            if ((sflag & SyntaxObjectFlags.AdventurerMustHave) == 0)
            {
                goto L4000;
            }

            MessageHandler.Speak(445, objDescription2, game);
            return ret_val;

            // 1000--	IT SHOULD NOT BE IN THE ROOM.
            // 2000--	IT CANT BE TAKEN.

            L2000:
            if ((sflag & SyntaxObjectFlags.AdventurerMustHave) == 0)
            {
                goto L4000;
            }

            SEARCHROOM:
            var foundObjId = (ObjectIds)(-1 * (int)SearchForObject(0, 0, game.Player.Here, 0, 0, obj, game).Id);
            if (foundObjId <= 0)
            {
                goto L4000;
            }

            MessageHandler.Speak(665, objDescription2, game);

            return ret_val;
            // TAKEIT, PAGE 3

            // OBJECT IS IN THE ROOM, CAN BE TAKEN BY THE PARSER,
            // AND IS TAKEABLE IN GENERAL.  IT IS NOT A STAR.
            // TAKING IT SHOULD NOT HAVE SIDE AFFECTS.
            // IF IT IS INSIDE SOMETHING, THE CONTAINER IS OPEN.
            // THE FOLLOWING CODE IS LIFTED FROM SUBROUTINE TAKE.

            L3000:
            // !TAKE VEHICLE?
            if (obj != (ObjectIds)game.Adventurers[game.Player.Winner].VehicleId)
            {
                goto L3500;
            }

            MessageHandler.Speak(672, game);
            return ret_val;

            L3500:
            if (x != 0 && game.Objects[x].Adventurer == game.Player.Winner ||
                ObjectHandler.GetWeight(0, obj, game.Player.Winner, game) + game.Objects[obj].Size <= game.State.MaxLoad)
            {
                goto L3700;
            }

            // !TOO BIG.
            MessageHandler.Speak(MessageConstants.TOOBIG, game);
            return ret_val;

            L3700:
            // !DO TAKE.
            ObjectHandler.SetNewObjectStatus(obj, 559, 0, 0, game.Player.Winner, game);

            game.Objects[obj].Flag2 |= ObjectFlags2.WasTouched;
            AdventurerHandler.ScoreUpdate(game, game.Objects[obj].Value);
            game.Objects[obj].Value = 0;

            L4000:
            ret_val = true;
            // !SUCCESS.
            return ret_val;
        }

        /// <summary>
        /// syneql_ - Test for syntax equality.
        /// </summary>
        /// <param name="prep"></param>
        /// <param name="obj"></param>
        /// <param name="sprep"></param>
        /// <param name="sfl1"></param>
        /// <param name="sfl2"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public bool IsSyntaxEqual(int prep, ObjectIds obj, SyntaxObjectFlags sprep, int sfl1, int sfl2, Game game)
        {
            bool ret_val;

            if (obj == 0)
            {
                goto L100;
            }

            // !ANY OBJECT?
            ret_val = prep == (int)(sprep & SyntaxObjectFlags.Mask) && (sfl1 & (int)game.Objects[obj].Flag1 | sfl2 & (int)game.Objects[obj].Flag2) != 0;
            return ret_val;

            L100:
            ret_val = prep == 0 && sfl1 == 0 && sfl2 == 0;
            return ret_val;
        }

        /// <summary>
        /// unpack_ - Unpack syntax specification
        /// </summary>
        /// <param name="oldj"></param>
        /// <param name="j"></param>
        /// <param name="game"></param>
        private void UnpackSyntax(int oldj, out int j, Game game)
        {
            // !CLEAR SYNTAX.
            game.Syntax.DirectObject = 0;
            game.Syntax.DirectObjectFlag1 = 0;
            game.Syntax.DirectObjectFlag2 = 0;
            game.Syntax.DirectObjectWord1 = 0;
            game.Syntax.DirectObjectWord2 = 0;

            game.Syntax.IndirectObject = 0;
            game.Syntax.IndirectObjectFlag1 = 0;
            game.Syntax.IndirectObjectFlag2 = 0;
            game.Syntax.IndirectObjectWord1 = 0;
            game.Syntax.IndirectObjectWord2 = 0;
            game.Syntax.Flags = 0;
            // L10:

            game.Syntax.Flags = (SyntaxFlags)ParserConstants.Verbs[oldj - 1];
            j = oldj + 1;
            if (!game.Syntax.Flags.HasFlag(SyntaxFlags.DirectObject))
            {
                return;
            }

            game.Syntax.DirectObjectFlag1 = -1;
            // !ASSUME STD.
            game.Syntax.DirectObjectFlag2 = -1;
            if (!game.Syntax.Flags.HasFlag(SyntaxFlags.SSTD))
            {
                goto L100;
            }

            game.Syntax.DirectObjectWord1 = -1;
            // !YES.
            game.Syntax.DirectObjectWord2 = -1;
            game.Syntax.DirectObject = SyntaxObjectFlags.SearchAdventurer + (int)SyntaxObjectFlags.SearchRoom + (int)SyntaxObjectFlags.MustBeReachable;
            goto L200;

            L100:
            game.Syntax.DirectObject = (SyntaxObjectFlags)ParserConstants.Verbs[j - 1];
            // !NOT STD.
            game.Syntax.DirectObjectWord1 = ParserConstants.Verbs[j];
            game.Syntax.DirectObjectWord2 = ParserConstants.Verbs[j + 1];
            j += 3;

            if (!game.Syntax.DirectObject.HasFlag(SyntaxObjectFlags.VEBIT))
            {
                goto L200;
            }

            game.Syntax.DirectObjectFlag1 = game.Syntax.DirectObjectWord1;
            // !YES.
            game.Syntax.DirectObjectFlag2 = game.Syntax.DirectObjectWord2;

            L200:
            if (!game.Syntax.Flags.HasFlag(SyntaxFlags.IndirectObject))
            {
                return;
            }

            game.Syntax.IndirectObjectFlag1 = -1;
            // !ASSUME STD.
            game.Syntax.IndirectObjectFlag2 = -1;
            game.Syntax.IndirectObject = (SyntaxObjectFlags)ParserConstants.Verbs[j - 1];
            game.Syntax.IndirectObjectWord1 = ParserConstants.Verbs[j];
            game.Syntax.IndirectObjectWord2 = ParserConstants.Verbs[j + 1];
            j += 3;

            if (!game.Syntax.IndirectObject.HasFlag(SyntaxObjectFlags.VEBIT))
            {
                return;
            }

            game.Syntax.IndirectObjectFlag1 = game.Syntax.IndirectObjectWord1;
            // !YES.
            game.Syntax.IndirectObjectFlag2 = game.Syntax.IndirectObjectWord2;
        }

        /// <summary>
        /// schlst_ - Search for object
        /// </summary>
        /// <param name="oidx"></param>
        /// <param name="aidx"></param>
        /// <param name="roomId"></param>
        /// <param name="container"></param>
        /// <param name="actorId"></param>
        /// <param name="spcobj"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        private Object SearchForObject(int oidx, int aidx, RoomIds roomId, ObjectIds container, ActorIds actorId, ObjectIds spcobj, Game game)
        {
            var objs = game.Objects.Where(o => ValidateObject(oidx, aidx, o.Value.Id, spcobj, game));

            if (roomId != RoomIds.NoWhere)
            {
                var room = game.Rooms[roomId];

                foreach (var foundObj in objs)
                {
                    if (room.HasObject(foundObj.Value.Id))
                    {
                        return foundObj.Value;
                    }
                }

                return null;
            }

            if (actorId != ActorIds.NoOne)
            {
                foreach (var foundObj in objs)
                {
                    if (foundObj.Value.Adventurer == actorId)
                    {
                        return foundObj.Value;
                    }
                }

                return null;
            }

            return null;
        }

        /// <summary>
        /// thisisit_ - Validate object vs description
        /// </summary>
        /// <param name="oidx"></param>
        /// <param name="aidx"></param>
        /// <param name="obj"></param>
        /// <param name="spcobj"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        private bool ValidateObject(int oidx, int aidx, ObjectIds obj, ObjectIds spcobj, Game game)
        {
            // THE FOLLOWING DATA STATEMENT USED RADIX-50 NOTATION (R50MIN/1RA/)

            // IN RADIX-50 NOTATION, AN "A" IN THE FIRST POSITION IS
            // ENCODED AS 1*40*40 = 1600.
            const int r50min = 1600;

            // !ASSUME NO MATCH.
            bool ret_val = false;
            int i;

            // CHECK FOR OBJECT NAMES
            if (spcobj != 0 && obj == spcobj)
            {
                goto L500;
            }


            i = oidx + 1;
            L100:
            ++i;

            // !IF DONE, LOSE.
            if (ParserConstants.Objects[i - 1] <= 0 || ParserConstants.Objects[i - 1] >= r50min)
            {
                return ret_val;
            }

            // !IF FAIL, CONT.
            if (ParserConstants.Objects[i - 1] != (int)obj)
            {
                goto L100;
            }

            // !ANY ADJ?
            if (aidx == 0)
            {
                goto L500;
            }

            i = aidx + 1;
            L200:
            ++i;

            // !IF DONE, LOSE.
            if (ParserConstants.Adjectives[i - 1] <= 0 || ParserConstants.Adjectives[i - 1] >= r50min)
            {
                return ret_val;
            }

            // !IF FAIL, CONT.
            if (ParserConstants.Adjectives[i - 1] != (int)obj)
            {
                goto L200;
            }

            L500:
            ret_val = true;
            return ret_val;
        }

        /// <summary>
        /// sparse_- Start of parsing
        /// </summary>
        /// <param name="lbuf"></param>
        /// <param name="tokenCount"></param>
        /// <param name="vbflag"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        private int ParseStart(int[] lbuf, int tokenCount, bool vbflag, Game game)
        {
            // DATA R50MIN/1RA/,R50WAL/3RWAL/
            const int r50min = 1600;
            const int r50wal = 36852;

            // !ASSUME PARSE FAILS.
            int ret_val = -1;

            int i, j, adj;
            ObjectIds obj;
            int prep, pptr, lbuf1 = 0, lbuf2 = 0;

            // !CLEAR PARTS HOLDERS.
            adj = 0;

            game.pv_1.act = 0;
            prep = 0;
            pptr = 0;
            game.ObjectVector.o1 = 0;
            game.ObjectVector.o2 = 0;
            game.prpvec.p1 = 0;
            game.prpvec.p2 = 0;

            // NOW LOOP OVER INPUT BUFFER OF LEXICAL TOKENS.
            for (i = 1; i <= tokenCount; i += 2)
            {
                // !TWO WORDS/TOKEN.
                lbuf1 = lbuf[i];
                // !GET CURRENT TOKEN.
                lbuf2 = lbuf[i + 1];

                if (lbuf1 == 0)
                {
                    goto L1500;
                }

                // CHECK FOR BUZZ WORD
                for (j = 1; j <= ParserConstants.Buzzwords.Length; j += 2)
                {
                    if (lbuf1 == ParserConstants.Buzzwords[j - 1] && lbuf2 == ParserConstants.Buzzwords[j])
                    {
                        goto L1000;
                    }
                    // L50:
                }

                // CHECK FOR ACTION OR DIRECTION
                if (game.pv_1.act != 0)
                {
                    goto L75;
                }

                // !GOT ACTION ALREADY?
                j = 1;
                // !CHECK FOR ACTION.
                L125:
                if (lbuf1 == ParserConstants.Verbs[j - 1] && lbuf2 == ParserConstants.Verbs[j])
                {
                    goto ACTION;
                }

                // L150:
                j += 2;

                // !ADV TO NEXT SYNONYM.
                if (!(ParserConstants.Verbs[j - 1] > 0 && ParserConstants.Verbs[j - 1] < r50min))
                {
                    goto L125;
                }

                // !ANOTHER VERB?
                j = j + ParserConstants.Verbs[j - 1] + 1;

                // !NO, ADVANCE OVER SYNTAX.
                if (ParserConstants.Verbs[j - 1] != -1)
                {
                    goto L125;
                }
                // !TABLE DONE?

                L75:
                if (game.pv_1.act != 0 && (ParserConstants.Verbs[game.pv_1.act - 1] != r50wal || prep != 0))
                {
                    goto L200;
                }

                for (j = 1; j <= ParserConstants.Directions.Length; j += 3)
                {
                    // !THEN CHK FOR DIR.
                    if (lbuf1 == ParserConstants.Directions[j - 1] && lbuf2 == ParserConstants.Directions[j])
                    {
                        goto DIRECTION;
                    }
                    // L100:
                }

                // NOT AN ACTION, CHECK FOR PREPOSITION, ADJECTIVE, OR OBJECT.

                L200:
                for (j = 1; j <= ParserConstants.Prepositions.Length; j += 3)
                {
                    // !LOOK FOR PREPOSITION.
                    if (lbuf1 == ParserConstants.Prepositions[j - 1] &&
                        lbuf2 == ParserConstants.Prepositions[j])
                    {
                        goto PREPOSITION;
                    }
                    // L250:
                }

                j = 1;
                // !LOOK FOR ADJECTIVE.
                L300:
                if (lbuf1 == ParserConstants.Adjectives[j - 1] &&
                    lbuf2 == ParserConstants.Adjectives[j])
                {
                    goto ADJECTIVE;
                }

                ++j;
                L325:
                ++j;

                // !ADVANCE TO NEXT ENTRY.
                if (ParserConstants.Adjectives[j - 1] > 0 &&
                    ParserConstants.Adjectives[j - 1] < r50min)
                {
                    goto L325;
                }

                // !A RADIX 50 CONSTANT?
                if (ParserConstants.Adjectives[j - 1] != -1)
                {
                    goto L300;
                }

                // !POSSIBLY, END TABLE?
                j = 1;
                // !LOOK FOR OBJECT.
                L450:
                if (lbuf1 == ParserConstants.Objects[j - 1] && lbuf2 == ParserConstants.Objects[j])
                {
                    goto OBJECTPROCESSING;
                }

                ++j;
                L500:
                ++j;

                if (ParserConstants.Objects[j - 1] > 0 && ParserConstants.Objects[j - 1] < r50min)
                {
                    goto L500;
                }

                if (ParserConstants.Objects[j - 1] != -1)
                {
                    goto L450;
                }

                // NOT RECOGNIZABLE

                if (vbflag)
                {
                    MessageHandler.Speak(601, game);
                }

                return ret_val;
                // SPARSE, PAGE 9

                // OBJECT PROCESSING (CONTINUATION OF DO LOOP ON PREV PAGE)

                OBJECTPROCESSING:
                // !IDENTIFY OBJECT.
                obj = (ObjectIds)FindObjectDescribed(j, adj, 0, game);

                // !IF LE, COULDNT.
                if (obj <= 0)
                {
                    goto UNIDENTIFIABLEOBJECT;
                }

                // !"IT"?
                if (obj != ObjectIds.itobj)
                {
                    goto L650;
                }

                // !FIND LAST.
                obj = (ObjectIds)FindObjectDescribed(0, 0, game.Last.lastit, game);

                // !IF LE, COULDNT.
                if (obj <= 0)
                {
                    goto UNIDENTIFIABLEOBJECT;
                }

                L650:
                if (prep == 9)
                {
                    goto L8000;
                }

                // !"OF" OBJ?
                if (pptr == 2)
                {
                    goto L7000;
                }

                // !TOO MANY OBJS?
                ++pptr;

                if (pptr == 1)
                {
                    game.ObjectVector.o1 = obj;
                }
                else if (pptr == 2)
                {
                    game.ObjectVector.o2 = obj;
                }
                else
                {
                    throw new InvalidOperationException();
                }

                // !STUFF INTO VECTOR.
                if (pptr == 1)
                {
                    game.prpvec.p1 = prep;
                }
                else if (pptr == 2)
                {
                    game.prpvec.p2 = prep;
                }
                else
                {
                    throw new InvalidOperationException();
                }

                L700:
                prep = 0;
                adj = 0;
                // Go to end of do loop (moved "1000 CONTINUE" to end of module, to avoid
                // complaints about people jumping back into the doloop.)
                goto L1000;
                // SPARSE, PAGE 10

                // SPECIAL PARSE PROCESSORS

                // 2000--	DIRECTION

                DIRECTION:

                game.ParserVectors.prsa = VerbId.Walk;
                game.ParserVectors.DirectObject = (ObjectIds)ParserConstants.Directions[j + 1];

                ret_val = 1;
                return ret_val;

                // 3000--	ACTION

                ACTION:
                game.pv_1.act = j;
                game.Orphans.oact = 0;
                goto L1000;

                // 4000--	PREPOSITION

                PREPOSITION:
                if (prep != 0)
                {
                    goto L4500;
                }

                prep = ParserConstants.Prepositions[j + 1];
                adj = 0;
                goto L1000;

                L4500:
                if (vbflag)
                {
                    MessageHandler.Speak(616, game);
                }

                return ret_val;

                // 5000--	ADJECTIVE

                ADJECTIVE:
                adj = j;
                j = game.Orphans.oname & game.Orphans.Flag;
                if (j != 0 && i >= tokenCount)
                {
                    goto OBJECTPROCESSING;
                }

                goto L1000;

                // 6000--	UNIDENTIFIABLE OBJECT (INDEX INTO OVOC IS J)

                UNIDENTIFIABLEOBJECT:
                if (obj < 0)
                {
                    goto L6100;
                }

                j = 579;

                if (RoomHandler.IsRoomLit(game.Player.Here, game))
                {
                    j = 618;
                }

                if (vbflag)
                {
                    MessageHandler.Speak(j, game);
                }

                return ret_val;

                L6100:
                if (obj != (ObjectIds)(-10000))
                {
                    goto L6200;
                }

                if (vbflag)
                {
                    MessageHandler.Speak(620, game.Objects[(ObjectIds)game.Adventurers[game.Player.Winner].VehicleId].ShortDescription, game);
                }

                return ret_val;

                L6200:
                if (vbflag)
                {
                    MessageHandler.Speak(619, game);
                }

                if (game.pv_1.act == 0)
                {
                    game.pv_1.act = game.Orphans.Flag & game.Orphans.oact;
                }

                Orphans.Orphan(-1, game.pv_1.act, game.ObjectVector.o1, prep, j, game);
                return ret_val;

                // 7000--	TOO MANY OBJECTS.

                L7000:
                if (vbflag)
                {
                    MessageHandler.Speak(617, game);
                }

                return ret_val;

                // 8000--	RANDOMNESS FOR "OF" WORDS

                L8000:
                if ((pptr == 1 && game.ObjectVector.o1 == obj) || (pptr == 2 && game.ObjectVector.o2 == obj))
                {
                    {
                        goto L700;
                    }
                }

                if (vbflag)
                {
                    MessageHandler.Speak(601, game);
                }

                return ret_val;

                // End of do-loop.

                L1000:
                ;
            }
            // !AT LAST.

            // NOW SOME MISC CLEANUP -- We fell out of the do-loop

            L1500:
            if (game.pv_1.act == 0)
            {
                game.pv_1.act = game.Orphans.Flag & game.Orphans.oact;
            }

            if (game.pv_1.act == 0)
            {
                goto L9000;
            }

            // !IF STILL NONE, PUNT.
            if (adj != 0)
            {
                goto L10000;
            }

            // !IF DANGLING ADJ, PUNT.
            if (game.Orphans.Flag != 0 &&
                game.Orphans.oprep != 0 &&
                prep == 0 &&
                game.ObjectVector.o1 != 0 &&
                game.ObjectVector.o2 == 0 &&
                game.pv_1.act == game.Orphans.oact)
            {
                goto L11000;
            }

            ret_val = 0;
            // !PARSE SUCCEEDS.
            if (prep == 0)
            {
                goto L1750;
            }

            // !IF DANGLING PREP,
            if (pptr == 1 && game.prpvec.p1 != 0 || (pptr == 2 && game.prpvec.p2 != 0))
            {
                goto L12000;
            }

            if (pptr == 1)
            {
                game.prpvec.p1 = prep;
            }
            else
            {
                game.prpvec.p2 = prep;
            }

            // !CVT TO 'PICK UP FROB'.

            // 1750--	RETURN A RESULT

            L1750:
            // !WIN.
            return ret_val;
            // !LOSE.

            // 9000--	NO ACTION, PUNT

            L9000:
            if (game.ObjectVector.o1 == 0)
            {
                goto L10000;
            }

            // !ANY DIRECT OBJECT?
            if (vbflag)
            {
                MessageHandler.Speak(621, game.Objects[(ObjectIds)game.ObjectVector.o1].ShortDescription, game);
            }

            // !WHAT TO DO?
            Orphans.Orphan(-1, 0, game.ObjectVector.o1, 0, 0, game);
            return ret_val;

            // 10000--	TOTAL CHOMP

            L10000:
            if (vbflag)
            {
                MessageHandler.Speak(622, game);
            }

            // !HUH?
            return ret_val;

            // 11000--	ORPHAN PREPOSITION.  CONDITIONS ARE
            // 		O1.NE.0, O2=0, PREP=0, ACT=OACT

            L11000:
            if (game.Orphans.Slot != 0)
            {
                goto L11500;
            }

            // !ORPHAN OBJECT?
            game.prpvec.p1 = game.Orphans.oprep;
            // !NO, JUST USE PREP.
            goto L1750;

            L11500:
            game.ObjectVector.o2 = game.ObjectVector.o1;

            // !YES, USE AS DIRECT OBJ.
            game.prpvec.p2 = game.Orphans.oprep;
            game.ObjectVector.o1 = game.Orphans.Slot;
            game.prpvec.p1 = 0;
            goto L1750;

            // 12000--	TRUE HANGING PREPOSITION.
            // 		ORPHAN FOR LATER.

            L12000:
            Orphans.Orphan(-1, game.pv_1.act, 0, prep, 0, game);
            // !ORPHAN PREP.
            goto L1750;
        }

        /// <summary>
        /// getobj_ - FIND OBJ DESCRIBED BY ADJ, NAME PAIR
        /// </summary>
        /// <param name="oidx"></param>
        /// <param name="aidx"></param>
        /// <param name="spcobj"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        private int FindObjectDescribed(int oidx, int aidx, ObjectIds spcobj, Game game)
        {
            int ret_val;

            ObjectIds av;
            ObjectIds obj, i;
            ObjectIds nobj;
            bool chomp;

            chomp = false;
            av = (ObjectIds)game.Adventurers[game.Player.Winner].VehicleId;

            obj = 0;

            // !ASSUME DARK.
            // !LIT?
            if (!RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                goto L200;
            }

            // !SEARCH ROOM.
            obj = SearchForObject(oidx, aidx, game.Player.Here, 0, 0, spcobj, game)?.Id ?? ObjectIds.Nothing;

            if (obj < 0)
            {
                goto L1000;
            }
            else if (obj == 0)
            {
                goto L200;
            }
            else
            {
                goto L100;
            }

            // !TEST RESULT.
            L100:
            if (av == 0 || av == obj || (game.Objects[obj].Flag2 & ObjectFlags2.FINDBT) != 0)
            {
                goto L200;
            }

            if (game.Objects[obj].Container == av)
            {
                goto L200;
            }

            // !TEST IF REACHABLE.
            chomp = true;
            // !PROBABLY NOT.

            L200:
            if (av == 0)
            {
                goto L400;
            }

            // !IN VEHICLE?
            nobj = SearchForObject(oidx, aidx, 0, av, 0, spcobj, game).Id;

            // !SEARCH VEHICLE.
            if (nobj < 0)
            {
                goto L1100;
            }
            else if (nobj == 0)
            {
                goto L400;
            }
            else
            {
                goto L300;
            }

            // !TEST RESULT.
            L300:
            chomp = false;
            // !REACHABLE.
            if (obj == nobj)
            {
                goto L400;
            }

            // !SAME AS BEFORE?
            if (obj != 0)
            {
                nobj = (ObjectIds)(-(int)nobj);
            }

            // !AMB RESULT?
            obj = nobj;

            L400:
            // !SEARCH ADVENTURER.
            nobj = SearchForObject(oidx, aidx, 0, 0, game.Player.Winner, spcobj, game)?.Id ?? ObjectIds.Nothing;

            if (nobj < 0)
            {
                goto L1100;
            }
            else if (nobj == 0)
            {
                goto L600;
            }
            else
            {
                goto L500;
            }

            // !TEST RESULT
            L500:
            //if (obj != 0)
            //{
            //    nobj = (ObjectIds)(-(int)nobj);
            //}

            // !AMB RESULT?
            L1100:
            obj = nobj;
            // !RETURN NEW OBJECT.
            L600:
            if (chomp)
            {
                obj = (ObjectIds)(-10000);
            }

            // !UNREACHABLE.
            L1000:
            ret_val = (int)obj;

            // !GOT SOMETHING?
            if (ret_val != 0)
            {
                goto L1500;
            }

            // !NO, SEARCH GLOBALS.
            for (i = (ObjectIds)game.Star.strbit + 1; i <= (ObjectIds)game.Objects.Count; ++i)
            {
                if (!ValidateObject(oidx, aidx, i, spcobj, game))
                {
                    goto L1200;
                }

                if (!GlobalHandler.ghere_(i, game.Player.Here, game))
                {
                    goto L1200;
                }

                // !CAN IT BE HERE?
                if (ret_val != 0)
                {
                    ret_val = -(int)i;
                }

                // !AMB MATCH?
                if (ret_val == 0)
                {
                    ret_val = (int)i;
                }
                L1200:
                ;
            }

            L1500:
            // !END OF SEARCH.
            return ret_val;
        }

        /// <summary>
        /// vappli_ - Main Verb Processing Routine
        /// </summary>
        /// <param name="input"></param>
        /// <param name="verbId"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public bool ProcessVerb(string input, VerbId verbId, Game game)
        {
            const VerbId mxnop = (VerbId)39;
            const VerbId mxsmp = (VerbId)99;

            // !ASSUME WINS.
            bool ret_val = true;

            RoomIds melee;
            bool f;
            ObjectIds av;
            ObjectIds i, j;
            int remark;
            string indirectObjDescription2 = string.Empty;
            string objDescription2 = string.Empty;

            if (game.ParserVectors.DirectObject > (ObjectIds)220)
            {
                goto L5;
            }

            if (game.ParserVectors.DirectObject != ObjectIds.Nothing)
            {
                objDescription2 = game.Objects[game.ParserVectors.DirectObject].ShortDescription;
            }

            // !SET UP DESCRIPTORS.
            L5:
            if (game.ParserVectors.IndirectObject != ObjectIds.Nothing)
            {
                indirectObjDescription2 = game.Objects[game.ParserVectors.IndirectObject].ShortDescription;
            }

            av = (ObjectIds)game.Adventurers[game.Player.Winner].VehicleId;
            remark = game.rnd_(6) + 372;
            // !REMARK FOR HACK-HACKS.

            if (verbId == VerbId.Unknown)
            {
                goto L10;
            }

            if (this.Verbs.TryGetValue(verbId, out var method))
            {
                return method.action.Invoke(game);
            }

            // !ZERO IS FALSE.
            if (verbId <= mxnop)
            {
                return ret_val;
            }

            // !NOP?
            if (verbId <= mxsmp)
            {
                goto L100;
            }

            // !SIMPLE VERB?
            switch (verbId - mxsmp)
            {
                case 02: goto MELT;
                case 03: goto INFLATE;
                case 04: goto DEFLATE;
                case 05: goto ALARM;
                case 06: goto EXORCISE;
                case 07: goto PLUG;
                case 08: goto KICK;
                case 09: goto WAVE;
                case 10: goto RAISE;
                case 11: goto LOWER;
                case 12: goto RUB;
                case 13: goto PUSH;
                case 14: goto UNTIE;
                case 15: goto TIE;
                case 16: goto TIEUP;
                case 17: goto TURN;
                case 18: goto BREATHE;
                case 19: goto KNOCK;
                case 21: goto EXAMINE;
                case 22: goto SHAKE;
                case 23: goto MOVE;
                case 24: goto TURNON;
                case 25: goto TURNOFF;
                case 26: goto OPENVERB;
                case 27: goto CLOSE;
                case 28: goto FIND;
                case 29: goto WAIT;
                case 30: goto SPIN;
                case 31: goto BOARD;
                case 32: goto DISEMBARK;
                case 33: goto TAKE;
                case 34: goto INVENTORY;
                case 35: goto FILL;
                case 36: goto EAT;
                case 37: goto DRINK;
                case 38: goto BURN;
                case 39: goto MUNG;
                case 40: goto KILL;
                case 41: goto SWING;
                case 42: goto ATTACK;
                case 44: goto TELL;
                case 45: goto PUT;
                case 46: goto DROP;
                case 47: goto GIVE;
                case 48: goto POUR;
                case 49: goto THROW;
                case 52: goto HELLO;
                case 53: goto LOOKINTO;
                case 54: goto LOOKUNDER;
                case 55: goto PUMP;
                case 56: goto WIND;
                case 57: goto CLIMB;
                case 58: goto CLIMBUP;
                case 59: goto CLIMBDOWN;
                case 60: goto TURNTO;
            }

            throw new InvalidOperationException("7");

            // ALL VERB PROCESSORS RETURN HERE TO DECLARE FAILURE.

            L10:
            // !LOSE.
            return false;

            // SIMPLE VERBS ARE HANDLED EXTERNALLY.

            L100:
            ret_val = SimpleVerbActions(game, input, verbId);
            return ret_val;
            // VAPPLI, PAGE 3

            // V101--	MELT.  UNLESS OBJECT HANDLES, JOKE.

            MELT:
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(361, objDescription2, game);
            }

            return ret_val;

            // V102--	INFLATE.  WORKS ONLY WITH BOATS.

            INFLATE:
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(368, game);
            }


            return ret_val;

            // V103--	DEFLATE.

            DEFLATE:
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(369, game);
            }


            return ret_val;
            // VAPPLI, PAGE 4

            // V104--	ALARM.  IF SLEEPING, WAKE HIM UP.

            ALARM:
            // !SLEEPING, LET OBJ DO.
            if (!game.Objects[game.ParserVectors.DirectObject].Flag2.HasFlag(ObjectFlags2.IsSleeping))
            {
                goto L24100;
            }

            ret_val = ObjectHandler.ApplyObjectsFromParseVector(game);
            return ret_val;

            L24100:
            // !JOKE.
            MessageHandler.Speak(370, objDescription2, game);

            return ret_val;

            // V105--	EXORCISE.  OBJECTS HANDLE.

            EXORCISE:
            // !OBJECTS HANDLE.
            f = ObjectHandler.ApplyObjectsFromParseVector(game);

            return ret_val;

            // V106--	PLUG.  LET OBJECTS HANDLE.

            PLUG:
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(371, game);
            }

            return ret_val;

            // V107--	KICK.  IF OBJECT IGNORES, JOKE.

            KICK:
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(378, objDescription2, (ObjectIds)remark, game);
            }

            return ret_val;

            // V108--	WAVE.  SAME.

            WAVE:
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(379, objDescription2, (ObjectIds)remark, game);
            }

            return ret_val;

            // V109,V110--	RAISE, LOWER.  SAME.

            RAISE:
            LOWER:
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(380, objDescription2, (ObjectIds)remark, game);
            }

            return ret_val;

            // V111--	RUB.  SAME.

            RUB:
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(381, objDescription2, (ObjectIds)remark, game);
            }

            return ret_val;

            // V112--	PUSH.  SAME.

            PUSH:
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(382, objDescription2, (ObjectIds)remark, game);
            }

            return ret_val;
            // VAPPLI, PAGE 5

            // V113--	UNTIE.  IF OBJECT IGNORES, JOKE.

            UNTIE:
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }

            // !OBJECT HANDLE?
            i = (ObjectIds)383;
            // !NO, NOT TIED.
            if ((game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsTied) == 0)
            {
                i = (ObjectIds)384;
            }

            MessageHandler.Speak(i, game);
            return ret_val;

            // V114--	TIE.  NEVER REALLY WORKS.

            TIE:
            if ((game.Objects[game.ParserVectors.DirectObject].Flag2.HasFlag(ObjectFlags2.IsTied)))
            {
                goto L34100;
            }

            MessageHandler.Speak(385, game);
            // !NOT TIEABLE.
            return ret_val;

            L34100:
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(386, objDescription2, game);
            }
            // !JOKE.
            return ret_val;

            // V115--	TIE UP.  NEVER REALLY WORKS.

            TIEUP:
            if ((game.Objects[game.ParserVectors.IndirectObject].Flag2 & ObjectFlags2.IsTied) != 0)
            {
                goto L35100;
            }

            MessageHandler.Speak(387, objDescription2, game);
            // !NOT TIEABLE.
            return ret_val;

            L35100:
            i = (ObjectIds)388;

            // !ASSUME VILLAIN.
            if ((game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsVillian) == 0)
            {
                i = (ObjectIds)389;
            }

            MessageHandler.Speak((int)i, objDescription2, game);
            // !JOKE.
            return ret_val;

            // V116--	TURN.  OBJECT MUST HANDLE.

            TURN:
            if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.TURNBT) != 0)
            {
                goto L36100;
            }

            MessageHandler.Speak(390, game);
            // !NOT TURNABLE.
            return ret_val;

            L36100:
            if ((game.Objects[game.ParserVectors.IndirectObject].Flag1 & ObjectFlags.IsTool) != 0)
            {
                goto L36200;
            }

            MessageHandler.Speak(391, indirectObjDescription2, game);
            // !NOT A TOOL.
            return ret_val;

            L36200:
            ret_val = ObjectHandler.ApplyObjectsFromParseVector(game);
            // !LET OBJECT HANDLE.
            return ret_val;

            // V117--	BREATHE.  BECOMES INFLATE WITH LUNGS.

            BREATHE:
            game.ParserVectors.prsa = VerbId.Inflate;
            game.ParserVectors.IndirectObject = ObjectIds.lungs;
            goto INFLATE;
            // !HANDLE LIKE INFLATE.

            // V118--	KNOCK.  MOSTLY JOKE.

            KNOCK:
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }


            i = (ObjectIds)394;
            // !JOKE FOR DOOR.
            if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.IsDoor) == 0)
            {
                i = (ObjectIds)395;
            }

            MessageHandler.Speak((int)i, objDescription2, game);
            // !JOKE FOR NONDOORS TOO.
            return ret_val;

            // V119--	LOOK.

            // V120--	EXAMINE.

            EXAMINE:
            if (game.ParserVectors.DirectObject != 0)
            {
                goto L41500;
            }

            // !SOMETHING TO EXAMINE?
            ret_val = RoomHandler.RoomDescription(0, game);
            // !HANDLED BY RMDESC.
            return ret_val;

            L41500:
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }


            i = (ObjectIds)game.Objects[game.ParserVectors.DirectObject].oreadId;
            // !GET READING MATERIAL.
            if (i != 0)
            {
                MessageHandler.Speak(i, game);
            }

            // !OUTPUT IF THERE,
            if (i == 0)
            {
                MessageHandler.Speak(429, objDescription2, game);
            }

            // !OTHERWISE DEFAULT.
            game.ParserVectors.prsa = VerbId.foow;
            // !DEFUSE ROOM PROCESSORS.
            return ret_val;

            // V121--	SHAKE.  IF HOLLOW OBJECT, SOME ACTION.

            SHAKE:
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }

            // !OBJECT HANDLE?
            if ((game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsVillian) == 0)
            {
                goto L42100;
            }

            MessageHandler.Speak(371, game);

            // !JOKE FOR VILLAINS.
            return ret_val;

            L42100:
            if (ObjectHandler.IsObjectEmpty(game.ParserVectors.DirectObject, game) || (game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.IsTakeable) == 0)
            {
                goto L10;
            }

            if ((game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L42300;
            }
            // !OPEN?  SPILL.
            MessageHandler.Speak(396, objDescription2, game);
            // !NO, DESCRIBE NOISE.
            return ret_val;

            L42300:
            MessageHandler.Speak(397, objDescription2, game);
            // !SPILL THE WORKS.

            for (i = (ObjectIds)1; i <= (ObjectIds)game.Objects.Count; ++i)
            {
                // !SPILL CONTENTS.
                if (game.Objects[i].Container != game.ParserVectors.DirectObject)
                {
                    goto L42500;
                }

                // !INSIDE?
                game.Objects[i].Flag2 |= ObjectFlags2.WasTouched;
                if (av == 0)
                {
                    goto L42400;
                }
                // !IN VEHICLE?

                // !YES, SPILL IN THERE.
                ObjectHandler.SetNewObjectStatus(i, 0, 0, av, 0, game);
                goto L42500;

                L42400:
                // !NO, SPILL ON FLOOR,
                ObjectHandler.SetNewObjectStatus(i, 0, game.Player.Here, 0, 0, game);

                // !BUT WATER DISAPPEARS.
                if (i == ObjectIds.Water)
                {
                    ObjectHandler.SetNewObjectStatus(i, 133, 0, 0, 0, game);
                }
                L42500:
                ;
            }

            return ret_val;

            // V122--	MOVE.  MOSTLY JOKES.

            MOVE:
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }


            i = (ObjectIds)398;
            // !ASSUME NOT HERE.
            if (ObjectHandler.IsObjectInRoom(game.ParserVectors.DirectObject, game.Player.Here, game))
            {
                i = (ObjectIds)399;
            }

            MessageHandler.Speak((int)i, objDescription2, game);
            // !JOKE.
            return ret_val;
            // VAPPLI, PAGE 6

            // V123--	TURN ON.

            TURNON:
            // !RECORD IF LIT.
            f = RoomHandler.IsRoomLit(game.Player.Here, game);

            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                goto L44300;
            }

            if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.LITEBT) != 0
                && game.Objects[game.ParserVectors.DirectObject].Adventurer == game.Player.Winner)
            {
                goto L44100;
            }

            MessageHandler.Speak(400, game);
            // !CANT DO IT.
            return ret_val;

            L44100:
            if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.IsOn) == 0)
            {
                goto L44200;
            }
            MessageHandler.Speak(401, game);
            // !ALREADY ON.
            return ret_val;

            L44200:
            game.Objects[game.ParserVectors.DirectObject].Flag1 |= ObjectFlags.IsOn;
            MessageHandler.Speak(404, objDescription2, game);

            L44300:
            // !ROOM NEWLY LIT.
            if (!f && RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                f = RoomHandler.RoomDescription(0, game);
            }
            return ret_val;

            // V124--	TURN OFF.

            TURNOFF:
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                goto L45300;
            }


            if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.LITEBT) != 0 &&
                game.Objects[game.ParserVectors.DirectObject].Adventurer == game.Player.Winner)
            {
                goto L45100;
            }

            MessageHandler.Speak(402, game);
            // !CANT DO IT.
            return ret_val;

            L45100:
            if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.IsOn) != 0)
            {
                goto L45200;
            }

            MessageHandler.Speak(403, game);
            // !ALREADY OFF.
            return ret_val;

            L45200:
            game.Objects[game.ParserVectors.DirectObject].Flag1 &= ~ObjectFlags.IsOn;
            MessageHandler.Speak(405, objDescription2, game);

            L45300:
            if (!RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                MessageHandler.Speak(406, game);
            }

            // !MAY BE DARK.
            return ret_val;

            // V125--	OPEN.  A FINE MESS.
            OPENVERB:
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }

            if ((game.Objects[game.ParserVectors.DirectObject].Flag1.HasFlag(ObjectFlags.CONTBT)))
            {
                goto L46100;
            }

            // !NOT OPENABLE.
            L46050:
            MessageHandler.Speak(407, objDescription2, game);
            return ret_val;

            L46100:
            if (game.Objects[game.ParserVectors.DirectObject].Capacity != 0)
            {
                goto L46200;
            }

            // !NOT OPENABLE.
            MessageHandler.Speak(408, objDescription2, game);
            return ret_val;

            L46200:
            if (!((game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsOpen) != 0))
            {
                goto ISCLOSED;
            }

            // !ALREADY OPEN.
            MessageHandler.Speak(412, game);
            return ret_val;

            ISCLOSED:
            game.Objects[game.ParserVectors.DirectObject].Flag2 |= ObjectFlags2.IsOpen;

            if ((game.Objects[game.ParserVectors.DirectObject].Flag1.HasFlag(ObjectFlags.IsTransparent)) ||
                ObjectHandler.IsObjectEmpty(game.ParserVectors.DirectObject, game))
            {
                goto L46300;
            }

            // !PRINT CONTENTS.
            ObjectHandler.PrintDescription(game.ParserVectors.DirectObject, 410, game);

            return ret_val;

            L46300:
            MessageHandler.Speak(409, game);
            // !DONE
            return ret_val;

            // V126--	CLOSE.

            CLOSE:

            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }

            if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.CONTBT) == 0)
            {
                goto L46050;
            }

            if (game.Objects[game.ParserVectors.DirectObject].Capacity != 0)
            {
                goto L47100;
            }

            // !NOT CLOSABLE.
            MessageHandler.Speak(411, objDescription2, game);
            return ret_val;

            L47100:
            // !OPEN?
            if ((game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L47200;
            }

            // !NO, JOKE.
            MessageHandler.Speak(413, game);

            return ret_val;

            L47200:
            game.Objects[game.ParserVectors.DirectObject].Flag2 &= ~ObjectFlags2.IsOpen;
            MessageHandler.Speak(414, game);
            // !DONE.
            return ret_val;
            // VAPPLI, PAGE 7

            // V127--	FIND.  BIG MEGILLA.

            FIND:

            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }

            // !DEFAULT CASE.
            i = (ObjectIds)415;

            // !IN ROOM?
            if (ObjectHandler.IsObjectInRoom(game.ParserVectors.DirectObject, game.Player.Here, game))
            {
                goto L48300;
            }

            // !ON WINNER?
            if (game.Objects[game.ParserVectors.DirectObject].Adventurer == game.Player.Winner)
            {
                goto L48200;
            }

            // !DOWN ONE LEVEL.
            j = game.Objects[game.ParserVectors.DirectObject].Container;

            if (j == 0)
            {
                goto L10;
            }

            if ((game.Objects[j].Flag1 & ObjectFlags.IsTransparent) == 0 &&
                (!((game.Objects[j].Flag2 & ObjectFlags2.IsOpen) != 0) ||
                (game.Objects[j].Flag1 & (int)ObjectFlags.IsDoor + ObjectFlags.CONTBT) == 0))
            {
                goto L10;
            }

            i = (ObjectIds)417;
            // !ASSUME IN ROOM.
            if (ObjectHandler.IsObjectInRoom(j, game.Player.Here, game))
            {
                goto L48100;
            }

            // !NOT HERE OR ON PERSON.
            if (game.Objects[j].Adventurer != game.Player.Winner)
            {
                goto L10;
            }

            i = (ObjectIds)418;
            L48100:
            // !DESCRIBE FINDINGS.
            MessageHandler.Speak((int)i, game.Objects[j].ShortDescription, game);
            return ret_val;

            L48200:
            i = (ObjectIds)416;
            L48300:
            // !DESCRIBE FINDINGS.
            MessageHandler.Speak((int)i, objDescription2, game);

            return ret_val;

            // V128--	WAIT.  RUN CLOCK DEMON.

            WAIT:
            MessageHandler.Speak(419, game);
            // !TIME PASSES.
            for (i = (ObjectIds)1; i <= (ObjectIds)3; ++i)
            {
                if (ClockEvents.clockd_(game))
                {
                    return ret_val;
                }
                // L49100:
            }
            return ret_val;

            // V129--	SPIN.
            // V159--	TURN TO.

            SPIN:
            TURNTO:
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(663, game);
            }
            // !IF NOT OBJ, JOKE.
            return ret_val;

            // V130--	BOARD.  WORKS WITH VEHICLES.

            BOARD:
            if ((game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsVehicle) != 0)
            {
                goto L51100;
            }
            MessageHandler.Speak(421, objDescription2, game);
            // !NOT VEHICLE, JOKE.
            return ret_val;

            L51100:
            if (ObjectHandler.IsObjectInRoom(game.ParserVectors.DirectObject, game.Player.Here, game))
            {
                goto L51200;
            }

            // !HERE?
            MessageHandler.Speak(420, objDescription2, game);
            // !NO, JOKE.
            return ret_val;

            L51200:
            if (av == 0)
            {
                goto L51300;
            }

            // !ALREADY GOT ONE?
            MessageHandler.Speak(422, objDescription2, game);
            // !YES, JOKE.
            return ret_val;

            L51300:
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }


            MessageHandler.Speak(423, objDescription2, game);
            // !DESCRIBE.

            game.Adventurers[game.Player.Winner].VehicleId = (int)game.ParserVectors.DirectObject;

            if (game.Player.Winner != ActorIds.Player)
            {
                game.Objects[game.Adventurers[game.Player.Winner].ObjectId].Container = game.ParserVectors.DirectObject;
            }

            return ret_val;

            // V131--	DISEMBARK.

            DISEMBARK:
            if (av == game.ParserVectors.DirectObject)
            {
                goto L52100;
            }
            // !FROM VEHICLE?
            MessageHandler.Speak(424, game);
            // !NO, JOKE.
            return ret_val;

            L52100:
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }

            if (game.Rooms[game.Player.Here].Flags.HasFlag(RoomFlags.LAND))
            {
                goto L52200;
            }

            MessageHandler.Speak(425, game);
            // !NOT ON LAND.
            return ret_val;

            L52200:
            game.Adventurers[game.Player.Winner].VehicleId = 0;
            MessageHandler.Speak(426, game);
            if (game.Player.Winner != ActorIds.Player)
            {
                ObjectHandler.SetNewObjectStatus(game.Adventurers[game.Player.Winner].ObjectId, 0, game.Player.Here, 0, 0, game);
            }

            return ret_val;

            // V132--	TAKE.  HANDLED EXTERNALLY.

            TAKE:
            return dverb1.TakeParsedObject(game, true);

            // V133--	INVENTORY.  PROCESSED EXTERNALLY.

            INVENTORY:
            AdventurerHandler.PrintContents(game.Player.Winner, game);
            return ret_val;
            // VAPPLI, PAGE 8

            // V134--	FILL.  STRANGE DOINGS WITH WATER.

            FILL:
            if (game.ParserVectors.IndirectObject != 0)
            {
                goto L56050;
            }

            // !ANY OBJ SPECIFIED?
            if ((game.Rooms[game.Player.Here].Flags & (int)RoomFlags.WATER + RoomFlags.RFILL) != 0)
            {
                goto L56025;
            }

            MessageHandler.Speak(516, game);

            // !NOTHING TO FILL WITH.
            game.ParserVectors.prswon = false;
            // !YOU LOSE.
            return ret_val;

            L56025:
            game.ParserVectors.IndirectObject = ObjectIds.gwate;

            // !USE GLOBAL WATER.
            L56050:
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }

            if (game.ParserVectors.IndirectObject != ObjectIds.gwate && game.ParserVectors.IndirectObject != ObjectIds.Water)
            {
                // TODO: chadj - 8/6 fix this
                //MessageHandler.Speak(444, odi2, objDescription2, game);
            }

            return ret_val;

            // V135,V136--	EAT/DRINK

            EAT:
            DRINK:

            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }

            // !DRINK GLOBAL WATER?
            if (game.ParserVectors.DirectObject == ObjectIds.gwate)
            {
                goto L59500;
            }

            // !EDIBLE?
            if (!((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.IsFood) != 0))
            {
                goto L59400;
            }

            // !YES, ON WINNER?
            if (game.Objects[game.ParserVectors.DirectObject].Adventurer == game.Player.Winner)
            {
                goto L59200;
            }

            L59100:
            // !NOT ACCESSIBLE.
            MessageHandler.Speak(454, objDescription2, game);
            return ret_val;

            L59200:
            // !DRINK FOOD?
            if (game.ParserVectors.prsa == VerbId.Drink)
            {
                goto L59300;
            }

            // !NO, IT DISAPPEARS.
            ObjectHandler.SetNewObjectStatus(game.ParserVectors.DirectObject, 455, 0, 0, 0, game);
            return ret_val;

            L59300:
            // !YES, JOKE.
            MessageHandler.Speak(456, game);
            return ret_val;

            L59400:
            // !DRINKABLE?
            if (!((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.IsDrinkable) != 0))
            {
                goto L59600;
            }

            // !YES, IN SOMETHING?
            if (game.Objects[game.ParserVectors.DirectObject].Container == 0)
            {
                goto L59100;
            }

            if (game.Objects[game.Objects[game.ParserVectors.DirectObject].Container].Adventurer != game.Player.Winner)
            {
                goto L59100;
            }

            if ((game.Objects[game.Objects[game.ParserVectors.DirectObject].Container].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L59500;
            }

            // !CONT OPEN?
            MessageHandler.Speak(457, game);
            // !NO, JOKE.
            return ret_val;

            L59500:
            ObjectHandler.SetNewObjectStatus(game.ParserVectors.DirectObject, 458, 0, 0, 0, game);
            // !GONE.
            return ret_val;

            L59600:
            // !NOT FOOD OR DRINK.
            MessageHandler.Speak(453, objDescription2, game);
            return ret_val;

            // V137--	BURN.  COMPLICATED.

            BURN:
            if (((int)game.Objects[game.ParserVectors.IndirectObject].Flag1 & (int)ObjectFlags.FLAMBT + (int)ObjectFlags.LITEBT + (int)ObjectFlags.IsOn) != ((int)ObjectFlags.FLAMBT + (int)ObjectFlags.LITEBT + (int)ObjectFlags.IsOn))
            {
                goto L60400;
            }

            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }


            if (game.Objects[game.ParserVectors.DirectObject].Container != ObjectIds.recep)
            {
                goto L60050;
            }

            // !BALLOON?
            if (ObjectHandler.DoObjectSpecialAction(game.Objects[ObjectIds.Balloon], 0, game))
            {
                return ret_val;
            }

            // !DID IT HANDLE?
            L60050:
            if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.BURNBT) == 0)
            {
                goto L60300;
            }

            // !CARRYING IT?
            if (game.Objects[game.ParserVectors.DirectObject].Adventurer != game.Player.Winner)
            {
                goto L60100;
            }

            MessageHandler.Speak(459, objDescription2, game);
            AdventurerHandler.jigsup_(game, 460);

            return ret_val;

            L60100:
            // !GET CONTAINER.
            j = game.Objects[game.ParserVectors.DirectObject].Container;

            if (ObjectHandler.IsObjectInRoom(game.ParserVectors.DirectObject, game.Player.Here, game)
                || av != 0
                && j == av)
            {
                goto L60200;
            }

            if (j == 0)
            {
                goto L60150;
            }

            // !INSIDE?
            if (!((game.Objects[j].Flag2 & ObjectFlags2.IsOpen) != 0))
            {
                goto L60150;
            }

            // !OPEN?
            if (ObjectHandler.IsObjectInRoom(j, game.Player.Here, game)
                || av != 0
                && game.Objects[j].Container == av)
            {
                goto L60200;
            }

            L60150:
            MessageHandler.Speak(461, game);
            // !CANT REACH IT.
            return ret_val;

            L60200:
            MessageHandler.Speak(462, objDescription2, game);

            // !BURN IT.
            ObjectHandler.SetNewObjectStatus(game.ParserVectors.DirectObject, 0, 0, 0, 0, game);
            return ret_val;

            L60300:
            MessageHandler.Speak(463, objDescription2, game);
            // !CANT BURN IT.
            return ret_val;

            L60400:
            MessageHandler.Speak(301, indirectObjDescription2, game);
            // !CANT BURN IT WITH THAT.
            return ret_val;
            // VAPPLI, PAGE 9

            // V138--	MUNG.  GO TO COMMON ATTACK CODE.

            MUNG:
            i = (ObjectIds)466;
            // !CHOOSE PHRASE.
            if ((game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsVillian) != 0)
            {
                goto L66100;
            }

            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(466, objDescription2, (ObjectIds)remark, game);
            }

            return ret_val;

            // V139--	KILL.  GO TO COMMON ATTACK CODE.

            KILL:
            i = (ObjectIds)467;
            // !CHOOSE PHRASE.
            goto L66100;

            // V140--	SWING.  INVERT OBJECTS, FALL THRU TO ATTACK.

            SWING:
            j = game.ParserVectors.DirectObject;
            // !INVERT.
            game.ParserVectors.DirectObject = game.ParserVectors.IndirectObject;
            game.ParserVectors.IndirectObject = j;
            // TODO: chadj - 8/6 fix this
            //j = (ObjectIds)objDescription2;
            //objDescription2 = odi2;
            //odi2 = j;
            game.ParserVectors.prsa = VerbId.Attack;
            // !FOR OBJACT.

            // V141--	ATTACK.  FALL THRU TO ATTACK CODE.

            ATTACK:
            i = (ObjectIds)468;

            // COMMON MUNG/ATTACK/SWING/KILL CODE.

            L66100:
            if (game.ParserVectors.DirectObject != 0)
            {
                goto L66200;
            }

            // !ANYTHING?
            MessageHandler.Speak(469, game);
            // !NO, JOKE.
            return ret_val;

            L66200:
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }

            if ((game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsVillian) != 0)
            {
                goto L66300;
            }

            if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.VICTBT) == 0)
            {
                MessageHandler.Speak(470, objDescription2, game);
            }

            return ret_val;

            L66300:
            j = (ObjectIds)471;
            // !ASSUME NO WEAPON.
            if (game.ParserVectors.IndirectObject == 0)
            {
                goto L66500;
            }

            if ((game.Objects[game.ParserVectors.IndirectObject].Flag2 & ObjectFlags2.IsWeapon) == 0)
            {
                goto L66400;
            }

            melee = (RoomIds)1;

            // !ASSUME SWORD.
            if (game.ParserVectors.IndirectObject != ObjectIds.Sword)
            {
                melee = (RoomIds)2;
            }

            // !MUST BE KNIFE.
            i = (ObjectIds)DemonHandler.StrikeBlow(game, ActorIds.Player, game.ParserVectors.DirectObject, (int)melee, true, 0);
            // !STRIKE BLOW.
            return ret_val;

            L66400:
            j = (ObjectIds)472;
            // !NOT A WEAPON.
            L66500:
            MessageHandler.Speak((int)i, objDescription2, j, game);
            // !JOKE.
            return ret_val;
            // VAPPLI, PAGE 10

            // V143--	TELL.  PROCESSED IN GAME.

            TELL:
            MessageHandler.Speak(603, game);
            return ret_val;

            // V144--	PUT.  PROCESSED EXTERNALLY.

            PUT:
            ret_val = dverb1.put_(game, true);
            return ret_val;

            // V145,V146,V147,V148--	DROP/GIVE/POUR/THROW

            DROP:
            GIVE:
            POUR:
            THROW:
            ret_val = dverb1.drop_(game, false);
            return ret_val;

            // VAPPLI, PAGE 11

            // V151--	HELLO

            HELLO:
            if (game.ParserVectors.DirectObject != 0)
            {
                goto L80100;
            }

            // !ANY OBJ?
            // !NO, VANILLA HELLO.
            MessageHandler.Speak(game.rnd_(4) + 346, game);
            return ret_val;

            L80100:
            // !HELLO AVIATOR?
            if (game.ParserVectors.DirectObject != ObjectIds.Aviator)
            {
                goto L80200;
            }

            // !NOTHING HAPPENS.
            MessageHandler.Speak(350, game);
            return ret_val;

            L80200:
            // !HELLO SAILOR?
            if (game.ParserVectors.DirectObject != ObjectIds.Sailor)
            {
                goto L80300;
            }

            // !COUNT.
            ++game.State.HelloSailor;
            i = (ObjectIds)351;

            // !GIVE NORMAL OR
            if (game.State.HelloSailor % 10 == 0)
            {
                i = (ObjectIds)352;
            }

            // !RANDOM MESSAGE.
            if (game.State.HelloSailor % 20 == 0)
            {
                i = (ObjectIds)353;
            }

            // !SPEAK UP.
            MessageHandler.Speak(i, game);
            return ret_val;

            L80300:
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }


            i = (ObjectIds)354;
            // !ASSUME VILLAIN.
            if ((game.Objects[game.ParserVectors.DirectObject].Flag2 & (int)ObjectFlags2.IsVillian + ObjectFlags2.IsActor) == 0)
            {
                i = (ObjectIds)355;
            }

            // !HELLO THERE
            MessageHandler.Speak((int)i, objDescription2, game);
            // !
            return ret_val;

            // V152--	LOOK INTO

            LOOKINTO:
            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }


            if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.IsDoor) == 0)
            {
                goto L81300;
            }

            if (!((game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsOpen) != 0))
            {
                goto L81200;
            }

            // !OPEN?
            // !OPEN DOOR- UNINTERESTING.
            MessageHandler.Speak(628, objDescription2, game);
            return ret_val;

            L81200:
            // !CLOSED DOOR- CANT SEE.
            MessageHandler.Speak(525, objDescription2, game);
            return ret_val;

            L81300:
            if ((game.Objects[game.ParserVectors.DirectObject].Flag2.HasFlag(ObjectFlags2.IsVehicle)))
            {
                goto L81400;
            }

            if (game.Objects[game.ParserVectors.DirectObject].Flag2.HasFlag(ObjectFlags2.IsOpen) ||
                game.Objects[game.ParserVectors.DirectObject].Flag1.HasFlag(ObjectFlags.IsTransparent))
            {
                goto L81400;
            }

            if (game.Objects[game.ParserVectors.DirectObject].Flag1.HasFlag(ObjectFlags.CONTBT))
            {
                goto L81200;
            }

            // !CANT LOOK INSIDE.
            MessageHandler.Speak(630, objDescription2, game);
            return ret_val;

            L81400:
            // !VEH OR SEE IN.  EMPTY?
            if (ObjectHandler.IsObjectEmpty(game.ParserVectors.DirectObject, game))
            {
                goto L81500;
            }

            // !NO, LIST CONTENTS.
            ObjectHandler.PrintDescription(game.ParserVectors.DirectObject, 573, game);
            return ret_val;

            L81500:
            // !EMPTY.
            MessageHandler.Speak(629, objDescription2, game);
            return ret_val;

            // V153--	LOOK UNDER

            LOOKUNDER:
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(631, game);
            }

            // !OBJECT HANDLE?
            return ret_val;
            // VAPPLI, PAGE 12

            // V154--	PUMP

            PUMP:
            if (RoomHandler.GetRoomThatContainsObject(ObjectIds.Pump, game).Id == game.Player.Here ||
                game.Objects[ObjectIds.Pump].Adventurer == game.Player.Winner)
            {
                goto L83100;
            }

            // !NO.
            MessageHandler.Speak(632, game);

            return ret_val;

            L83100:
            // !BECOMES INFLATE
            game.ParserVectors.IndirectObject = ObjectIds.Pump;

            // !X WITH PUMP.
            game.ParserVectors.prsa = VerbId.Inflate;

            goto INFLATE;
            // !DONE.

            // V155--	WIND

            WIND:
            if (!ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                MessageHandler.Speak(634, objDescription2, game);
            }


            return ret_val;

            // V156--	CLIMB
            // V157--	CLIMB UP
            // V158--	CLIMB DOWN

            CLIMB:
            CLIMBUP:
            CLIMBDOWN:
            // !ASSUME UP.
            i = (ObjectIds)XSearch.xup;

            // !UNLESS CLIMB DN.
            if (game.ParserVectors.prsa == VerbId.ClimbDown)
            {
                i = (ObjectIds)XSearch.xdown;
            }

            // !ANYTHING TO CLIMB?
            f = (game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsClimbable) != 0;
            if (f && dso3.FindExit(game, (int)i, game.Player.Here))
            {
                goto L87500;
            }

            if (ObjectHandler.ApplyObjectsFromParseVector(game))
            {
                return ret_val;
            }


            i = (ObjectIds)657;
            if (f)
            {
                i = (ObjectIds)524;
            }

            // !VARIETY OF JOKES.
            if (!f && (game.ParserVectors.DirectObject == ObjectIds.Wall
                || game.ParserVectors.DirectObject >= ObjectIds.wnort
                && game.ParserVectors.DirectObject <= ObjectIds.wnort + 3))
            {
                i = (ObjectIds)656;
            }

            // !JOKE.
            MessageHandler.Speak(i, game);
            return ret_val;

            L87500:
            // !WALK
            game.ParserVectors.prsa = VerbId.Walk;
            // !IN SPECIFIED DIR.
            game.ParserVectors.DirectObject = i;

            ret_val = dverb2.walk_(game);
            return ret_val;
        }

        public bool SimpleVerbActions(Game game, string input, VerbId ri)
        {
            VerbId mxnop = (VerbId)39;
            VerbId mxjoke = (VerbId)64;
            int [] jokes = new int [] { 4, 5, 3, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 5314, 5319, 324, 325, 883, 884, 120, 120, 0, 0, 0, 0 };
            int [] answer = new int [] { 0, 1, 2, 3, 4, 4, 4, 4, 5, 5, 5, 6, 7, 7 };
            string[] ansstr = new string[]
           { "TEMPLE", "FOREST", "30003", "FLASK", "RUB", "FONDLE",
      "CARRES", "TOUCH", "BONES", "BODY", "SKELE", "RUSTYKNIFE",
      "NONE", "NOWHER" };

            int i__1, i__2;
            bool ret_val;
            bool f;
            char z;
            char z2;
            int i, j;
            int k;
            int l;
            char [] ch = new char[1 * 6];
            int cp, wp;
            char [] pp1 = new char[1 * 6];
            char [] pp2 = new char[1 * 6];
            string odi2 = string.Empty;
            string odo2 = string.Empty;

            ret_val = true;
            // !ASSUME WINS.
            if (game.ParserVectors.DirectObject != 0)
            {
                odo2 = game.Objects[game.ParserVectors.DirectObject].ShortDescription;
            }

            // !SET UP DESCRIPTORS.
            if (game.ParserVectors.IndirectObject != 0)
            {
                odi2 = game.Objects[game.ParserVectors.IndirectObject].ShortDescription;
            }

            if (ri == 0)
            {
                throw new InvalidOperationException();
                // bug_(7, ri);
            }

            // !ZERO IS VERBOTEN.
            if (ri <= mxnop)
            {
                return ret_val;
            }

            // !NOP?
            if (ri <= mxjoke)
            {
                goto L100;
            }

            // !JOKE?
            switch (ri - mxjoke)
            {
                case 1: goto ROOM;
                case 2: goto OBJECTS;
                case 3: goto RNAME;
                case 4: goto RESERVED1;
                case 5: goto RESERVED2;
                case 6: goto BRIEF;
                case 7: goto VERBOSE;
                case 8: goto SUPERBRIEF;
                case 9: goto STAY;
                case 10: goto VERSION;
                case 11: goto SWIM;
                case 12: goto GERONIMO;
                case 13: goto SINBAD;
                case 14: goto L9000;
                case 15: goto PRAY;
                case 16: goto TREASURE;
                case 17: goto TEMPLE;
                case 18: goto BLAST;
                case 19: goto SCORE;
                case 20: goto QUIT;
                case 21: goto FOLLOW;
                case 22: goto L17000;
                case 23: goto RING;
                case 24: goto BRUSH;
                case 25: goto DIG;
                case 26: goto TIME;
                case 27: goto LEAP;
                case 28: goto LOCK;
                case 29: goto UNLOCK;
                case 30: goto DIAGNOSE;
                case 31: goto L26000;
                case 32: goto ANSWER;
            }

            throw new InvalidOperationException();
            //bug_(7, ri);

            // ALL VERB PROCESSORS RETURN HERE TO DECLARE FAILURE.

            // L10:
            ret_val = false;
            // !LOSE.
            return ret_val;

            // JOKE PROCESSOR.
            // FIND PROPER ENTRY IN JOKES, USE IT TO SELECT STRING TO PRINT.

            L100:
            i = jokes[ri - mxnop - 1];
            // !GET TABLE ENTRY.
            j = i / 1000;
            // !ISOLATE # STRINGS.
            if (j != 0)
            {
                i = i % 1000 + game.rnd_(j);
            }

            // !IF RANDOM, CHOOSE.
            MessageHandler.Speak(i, game);
            // !PRINT JOKE.
            return ret_val;
            // SVERBS, PAGE 2A

            // V65--	ROOM

            ROOM:
            // !DESCRIBE ROOM ONLY.
            ret_val = RoomHandler.RoomDescription(game, Verbosity.RoomAndContents);
            return ret_val;

            // V66--	OBJECTS

            OBJECTS:
            // !DESCRIBE OBJ ONLY.
            ret_val = RoomHandler.RoomDescription(game, Verbosity.ObjectsOnly);
            if (!game.Player.TelFlag)
            {
                MessageHandler.Speak(138, game);
            }
            // !NO OBJECTS.
            return ret_val;

            // V67--	RNAME

            RNAME:
            i__1 = game.Rooms[game.Player.Here].Description2Id;
            // !SHORT ROOM NAME.
            MessageHandler.Speak(i__1, game);
            game.WriteOutput($"(RoomId: {game.Player.Here} or {game.Player.Here:d}){Environment.NewLine}");

            return ret_val;

            // V68--	RESERVED

            RESERVED1:
            return ret_val;

            // V69--	RESERVED

            RESERVED2:
            return ret_val;
            // SVERBS, PAGE 3

            // V70--	BRIEF.  SET FLAG.

            BRIEF:
            // !BRIEF DESCRIPTIONS.
            game.Flags.BriefDescriptions = true;
            game.Flags.SuperBriefDescriptions = false;

            MessageHandler.Speak(326, game);
            return ret_val;

            // V71--	VERBOSE.  CLEAR FLAGS.

            VERBOSE:
            // !LONG DESCRIPTIONS.
            game.Flags.BriefDescriptions = false;
            game.Flags.SuperBriefDescriptions = false;
            MessageHandler.Speak(327, game);
            return ret_val;

            // V72--	SUPERBRIEF.  SET FLAG.

            SUPERBRIEF:
            game.Flags.SuperBriefDescriptions = true;
            MessageHandler.Speak(328, game);
            return ret_val;

            // V73-- STAY (USED IN ENDGAME).

            STAY:
            // !TELL MASTER, STAY.
            if (game.Player.Winner != ActorIds.Master)
            {
                goto L4100;
            }

            // !HE DOES.
            MessageHandler.Speak(781, game);
            // !NOT FOLLOWING.
            game.Clock[ClockId.cevfol].Ticks = 0;

            return ret_val;

            L4100:
            if (game.Player.Winner == ActorIds.Player)
            {
                MessageHandler.Speak(664, game);
            }

            // !JOKE.
            return ret_val;

            // V74--	VERSION.  PRINT INFO.

            VERSION:
            MessageHandler.more_output(game, string.Empty);
//            game.WriteOutputLine("V%1d.%1d%c\n", vers_1.vmaj, vers_1.vmin, vers_1.vedit);
            game.Player.TelFlag = true;
            return ret_val;

            // V75--	SWIM.  ALWAYS A JOKE.

            SWIM:
            i = 330;
            // !ASSUME WATER.
            if ((game.Rooms[game.Player.Here].Flags & (int)RoomFlags.WATER + RoomFlags.RFILL) == 0)
            {
                i = game.rnd_(3) + 331;
            }

            MessageHandler.Speak(i, game);
            return ret_val;

            // V76--	GERONIMO.  IF IN BARREL, FATAL, ELSE JOKE.

            GERONIMO:
            // !IN BARREL?
            if (game.Player.Here == RoomIds.Barrel)
            {
                goto L7100;
            }

            // !NO, JOKE.
            MessageHandler.Speak(334, game);

            return ret_val;

            L7100:
            AdventurerHandler.jigsup_(game, 335);
            // !OVER FALLS.
            return ret_val;

            // V77--	SINBAD ET AL.  CHASE CYCLOPS, ELSE JOKE.

            SINBAD:
            if (game.Player.Here == RoomIds.Cyclops &&
                ObjectHandler.IsObjectInRoom(ObjectIds.Cyclops, game.Player.Here, game))
            {
                goto L8100;
            }

            MessageHandler.Speak(336, game);
            // !NOT HERE, JOKE.
            return ret_val;

            L8100:
            // !CYCLOPS FLEES.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Cyclops, 337, 0, 0, 0, game);

            // !SET ALL FLAGS.
            game.Flags.cyclof = true;
            game.Flags.IsDoorToCyclopsRoomOpen = true;

            game.Objects[ObjectIds.Cyclops].Flag2 &= ~ObjectFlags2.IsFighting;
            return ret_val;

            // V78--	WELL.  OPEN DOOR, ELSE JOKE.

            L9000:
            // !IN RIDDLE ROOM?
            if (game.Flags.WasRiddleSolved || game.Player.Here != RoomIds.Riddle)
            {
                goto L9100;
            }

            // !YES, SOLVED IT.
            game.Flags.WasRiddleSolved = true;

            MessageHandler.Speak(338, game);
            return ret_val;

            L9100:
            MessageHandler.Speak(339, game);
            // !WELL, WHAT?
            return ret_val;

            // V79--	PRAY.  IF IN TEMP2, POOF
            // !

            PRAY:
            // !IN TEMPLE?
            if (game.Player.Here != RoomIds.Temple2)
            {
                goto L10050;
            }

            // !FORE1 STILL THERE?
            if (AdventurerHandler.MoveToNewRoom(game, RoomIds.Forest1, game.Player.Winner))
            {
                goto L10100;
            }

            L10050:
            // !JOKE.
            MessageHandler.Speak(340, game);

            return ret_val;

            L10100:
            f = RoomHandler.RoomDescription(Verbosity.Full, game);
            // !MOVED, DESCRIBE.
            return ret_val;

            // V80--	TREASURE.  IF IN TEMP1, POOF
            // !

            TREASURE:
            // !IN TEMPLE?
            if (game.Player.Here != RoomIds.Temple1)
            {
                goto L11050;
            }

            // !TREASURE ROOM THERE?
            if (AdventurerHandler.MoveToNewRoom(game, RoomIds.Treasure, game.Player.Winner))
            {
                goto L10100;
            }

            L11050:
            // !NOTHING HAPPENS.
            MessageHandler.Speak(341, game);

            return ret_val;

            // V81--	TEMPLE.  IF IN TREAS, POOF
            // !

            TEMPLE:
            if (game.Player.Here != RoomIds.Treasure)
            {
                goto L12050;
            }

            // !IN TREASURE?
            if (AdventurerHandler.MoveToNewRoom(game, RoomIds.Temple1, game.Player.Winner))
            {
                goto L10100;
            }
            // !TEMP1 STILL THERE?
            L12050:
            MessageHandler.Speak(341, game);
            // !NOTHING HAPPENS.
            return ret_val;

            // V82--	BLAST.  USUALLY A JOKE.

            BLAST:
            i = 342;
            // !DONT UNDERSTAND.
            if (game.ParserVectors.DirectObject == ObjectIds.safe)
            {
                i = 252;
            }
            // !JOKE FOR SAFE.
            MessageHandler.Speak(i, game);
            return ret_val;

            // V83--	SCORE.  PRINT SCORE.

            SCORE:
            AdventurerHandler.PrintScore(game, false);
            return ret_val;

            // V84--	QUIT.  FINISH OUT THE GAME.

            QUIT:
            // !TELLL SCORE.
            AdventurerHandler.PrintScore(game, true);
            // !ASK FOR Y/N DECISION.
            if (!MessageHandler.AskYesNoQuestion(game, 343, 0, 0))
            {
                return ret_val;
            }

            // !BYE.
            game.Exit();

            // SVERBS, PAGE 4

            // V85--	FOLLOW (USED IN ENDGAME)

            FOLLOW:
            if (game.Player.Winner != ActorIds.Master)
            {
                return ret_val;
            }

            // !TELL MASTER, FOLLOW.
            MessageHandler.Speak(782, game);
            game.Clock[ClockId.cevfol].Ticks = -1;
            // !STARTS FOLLOWING.
            return ret_val;

            // V86--	WALK THROUGH

            L17000:
            if (game.Screen.scolrm == 0
                || game.ParserVectors.DirectObject != ObjectIds.ScreenOfLight
                && (game.ParserVectors.DirectObject != ObjectIds.wnort
                || game.Player.Here != RoomIds.bkbox))
            {
                goto L17100;
            }

            // !WALKED THRU SCOL.
            game.Screen.scolac = game.Screen.scolrm;
            // !FAKE OUT FROMDR.
            game.ParserVectors.DirectObject = 0;

            // !START ALARM.
            game.Clock[ClockId.cevscl].Ticks = 6;

            // !DISORIENT HIM.
            MessageHandler.Speak(668, game);

            // !INTO ROOM.
            f = AdventurerHandler.MoveToNewRoom(game, game.Screen.scolrm, game.Player.Winner);

            // !DESCRIBE.
            f = RoomHandler.RoomDescription(game, Verbosity.Full);
            return ret_val;

            L17100:
            if (game.Player.Here != game.Screen.scolac)
            {
                goto L17300;
            }

            // !ON OTHER SIDE OF SCOL?
            for (i = 1; i <= 12; i += 3)
            {
                // !WALK THRU PROPER WALL?
                if (game.Screen.scolwl[i - 1] == (int)game.Player.Here && game.Screen.scolwl[i] == (int)game.ParserVectors.DirectObject)
                {
                    goto L17500;
                }
                // L17200:
            }

            L17300:
            if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.IsTakeable) != 0)
            {
                goto L17400;
            }

            i = 669;
            // !NO, JOKE.
            if (game.ParserVectors.DirectObject == ObjectIds.ScreenOfLight)
            {
                i = 670;
            }

            // !SPECIAL JOKE FOR SCOL.
            MessageHandler.Speak((int)i, odo2, game);
            return ret_val;

            L17400:
            i = 671;
            // !JOKE.
            if (RoomHandler.GetRoomThatContainsObject(game.ParserVectors.DirectObject, game).Id != 0)
            {
                i = game.rnd_(5) + 552;
            }

            // !SPECIAL JOKES IF CARRY.
            MessageHandler.Speak(i, game);
            return ret_val;

            L17500:
            game.ParserVectors.DirectObject = (ObjectIds)game.Screen.scolwl[i + 1];
            // !THRU SCOL WALL...
            for (i = 1; i <= 8; i += 2)
            {
                // !FIND MATCHING ROOM.
                if (game.ParserVectors.DirectObject == (ObjectIds)game.Screen.scoldr[i - 1])
                {
                    game.Screen.scolrm = game.Screen.scoldr[i];
                }
                // L17600:
            }

            // !DECLARE NEW SCOLRM.
            // !CANCEL ALARM.
            game.Clock[ClockId.cevscl].Ticks = 0;
            // !DISORIENT HIM.
            MessageHandler.Speak(668, game);
            // !BACK IN BOX ROOM.
            f = AdventurerHandler.MoveToNewRoom(game, RoomIds.bkbox, game.Player.Winner);
            f = RoomHandler.RoomDescription(game, Verbosity.Full);

            return ret_val;

            // V87--	RING.  A JOKE.

            RING:
            i = 359;
            // !CANT RING.
            if (game.ParserVectors.DirectObject == ObjectIds.Bell)
            {
                i = 360;
            }

            // !DING, DONG.
            // !JOKE.
            MessageHandler.Speak(i, game);
            return ret_val;

            // V88--	BRUSH.  JOKE WITH OBSCURE TRAP.

            BRUSH:
            if (game.ParserVectors.DirectObject == ObjectIds.Teeth)
            {
                goto L19100;
            }

            // !BRUSH TEETH?
            // !NO, JOKE.
            MessageHandler.Speak(362, game);

            return ret_val;

            L19100:
            if (game.ParserVectors.IndirectObject != 0)
            {
                goto L19200;
            }

            // !WITH SOMETHING?
            // !NO, JOKE.
            MessageHandler.Speak(363, game);
            return ret_val;

            L19200:
            if (game.ParserVectors.IndirectObject == ObjectIds.Putty &&
                game.Objects[ObjectIds.Putty].Adventurer == game.Player.Winner)
            {
                goto L19300;
            }

            // !NO, JOKE.
            MessageHandler.Speak(364, odi2, game);
            return ret_val;

            L19300:
            AdventurerHandler.jigsup_(game, 365);
            // !YES, DEAD
            // !
            // !
            // !
            // !
            // !
            return ret_val;
            // SVERBS, PAGE 5

            // V89--	DIG.  UNLESS SHOVEL, A JOKE.

            DIG:
            if (game.ParserVectors.DirectObject == ObjectIds.Shovel)
            {
                return ret_val;
            }

            // !SHOVEL?
            i = 392;
            // !ASSUME TOOL.
            if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.IsTool) == 0)
            {
                i = 393;
            }

            MessageHandler.Speak(i, odo2, game);
            return ret_val;

            // V90--	TIME.  PRINT OUT DURATION OF GAME.

            TIME:
            // !GET PLAY TIME.
            //gttime_(k);
            k = DateTime.Now.Minute;
            i = k / 60;
            j = k % 60;

            MessageHandler.more_output(game, string.Empty);
            game.WriteOutput("You have been playing Dungeon for ");
            if (i >= 1)
            {
                game.WriteOutput($"{i} hour");
                if (i >= 2)
                {
                    game.WriteOutput("s");
                }

                game.WriteOutput(" and ");
            }

            game.WriteOutput($"{j} minute");
            if (j != 1)
            {
                game.WriteOutput("s");
            }

            game.WriteOutput(".\n");
            game.Player.TelFlag = true;
            return ret_val;


            // V91--	LEAP.  USUALLY A JOKE, WITH A CATCH.

            LEAP:
            if (game.ParserVectors.DirectObject == 0) {
                goto L22200;
            }
            // !OVER SOMETHING?
            if (ObjectHandler.IsObjectInRoom(game.ParserVectors.DirectObject, game.Player.Here, game))
            {
                goto L22100;
            }
            // !HERE?
            MessageHandler.Speak(447, game);
            // !NO, JOKE.
            return ret_val;

            L22100:
            if ((game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsVillian) == 0)
            {
                goto L22300;
            }

            // !CANT JUMP VILLAIN.
            MessageHandler.Speak(448, odo2, game);
            return ret_val;

            L22200:
            if (!dso3.FindExit(game, (int)XSearch.xdown, game.Player.Here)) {
                goto L22300;
            }

            // !DOWN EXIT?
            if (game.CurrentExit.ExitType == xpars_.xno || game.CurrentExit.ExitType == xpars_.xcond)// && !game.Flags[xflag - 1])
            {
                goto L22400;
            }

            L22300:
            i__1 = game.rnd_(5) + 314;
            MessageHandler.Speak(i__1, game);
            // !WHEEEE
            // !
            return ret_val;

            L22400:
            i__1 = game.rnd_(4) + 449;
            AdventurerHandler.jigsup_(game, i__1);
            // !FATAL LEAP.
            return ret_val;
            // SVERBS, PAGE 6

            // V92--	LOCK.

            LOCK:
            if (game.ParserVectors.DirectObject == ObjectIds.Grate &&
                game.Player.Here == RoomIds.mgrat)
            {
                goto L23200;
            }

            L23100:
            // !NOT LOCK GRATE.
            MessageHandler.Speak(464, game);
            return ret_val;

            L23200:
            // !GRATE NOW LOCKED.
            game.Flags.IsGrateUnlocked = false;

            MessageHandler.Speak(214, game);
            // !CHANGE EXIT STATUS.
            game.Exits.Travel[game.Rooms[game.Player.Here].Exit] = 214;
            return ret_val;

            // V93--	UNLOCK

            UNLOCK:
            if (game.ParserVectors.DirectObject != ObjectIds.Grate ||
                game.Player.Here != RoomIds.mgrat)
            {
                goto L23100;
            }

            // !GOT KEYS?
            if (game.ParserVectors.IndirectObject == ObjectIds.Keys)
            {
                goto L24200;
            }

            // !NO, JOKE.
            MessageHandler.Speak(465, odi2, game);

            return ret_val;

            L24200:
            // !UNLOCK GRATE.
            game.Flags.IsGrateUnlocked = true;
            MessageHandler.Speak(217, game);

            // !CHANGE EXIT STATUS.
            game.Exits.Travel[game.Rooms[game.Player.Here].Exit] = 217;
            return ret_val;

            // V94--	DIAGNOSE.

            DIAGNOSE:
            // !GET FIGHTS STRENGTH.
            i = dso4.ComputeFightStrength(game, game.Player.Winner, false);
            // !GET HEALTH.
            j = game.Adventurers[game.Player.Winner].Strength;
            i__1 = i + j;
            // Computing MIN
            k = Math.Min(i__1, 4);

            // !GET STATE.
            if (!game.Clock[ClockId.cevcur].Flags)
            {
                j = 0;
            }

            // !IF NO WOUNDS.
            // Computing MIN
            i__1 = 4;
            i__2 = Math.Abs(j);
            l = Math.Min(i__1, i__2);
            // !SCALE.
            i__1 = l + 473;
            // !DESCRIBE HEALTH.
            MessageHandler.Speak(i__1, game);

            // !COMPUTE WAIT.
            i = (-j - 1) * 30 + game.Clock[ClockId.cevcur].Ticks;

            if (j != 0)
            {
                MessageHandler.more_output(game, string.Empty);
                game.WriteOutput($"You will be cured after {i} moves.{Environment.NewLine}");
            }

            i__1 = k + 478;
            // !HOW MUCH MORE?
            MessageHandler.Speak(i__1, game);
            // !HOW MANY DEATHS?
            if (game.State.Deaths != 0)
            {
                i__1 = game.State.Deaths + 482;
                MessageHandler.Speak(i__1, game);
            }

            return ret_val;
            // SVERBS, PAGE 7

            // V95--	INCANT

            L26000:
            for (i = 1; i <= 6; ++i)
            {
                // !SET UP PARSE.
                pp1[i - 1] = ' ';
                pp2[i - 1] = ' ';
                // L26100:
            }

            wp = 1;
            // !WORD POINTER.
            cp = 1;
            // !CHAR POINTER.
            if (game.ParserVectors.prscon <= 1)
            {
                goto L26300;
            }

            for (z = (char)(input[0] + game.ParserVectors.prscon - 1); z != '\0'; ++z)
            {
                // !PARSE INPUT
                if (z == ',')
                {
                    goto L26300;
                }

                // !END OF PHRASE?
                if (z != ' ')
                {
                    goto L26150;
                }

                // !SPACE?
                if (cp != 1)
                {
                    ++wp;
                }

                cp = 1;
                goto L26200;
                L26150:
                if (wp == 1)
                {
                    pp1[cp - 1] = z;
                }
                // !STUFF INTO HOLDER.
                if (wp == 2) {
                    pp2[cp - 1] = z;
                }

                // Computing MIN
                i__2 = cp + 1;
                cp = Math.Min(i__2, 6);
                L26200:
                ;
            }

            L26300:
            // !KILL REST OF LINE.
            game.ParserVectors.prscon = 1;
            if (pp1[0] != ' ')
            {
                goto L26400;
            }

            // !ANY INPUT?
            MessageHandler.Speak(856, game);
            // !NO, HO HUM.
            return ret_val;

            L26400:
            dso7.encryp_(game, pp1, ch);
            // !COMPUTE RESPONSE.
            if (pp2[0] != ' ')
            {
                goto L26600;
            }

            // !TWO PHRASES?

            if (game.Flags.spellf)
            {
                goto L26550;
            }

            // !HE'S TRYING TO LEARN.
            if ((game.Rooms[RoomIds.tstrs].Flags & RoomFlags.SEEN) == 0)
            {
                goto L26575;
            }

            game.Flags.spellf = true;
            // !TELL HIM.
            game.Player.TelFlag = true;
            MessageHandler.more_output(game, string.Empty);
            game.WriteOutput($"A hollow voice replies: \"{pp1} {ch} {Environment.NewLine}\"");//%.6s %.6s\".\n", pp1, ch);

            return ret_val;

            L26550:
            // !HE'S GOT ONE ALREADY.
            MessageHandler.Speak(857, game);
            return ret_val;

            L26575:
            // !HE'S NOT IN ENDGAME.
            MessageHandler.Speak(858, game);
            return ret_val;

            L26600:
            if ((game.Rooms[RoomIds.tstrs].Flags & RoomFlags.SEEN) != 0)
            {
                goto L26800;
            }

            for (i = 1; i <= 6; ++i)
            {
                if (pp2[i - 1] != ch[i - 1])
                {
                    goto L26575;
                }
                // !WRONG.
                // L26700:
            }

            game.Flags.spellf = true;
            // !IT WORKS.
            MessageHandler.Speak(859, game);
            game.Clock[ClockId.cevste].Ticks = 1;
            // !FORCE START.
            return ret_val;

            L26800:
            MessageHandler.Speak(855, game);
            // !TOO LATE.
            return ret_val;
            // SVERBS, PAGE 8

            // V96--	ANSWER

            ANSWER:
            if (game.ParserVectors.prscon > 1 &&
                game.Player.Here == RoomIds.FrontDoor &&
                game.Flags.inqstf)
            {
                goto L27100;
            }

            MessageHandler.Speak(799, game);
            // !NO ONE LISTENS.
            game.ParserVectors.prscon = 1;
            return ret_val;

            L27100:
            for (j = 1; j <= 14; j++)
            {
                // !CHECK ANSWERS.
                if (game.Switches.quesno != answer[j - 1])
                {
                    goto L27300;
                }

                // !ONLY CHECK PROPER ANS.
                z = ansstr[j - 1][0];
                z2 = (char)(input[0] + game.ParserVectors.prscon - 1);

                while (z != '\0')
                {
                    while (z2 == ' ')
                    {
                        z2++;
                    }

                    // !STRIP INPUT BLANKS.
                    if (z++ != z2++)
                    {
                        goto L27300;
                    }
                }

                goto L27500;
                // !RIGHT ANSWER.
                L27300:
                ;
            }

            game.ParserVectors.prscon = 1;
            // !KILL REST OF LINE.
            ++game.Switches.nqatt;
            // !WRONG, CRETIN.
            if (game.Switches.nqatt >= 5)
            {
                goto L27400;
            }

            // !TOO MANY WRONG?
            i__1 = game.Switches.nqatt + 800;
            MessageHandler.Speak(i__1, game);
            // !NO, TRY AGAIN.
            return ret_val;

            L27400:
            MessageHandler.Speak(826, game);
            // !ALL OVER.
            game.Clock[ClockId.cevinq].Flags = false;
            // !LOSE.
            return ret_val;

            L27500:
            game.ParserVectors.prscon = 1;
            // !KILL REST OF LINE.
            ++game.Switches.corrct;
            // !GOT IT RIGHT.
            MessageHandler.Speak(800, game);
            // !HOORAY.
            if (game.Switches.corrct >= 3)
            {
                goto L27600;
            }
            // !WON TOTALLY?
            game.Clock[ClockId.cevinq].Ticks = 2;
            // !NO, START AGAIN.
            game.Switches.quesno = (game.Switches.quesno + 3) % 8;
            game.Switches.nqatt = 0;
            MessageHandler.Speak(769, game);
            // !ASK NEXT QUESTION.
            i__1 = game.Switches.quesno + 770;
            MessageHandler.Speak(i__1, game);
            return ret_val;

            L27600:
            MessageHandler.Speak(827, game);
            // !QUIZ OVER,
            game.Clock[ClockId.cevinq].Flags = false;
            game.Objects[ObjectIds.qdoor].Flag2 |= ObjectFlags2.IsOpen;
            return ret_val;

        }
    }

    public class pv
    {
        public int act { get; set; }//, o1, o2, p1, p2;
    }

    public class objvec
    {
        public ObjectIds o1 { get; set; }
        public ObjectIds o2 { get; set; }
    }

    public class prpvec
    {
        public int p1 { get; set; }
        public int p2 { get; set; }
    }
}