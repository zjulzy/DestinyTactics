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
    
    public class GameMode:MonoBehaviour
    {
        private GameState _state;
        public GridSystem gridSystem;
        public AIPlayer AI;
        public void Start()
        {
            _state = GameState.player;
        }

        public void EndTurn()
        {
            _state = _state == GameState.player ? GameState.AI : GameState.player;
            if(_state == GameState.AI)
            {
                gridSystem.ResetTurn();
                AI.execute();
            }
            else
            {
                foreach (var character in FindObjectsOfType<Character>())
                {
                    if (character.type == CharacterType.Player)
                    {
                        character.Reset();
                    }
                }
            }
        }
    }
}