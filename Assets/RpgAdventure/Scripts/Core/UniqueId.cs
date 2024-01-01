using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RpgAdventure
{
    public class UniqueId : MonoBehaviour
    {
        [SerializeField]
        private string m_Uid = Guid.NewGuid().ToString();
        public string Uid {get {return m_Uid; } }
    }
}

