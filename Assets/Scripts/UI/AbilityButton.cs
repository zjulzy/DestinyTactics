using UnityEngine;
using System;

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
        }
    }
}