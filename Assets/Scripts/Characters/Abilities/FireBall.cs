using System;
using UnityEngine;

namespace DestinyTactics.Characters.Abilities
{
    public class FireBall:Ability
    {
        public FireBall()
        {
            displayName = "火球术";
            mPConsume = 5;
            attackRatio = 3.0f;
            coolDown = 3;
            attackRange = 5;
        }

        public override bool TryActivate(Character insighter, Character target)
        {
            if (!CommitAbility()) return false;
            insighter.bCanAttack = false;
            insighter.Attack(target,(int)(insighter.AttackValue*attackRatio));
            return true;
        }
    }
}