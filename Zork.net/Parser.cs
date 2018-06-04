using System;
using System.Collections.Generic;
using System.Linq;
using Zork.Core.Object;

namespace Zork.Core
{
    public static class Parser
    {
        public static string ReadLine(int who)
        {
            string buffer;

            switch (who + 1)
            {
                case 1: goto L90;
                case 2: goto L10;
            }

            L10:
            Console.Write(">");

            L90:
            buffer = Console.ReadLine();
            if (buffer[0] == '!')
            {
                buffer = buffer.TrimStart(new[] { '!' });
            }

            return buffer.ToUpper();
        }

        /// <summary>
        /// parse_ - Top level parse routine
        /// </summary>
        /// <param name="input"></param>
        /// <param name="vbflag"></param>
        public static bool Parse(string input, bool vbflag, Game game)
        {
            int i__1;
            bool ret_val;

            int[] outbuf = new int[40];
            int outlnt;

            // Parameter adjustments
            //--input;

            // Function Body
            ret_val = false;

            // !ASSUME FAILS.
            game.ParserVectors.prsa = 0;

            // !ZERO OUTPUTS.
            game.ParserVectors.prsi = 0;
            game.ParserVectors.prso = 0;

            if (!Parser.Lex(input, outbuf, out outlnt, vbflag, game))
            {
                goto L100;
            }

            if ((i__1 = sparse_(outbuf, outlnt, vbflag, game)) < 0)
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
            if (!(vbflag))
            {
                goto L350;
            }
            // !ECHO MODE, FORCE FAIL.
            if (!synmch_(game))
            {
                goto L100;
            }

            // !DO SYN MATCH.
            if (game.ParserVectors.prso > 0 & game.ParserVectors.prso < (int)XSearch.xmin)
            {
                game.Last.lastit = game.ParserVectors.prso;
            }

            // SUCCESSFUL PARSE OR SUCCESSFUL VALIDATION

            L300:
            ret_val = true;
            L350:
            Orphans.Orphan(0, 0, 0, 0, 0, game);

            // !CLEAR ORPHANS.
            return ret_val;

            // PARSE FAILS, DISALLOW CONTINUATION

            L100:
            game.ParserVectors.prscon = 1;

            return ret_val;
        }

        /// <summary>
        /// Lex_ - lexical analyzer
        /// </summary>
        /// <param name="inbuf"></param>
        /// <param name="outbuf"></param>
        /// <param name="count"></param>
        /// <param name="vbflag"></param>
        /// <returns></returns>
        public static bool Lex(string inbuf, int [] outbuf, out int count, bool vbflag, Game game)
        {
            char[] dlimit = new char[9] { 'A', 'Z', (char)('A' - 1), '1', '9', (char)('1' - 31), '-', '-', (char)('-' - 27) };
            bool ret_val = false;

            int i;
            char j;
            int k, j1, j2, cp;

            // Parameter adjustments
            // --outbuf;
            // --inbuf;

            // !ASSUME LEX FAILS.
            count = -1;

            // !OUTPUT PTR.
            L50:
            count += 2;

            // !ADV OUTPUT PTR.
            cp = 0;

            // !CHAR PTR=0.

            L200:
            j = inbuf[game.ParserVectors.prscon];

            // !GET CHARACTER
            if (j == '\0')
            {
                goto L1000;
            }

            // !END OF INPUT?

            ++game.ParserVectors.prscon;
            // !ADVANCE PTR.

            if (j == '.')
            {
                goto L1000;
            }

            // !END OF COMMAND?
            if (j == ',')
            {
                goto L1000;
            }

            // !END OF COMMAND?
            if (j == ' ')
            {
                goto L6000;
            }

            // !SPACE?
            for (i = 1; i <= 9; i += 3)
            {
                // !SCH FOR CHAR.
                if (j >= dlimit[i - 1] & j <= dlimit[i])
                {
                    goto L4000;
                }
                // L500:
            }

            if (vbflag)
            {
                MessageHandler.Speak(601, game);
            }

            // !GREEK TO ME, FAIL.
            return ret_val;

            // END OF INPUT, SEE IF PARTIAL WORD AVAILABLE.

            L1000:
            if (inbuf[game.ParserVectors.prscon] == '\0')
            {
                game.ParserVectors.prscon = 1;
            }

            // !FORCE PARSE RESTART.
            if (cp == 0 & count == 1)
            {
                return ret_val;
            }

            if (cp == 0)
            {
                count += -2;
            }

            // !ANY LAST WORD?
            ret_val = true;
            return ret_val;

            // LEGITIMATE CHARACTERS: LETTER, DIGIT, OR HYPHEN.

            L4000:
            j1 = j - dlimit[i + 1];
            if (cp >= 6)
            {
                goto L200;
            }

            // !IGNORE IF TOO MANY CHAR.
            k = count + cp / 3;

            // !COMPUTE WORD INDEX.
            switch (cp % 3 + 1)
            {
                case 1: goto L4100;
                case 2: goto L4200;
                case 3: goto L4300;
            }

            // !BRANCH ON CHAR.
            L4100:
            j2 = j1 * 780;

            // !CHAR 1... *780
            outbuf[k] = outbuf[k] + j2 + j2;

            // !*1560 (40 ADDED BELOW).
            L4200:
            outbuf[k] += j1 * 39;

            // !*39 (1 ADDED BELOW).
            L4300:
            outbuf[k] += j1;

            // !*1.
            ++cp;
            goto L200;
            // !GET NEXT CHAR.

            // SPACE

            L6000:
            if (cp == 0)
            {
                goto L200;
            }

            // !ANY WORD YET?
            goto L50;
            // !YES, ADV OP.
        }

