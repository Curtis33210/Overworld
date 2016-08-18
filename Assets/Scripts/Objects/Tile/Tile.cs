using UnityEngine;
using System.Collections.Generic;

public enum TileType
{
    Empty = -1, 
    Grass, 
    Stone
}

public class Tile
{
    public int XPos { get; private set; }
    public int YPos { get; private set; }

    public TileType Type { get; private set; }

    private EventManager _eventManager;

    public Tile(int xPos, int yPos) {
        XPos = xPos;
        YPos = yPos;

        _eventManager = new EventManager();
    }

    public void ChangeTileType(TileType newType, bool sendEvent = true) {
        Type = newType;

        if (sendEvent)
            _eventManager.RegisterEvent(TileEvents.TileTypeChanged, this);
    }

    public override int GetHashCode() {
        int hash = 23;

        hash = (hash * 31) + XPos;
        hash = (hash * 31) + YPos;

        return hash;
    }

    /* 
     
    Will this ever be needed?

    public void MoveTo(int xPos, int yPos, bool sendEvent = true) {
        var xMove = xPos - XPos;
        var yMove = yPos - YPos;

        if (xMove == 0 && yMove == 0)
            return;

        XPos = xPos;
        YPos = yPos;

        if (sendEvent)
            _eventManager.RegisterEvent(TileEvents.TileTypeChanged, new TileMovedInfo(this, xMove, yMove));
    }

    */
}