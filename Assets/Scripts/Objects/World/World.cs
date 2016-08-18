using UnityEngine;
using System.Collections.Generic;
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
                _tiles[y][x] = new Tile(x, y);

                _eventManager.RegisterEvent(WorldEvents.TileAdded, _tiles[y][x]);
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
}