        /// <summary>
        /// THIS ROUTINE DETAILS ON BIT 4 OF PRSFLG
        /// </summary>
        private static bool synmch_(Game game)
        {
            //  THE FOLLOWING DATA STATEMENT WAS ORIGINALLY:
            // 	DATA R50MIN/1RA/

            const int r50min = 1600;

            // System generated locals
            int i__1;
            bool ret_val;

            // Local variables
            int j;
            int newj;
            int drive, limit, qprep, sprep, dforce;

            ret_val = false;
            j = game.ParserVector.act;
            // !SET UP PTR TO SYNTAX.
            drive = 0;
            // !NO DEFAULT.
            dforce = 0;
            // !NO FORCED DEFAULT.
            qprep = game.Orphans.oflag & game.Orphans.oprep;
            L100:
            j += 2;

            // !FIND START OF SYNTAX.
            if (ParserConstants.vvoc[j - 1] <= 0 || ParserConstants.vvoc[j - 1] >= r50min)
            {
                goto L100;
            }

            limit = j + ParserConstants.vvoc[j - 1] + 1;
            // !COMPUTE LIMIT.
            ++j;
            // !ADVANCE TO NEXT.

            L200:
            unpack_(j, out newj, game);
            // !UNPACK SYNTAX.
            sprep = game.Syntax.dobj & (int)SyntaxObjectFlags.VPMASK;
            if (!syneql_(game.ParserVector.p1, game.ParserVector.o1, game.Syntax.dobj, game.Syntax.dfl1, game.Syntax.dfl2, game))
            {
                goto L1000;
            }
            sprep = game.Syntax.iobj & (int)SyntaxObjectFlags.VPMASK;
            if (syneql_(game.ParserVector.p2, game.ParserVector.o2, game.Syntax.iobj, game.Syntax.ifl1, game.Syntax.ifl2, game))
            {
                goto L6000;
            }

            // SYNTAX MATCH FAILS, TRY NEXT ONE.

            if (game.ParserVector.o2 != 0)
            {
                goto L3000;
            }
            else
            {
                goto L500;
            }

            // !IF O2=0, SET DFLT.
            L1000:
            if (game.ParserVector.o1 != 0)
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
                dforce = j;
            }

            // !IF PREP MCH.
            if ((game.Syntax.vflag & SyntaxFlags.SDRIV) != 0)
            {
                drive = j;
            }

            L3000:
            j = newj;
            if (j < limit)
            {
                goto L200;
            }
            // !MORE TO DO?
            // SYNMCH, PAGE 2

            // MATCH HAS FAILED.  IF DEFAULT SYNTAX EXISTS, TRY TO SNARF
            // ORPHANS OR GWIMS, OR MAKE NEW ORPHANS.

            if (drive == 0)
            {
                drive = dforce;
            }
            // !NO DRIVER? USE FORCE.
            if (drive == 0)
            {
                goto L10000;
            }
            // !ANY DRIVER?
            unpack_(drive, out dforce, game);
            // !UNPACK DFLT SYNTAX.

            // TRY TO FILL DIRECT OBJECT SLOT IF THAT WAS THE PROBLEM.
            if (game.Syntax.vflag.HasFlag(SyntaxFlags.SDIR) || game.ParserVector.o1 != 0)
            {
                goto L4000;
            }

            // FIRST TRY TO SNARF ORPHAN OBJECT.

            game.ParserVector.o1 = game.Orphans.oflag & game.Orphans.oslot;
            if (game.ParserVector.o1 == 0)
            {
                goto L3500;
            }
            // !ANY ORPHAN?
            if (syneql_(game.ParserVector.p1, game.ParserVector.o1, game.Syntax.dobj, game.Syntax.dfl1, game.Syntax.dfl2, game))
            {
                goto L4000;
            }

            // ORPHAN FAILS, TRY GWIM.

            L3500:
            game.ParserVector.o1 = gwim_(game.Syntax.dobj, game.Syntax.dfw1, game.Syntax.dfw2, game);
            // !GET GWIM.
            if (game.ParserVector.o1 > 0)
            {
                goto L4000;
            }
            // !TEST RESULT.
            i__1 = game.Syntax.dobj & (int)SyntaxObjectFlags.VPMASK;
            Orphans.Orphan(-1, game.ParserVector.act, 0, i__1, 0, game);
            MessageHandler.Speak(623, game);
            return ret_val;

            // TRY TO FILL INDIRECT OBJECT SLOT IF THAT WAS THE PROBLEM.

