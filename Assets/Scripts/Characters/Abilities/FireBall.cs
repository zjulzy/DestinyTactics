using System;
using UnityEditor;
using UnityEngine;

namespace DestinyTactics.Characters.Abilities
{
    public class FireBall : Ability
    {
        public GameObject FireBallPrefab;

        public FireBall()
        {
            displayName = "火球术";
            mPConsume = 10;
            attackRatio = 3.0f;
            coolDown = 3;
            attackRange = 5;
            FireBallPrefab = Resources.Load<GameObject>("Prefabs/Ef_IceMagicGlowFree01");
        }

        public override bool TryActivate(Character insighter, Character target)
        {
            
            if (!CommitAbility()) return false;
            Insighter = insighter;
            Target = target;
            BIsActivated = true;
            insighter.bCanAttack = false;
            insighter.Attack(target, (int)(insighter.AttackValue * attackRatio));
            return true;
        }

        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
            // 发射带有数值的火球
            if (BIsActivated)
            {
                GameObject fireball = Instantiate(FireBallPrefab, Insighter.transform);
                fireball.GetComponent<ProjectileController>().Target = Target;
                fireball.GetComponent<ProjectileController>().Insighter = Insighter;
                fireball.GetComponent<ProjectileController>().damage = (int)(Insighter.AttackValue * attackRatio);
                BIsActivated = false;
            }
        }
    }
}