
public enum TileEvent
{
    /// <summary>
    /// Called when a tiles type is changed <para/>
    /// Args: Tile object that was changed
    /// </summary>
    TileTypeChanged,
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