            L4000:
            if ((game.Syntax.vflag & SyntaxFlags.SIND) == 0 || game.ParserVector.o2 != 0)
            {
                goto L6000;
            }
            game.ParserVector.o2 = gwim_(game.Syntax.iobj, game.Syntax.ifw1, game.Syntax.ifw2, game);
            // !GWIM.
            if (game.ParserVector.o2 > 0)
            {
                goto L6000;
            }
            if (game.ParserVector.o1 == 0)
            {
                game.ParserVector.o1 = game.Orphans.oflag & game.Orphans.oslot;
            }
            i__1 = game.Syntax.dobj & (int)SyntaxObjectFlags.VPMASK;
            Orphans.Orphan(-1, game.ParserVector.act, game.ParserVector.o1, i__1, 0, game);
            MessageHandler.Speak(624, game);
            return ret_val;

            // TOTAL CHOMP

            L10000:
            MessageHandler.Speak(601, game);
            // !CANT DO ANYTHING.
            return ret_val;
            // SYNMCH, PAGE 3

            // NOW TRY TO TAKE INDIVIDUAL OBJECTS AND
            // IN GENERAL CLEAN UP THE PARSE VECTOR.

            L6000:
            if ((game.Syntax.vflag & SyntaxFlags.SFLIP) == 0)
            {
                goto L5000;
            }
            j = game.ParserVector.o1;
            // !YES.
            game.ParserVector.o1 = game.ParserVector.o2;
            game.ParserVector.o2 = j;

            L5000:
            game.ParserVectors.prsa = (int)(game.Syntax.vflag & SyntaxFlags.SVMASK);
            game.ParserVectors.prso = game.ParserVector.o1;
            // !GET DIR OBJ.
            game.ParserVectors.prsi = game.ParserVector.o2;
            // !GET IND OBJ.
            if (!takeit_(game.ParserVectors.prso, game.Syntax.dobj, game))
            {
                return ret_val;
            }
            // !TRY TAKE.
            if (!takeit_(game.ParserVectors.prsi, game.Syntax.iobj, game))
            {
                return ret_val;
            }
            // !TRY TAKE.
            ret_val = true;
            return ret_val;
        }

        /// <summary>
        /// gwim_ - Get what I mean
        /// </summary>
        /// <param name="sflag"></param>
        /// <param name="sfw1"></param>
        /// <param name="sfw2"></param>
        /// <returns></returns>
        private static int gwim_(int sflag, int sfw1, int sfw2, Game game)
        {
            // System generated locals
            int ret_val;

            // Local variables
            int av;
            int nobj, robj;
            bool nocare;

            // GWIM, PAGE 2

            ret_val = -1;
            // !ASSUME LOSE.
            av = game.Adventurers.Vehicles[game.Player.Winner - 1];
            nobj = 0;
            nocare = (sflag & (int)SyntaxObjectFlags.VCBIT) == 0;

            // FIRST SEARCH ADVENTURER

            if ((sflag & (int)SyntaxObjectFlags.VABIT) != 0)
            {
                nobj = fwim_(sfw1, sfw2, 0, 0, game.Player.Winner, nocare, game);
            }

            if ((sflag & (int)SyntaxObjectFlags.VRBIT) != 0)
            {
                goto L100;
            }

            L50:
            ret_val = nobj;
            return ret_val;

            // ALSO SEARCH ROOM

            L100:
            robj = fwim_(sfw1, sfw2, game.Player.Here, 0, 0, nocare, game);
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
            if (av == 0 || robj == av || (game.Objects.oflag2[robj - 1] & ObjectFlags2.FINDBT)
                != 0)
            {
                goto L300;
            }
            if (game.Objects.ocan[robj - 1] != av)
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
            if (!takeit_(robj, sflag, game))
            {
                return ret_val;
            }
            // !IF UNTAKEABLE, RETURN
            ret_val = robj;
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
        /// <param name="adv"></param>
        /// <param name="nocare"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        private static int fwim_(int f1, int f2, int rm, int con, int adv, bool nocare, Game game)
        {
            int ret_val, i__1, i__2;

            // Local variables
            int i, j;

            // OBJECTS
            ret_val = 0;
            // !ASSUME NOTHING.
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // !LOOP
                if ((rm == 0 || game.Objects.oroom[i - 1] != rm) && (adv == 0 ||
                    game.Objects.oadv[i - 1] != adv) && (con == 0 || game.Objects.ocan[i - 1] != con))
                {
                    goto L1000;
                }

                // OBJECT IS ON LIST... IS IT A MATCH?

                if ((game.Objects.oflag1[i - 1] & ObjectFlags.VISIBT) == 0)
                {
                    goto L1000;
                }

                // double check: was ~(nocare)
                if (!nocare & (game.Objects.oflag1[i - 1] & ObjectFlags.TAKEBT) == 0
                    || ((int)game.Objects.oflag1[i - 1] & f1) == 0 && ((int)game.Objects.oflag2[i - 1] & f2) == 0)
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
                ret_val = i;
                // !NOTE MATCH.

                // DOES OBJECT CONTAIN A MATCH?

                L500:
                if ((game.Objects.oflag2[i - 1] & ObjectFlags2.OPENBT) == 0)
                {
                    goto L1000;
                }

                i__2 = game.Objects.Count;
                for (j = 1; j <= i__2; ++j)
                {
                    // !NO, SEARCH CONTENTS.
                    if (game.Objects.ocan[j - 1] != i
                        || (game.Objects.oflag1[j - 1] & ObjectFlags.VISIBT) == 0
                        || ((int)game.Objects.oflag1[j - 1] & f1) == 0
                        && ((int)game.Objects.oflag2[j - 1] & f2) == 0)
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
                    ret_val = j;
                    L700:
                    ;
                }
                L1000:
                ;
            }
            return ret_val;
        }

