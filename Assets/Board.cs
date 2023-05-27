using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Board : NetworkBehaviour
{
    public EventHandler TileClicked;
    [SerializeField] private GameObject prefabTile;
    [SerializeField] private GameObject prefabChecker;

    private Checker selectedChecker;
    private Tile[,] tiles;
    private Tile selectedTile;
    private int width = 8;
    private int height = 8;
    private int[,] distribution =
    {
        { 0, 1, 0, 1, 0, 1, 0, 1},
        { 1, 0, 1, 0, 1, 0, 1, 0},
        { 0, 1, 0, 1, 0, 1, 0, 1},
        { 0, 0, 0, 0, 0, 0, 0, 0},
        { 0, 0, 0, 0, 0, 0, 0, 0},
        { 1, 0, 1, 0, 1, 0, 1, 0},
        { 0, 1, 0, 1, 0, 1, 0, 1},
        { 1, 0, 1, 0, 1, 0, 1, 0}
    };

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
    }

    private void Singleton_OnServerStarted()
    {
        Initialize();
    }

    private void Initialize ()
    {
        PlaceTiles();
        PlaceCheckers();
    }

    private void Tile_OnClicked (object s, EventArgs e)
    {
        TileClicked?.Invoke(s, e);
        if (selectedTile != null) 
        { 
            selectedTile.Selected = false;
        }
        
        selectedTile = s as Tile;
        selectedTile.Selected = true;
        if (selectedTile.Checker != null)
        {
            selectedChecker = selectedTile.Checker;
        }
    }
    private void PlaceCheckers ()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (distribution[j, i] == 0) continue;
                Checker checker = Instantiate(prefabChecker).GetComponent<Checker>();
                checker.transform.position = tiles[i,j].transform.position;
                tiles[i, j].Checker = checker;
                checker.NetworkObject.Spawn();
            }
        }
    }

    private void PlaceTiles ()
    {
        tiles = new Tile[width, height];
        // TO DO: Replace by static constant
        float tileSide = 1f;
        float padding = 0.1f;
        float separation = tileSide + padding;
        Vector2 center = Vector2.zero;
        Vector2 pivot = center + new Vector2(-width / 2 * separation, height / 2 * separation);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Tile tile = Instantiate(prefabTile).GetComponent<Tile>();
                tiles[i, j] = tile;
                tile.transform.position = pivot + new Vector2((i + 0.5f) * separation, -(j + 0.5f) * separation);
                tile.Clicked += Tile_OnClicked;
                tile.NetworkObject.Spawn();
            }
        }
    }
}
