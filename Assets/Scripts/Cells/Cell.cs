using UnityEngine;
using System.Collections;
using System;
using DestinyTactics.Characters;
using DestinyTactics.Systems;
using UnityEngine.EventSystems;

namespace DestinyTactics.Cells
{
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

        public void Awake()
        {
            var gridSystem = FindObjectOfType<Systems.GridSystem>();
            GetComponentInChildren<Grid>().Unactivate();
        }

        #region IOEvent

        public void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

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

        public void ChangeCellType(CellState type)
        {
            //TODO: 根据输入值改变激活状态
            var grid = GetComponentInChildren<Grid>();
            switch (type)
            {
                case CellState.Activated:
                    break;
                case CellState.Normal:
                    grid.Unactivate();
                    transform.GetComponent<Renderer>().material.color = Color.white;
                    break;
                case CellState.Available:
                    grid.Activate();
                    break;
                case CellState.OnPath:
                    grid.ActivatePath();
                    break;
                case CellState.Hovered:
                    transform.GetComponent<Renderer>().material.color = Color.red;
                    break;
            }
        }

        #endregion
    }
}