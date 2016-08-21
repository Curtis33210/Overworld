
public enum WorldEvent
{
    /// <summary>
    /// Called when a tile is added to the world. <para/>
    /// Args: Tile object being added.
    /// </summary>
	TileAdded,

    /// <summary>
    /// Called when a tile is removed from the world. <para/>
    /// Args: Tile object being removed.
    /// </summary>
    TileRemoved,

    /// <summary>
    /// Called when a creature has been added to the world. <para/>
    /// Args: Creature being Added.
    /// </summary>
    CreatureAdded,

    /// <summary>
    /// Called when a creature is removed from the world. <para/>
    /// Args: Creature being Removed.
    /// </summary>
    CreatureRemoved
}