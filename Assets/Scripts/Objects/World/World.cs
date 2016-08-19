using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class World
{
    private Tile[][] _tiles;

    private EventManager _eventManager;

    public World(int width, int height) {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Map width or height cannot be 0 or negative.", "width/height");

        _eventManager = new EventManager();

        InitializeWorld(width, height);
    }

    private void InitializeWorld(int width, int height) {
        _tiles = new Tile[height][];

        for (int i = 0; i < height; i++) {
            _tiles[i] = new Tile[width];
        }
    }
    
    public void CreateTestWorld() {
        for (int y = 0; y < _tiles.Length; y++) {
            for (int x = 0; x < _tiles[y].Length; x++) {
                AddTileToWorld(new Tile(x, y));
            }
        }
    }

    public void RandomizeTiles() {
        for (int y = 0; y < _tiles.Length; y++) {
            for (int x = 0; x < _tiles[y].Length; x++) {
                _tiles[y][x].ChangeTileType((TileType)UnityEngine.Random.Range(0, 2));
            }
        }
    }

    public Tile GetTileAt(int x, int y) {
        if (y < 0 || y >= _tiles.Length || x < 0 || x >= _tiles[y].Length) // Out of Range
            return null;

        return _tiles[y][x];
    }

    private void AddTileToWorld(Tile newTile, bool sendEvent = false) {
        if (_tiles[newTile.YPos][newTile.XPos] != null) {
            Debug.LogError("Tile is already created at (" + newTile.XPos + ", " + newTile.YPos + "). Delete the old one before making a new one");
            return;
        }

        _tiles[newTile.YPos][newTile.XPos] = newTile;

        if (sendEvent)
            _eventManager.RegisterEvent(WorldEvent.TileAdded, newTile);
    }

    private void RemoveTileFromWorld(int xPos, int yPos) { 
        if (_tiles[yPos][xPos] == null) 
            return;

        _eventManager.RegisterEvent(WorldEvent.TileRemoved, _tiles[yPos][xPos]);
        _tiles[yPos][xPos] = null;
    }
}