        /// <summary>
        /// takeit_ - PARSER BASED TAKE OF OBJECT
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        private static bool takeit_(int obj, int sflag, Game game)
        {
            bool ret_val;

            // Local variables
            int x;
            int odo2;

            // TAKEIT, PAGE 2

            ret_val = false;
            // !ASSUME LOSES.
            if (obj == 0 || obj > game.Star.strbit)
            {
                goto L4000;
            }

            // !NULL/STARS WIN.
            odo2 = game.Objects.odesc2[obj - 1];
            // !GET DESC.
            x = game.Objects.ocan[obj - 1];
            // !GET CONTAINER.
            if (x == 0 || (sflag & (int)SyntaxObjectFlags.VFBIT) == 0)
            {
                goto L500;
            }
            if ((game.Objects.oflag2[x - 1] & ObjectFlags2.OPENBT) != 0)
            {
                goto L500;
            }

            MessageHandler.rspsub_(566, odo2, game);
            // !CANT REACH.
            return ret_val;

            L500:
            if ((sflag & (int)SyntaxObjectFlags.VRBIT) == 0)
            {
                goto L1000;
            }
            if ((sflag & (int)SyntaxObjectFlags.VTBIT) == 0)
            {
                goto L2000;
            }

            // SHOULD BE IN ROOM (VRBIT NE 0) AND CAN BE TAKEN (VTBIT NE 0)

            if (schlst_(0, 0, game.Player.Here, 0, 0, obj, game) <= 0)
            {
                goto L4000;
            }
            // !IF NOT, OK.

            // ITS IN THE ROOM AND CAN BE TAKEN.

            if ((game.Objects.oflag1[obj - 1] & ObjectFlags.TAKEBT) != 0
                && (game.Objects.oflag2[obj - 1] & ObjectFlags2.TRYBT) == 0)
            {
                goto L3000;
            }

            // NOT TAKEABLE.  IF WE CARE, FAIL.

            if ((sflag & (int)SyntaxObjectFlags.VCBIT) == 0)
            {
                goto L4000;
            }

            MessageHandler.rspsub_(445, odo2, game);
            return ret_val;

            // 1000--	IT SHOULD NOT BE IN THE ROOM.
            // 2000--	IT CANT BE TAKEN.

            L2000:
            if ((sflag & (int)SyntaxObjectFlags.VCBIT) == 0)
            {
                goto L4000;
            }
            L1000:
            if (schlst_(0, 0, game.Player.Here, 0, 0, obj, game) <= 0)
            {
                goto L4000;
            }

            MessageHandler.rspsub_(665, odo2, game);

            return ret_val;
            // TAKEIT, PAGE 3

            // OBJECT IS IN THE ROOM, CAN BE TAKEN BY THE PARSER,
            // AND IS TAKEABLE IN GENERAL.  IT IS NOT A STAR.
            // TAKING IT SHOULD NOT HAVE SIDE AFFECTS.
            // IF IT IS INSIDE SOMETHING, THE CONTAINER IS OPEN.
            // THE FOLLOWING CODE IS LIFTED FROM SUBROUTINE TAKE.

            L3000:
            if (obj != game.Adventurers.Vehicles[game.Player.Winner - 1])
            {
                goto L3500;
            }
            // !TAKE VEHICLE?
            MessageHandler.Speak(672, game);
            return ret_val;

            L3500:
            if (x != 0 && game.Objects.oadv[x - 1] == game.Player.Winner || ObjectHandler.weight_(0, obj, game.Player.Winner, game) + game.Objects.osize[obj - 1] <= game.State.MaxLoad)
            {
                goto L3700;
            }

            MessageHandler.Speak(558, game);
            // !TOO BIG.
            return ret_val;

            L3700:
            //newsta_(obj, 559, 0, 0, game.Player.Winner, game);
            // !DO TAKE.
            game.Objects.oflag2[obj - 1] |= ObjectFlags2.TCHBT;
            //scrupd_(game.Objects.ofval[obj - 1], game);
            game.Objects.ofval[obj - 1] = 0;

            L4000:
            ret_val = true;
            // !SUCCESS.
            return ret_val;
        }

