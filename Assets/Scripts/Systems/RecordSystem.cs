using System;
using DestinyTactics.Characters;
using DestinyTactics.UI;
using UnityEngine;

namespace DestinyTactics.Systems
{
    public class RecordSystem:MonoBehaviour
    {
        private RecordUI _recordUI;

        public void Start()
        {
            _recordUI = FindObjectOfType<RecordUI>();
            //绑定character的移动攻击死亡委托
            foreach (var character in FindObjectsOfType<Character>())
            {
                character.CharacterDead += RecordDead;
                character.CharacterAttack += RecordAttack;
                character.CharacterMove += RecordMove;

            }
        }
        
        //记录character的移动信息
        protected void RecordMove(Character character,Vector3 start,Vector3 destination)
        {
            _recordUI.AddRecord(character.name+"移动"+start.ToString()+destination.ToString());
        }
        
        //记录character攻击信息并加入到相关UI中
        //通过血量判断会输出不同语句
        protected void RecordAttack(Character attacker,Character victim,int damage)
        {
            _recordUI.AddRecord(attacker.name+"攻击"+victim.name+damage.ToString());
        }
        
        //记录character死亡信息
        protected void RecordDead(Character deadCharacter)
        {
            _recordUI.AddRecord(deadCharacter.name+"dead");
        }
    }
}