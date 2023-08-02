using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Numerics;
using DestinyTactics.Cells;
using DestinyTactics.Characters.Abilities;
using DestinyTactics.Systems;
using DestinyTactics.UI;
using Unity.VisualScripting;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace DestinyTactics.Characters
{
    public enum CharacterType
    {
        Player,
        AI
    }

    public class Character : MonoBehaviour
    {
        #region Property

        public string displayName;
        public int defaultAP;
        public int defaultHP;
        public int defaultMP;
        public int defaultAttack;
        public int defaultAttackRange;

        private int _AP;
        private int _MP;
        private int _health;
        private int _attack;
        private int _attackRange;

        // 角色拥有的ability集合
        [System.NonSerialized] public List<Ability> abilities;
        public List<AbilityType> abilityTypes;

        public CharacterType type;
        public Cell correspondingCell;
        private Cell _destination;

        public bool bCanAttack;

        public Action allowInput;
        public Action blockInput;
        public Action<int, int> ChangeHealth;
        public Action<int, int> ChangeMP;
        public Action<Character> CharacterDead;
        public Action<Character, Vector3, Vector3> CharacterMove;
        public Action<Character, Character, int> CharacterAttack;

        private Animator _animator;

        public int AP
        {
            get { return _AP; }
            set
            {
                _AP = value;
                if (_AP <= 0) _AP = 0;
            }
        }

        public int MP
        {
            get { return _MP; }
            set
            {
                _MP = value;
                ChangeMP(value, defaultMP);
            }
        }

        public int Health
        {
            get { return _health; }
            set
            {
                _health = value;
                ChangeHealth(value, defaultHP);
                if (_health <= 0)
                {
                    _health = 0;
                    CharacterDead(this);
                    //TODO:死亡特效和音效
                    Destroy(gameObject);
                }
            }
        }

        public int AttackRange
        {
            get { return _attackRange; }
        }

        public int AttackValue
        {
            get { return _attack; }
        }

        #endregion


        public void Awake()
        {
            _animator = GetComponent<Animator>();
            _destination = correspondingCell;
            _attackRange = defaultAttackRange;
            CharacterDead += ((a) => { correspondingCell.correspondingCharacter = null; });
            ChangeHealth += GetComponentInChildren<HealthBar>().OnChangeHealth;
            CharacterDead += ((a) => { _animator.SetTrigger("Dead");});
            ChangeMP += GetComponentInChildren<HealthBar>().OnChangeMP;

            abilities = new List<Ability>();
            abilityTypes.ForEach(a =>
            {
                Type t = AbilityMapping.Mapping[a];
                var ability = (Ability)t.GetConstructor(new Type[] { }).Invoke(new object[] { });
                ability.SetOwner(this);
                abilities.Add(ability);
            });

            // 为技能设置冷却递减
            FindObjectOfType<GameMode>().ChangeTurn += (state, b) =>
            {
                if (state == GameState.player)
                    abilities.ForEach(a => { a.coolDown--; });
            };
        }

        public void Start()
        {
            _AP = defaultAP;
            bCanAttack = true;
            _attack = defaultAttack;
            Health = defaultHP;
            MP = defaultMP;

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

        public void Attack(Character target, int attackValue)
        {
            if (target.type == type)
            {
                Debug.LogError("同阵营攻击");
                return;
            }

            Debug.Log("attack");
            _animator.SetTrigger("IsAttacking");
            CharacterAttack(this, target, attackValue);
            target.Health -= _attack;
            bCanAttack = false;
            AP = 0;
        }

        public void Move(Cell destination, List<Cell> path)
        {
            StartCoroutine(MoveTorwards(destination, path));
            CharacterMove(this, transform.position, _destination.transform.position);
        }

        //使用协程处理移动
        protected IEnumerator MoveTorwards(Cell destination, List<Cell> path)
        {
            // 根据path向目的地移动,异步移动
            blockInput();
            _animator.SetBool("IsMoving", true);

            foreach (var cell in path)
            {
                _destination = cell;
                while (correspondingCell != cell)
                {
                    yield return null;
                }
            }

            _animator.SetBool("IsMoving", false);
            allowInput();
        }

        public void BeAttacked()
        {
            _animator.SetTrigger("IsAttacked");
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
                var v = (_destination.transform.position + new Vector3(0, 1, 0))-transform.position;
                // transform.rotation = Quaternion.LookRotation(v,Vector3.up);
                transform.Find("Rotate").rotation = Quaternion.LookRotation(v,Vector3.up);

                transform.position += (v / v.magnitude) * 0.02f;
                if ((transform.position - _destination.transform.position - new Vector3(0, 1, 0)).magnitude < 0.01f)
                {
                    correspondingCell = _destination;
                    transform.position = (_destination.transform.position + new Vector3(0, 1, 0));
                }
                
            }
            else
            {
                correspondingCell = _destination;
            }
        }
    }
}