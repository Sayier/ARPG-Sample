using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName ="Player", menuName ="Scriptable Objects/Player")]
    public class PlayerData : ScriptableObject
    {
        public float moveSpeed;

        public float attackRange;
        public float attackDamage;
    }
}