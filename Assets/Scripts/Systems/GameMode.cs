using UnityEngine;
using System.Collections;
using System;
using DestinyTactics.Characters;
using DestinyTactics.Players;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

// ReSharper disable All

namespace DestinyTactics.Systems
{
    public enum GameState
    {
        player,
        AI
    }

    public class GameMode : MonoBehaviour
    {
        private int round;
        private GameState _state;
        public GridSystem gridSystem;
        public AIPlayer AI;
        public Action<GameState,int> ChangeTurn;

        public void Start()
        {
            _state = GameState.player;
            round = 1;
        }

        public void Reset()
        {
            Debug.Log("reload");
            SceneManager.LoadSceneAsync("Scenes/SampleScene");
        }

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
                round++;
                gridSystem.OnAllowInput();
                foreach (var character in FindObjectsOfType<Character>())
                {
                    if (character.type == CharacterType.Player)
                    {
                        character.Reset();
                    }
                }
            }

            ChangeTurn(_state,round);
        }
    }
}