        public static bool syneql_(int prep, int obj, int sprep, int sfl1, int sfl2, Game game)
        {
            bool ret_val;

            if (obj == 0)
            {
                goto L100;
            }
            /* 						!ANY OBJECT? */
            ret_val = prep == (sprep & (int)SyntaxObjectFlags.VPMASK) && (sfl1 & (int)game.Objects.oflag1[obj - 1] | sfl2 & (int)game.Objects.oflag2[obj - 1]) != 0;
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
        private static void unpack_(int oldj, out int j, Game game)
        {
            int i;

            for (i = 1; i <= 11; ++i)
            {
                // !CLEAR SYNTAX. */
                //syn[i - 1] = 0;
                /* L10: */
            }

            game.Syntax.vflag = (SyntaxFlags)ParserConstants.vvoc[oldj - 1];
            j = oldj + 1;
            if ((game.Syntax.vflag & SyntaxFlags.SDIR) == 0)
            {
                return;
            }
            game.Syntax.dfl1 = -1;
            /* 						!ASSUME STD. */
            game.Syntax.dfl2 = -1;
            if ((game.Syntax.vflag & SyntaxFlags.SSTD) == 0)
            {
                goto L100;
            }
            game.Syntax.dfw1 = -1;
            /* 						!YES. */
            game.Syntax.dfw2 = -1;
            game.Syntax.dobj = (int)SyntaxObjectFlags.VABIT + (int)SyntaxObjectFlags.VRBIT + (int)SyntaxObjectFlags.VFBIT;
            goto L200;

            L100:
            game.Syntax.dobj = ParserConstants.vvoc[j - 1];
            /* 						!NOT STD. */
            game.Syntax.dfw1 = ParserConstants.vvoc[j];
            game.Syntax.dfw2 = ParserConstants.vvoc[j + 1];
            j += 3;
            if ((game.Syntax.dobj & (int)SyntaxObjectFlags.VEBIT) == 0)
            {
                goto L200;
            }
            game.Syntax.dfl1 = game.Syntax.dfw1;
            /* 						!YES. */
            game.Syntax.dfl2 = game.Syntax.dfw2;

            L200:
            if ((game.Syntax.vflag & SyntaxFlags.SIND) == 0)
            {
                return;
            }
            game.Syntax.ifl1 = -1;
            /* 						!ASSUME STD. */
            game.Syntax.ifl2 = -1;
            game.Syntax.iobj = ParserConstants.vvoc[j - 1];
            game.Syntax.ifw1 = ParserConstants.vvoc[j];
            game.Syntax.ifw2 = ParserConstants.vvoc[j + 1];
            j += 3;

            if ((game.Syntax.iobj & (int)SyntaxObjectFlags.VEBIT) == 0)
            {
                return;
            }
            game.Syntax.ifl1 = game.Syntax.ifw1;
            /* 						!YES. */
            game.Syntax.ifl2 = game.Syntax.ifw2;
        }

        /* SCHLST--	SEARCH FOR OBJECT */

        /* DECLARATIONS */

        private static int schlst_(int oidx, int aidx, int rm, int cn, int ad, int spcobj, Game game)
        {
            /* System generated locals */
            int ret_val, i__1, i__2;

            /* Local variables */
            int i, j, x;

            ret_val = 0;
            /* 						!NO RESULT. */
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // !SEARCH OBJECTS.
                if ((game.Objects.oflag1[i - 1] & ObjectFlags.VISIBT) == 0 || (rm == 0 ||
                    !ObjectHandler.qhere_(i, rm, game)) && (cn == 0 || game.Objects.ocan[i - 1] != cn)
                    && (ad == 0 || game.Objects.oadv[i - 1] != ad))
                {
                    goto L1000;
                }

                if (!thisit_(oidx, aidx, i, spcobj, game))
                {
                    goto L200;
                }
                if (ret_val != 0)
                {
                    goto L2000;
                }
                /* 						!GOT ONE ALREADY? */
                ret_val = i;
                /* 						!NO. */

                /* IF OPEN OR TRANSPARENT, SEARCH THE OBJECT ITSELF. */

                L200:
                if ((game.Objects.oflag1[i - 1] & ObjectFlags.TRANBT) == 0 && (
                    game.Objects.oflag2[i - 1] & ObjectFlags2.OPENBT) == 0)
                {
                    goto L1000;
                }

                /* SEARCH IS CONDUCTED IN REVERSE.  ALL OBJECTS ARE CHECKED TO */
                /* SEE IF THEY ARE AT SOME LEVEL OF CONTAINMENT INSIDE OBJECT 'I'. */
                /* IF THEY ARE AT LEVEL 1, OR IF ALL LINKS IN THE CONTAINMENT */
                /* CHAIN ARE OPEN, VISIBLE, AND HAVE SEARCHME SET, THEY CAN QUALIFY */

                /* AS A POTENTIAL MATCH. */

                i__2 = game.Objects.Count;
                for (j = 1; j <= i__2; ++j)
                {
                    /* 						!SEARCH OBJECTS. */
                    if ((game.Objects.oflag1[j - 1] & ObjectFlags.VISIBT) == 0 || !thisit_(oidx, aidx, j, spcobj, game))
                    {
                        goto L500;
                    }
                    x = game.Objects.ocan[j - 1];
                    /* 						!GET CONTAINER. */
                    L300:
                    if (x == i)
                    {
                        goto L400;
                    }
                    /* 						!INSIDE TARGET? */
                    if (x == 0)
                    {
                        goto L500;
                    }
                    /* 						!INSIDE ANYTHING? */
                    if ((game.Objects.oflag1[x - 1] & ObjectFlags.VISIBT) == 0 || (
                        game.Objects.oflag1[x - 1] & ObjectFlags.TRANBT) == 0 && (
                        game.Objects.oflag2[x - 1] & ObjectFlags2.OPENBT) == 0 || (
                        game.Objects.oflag2[x - 1] & ObjectFlags2.SCHBT) == 0)
                    {
                        goto L500;
                    }
                    x = game.Objects.ocan[x - 1];
                    /* 						!GO ANOTHER LEVEL. */
                    goto L300;

                    L400:
                    if (ret_val != 0)
                    {
                        goto L2000;
                    }
                    /* 						!ALREADY GOT ONE? */
                    ret_val = j;
                    /* 						!NO. */
                    L500:
                    ;
                }

                L1000:
                ;
            }
            return ret_val;

            L2000:
            ret_val = -ret_val;
            /* 						!AMB RETURN. */
            return ret_val;

        }

