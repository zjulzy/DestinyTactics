using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using DestinyTactics.Cells;

namespace DestinyTactics.Characters
{
    public enum CharacterType
    {
        Player,
        AI
    }

    public class Character : MonoBehaviour
    {
        public string name;
        public int defaultAP;
        public int defaultHP;
        public int defaultAttack;
        public int defaultAttackRange;

        private int _AP;
        private int _health;
        private int _attack;
        private int _attackRange;

        public CharacterType type;
        public Cell correspondingCell;
        private Cell _destination;

        public bool bCanAttack;

        public Action allowInput;
        public Action blockInput;
        public Action<Character> CharacterDead;

        public int AP
        {
            get { return _AP; }
            set
            {
                _AP = value;
                if (_AP <= 0) _AP = 0;
            }
        }

        public int health
        {
            get { return _health; }
            set
            {
                _health = value;
                if (_health <= 0)
                {
                    _health = 0;
                    CharacterDead(this);
                    //TODO:死亡特效和音效
                    Destroy(gameObject);
                }
            }
        }

        public int attackRange
        {
            get { return _attackRange; }
        }

        public int attack
        {
            get { return _attack; }
        }

        public void Awake()
        {
            _destination = correspondingCell;
            _attackRange = defaultAttackRange;
            CharacterDead += ((a) => { correspondingCell.correspondingCharacter = null; });
        }

        public void Start()
        {
            _AP = defaultAP;
            bCanAttack = true;
            _attack = defaultAttack;
            _health = defaultHP;

            //correspondingCell在editor中设定，不需要在代码中设定
        }

        public void Activate()
        {
            //TODO:进入待机动画
        }

        public void AttackPrepare()
        {
            //TODO:进入准备攻击状态
        }

        public void Attack(Character target)
        {
            if (target.type == type)
            {
                Debug.LogError("同阵营攻击");
                return;
            }

            Debug.Log("attack");
            target.health -= _attack;
            bCanAttack = false;
            AP = 0;
        }

        public IEnumerator Move(Cell destination, List<Cell> path)
        {
            //根据path向目的地移动,异步移动
            blockInput();
            foreach (var cell in path)
            {
                _destination = cell;
                while (correspondingCell != cell)
                {
                    yield return null;
                }
            }

            allowInput();
        }

        public void Reset()
        {
            bCanAttack = true;
            _AP = defaultAP;
        }

        public void Update()
        {
            if (_destination.GetLocation() != transform.position - new Vector3(0, 1, 0))
            {
                transform.position = Vector3.Lerp(transform.position,
                    _destination.transform.position + new Vector3(0, 1, 0), 0.1f);
                correspondingCell = _destination;
            }
        }
    }
}