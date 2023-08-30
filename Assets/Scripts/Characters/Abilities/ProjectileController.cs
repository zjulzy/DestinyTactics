using UnityEngine;
using System;

namespace DestinyTactics.Characters.Abilities
{
    public class ProjectileController : MonoBehaviour
    {
        public Character Target;
        public Character Insighter;
        public int damage;

        private void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, Target.transform.position, 0.1f);
            if (Vector3.Distance(transform.position, Target.transform.position) < 0.1f)
            {
                Target.Health -= damage;
                Destroy(gameObject);
            }
        }
    }
}