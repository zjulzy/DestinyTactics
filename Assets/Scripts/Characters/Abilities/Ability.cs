using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.Rendering;

namespace DestinyTactics.Characters.Abilities
{
    [System.Serializable]
    public class Ability
    {
        public string displayName;
        public int coolDown;
        public int attackRange;
        private int _currentCoolDown;
        // 技能的mp消耗
        public int mPConsume;
        protected Character _owner;

        public int CurrentCoolDown
        {
            get { return _currentCoolDown; }
            set { _currentCoolDown = math.max(0, value); }
        }

        public void SetOwner(Character owner)
        {
            _owner = owner;
        }

        #region AbilityLifeCycle

        // 对target激活技能,该类的派生类会继承这个函数并实现各自逻辑
        // 同时根据是否能够成功激活技能返回一个bool值
        public virtual bool TryActivate(Character insighter, Character target)
        {
            return CommitCost();
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

        #endregion
    }
}