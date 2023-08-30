using System;

namespace DestinyTactics.Characters.Abilities
{
    public class AttackAbility : Ability
    {
        public AttackAbility()
        {
            mPConsume = 1;
            coolDown = 1;
            attackRange = 1;
            displayName = "普通攻击";
        }

        public override bool TryActivate(Character insighter, Character target)
        {
            if (!CommitAbility()) return false;
            BIsActivated = true;
            Insighter = insighter;
            Target = target;
            insighter.bCanAttack = false;
            insighter.Attack(target, insighter.AttackValue);
            return true;
        }

        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();

            if (BIsActivated)
            {
                Target.Health -= Insighter.AttackValue;
                BIsActivated = false;
            }
        }
    }
}