        /* THISIT--	VALIDATE OBJECT VS DESCRIPTION */
        private static bool thisit_(int oidx, int aidx, int obj, int spcobj, Game game)
        {

            /*    THE FOLLOWING DATA STATEMENT USED RADIX-50 NOTATION (R50MIN/1RA/) */

            /*       IN RADIX-50 NOTATION, AN "A" IN THE FIRST POSITION IS */
            /*       ENCODED AS 1*40*40 = 1600. */

            const int r50min = 1600;

            bool ret_val;
            int i;

            ret_val = false;
            /* 						!ASSUME NO MATCH. */
            if (spcobj != 0 && obj == spcobj)
            {
                goto L500;
            }

            /* CHECK FOR OBJECT NAMES */

            i = oidx + 1;
            L100:
            ++i;
            if (ParserConstants.ovoc[i - 1] <= 0 || ParserConstants.ovoc[i - 1] >= r50min)
            {
                return ret_val;
            }
            /* 						!IF DONE, LOSE. */
            if (ParserConstants.ovoc[i - 1] != obj)
            {
                goto L100;
            }
            /* 						!IF FAIL, CONT. */

            if (aidx == 0)
            {
                goto L500;
            }
            /* 						!ANY ADJ? */
            i = aidx + 1;
            L200:
            ++i;
            if (ParserConstants.avoc[i - 1] <= 0 || ParserConstants.avoc[i - 1] >= r50min)
            {
                return ret_val;
            }
            /* 						!IF DONE, LOSE. */
            if (ParserConstants.avoc[i - 1] != obj)
            {
                goto L200;
            }
            /* 						!IF FAIL, CONT. */

            L500:
            ret_val = true;
            return ret_val;
        }

        /* THIS ROUTINE DETAILS ON BIT 2 OF PRSFLG */

