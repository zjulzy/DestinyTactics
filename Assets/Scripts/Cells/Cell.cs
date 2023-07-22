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
    }
}