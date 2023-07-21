using UnityEngine;
using System;
using System.Collections;
using DestinyTactics.Characters;
using DestinyTactics.GridSystem;
using TMPro;

namespace DestinyTactics.UI
{
    public class UI : MonoBehaviour
    {
        public TextMeshProUGUI nameInfo;
        public TextMeshProUGUI healthInfo;
        public TextMeshProUGUI attackInfo;
        public TextMeshProUGUI attackRangeInfo;
        public TextMeshProUGUI APInfo;
        public TextMeshProUGUI canAttackInfo;
        public TextMeshProUGUI turnInfo;
        public GridSystem.GridSystem gridSystem;
        public GameMode gameMode;

        public void Awake()
        {
            gridSystem.ChangeDisplayerCharacterInfo += OnChangeDisplayerCharacterInfo;
            gameMode.ChangeTurn += OnChangeTurn;
        }

        public void OnChangeTurn(GameState state)
        {
            if (state == GameState.AI)
            {
                turnInfo.text = "AI turn";
            }
            else
            {
                turnInfo.text = "Player turn";
            }
        }

        protected void OnChangeDisplayerCharacterInfo(Character character)
        {
            Debug.Log("UI:OnChangeDisplayerCharacterInfo");
            nameInfo.text = "name: " + character.name;
            healthInfo.text = "health: " + character.health;
            attackInfo.text = "attack: " + character.attack;
            attackRangeInfo.text = "attackRange: " + character.attackRange;
            APInfo.text = "AP: " + character.AP;
            canAttackInfo.text = "canattack: " + (character.bCanAttack?"yes":"no");
        }
    }
}