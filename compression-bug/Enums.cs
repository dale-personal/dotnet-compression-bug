namespace MyLib
{
    public enum InstructionType
    {
        EndOfFile = -1,
        Clear = 0,
        DrawPath = 1,
        SetClip = 2,
        ResetClip = 3,
        FillPath = 4,
        DrawLites = 5,
        DrawCasing = 6,
        DrawOperation = 7,
        DrawMulls = 8,
        DrawHanding = 9,
        DrawText = 10,
        CadDetail = 11,
        CadPath = 12,
        CadDimension = 13,
        CadGraphicsPath = 14,
        CadOperation = 15,
        CadPitchTriangle = 16,
        CadElevationComponent = 17,
        LocationLabel = 18,
        HorizontalMullLabel = 19,
        VerticalMullLabel = 20
    }
    public enum DimensionOrientation
    {
        Horizontal,
        Vertical,
        Angled
    }
    public enum DimensionLevel
    {
        Springline,
        DaylightOpening,
        Sash,
        Unit,
        Mull,
        Assembly,
        Assembly2
    }
    public enum DimensionLocation
    {
        Top,
        Right,
        Left,
        Bottom,
        UpperLeft,
        UpperRight
    }
    public enum DimensionMeasure
    {
        MasonryOpening,
        RoughOpening,
        InsideOpening,
        FrameSize,
        SashOpening,
        RoughOpeningSpringLine,
        RoughOpeningRadius,
        Mull,
        InsideOpeningSpringLine,
        InsideOpeningRadius,
        OutsideCasing,
        RoughOpeningPolyExtra,
        InsideOpeningPolyExtra,
        BasicFrame,
        TotalRoughOpening,
        DaylightOpening
    }
    public enum PathItemType
    { 
        Point, 
        Arc, 
        Bezier 
    }
}
