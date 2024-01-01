using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RpgAdventure
{
    public class PlayerStats : MonoBehaviour, IMessageReceiver
    {
        public int maxLevel;
        public int currentLevel;
        public int currentExp;
        public int[] availableLevels;

        public int ExperienceToNextLevel
        {
            get
            {
                return availableLevels[currentLevel] - currentExp;
            }
        }

        private void Awake()
        {
            availableLevels = new int[maxLevel];
            ComputeLevels(maxLevel);
        }

        private void ComputeLevels(int levelCount)
        {
            for (int i = 0; i < levelCount; i++)
            {
                var level = i + 1;
                var levelPow = Mathf.Pow(level, 2);
                var expToLevel = Convert.ToInt32(levelPow * levelCount);
                availableLevels[i] = expToLevel;
            }
        }

        void IMessageReceiver.OnReceiveMessage(MessageType type, object sender, object message)
        {
            if (type == MessageType.DEAD)
            {
                var exp = ((Damageable)sender).experience;
                GainExperience(exp);
            }
        }
        public void GainExperience(int gainedExp)
        {
            if (gainedExp > ExperienceToNextLevel)
            {
                var remainderExp = gainedExp - ExperienceToNextLevel;
                currentExp = 0;
                currentLevel ++;
                GainExperience(remainderExp);
            }
            else if (gainedExp == ExperienceToNextLevel)
            {
                currentLevel ++;
                currentExp = 0;
            } else
            {
                currentExp += gainedExp;
            }
        }
    }

}
