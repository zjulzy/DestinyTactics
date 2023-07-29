using System;

namespace DestinyTactics.Characters.Abilities
{
    public class AttackAbility : Ability
    {
        public override bool TryActivate(Character insighter, Character target)
        {
            if (!CommitAbility()) return false;
            insighter.MP -= mPConsume;
            insighter.bCanAttack = false;
            target.Health -= _owner.AttackValue;
            return true;
        }
    }
}