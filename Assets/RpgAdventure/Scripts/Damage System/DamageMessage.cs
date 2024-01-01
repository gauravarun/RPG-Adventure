using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RpgAdventure
{
    public partial class  Damageable : MonoBehaviour
    {
        public struct DamageMessage
        {
            public MonoBehaviour damager;
            public int amount;
            public GameObject damageSource;
        }
    }

}

