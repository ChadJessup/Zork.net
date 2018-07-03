namespace Zork.Core
{
    public class Flags
    {
        public bool trollf { get; set; }
        public bool cagesf { get; set; }
        public bool IsBucketAtTop { get; set; }
        public bool IsCarouselOff { get; set; }
        public bool carozf { get; set; }
        public bool IsLowTide { get; set; }
        public bool IsRopeTiedToRailingInDomeRoom { get; set; }
        public bool IsGlacierFullyMelted { get; set; }
        public bool echof { get; set; }
        public bool riddlf { get; set; }
        public bool lldf { get; set; }
        public bool cyclof { get; set; }
        public bool IsDoorToCyclopsRoomOpen { get; set; }
        public bool litldf { get; set; }
        public bool WasSafeBlown { get; set; }
        public bool gnomef { get; set; }
        public bool gnodrf { get; set; }
        public bool IsMirrorBroken { get; set; }
        public bool egyptf { get; set; }
        public bool IsHeadOnPole { get; set; }
        public bool blabf { get; set; }
        public bool BriefDescriptions { get; set; }
        public bool SuperBriefDescriptions { get; set; }
        public bool buoyf { get; set; }
        public bool IsGrateUnlocked { get; set; }
        public bool gatef { get; set; }
        public bool IsRainbowOn { get; set; }
        public bool IsCageAtTop { get; set; }
        public bool empthf { get; set; }
        public bool deflaf { get; set; }
        public bool IsGlacierPartiallyMelted { get; set; }
        public bool frobzf { get; set; }
        public bool EndGame { get; set; }
        public bool HasBadLuck { get; set; }
        public bool thfenf { get; set; }
        public bool HasBirdSangSong { get; set; }
        public bool mrpshf { get; set; }
        public bool mropnf { get; set; }
        public bool wdopnf { get; set; }
        public bool mr1f { get; set; }
        public bool mr2f { get; set; }
        public bool inqstf { get; set; }
        public bool follwf { get; set; }
        public bool spellf { get; set; }
        public bool cpoutf { get; set; }
        public bool cpushf { get; set; }
    }

    public class Switches
    {
        public int IsBalloonTiedUp { get; set; }
        public int IsBalloonInflated { get; set; }
        public int IsReservoirLeaking { get; set; }
        public int rvclr { get; set; }
        public int rvcyc { get; set; }
        public int rvsnd { get; set; }
        public int rvgua { get; set; }
        public int IsRugMoved { get; set; }
        public int orcand { get; set; }

        /// <summary>
        /// Gets or sets the number of matches.
        /// </summary>
        public int MatchCount { get; set; }
        public int orlamp { get; set; }
        public int mdir { get; set; }
        public RoomIds mloc { get; set; }
        public int poleuf { get; set; }
        public int quesno { get; set; }
        public int nqatt { get; set; }
        public int corrct { get; set; }
        public int LeftCell { get; set; }
        public int pnumb { get; set; }
        public int acell { get; set; }
        public int dcell { get; set; }
        public int cphere { get; set; }
    }
}