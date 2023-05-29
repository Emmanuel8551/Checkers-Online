using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Board board;
    private State currentState;
    private Tile selectedTile;

    private void Start()
    {
        ChangeState(new InitialState());
        board.TileClicked += Board_OnTileClicked;
    }

    private void Board_OnTileClicked (object s, EventArgs e)
    {
        if (NetworkManager.Singleton.IsClient)
        {
            currentState.OnTileClicked(s as Tile);
        }
    }

    private void ChangeState (State state)
    {
        currentState = state;
        currentState.GameManager = this;
    }

    private class State
    {
        public GameManager GameManager { get; set; }

        public virtual void OnTileClicked (Tile tile) { }
    }

    // No selected tile nor selected checker
    private class InitialState : State
    {
        public override void OnTileClicked(Tile tile)
        {
            if (tile.Checker != null)
            {
                GameManager.selectedTile = tile;
                GameManager.selectedTile.Selected = true;
                GameManager.ChangeState(new SelectedCheckerState());
            }
        }
    }
    // Moving tile state
    private class SelectedCheckerState : State
    {
        public override void OnTileClicked(Tile tile)
        {
            if (tile.Checker != null) return;
            if (tile == GameManager.selectedTile) return;
            if (!NetworkManager.Singleton.IsClient) return;
            Checker checker = GameManager.selectedTile.Checker;
            GameManager.selectedTile.Selected = false;
            GameManager.selectedTile = null;
            GameManager.board.RequestMoveCheckerTotTile(checker, tile);
            GameManager.ChangeState(new InitialState());
        }
    }

}
