using System;
using System.Numerics;

namespace RpgAdventure
{
    public enum QuestType
    {
        HUNT,
        GATHER,
        EXPLORE,
        TALK
    }
    [System.Serializable]
    public class Quest
    {
       public string uid;
       public string title;
       public string description;
       public int experience;
       public int gold;

       public int amount;
       public string [] targets;

       public string talkTo;
       public Vector3 explore;
       public string questGiver;
       public QuestType type;

    }
}

