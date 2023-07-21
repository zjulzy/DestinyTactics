using UnityEngine;
using System;
using System.Collections;
using DestinyTactics.Characters;
using DestinyTactics.GridSystem;
using TMPro;

namespace UI
{
    public class UI : MonoBehaviour
    {
        public TextMeshProUGUI nameInfo;
        public TextMeshProUGUI healthInfo;
        public TextMeshProUGUI attackInfo;
        public TextMeshProUGUI attackRangeInfo;
        public TextMeshProUGUI APInfo;
        public TextMeshProUGUI canAttackInfo;
        public GridSystem gridSystem;

        public void Awake()
        {
            gridSystem.ChangeDisplayerCharacterInfo += OnChangeDisplayerCharacterInfo;
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