using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DestinyTactics.UI
{
    public class HealthBar : MonoBehaviour
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public void OnChangeHealth(int health, int defaultHealth)
        {
            Debug.Log("HealthBar:OnChangeHealth");
            var canvas = (RectTransform)transform.Find("Canvas");
            var imageBackground = (RectTransform)canvas.Find("Image_background");
            var imageFill = (RectTransform)canvas.Find("Image_fill");
            imageBackground.sizeDelta = new Vector2((float)defaultHealth/50, imageBackground.sizeDelta.y);
            canvas.sizeDelta = new Vector2((float)defaultHealth/50, canvas.sizeDelta.y);
            imageFill.sizeDelta = new Vector2((float)health/50, imageFill.sizeDelta.y);
        }
    }
}