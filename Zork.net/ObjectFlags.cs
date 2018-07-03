using System;

namespace Zork.Core
{
    [Flags]
    public enum ObjectFlags : int
    {
        IsVisible = 32768,
        READBT = 16384,
        IsTakeable = 8192,
        DOORBT = 4096,
        IsTransparent = 2048,
        IsFood = 1024,
        HasNoDescription = 512,
        IsDrinkable = 256,
        CONTBT = 128,
        LITEBT = 64,
        VICTBT = 32,
        BURNBT = 16,
        FLAMBT = 8,
        IsTool = 4,
        TURNBT = 2,
        IsOn = 1,
    }

    [Flags]
    public enum ObjectFlags2
    {
        FINDBT = 32768,
        IsSleeping = 16384,
        SCRDBT = 8192,
        IsTied = 4096,
        IsClimbable = 2048,
        ACTRBT = 1024,
        IsWeapon = 512,
        IsFighting = 256,
        IsVillian = 128,
        IsStaggered = 64,
        TRYBT = 32,
        NOCHBT = 16,
        IsOpen = 8,
        // Was TCHBt - I believe that means 'touched', whatever that means.
        WasTouched = 4,
        IsVehicle = 2,
        // Was SCHBT - I believe that means 'searchable'.
        IsSearchable = 1,
    }

    [Flags]
    public enum SyntaxObjectFlags
    {
        SearchAdventurer = 16384,
        SearchRoom = 8192,
        TryTake = 4096,
        AdventurerMustHave = 2048,
        VEBIT = 1024,
        MustBeReachable = 512,
        VPMASK = 511,
    }

    [Flags]
    public enum SyntaxFlags
    {
        SDIR = 16384,
        SIND = 8192,
        SSTD = 4096,
        SFLIP = 2048,
        SDRIV = 1024,
        SVMASK = 511,
    }
}