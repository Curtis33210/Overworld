
public enum TileEvents
{
    /// <summary>
    /// Called when a tile is created. <para/>
    /// Args: Tile object being created passed as Args.
    /// </summary>
    TileCreated,
    /// <summary>
    /// Called when a tiles type is changed <para/>
    /// Args: Tile object that was changed
    /// </summary>
    TileTypeChanged,
    /// <summary>
    /// NOT IN USED RIGHT NOW
    /// </summary>
    TileMoved
}

public struct TileMovedInfo
{
    public int MoveAmountX { get; private set; }
    public int MoveAmountY { get; private set; }

    public Tile TargetTile { get; private set; }

    public TileMovedInfo(Tile targetTile, int moveX, int moveY) {
        TargetTile = targetTile;

        MoveAmountX = moveX;
        MoveAmountY = moveY;
    }
}