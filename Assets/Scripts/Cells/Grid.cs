using UnityEngine;
using System;

namespace DestinyTactics.Cells
{
    public class Grid:MonoBehaviour
    {
        public void Awake()
        {
            
        }

        public void Activate()
        {
            var render = GetComponent<Renderer>();
            render.enabled = true;
            render.material.color = new Color(1,1,1,160.0f/255);

        }

        public void ActivatePath()
        {
            var render = GetComponent<MeshRenderer>();
            render.enabled = true;
            render.material.color = new Color(0,1,0,160.0f/255);
        }

        public void Unactivate()
        {
            var render = GetComponent<MeshRenderer>();
            render.enabled = false;
            render.material.color = new Color(1,1,1,160.0f/255);
        }

    }
}