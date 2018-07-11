namespace Zork.Core
{
    public class Syntax
    {
        public SyntaxFlags Flags { get; set; }
        public SyntaxObjectFlags DirectObject { get; set; }
        public int DirectObjectFlag1 { get; set; }
        public int DirectObjectFlag2 { get; set; }
        public int DirectObjectWord1 { get; set; }
        public int DirectObjectWord2 { get; set; }
        public SyntaxObjectFlags IndirectObject { get; set; }
        public int IndirectObjectFlag1 { get; set; }
        public int IndirectObjectFlag2 { get; set; }
        public int IndirectObjectWord1 { get; set; }
        public int IndirectObjectWord2 { get; set; }
    }
}
