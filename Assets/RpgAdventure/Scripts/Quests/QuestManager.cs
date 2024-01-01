using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI; 
namespace RpgAdventure
{

    public class JsonHelper
    {
        public class Wrapper<T>
        {
            public T[] array;
        }
        public static T[] GetJsonArray<T>(string json)
        {
            string newJson = "{\"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }
    }
    public class QuestManager : MonoBehaviour, IMessageReceiver
    {
        public Quest[] quests;
        public PlayerStats m_playerStats;

        private void Awake()
        {
            LoadQuestFromDB();
            AssignQuests();

            m_playerStats = FindObjectOfType<PlayerStats>();
        }

        private void LoadQuestFromDB()
        {
            using StreamReader reader = new StreamReader("C:/Users/Gaurav Arun/My First RPG adventure/Assets/RpgAdventure/DB/QuestDB.json");
            string json = reader.ReadToEnd();
            var loadedQuestes = JsonHelper.GetJsonArray<Quest>(json);
            quests = new Quest[loadedQuestes.Length];
            quests = loadedQuestes;
        }

        private void AssignQuests()
        {
            var questGivers = FindObjectsOfType<QuestGiver>();

            if (questGivers != null && questGivers.Length > 0)
            {
                foreach (var questGiver in questGivers)
                {
                    AssignQuestTo(questGiver);
                }
            }
        }

        private void AssignQuestTo(QuestGiver questGiver)
        {
            foreach (var quest in quests)
            {
                if (quest.questGiver == questGiver.GetComponent<UniqueId>().Uid)
                {
                    questGiver.quest = quest;
                }
            }
        }

        void IMessageReceiver.OnReceiveMessage(MessageType type, object sender, object message)
        {
            if (type == MessageType.DEAD)
            {
                CheckQuestWhenEnemyDead((Damageable)sender, (Damageable.DamageMessage)message);
            }
        }

        private void CheckQuestWhenEnemyDead(Damageable sender, Damageable.DamageMessage message)
        {
           var questLog = message.damageSource.GetComponent<QuestLog>();
           if (questLog == null) { return; }

           foreach (var quest in questLog.quests)
           {
                if (quest.Status == QuestStatus.ACTIVE)
                {
                    if (quest.type == QuestType.HUNT && Array.Exists(quest.targets,
                         (targetUid) => sender.GetComponent<UniqueId>().Uid == targetUid))
                    {
                        quest.amount -= 1;

                        if (quest.amount == 0)
                        {
                            quest.Status = QuestStatus.COMPLETED;
                            m_playerStats.GainExperience(quest.experience);
                        }
                    }
                }
           }
        }
    }

}
