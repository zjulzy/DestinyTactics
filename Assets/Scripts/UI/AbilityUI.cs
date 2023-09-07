using System;
using System.Collections.Generic;
using DestinyTactics.Characters;
using DestinyTactics.Characters.Abilities;
using DestinyTactics.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DestinyTactics.UI
{
    public class AbilityUI : MonoBehaviour
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

            // 在开始时需要使技能面板不可见
            GetComponent<CanvasGroup>().alpha = 0;
        }

        // <summary>
        // 修改技能面板上的按钮，需要传入一个新的技能列表
        // 根据技能内容实例化技能按钮
        // </summary>
        public void OnOpenAbilityPanel(Character character)
        {
            // 使技能面板可见
            GetComponent<CanvasGroup>().alpha = 1;
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
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = a.displayName;
                newButton.GetComponentInChildren<AbilityButton>().id = id;
                if (!a.TryCommitCost())
                {
                    newButton.GetComponent<Button>().interactable = false;
                }

                id++;
            });
            selectedID = -1;
            GameObject newButton = Instantiate(abilityButtonPrefab, transform);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = "取消技能";
            newButton.GetComponentInChildren<AbilityButton>().id = -1;
        }

        public void OnCloseAbilityPanel()
        {
            GetComponent<CanvasGroup>().alpha = 0;
            //销毁旧的子对象
            foreach (var trans in transform.GetComponentsInChildren<AbilityButton>())
            {
                Destroy(trans.gameObject);
            }
        }

        public void OnActivateAbility(int id)
        {
            selectedID = id;
            SelectedAbility(selectedID);
        }

        private void ClearData()
        {
            selectedID = -1;
        }
    }
}