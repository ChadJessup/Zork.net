namespace Zork.Core
{
    public class Orphans
    {
        public int oflag { get; set; }
        public int oact { get; set; }
        public int oslot { get; set; }
        public int oprep { get; set; }
        public int oname { get; set; }

        public static void Orphan(int o1, int o2, int o3, int o4, int o5, Game game)
        {
            game.Orphans.oflag = o1;
            game.Orphans.oact = o2;
            game.Orphans.oslot = o3;
            game.Orphans.oprep = o4;
            game.Orphans.oname = o5;
        }
    }
}