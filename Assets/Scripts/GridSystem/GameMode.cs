using UnityEngine;
using System.Collections;
using System;
using DestinyTactics.Characters;
using Players;
using Unity.VisualScripting;

namespace DestinyTactics.GridSystem
{
    public enum GameState
    {
        player,
        AI
    }

    public class GameMode : MonoBehaviour
    {
        private GameState _state;
        public GridSystem gridSystem;
        public AIPlayer AI;
        public Action<GameState> ChangeTurn;

        public void Start()
        {
            _state = GameState.player;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void EndTurn()
        {
            
            _state = _state == GameState.player ? GameState.AI : GameState.player;
            if (_state == GameState.AI)
            {
                gridSystem.ResetTurn();
                foreach (var character in FindObjectsOfType<Character>())
                {
                    if (character.type == CharacterType.AI)
                    {
                        character.Reset();
                    }
                }
                gridSystem.OnBlockInput();
                StartCoroutine(AI.execute());
            }
            else
            {
                gridSystem.OnAllowInput();
                foreach (var character in FindObjectsOfType<Character>())
                {
                    if (character.type == CharacterType.Player)
                    {
                        character.Reset();
                    }
                }
            }

            ChangeTurn(_state);
        }
    }
}