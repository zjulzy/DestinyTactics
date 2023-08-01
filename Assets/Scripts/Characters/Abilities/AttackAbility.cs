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
            insighter.bCanAttack = false;
            insighter.Attack(target,insighter.AttackValue);
            return true;
        }
    }
}