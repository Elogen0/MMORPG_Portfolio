public enum PlacementFitStatus
{
    /// <summary>
    /// Props fits in this location
    /// </summary>
    Fits,

    /// <summary>
    /// Tower overlaps another tower in the placement area
    /// </summary>
    Overlaps,

    /// <summary>
    /// Tower exceeds bounds of the placement area
    /// </summary>
    OutOfBounds
}