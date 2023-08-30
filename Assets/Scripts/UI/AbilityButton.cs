using UnityEngine;
using System;
using UnityEngine.UI;

namespace DestinyTactics.UI
{
    public class AbilityButton:MonoBehaviour
    {
        public int id;
        public Action<int> ActivateAbility;
        public void Awake()
        {
            
        }

        public void OnClick()
        {
            GetComponentInParent<AbilityUI>().OnActivateAbility(id);
            GetComponentInParent<AbilityUI>().GetComponentInChildren<Button>().image.color=Color.white;
            GetComponent<Button>().image.color= Color.gray;
        }
        
    }
}