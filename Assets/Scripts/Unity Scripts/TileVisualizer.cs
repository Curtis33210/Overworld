using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class TileVisualizer : MonoBehaviour
{
    private const string FilePath = @"Tiles/";

    [SerializeField]
    private Transform _tileContainer;
    [SerializeField]
    private Sprite _invalidSprite;
    private EventManager _eventManager;

    private Sprite[] _tileSprites;
    private Dictionary<Tile, GameObject> _tileGameObjects;

    private void Awake() {
        _tileGameObjects = new Dictionary<Tile, GameObject>();

        SetUpEventManager();
        LoadSprites();
    }

    private void SetUpEventManager() {
        _eventManager = new EventManager();
        _eventManager.RegisterListener(WorldEvents.TileAdded, OnTileAddedToWorld);
        _eventManager.RegisterListener(TileEvents.TileTypeChanged, OnTileTypeChanged);
    }
    
    private void LoadSprites() {
        var tileTypes = Enum.GetValues(typeof(TileType)).Cast<TileType>(); // Get all the values in the TileType
        _tileSprites = new Sprite[tileTypes.Count()];

        if (_invalidSprite == null)
            Debug.LogError("No sprite has been assigned to invalid sprite");

        foreach (var tileType in tileTypes) {
            if (tileType == TileType.Empty)
                continue;

            var sprite = Resources.Load<Sprite>(FilePath + tileType);

            if (sprite == null) {
                Debug.LogError("Sprite could not be found for " + tileType);
                sprite = _invalidSprite;
            }

            _tileSprites[(int)tileType] = sprite;
        }
    }

    private void OnTileAddedToWorld(GameEvent gEvent) {
        var newTile = (Tile)gEvent.Args;
        
        CreateTileVisualization(newTile);
    }

    private void OnTileTypeChanged(GameEvent gEvent) {
        var tile = (Tile)gEvent.Args;
        var tileGO = _tileGameObjects[tile];

        UpdateGameObjectSprite(tileGO, _tileSprites[(int)tile.Type]);
    }

    private void UpdateGameObjectSprite(GameObject tileGo, Sprite newSprite) {
        var spriteRenderer = tileGo.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null) {
            Debug.LogError("Sprite renderer not found on tile GameObject. Was it deleted?");
            spriteRenderer = tileGo.AddComponent<SpriteRenderer>();
        }

        spriteRenderer.sprite = newSprite;
    }

    private void CreateTileVisualization(Tile newTile) {
        var tileGO = new GameObject("Tile_" + newTile.XPos + "_" + newTile.YPos);
        tileGO.transform.SetParent(_tileContainer);
        tileGO.transform.position = new Vector2(newTile.XPos, newTile.YPos);

        tileGO.AddComponent<SpriteRenderer>();

        _tileGameObjects[newTile] = tileGO;

        UpdateGameObjectSprite(tileGO, _tileSprites[(int)newTile.Type]);
    }
}