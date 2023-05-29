using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Board : NetworkBehaviour
{
    public event EventHandler TileClicked;
    [SerializeField] private GameObject prefabTile;
    [SerializeField] private GameObject prefabChecker;

    private Tile[,] tiles;
    private List<Checker> checkers;
    private int width = 8;
    private int height = 8;
    private int[,] distribution =
    {
        { 0, 1, 0, 1, 0, 1, 0, 1},
        { 1, 0, 1, 0, 1, 0, 1, 0},
        { 0, 1, 0, 1, 0, 1, 0, 1},
        { 0, 0, 0, 0, 0, 0, 0, 0},
        { 0, 0, 0, 0, 0, 0, 0, 0},
        { 2, 0, 2, 0, 2, 0, 2, 0},
        { 0, 2, 0, 2, 0, 2, 0, 2},
        { 2, 0, 2, 0, 2, 0, 2, 0}
    };


    private void Start()
    {
        PlaceTiles();
        CreateCheckers();
    }

    private void Update()
    {
        if (IsServer)
        {
            //SendCheckersDispositionClientRpc(GetCheckerDispositions());
        }
    }

    private void Tile_OnClicked (object s, EventArgs e)
    {
        TileClicked?.Invoke(s, e);
    }
    private void CreateCheckers ()
    {
        checkers = new List<Checker>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (distribution[j, i] == 0) continue;
                Checker checker = Instantiate(prefabChecker).GetComponent<Checker>();
                checker.transform.position = tiles[i,j].transform.position;
                tiles[i, j].Checker = checker;
                checkers.Add(checker);
                if (distribution[j, i] == 1) checker.SpriteRenderer.color = Color.red;
                else checker.SpriteRenderer.color = Color.blue;
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
            }
        }
    }
    [ClientRpc]
    private void SendCheckersDispositionClientRpc (short[] dispositions)
    {
        foreach (Tile tile in tiles)
        {
            tile.Checker = null;
        }
        for (int i = 0; i < dispositions.Length; i++)
        {
            Tile tile = GetTileByIndex(dispositions[i]);
            checkers[i].transform.position = tile.transform.position;
            tile.Checker = checkers[i];
        }
    }
    public void RequestMoveCheckerTotTile (Checker checker, Tile tile)
    {
        short checkerIndex = GetCheckerIndex(checker);
        short targetTileIndex = GetTileIndex(tile);
        MoveCheckerToTileServerRpc(checkerIndex, targetTileIndex);
    }
    [ServerRpc(RequireOwnership = false)] // TODO: Add verifications (Target tile is empty)
    private void MoveCheckerToTileServerRpc (short checkerIndex, short targetTileIndex)
    {
        short currentTileIndex =GetTileIndexOfChecker(checkers[checkerIndex]);
        GetTileByIndex(currentTileIndex).Checker = null;
        GetTileByIndex(targetTileIndex).Checker = checkers[checkerIndex];
        checkers[checkerIndex].transform.position = GetTileByIndex(targetTileIndex).transform.position;
        SendCheckersDispositionClientRpc(GetCheckerDispositions());
    }
    private short[] GetCheckerDispositions ()
    {
        short[] dispositions = new short[checkers.Count];
        for (int i = 0; i < checkers.Count; i++)
        {
            dispositions[i] = GetTileIndexOfChecker(checkers[i]);
        }
        return dispositions;
    }
    private short GetTileIndexOfChecker (Checker checker)
    {
        short position = 0;
        foreach(Tile tile in tiles)
        {
            if (tile.Checker == checker)
            {
                return position;
            }
            position++;
        }
        return -1;
    }
    private short GetCheckerIndex (Checker checker)
    {
        short index = 0;
        foreach(Checker c in checkers)
        {
            if (c == checker)
            {
                return index;
            }
            index++;
        }
        return -1;
    }
    private short GetTileIndex (Tile tile)
    {
        short index = 0;
        foreach(Tile t in tiles)
        {
            if (t == tile)
            {
                return index;
            }
            index++;
        }
        return -1;
    }
    private Tile GetTileByIndex (short i)
    {
        return tiles[i / 8, i % 8];
    }
}
