using UnityEngine;
using System.Collections.Generic;
using System;

public class World
{
    private Tile[][] _tiles;

    public World(int width, int height) {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Map width or height cannot be 0 or negative.", "width/height");

        InitializeWorld(width, height);
    }

    private void InitializeWorld(int width, int height) {
        _tiles = new Tile[height][];

        for (int i = 0; i < height; i++) {
            _tiles[i] = new Tile[width];
        }
    }
}