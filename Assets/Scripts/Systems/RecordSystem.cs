using System;
using System.Collections.Generic;
using DestinyTactics.Characters;
using DestinyTactics.UI;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace DestinyTactics.Systems
{
    public class RecordSystem : MonoBehaviour
    {
        private RecordUI _recordUI;
        private Random _random;

        public void Start()
        {
            _recordUI = FindObjectOfType<RecordUI>();
            _random = new Random();
            //绑定character的移动攻击死亡委托
            foreach (var character in FindObjectsOfType<Character>())
            {
                character.CharacterDead += RecordDead;
                character.CharacterAttack += RecordAttack;
                character.CharacterMove += RecordMove;
            }
        }

        //记录character的移动信息
        protected void RecordMove(Character character, Vector3 start, Vector3 destination)
        {
            string msg = character.name + " ";
            if (Vector3.Distance(start, destination) > 7)
            {
                //在角色做远距离移动时的解说词
                List<string> santances = new List<string>
                {
                    "转进如风",
                    "长驱直入直达目的地",
                    "一路狂奔",
                    "向目的地进发",
                    "大步流星",
                    "施展疾风步",
                    "疾如风",
                    "随风飘荡",
                    "风驰电掣",
                    "全速前进，没有人能跟上他的速度",
                    "以最快的速度赶往目的地",
                    "风驰电掣般的速度"
                };
                msg+=santances[_random.Next(0, santances.Count)];
            }
            else if (Vector3.Distance(start, destination) > 5)
            {
                //角色做中距离移动时的解说词
                List<string> santances = new List<string>
                {
                    "战术转移",
                    "向目的地移动",
                    "向目的地进发",
                    "溜了溜了"
                };
                msg+=santances[_random.Next(0, santances.Count)];
            }
            else
            {
                //角色做短距离移动时的解说词
                List<string> santances = new List<string>
                {
                    "近距离移动",
                    "接到手谕，机枪手阵地向前移动十米",
                    "略微调整位置",
                    "向前移动",
                    "向前走了几步",
                    "移动一点",
                    "没病走两步"
                };
                msg+=santances[_random.Next(0, santances.Count)];
            }

            _recordUI.AddRecord(msg);
        }

        //记录character攻击信息并加入到相关UI中
        //通过血量判断会输出不同语句
        protected void RecordAttack(Character attacker, Character victim, int damage)
        {
            string msg = "{0}" + " ";
            // 根据造成伤害damage的不同，随机匹配解说词
            if (damage > 50)
            {
                List<string> santances = new List<string>
                {
                    "对{1}造成了高达{2}的巨额伤害",
                    "打了{1} {2}的伤害，把屎都打出来了",
                    "对{1}造成了{2}的伤害，把{1}打的屁滚尿流",
                    "对{1}造成了{2}的伤害，{1}的屁股都被打烂了",
                    "拍了拍{1} ,并一个大逼斗打了{1} {2}点血",
                    "会心一击！对{1}造成了{2}点伤害"
                };
                msg+=santances[_random.Next(0, santances.Count)];
                msg = string.Format(msg, attacker.name, victim.name, damage);
            }else if (damage > 20)
            {
                List<string> santances = new List<string>
                {
                    "全力一击，对{1}造成{2}点伤害",
                };
                msg+=santances[_random.Next(0, santances.Count)];
                msg = string.Format(msg, attacker.name, victim.name, damage);
            }
            //伤害较低但是能够打死对手
            else if (victim.Health < damage)
            {
                List<string> santances = new List<string>
                {
                    "打了{1} {2}点血量，不多，但够用",
                    "对{1}造成了{2}点伤害，精致的伤害计算"
                };
                msg+=santances[_random.Next(0, santances.Count)];
                msg = string.Format(msg, attacker.name, victim.name, damage);
            }
            else
            {
                List<string> santances = new List<string>
                {
                    "对{1}造成了{2}点伤害，打的不如回的多",
                    "对{1}造成了高达{2}的巨额伤害，没有活可以去咬打火机"
                    
                };
                msg+=santances[_random.Next(0, santances.Count)];
                msg = string.Format(msg, attacker.name, victim.name, damage);
            }
            
            _recordUI.AddRecord(msg);
        }

        //记录character死亡信息
        protected void RecordDead(Character deadCharacter)
        {
            List<string> santances = new List<string>
            {
                "寄了",
                "死了",
                "没活了",
                "结束了自己罪恶的一生",
                "躺下了",
                "进入了墓地",
                "去了天堂",
                    
            };
            string msg = deadCharacter.name + " " + santances[_random.Next(0, santances.Count)];
            _recordUI.AddRecord(msg);
        }
    }
}