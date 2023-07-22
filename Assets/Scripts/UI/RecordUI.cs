using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace DestinyTactics.UI
{
    public class RecordUI:MonoBehaviour
    {
        public GameObject recordPrefab;
        // private int i;
        public void Start()
        {
            
        }

        public void AddRecord(string msg)
        {
            GameObject newRecord = Instantiate(recordPrefab, transform);
            newRecord.transform.GetComponent<TextMeshProUGUI>().text = msg;
        }

        public void Update()
        {
            // 测试文本生成
            // i++;
            // if (i % 50 == 0)
            // {
            //     AddRecord(i.ToString());
            // }
        }
    }
}