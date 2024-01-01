using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
namespace RpgAdventure
{
    [System.Serializable]
    public class DialogueAnswer
    {
        [TextArea(3, 15)]
        public string text;
        public bool forceDialogueQuit;
        public string questId;
    }
    [System.Serializable]
    public class DialogueQuery
    {
        [TextArea(3, 15)]
        public string text;
        public DialogueAnswer answer;
        public bool isAsked;
        public bool isAlwaysAsked;
    }
    [System.Serializable]
    public class Dialogue
    {
        [TextArea(3, 15)]
        public String welcomeText;
        public DialogueQuery[] queries;
    }
}