        private static int sparse_(int[] lbuf, int llnt, bool vbflag, Game game)
        {
            /* 	DATA R50MIN/1RA/,R50WAL/3RWAL/ */
            const int r50min = 1600;
            const int r50wal = 36852;

            int ret_val, i__1, i__2;

            int i, j, adj;
            int obj;
            int prep, pptr, lbuf1 = 0, lbuf2 = 0;
            int buzlnt, prplnt, dirlnt;

            pv pv_1;

            /* Parameter adjustments */
            //--lbuf;

            ret_val = -1;
            /* 						!ASSUME PARSE FAILS. */
            adj = 0;
            /* 						!CLEAR PARTS HOLDERS. */
            pv_1.act = 0;
            prep = 0;
            pptr = 0;
            pv_1.o1 = 0;
            pv_1.o2 = 0;
            pv_1.p1 = 0;
            pv_1.p2 = 0;

            buzlnt = 20;
            prplnt = 48;
            dirlnt = 75;
            /* SPARSE, PAGE 8 */

            /* NOW LOOP OVER INPUT BUFFER OF LEXICAL TOKENS. */

            i__1 = llnt;
            for (i = 1; i <= i__1; i += 2)
            {
                /* 						!TWO WORDS/TOKEN. */
                lbuf1 = lbuf[i];
                /* 						!GET CURRENT TOKEN. */
                lbuf2 = lbuf[i + 1];

                if (lbuf1 == 0)
                {
                    goto L1500;
                }
                /* 						!END OF BUFFER? */

                /* CHECK FOR BUZZ WORD */

                i__2 = buzlnt;
                for (j = 1; j <= i__2; j += 2)
                {
                    if (false)//lbuf1 == buzvoc_1.bvoc[j - 1] && lbuf2 == buzvoc_1.bvoc[j])
                    {
                        goto L1000;
                    }
                    /* L50: */
                }

                /* CHECK FOR ACTION OR DIRECTION */

                if (pv_1.act != 0)
                {
                    goto L75;
                }

                /* 						!GOT ACTION ALREADY? */
                j = 1;
                /* 						!CHECK FOR ACTION. */
                L125:
                if (lbuf1 == ParserConstants.vvoc[j - 1] && lbuf2 == ParserConstants.vvoc[j])
                {
                    goto L3000;
                }
                /* L150: */
                j += 2;
                /* 						!ADV TO NEXT SYNONYM. */
                if (!(ParserConstants.vvoc[j - 1] > 0 && ParserConstants.vvoc[j - 1] < r50min))
                {
                    goto L125;
                }

                /* 						!ANOTHER VERB? */
                j = j + ParserConstants.vvoc[j - 1] + 1;
                /* 						!NO, ADVANCE OVER SYNTAX. */
                if (ParserConstants.vvoc[j - 1] != -1)
                {
                    goto L125;
                }
                /* 						!TABLE DONE? */

                L75:
                if (pv_1.act != 0 && (ParserConstants.vvoc[pv_1.act - 1] != r50wal || prep != 0))
                {
                    goto L200;
                }
                i__2 = dirlnt;
                for (j = 1; j <= i__2; j += 3)
                {
                    /* 						!THEN CHK FOR DIR. */
                    if (false)//lbuf1 == dirvoc_1.dvoc[j - 1] && lbuf2 == dirvoc_1.dvoc[j])
                    {
                        goto L2000;
                    }
                    /* L100: */
                }

                /* NOT AN ACTION, CHECK FOR PREPOSITION, ADJECTIVE, OR OBJECT. */

                L200:
                i__2 = prplnt;
                for (j = 1; j <= i__2; j += 3)
                {
                    /* 						!LOOK FOR PREPOSITION. */
                    if (false)//lbuf1 == prpvoc_1.pvoc[j - 1] && lbuf2 == prpvoc_1.pvoc[j])
                    {
                        goto L4000;
                    }
                    /* L250: */
                }

                j = 1;
                /* 						!LOOK FOR ADJECTIVE. */
                L300:
                if (lbuf1 == ParserConstants.avoc[j - 1] && lbuf2 == ParserConstants.avoc[j])
                {
                    goto L5000;
                }
                ++j;
                L325:
                ++j;
                /* !ADVANCE TO NEXT ENTRY. */
                if (ParserConstants.avoc[j - 1] > 0 && ParserConstants.avoc[j - 1] < r50min)
                {
                    goto L325;
                }
                /* !A RADIX 50 CONSTANT? */
                if (ParserConstants.avoc[j - 1] != -1)
                {
                    goto L300;
                }
                /* 						!POSSIBLY, END TABLE? */

                j = 1;
                /* 						!LOOK FOR OBJECT. */
                L450:
                if (lbuf1 == ParserConstants.ovoc[j - 1] && lbuf2 == ParserConstants.ovoc[j])
                {
                    goto L600;
                }

                ++j;
                L500:
                ++j;

                if (ParserConstants.ovoc[j - 1] > 0 && ParserConstants.ovoc[j - 1] < r50min)
                {
                    goto L500;
                }

                if (ParserConstants.ovoc[j - 1] != -1)
                {
                    goto L450;
                }

                /* NOT RECOGNIZABLE */

                if (vbflag)
                {
                    MessageHandler.Speak(601, game);
                }

                return ret_val;
                /* SPARSE, PAGE 9 */

                /* OBJECT PROCESSING (CONTINUATION OF DO LOOP ON PREV PAGE) */

                L600:
                obj = 0;// getobj_(j, adj, 0);
                /* 						!IDENTIFY OBJECT. */
                if (obj <= 0)
                {
                    goto L6000;
                }
                /* 						!IF LE, COULDNT. */
                if (obj != (int)ObjectIndices.itobj)
                {
                    goto L650;
                }
                /* 						!"IT"? */
                obj = 0;// getobj_(0, 0, game.Last.lastit);
                /* 						!FIND LAST. */
                if (obj <= 0)
                {
                    goto L6000;
                }
                /* 						!IF LE, COULDNT. */

                L650:
                if (prep == 9)
                {
                    goto L8000;
                }
                /* 						!"OF" OBJ? */
                if (pptr == 2)
                {
                    goto L7000;
                }
                /* 						!TOO MANY OBJS? */
                ++pptr;
                //objvec[pptr - 1] = obj;
                /* 						!STUFF INTO VECTOR. */
                //prpvec[pptr - 1] = prep;
                L700:
                prep = 0;
                adj = 0;
                /* Go to end of do loop (moved "1000 CONTINUE" to end of module, to avoid */
                /* complaints about people jumping back into the doloop.) */
                goto L1000;
                /* SPARSE, PAGE 10 */

                /* SPECIAL PARSE PROCESSORS */

                /* 2000--	DIRECTION */

                L2000:
                //prsvec_1.prsa = VIndices.walkw;
                //prsvec_1.prso = dirvoc_1.dvoc[j + 1];
                ret_val = 1;
                return ret_val;

                /* 3000--	ACTION */

                L3000:
                pv_1.act = j;
                game.Orphans.oact = 0;
                goto L1000;

                /* 4000--	PREPOSITION */

                L4000:
                if (prep != 0)
                {
                    goto L4500;
                }
                //prep = prpvoc_1.pvoc[j + 1];
                adj = 0;
                goto L1000;

                L4500:
                if (vbflag)
                {
                    MessageHandler.Speak(616, game);
                }
                return ret_val;

                /* 5000--	ADJECTIVE */

                L5000:
                adj = j;
                j = game.Orphans.oname & game.Orphans.oflag;
                if (j != 0 && i >= llnt)
                {
                    goto L600;
                }
                goto L1000;

                /* 6000--	UNIDENTIFIABLE OBJECT (INDEX INTO OVOC IS J) */

                L6000:
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
                if (obj != -10000)
                {
                    goto L6200;
                }

                if (vbflag)
                {
                    MessageHandler.rspsub_(620, game.Objects.odesc2[game.Adventurers.Vehicles[game.Player.Winner - 1] - 1], game);
                }

                return ret_val;

                L6200:
                if (vbflag)
                {
                    MessageHandler.Speak(619, game);
                }
                if (pv_1.act == 0)
                {
                    pv_1.act = game.Orphans.oflag & game.Orphans.oact;
                }

                Orphans.Orphan(-1, pv_1.act, pv_1.o1, prep, j, game);
                return ret_val;

                /* 7000--	TOO MANY OBJECTS. */

                L7000:
                if (vbflag)
                {
                    MessageHandler.Speak(617, game);
                }
                return ret_val;

                /* 8000--	RANDOMNESS FOR "OF" WORDS */

                L8000:
                if (false)//objvec[pptr - 1] == obj)
                {
                    goto L700;
                }

                if (vbflag)
                {
                    MessageHandler.Speak(601, game);
                }

                return ret_val;

                /* End of do-loop. */

                L1000:
                ;
            }
            /* 						!AT LAST. */

            /* NOW SOME MISC CLEANUP -- We fell out of the do-loop */

            L1500:
            if (pv_1.act == 0)
            {
                pv_1.act = game.Orphans.oflag & game.Orphans.oact;
            }
            if (pv_1.act == 0)
            {
                goto L9000;
            }
            /* 						!IF STILL NONE, PUNT. */
            if (adj != 0)
            {
                goto L10000;
            }
            /* 						!IF DANGLING ADJ, PUNT. */

            if (game.Orphans.oflag != 0 && game.Orphans.oprep != 0 && prep == 0 && pv_1.o1 != 0
                && pv_1.o2 == 0 && pv_1.act == game.Orphans.oact)
            {
                goto L11000;
            }

            ret_val = 0;
            /* 						!PARSE SUCCEEDS. */
            if (prep == 0)
            {
                goto L1750;
            }
            /* 						!IF DANGLING PREP, */
            if (pptr == 0 )//|| prpvec[pptr - 1] != 0)
            {
                goto L12000;
            }
            //prpvec[pptr - 1] = prep;
            /* 						!CVT TO 'PICK UP FROB'. */

            /* 1750--	RETURN A RESULT */

            L1750:
            /* 						!WIN. */
            return ret_val;
            /* 						!LOSE. */

            /* 9000--	NO ACTION, PUNT */

            L9000:
            if (pv_1.o1 == 0)
            {
                goto L10000;
            }
            /* 						!ANY DIRECT OBJECT? */
            if (vbflag)
            {
                MessageHandler.rspsub_(621, game.Objects.odesc2[pv_1.o1 - 1], game);
            }
            /* 						!WHAT TO DO? */
            Orphans.Orphan(-1, 0, pv_1.o1, 0, 0, game);
            return ret_val;

            /* 10000--	TOTAL CHOMP */

            L10000:
            if (vbflag)
            {
                MessageHandler.Speak(622, game);
            }
            /* 						!HUH? */
            return ret_val;

            /* 11000--	ORPHAN PREPOSITION.  CONDITIONS ARE */
            /* 		O1.NE.0, O2=0, PREP=0, ACT=OACT */

            L11000:
            if (game.Orphans.oslot != 0)
            {
                goto L11500;
            }
            /* 						!ORPHAN OBJECT? */
            pv_1.p1 = game.Orphans.oprep;
            /* 						!NO, JUST USE PREP. */
            goto L1750;

            L11500:
            pv_1.o2 = pv_1.o1;
            /* 						!YES, USE AS DIRECT OBJ. */
            pv_1.p2 = game.Orphans.oprep;
            pv_1.o1 = game.Orphans.oslot;
            pv_1.p1 = 0;
            goto L1750;

            /* 12000--	TRUE HANGING PREPOSITION. */
            /* 		ORPHAN FOR LATER. */

            L12000:
            Orphans.Orphan(-1, pv_1.act, 0, prep, 0, game);
            /* 						!ORPHAN PREP. */
            goto L1750;
        }
    }

    public struct pv
    {
        public int act, o1, o2, p1, p2;
    }
}