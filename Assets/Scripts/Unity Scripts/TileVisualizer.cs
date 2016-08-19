using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class TileVisualizer : MonoBehaviour
{
    private const string FilePath = @"Tiles/";

    private const int ChunkSize = 16; // Will be an 8x8 chunk of tiles
    private const int ChunkTileCount = ChunkSize * ChunkSize; // How many tiles are inside each chunk
    private const int LoadedChunksBuffer= 3; // Create a 1 wide square around the camera of chunks (Size of 1 will be 3x3 chunks)

    [SerializeField]
    private Transform _tileContainer;
    [SerializeField]
    private Sprite _invalidSprite;

    private EventManager _eventManager;

    private Sprite[] _tileSprites;

    private List<TileChunk> _inactiveTileChunks;
    private Dictionary<Vector2, TileChunk> _loadedChunks;
    
    private Vector2 _currentChunkCoordinates = new Vector2(-1, -1);
    private bool _loadingChunk = false;

    private void Awake() {
        _loadedChunks = new Dictionary<Vector2, TileChunk>();
        _inactiveTileChunks = new List<TileChunk>();

        SetUpEventManager();
        LoadSprites();
    }

    private void SetUpEventManager() {
        _eventManager = new EventManager();
        
        _eventManager.RegisterListener(TileEvent.TileTypeChanged, OnTileTypeChanged);
        _eventManager.RegisterListener(CameraEvent.CameraMoved, OnCameraMoved);
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

    private void OnTileTypeChanged(GameEvent gEvent) {
        var tile = (Tile)gEvent.Args;

    }

    private void OnCameraMoved(GameEvent gEvent) {
        Profiler.BeginSample("OnCameraMoved");
        Vector2 newPos = (Vector3)gEvent.Args;

        if (GetChunkCoordinatesAt((int)newPos.x, (int)newPos.y) != _currentChunkCoordinates) {
            //Debug.Log("Updating Chunks");
            _currentChunkCoordinates = GetChunkCoordinatesAt((int)newPos.x, (int)newPos.y);
            StartCoroutine("UpdateActiveChunks");
            //UpdateActiveChunks();
        }
        Profiler.EndSample();
    }

    private Vector2 GetChunkCoordinatesAt(int x, int y) {
        var chunkX = Mathf.FloorToInt(x / ChunkSize);
        var chunkY = Mathf.FloorToInt(y / ChunkSize);

        return new Vector2(chunkX, chunkY);
    }
    
    private IEnumerator UpdateActiveChunks() {
        var newChunks = NewChunks();

        foreach (var chunkCoordinates in _loadedChunks.Keys.Reverse()) {
            if (newChunks.Contains(chunkCoordinates)) 
                newChunks.Remove(chunkCoordinates);
             else 
                UnloadChunk(chunkCoordinates);
        }

        for (int i = 0; i < newChunks.Count; i++) {
            LoadChunk(newChunks[i]);
            yield return null;
        }
    }

    private List<Vector2> NewChunks() {
        var newChunk = new List<Vector2>((LoadedChunksBuffer * 2) + 1);


        for (int y = (int)_currentChunkCoordinates.y - LoadedChunksBuffer; y <= (int)_currentChunkCoordinates.y + LoadedChunksBuffer; y++) {
            for (int x = (int)_currentChunkCoordinates.x - LoadedChunksBuffer; x <= (int)_currentChunkCoordinates.x + LoadedChunksBuffer; x++) {
                if (x < 0 || y < 0)
                    continue;

                newChunk.Add(new Vector2(x, y));
            }
        }

        return newChunk;
    }

    private void LoadChunk(Vector2 chunkCoordinate) {
        var xOffset = (int)chunkCoordinate.x * ChunkSize;
        var yOffset = (int)chunkCoordinate.y * ChunkSize;

        var chunk = GetTileChunk();
        _loadedChunks.Add(chunkCoordinate, chunk);
        var world = FindObjectOfType<Game>().ActiveWorld;

        for (int x = 0; x < ChunkSize; x++) {
            for (int y = 0; y < ChunkSize; y++) {
                var tile = world.GetTileAt(xOffset + x, yOffset + y);
                if (tile == null)
                    continue;
                
                var tileGO = CreateTileVisualization(world.GetTileAt(xOffset + x, yOffset + y));

                chunk.SetTileObject(x, y, tileGO);
            }
        }
    }

    private void UnloadChunk(Vector2 chunkCoordinate) {
        if (_loadedChunks.ContainsKey(chunkCoordinate) == false)
            Debug.LogError("Trying to unload a chunk that is not loaded");

        var chunk = _loadedChunks[chunkCoordinate];
        _loadedChunks.Remove(chunkCoordinate);

        _inactiveTileChunks.Add(chunk);
        
        chunk.DisableAllGameObject();
    }

    private TileChunk GetTileChunk() {
        if (_inactiveTileChunks.Count > 0) {
            var newChunk = _inactiveTileChunks.Last();
            _inactiveTileChunks.RemoveAt(_inactiveTileChunks.Count - 1);

            return newChunk;
        }

        return new TileChunk(ChunkSize);
    }

    private void SetGameObjectSprite(GameObject tileGo, Sprite newSprite) {
        var spriteRenderer = tileGo.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null) {
            Debug.LogError("Sprite renderer not found on tile GameObject. Was it deleted?");
            spriteRenderer = tileGo.AddComponent<SpriteRenderer>();
        }

        spriteRenderer.sprite = newSprite;
    }

    private GameObject CreateTileVisualization(Tile newTile) {
        var tileGO = new GameObject("Tile_" + newTile.XPos + "_" + newTile.YPos);
        tileGO.isStatic = true;
        tileGO.transform.SetParent(_tileContainer);
        tileGO.transform.position = new Vector2(newTile.XPos, newTile.YPos);

        tileGO.AddComponent<SpriteRenderer>();
        
        SetGameObjectSprite(tileGO, _tileSprites[(int)newTile.Type]);

        return tileGO;
    }

    private class TileChunk
    {
        private GameObject[,] tileObjects;

        public TileChunk(int chunkSize) {
            tileObjects = new GameObject[chunkSize, chunkSize];
        }

        public void DisableAllGameObject() {
            for (int i = 0; i < tileObjects.GetLength(0); i++) {
                for (int j = 0; j < tileObjects.GetLength(1); j++) {
                    Destroy(tileObjects[i, j]);
                }
            }
        }

        public void SetTileObject(int x, int y, GameObject newTile) {
            tileObjects[x, y] = newTile;
        }

        public GameObject GetTileObject(int x, int y) {
            return tileObjects[x, y];
        }
    }
}