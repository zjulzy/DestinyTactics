using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace DestinyTactics.Characters.Abilities
{
    [Serializable]
    public enum AbilityType
    {
        SimpleAttack,
        FireBall
    }

    public static class AbilityMapping
    {
        public static Dictionary<AbilityType, Type> Mapping = new Dictionary<AbilityType, Type>
        {
            { AbilityType.SimpleAttack, typeof(AttackAbility) },
            { AbilityType.FireBall, typeof(FireBall) },
        };
    }

    [Serializable]
    // <remarks>
    // 所有技能的基类,具体的生命周期为:
    // <para>在角色创建或者学习技能时先通过Initial绑定拥有者,</para>
    // <para>使用TryActivate激活技能,该函数会先行调用CommitAbility来计算技能消耗,如果满足消耗则进入伤害计算</para>
    // <para>TryActivate会控制角色的动画,但伤害的施加需要在角色的动画释放完毕后通过委托在OnAnimationEnd中作为回调进行</para>
    // </remarks>
    public class Ability:Object
    {
        public string displayName;
        public int coolDown;
        public int attackRange;
        public float attackRatio;

        private int _currentCoolDown;
        protected bool BIsActivated;
        protected Character Insighter;
        protected Character Target;

        // 技能的mp消耗
        public int mPConsume;
        protected Character Owner;

        public int CurrentCoolDown
        {
            get { return _currentCoolDown; }
            set { _currentCoolDown = math.max(0, value); }
        }

        public void SetOwner(Character owner)
        {
            Owner = owner;
        }

        #region AbilityLifeCycle

        public virtual void Initial(Character owner)
        {
            SetOwner(owner);
        }

        // <summary>
        // 对target激活技能,该类的派生类会继承这个函数并实现各自逻辑
        // 同时根据是否能够成功激活技能返回一个bool值
        // </summary>
        public virtual bool TryActivate(Character insighter, Character target)
        {
            return CommitAbility();
        }

        // 检测是否能够满足技能cost消耗和技能cooldown
        // 如果测试通过则直接进行扣除
        public virtual bool CommitAbility()
        {
            if (!CommitCoolDown()) return false;
            if (!CommitCost())
            {
                ResetCost();
                return false;
            }

            return true;
        }

        // 用于技能耗费MP的扣除
        public virtual bool CommitCost()
        {
            Owner.MP -= mPConsume;
            return true;
        }

        public void ResetCost()
        {
            _currentCoolDown = 0;
        }

        //检查技能是否进入CD，如果是不是则进入CD，否则返回false
        public virtual bool CommitCoolDown()
        {
            if (_currentCoolDown > 0) return false;
            _currentCoolDown = coolDown;
            return true;
        }

        // 在角色动画播放完毕后进行伤害处理或者特效
        public virtual void OnAnimationEnd()
        {
            if (!BIsActivated) return;
        }

        #endregion
    }
}