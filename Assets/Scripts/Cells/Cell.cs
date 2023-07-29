using UnityEngine;
using System.Collections;
using System;
using DestinyTactics.Characters;

namespace DestinyTactics.Cells
{
    public enum CellType
    {
        Normal,
        Clicked
    }

    public enum ActivateType
    {
        Unactivated,
        Activated,
        MoveAvailable,
        AttackPrepared,
        AttackTargeted,
        OnPath
    }

    [Serializable]
    public class Cell : MonoBehaviour
    {
        public bool bIsObstacle;
        public int aPConsume;
        public Character correspondingCharacter;

        public Action<Cell> OnCellClick;
        public Action<Cell> OnCellHover;
        public Action<Cell> OnCellUnHover;

        public Vector3 GetLocation()
        {
            return transform.transform.position;
        }

        #region IOEvent

        public void OnMouseDown()
        {
            Debug.Log("Cell clicked");

            OnCellClick(this);
            
        }

        public void OnMouseEnter()
        {
            Debug.Log("Cell hovered");
            // GetComponent<Renderer>().material.color = GridSystem.CellColor.hover;
            OnCellHover(this);
            
            
        }

        public void OnMouseExit()
        {
            Debug.Log("Cell unhovered");
            // GetComponent<Renderer>().material.color = GridSystem.CellColor.normal;
            OnCellUnHover(this);
        }

        #endregion

        #region Display

        public void ChangeActivateType(ActivateType type)
        {
            //TODO: 根据输入值改变激活状态
        }

        #endregion
    }
}