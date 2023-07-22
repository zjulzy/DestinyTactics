using UnityEngine;
using System;
using System.Collections.Generic;
using DestinyTactics.Characters;
using UnityEngine.SceneManagement;

namespace GridSystem.Players
{
    public class Player:MonoBehaviour
    {
        private List<Character> _characters;

        public void Start()
        {
            foreach (var character in FindObjectsOfType<Character>())
            {
                if (character.type == CharacterType.Player)
                {
                    _characters.Add(character);
                    character.CharacterDead += (c) =>
                    {
                        _characters.Remove(c);
                    };
                }
            }
        }

        public void Update()
        {
            if (_characters.Count == 0)
            {
                SceneManager.LoadSceneAsync("Scenes/DefeatScene");
            }
        }
    }
}