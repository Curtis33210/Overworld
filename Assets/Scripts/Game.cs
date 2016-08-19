using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;

using Debug = UnityEngine.Debug;
using System;

public class Game : MonoBehaviour
{
    #region Unity Variables

    [SerializeField]
    private int _mapWidth;
    [SerializeField]
    private int _mapHeight;

    #endregion

    #region Game Variables

    public World ActiveWorld { get; private set; }

    #endregion
    
    private void Start() {
        ActiveWorld = new World(_mapWidth, _mapHeight);
        
        ActiveWorld.CreateTestWorld();
    }

    private void Update() {
        EventManager.ProcessEvents();

        if (Input.GetKeyDown(KeyCode.K))
            ActiveWorld.RandomizeTiles();
    }
}