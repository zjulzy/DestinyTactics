using System;
using System.Collections.Generic;
using DestinyTactics.Characters;
using DestinyTactics.Characters.Abilities;
using DestinyTactics.Systems;
using UnityEngine;

namespace DestinyTactics.UI
{
    public class AbilityUI:MonoBehaviour
    {
        public GameObject abilityButtonPrefab;
        public int selectedID;
        public Systems.GridSystem gridSystem;
        public Action<int> SelectedAbility;

        //角色拥有的ability集合
        private List<Ability> _abilities;
        public void Awake()
        {
            _abilities = new List<Ability>();
            gridSystem.OpenAbilityPanel += OnOpenAbilityPanel;
            gridSystem.CloseAbilityPanel += OnCloseAbilityPanel;
        }
        
        //修改技能面板上的按钮，需要传入一个新的技能列表
        public void OnOpenAbilityPanel(Character character)
        {
            // TODO:初始化技能面板
            _abilities = character.abilities;
            var list = transform.GetComponentsInChildren<AbilityButton>();
            foreach (var button in list)
            {
                Destroy(button.gameObject);
            }

            int id = 0;
            _abilities.ForEach(a =>
            {
                GameObject newButton = Instantiate(abilityButtonPrefab, transform);
                newButton.GetComponent<AbilityButton>().id = id;
                id++;
            });
            selectedID = -1;
        }

        public void OnCloseAbilityPanel()
        {
            GetComponent<CanvasGroup>().alpha = 0;
            //销毁旧的子对象
            foreach (var trans in transform.GetComponentsInChildren<Transform>())
            {
                Destroy(trans);
            }
            _abilities.Clear();
        }

        public void OnActivateAbility(int id)
        {
            selectedID = id;
        }

        private void ClearData()
        {
            _abilities.Clear();
            selectedID = -1;
        }

    }
}