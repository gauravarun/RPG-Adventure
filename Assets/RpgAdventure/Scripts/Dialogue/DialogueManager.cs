using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RpgAdventure
{
    public class DialogueManager : MonoBehaviour
    {
        public float timeToShowOptions = 2.0f;
        public float maxDialogueDistance;
        public GameObject dialogueUI;
        public TextMeshProUGUI dialogueHeaderText;
        public TextMeshProUGUI dialogueAnswerText;
        public GameObject dialogueOptionList;
        public Button dialogueOptionPrefab;

        private PlayerInput m_Player;
        private QuestGiver m_Npc;
        private Dialogue m_ActiveDialogue;
        private float m_OptionTopPosition;
        private float m_TimerToShowOptions;
        private bool m_ForceDialogueQuit;

        const float c_DistanceBetweenOption = 32.0f;
        public bool HasActiveDialogue { get { return m_ActiveDialogue != null; } }
        public float DialogueDistance
        {
            get
            {
                return Vector3.Distance(
                        m_Player.transform.position,
                        m_Npc.transform.position);
            }
        }


        private void Start()
        {
            m_Player = PlayerInput.Instance;
        }

        private void Update()
        {
            if (!HasActiveDialogue &&
                m_Player.OptionClickTrager != null)
            {
                if (m_Player.OptionClickTrager.CompareTag("QuestGiver"))
                {
                    m_Npc = m_Player.OptionClickTrager.GetComponent<QuestGiver>();

                    if (DialogueDistance < maxDialogueDistance)
                    {
                        StartDialogue();
                    }
                }

            }

            if (HasActiveDialogue && DialogueDistance > maxDialogueDistance + 1.0f)
            {
                StopDialogue();
            }

            if (m_TimerToShowOptions > 0)
            {
                m_TimerToShowOptions += Time.deltaTime;

                if (m_TimerToShowOptions >= timeToShowOptions)
                {
                    m_TimerToShowOptions = 0;

                    if (m_ForceDialogueQuit)
                    {
                        StopDialogue();
                    }

                    else
                    {
                        DisplayDialogueOptions();
                    }
                }
            }
        }

        private void StartDialogue()
        {
            m_ActiveDialogue = m_Npc.dialogue;
            dialogueHeaderText.text = m_Npc.name;
            dialogueUI.SetActive(true);

            ClearDialogueOptioms();
            DisplayAnswerText(m_ActiveDialogue.welcomeText);
            TriggerDialogueOptions();

        }

        private void DisplayAnswerText(string answerText)
        {
            dialogueAnswerText.gameObject.SetActive(true);
            dialogueAnswerText.text = answerText;
        }

        private void DisplayDialogueOptions()
        {
            HideAnswerText();
            CreateDialogueMenu();
        }

        private void TriggerDialogueOptions()
        {
            m_TimerToShowOptions = 0.001f;
        }

        private void HideAnswerText()
        {
            dialogueAnswerText.gameObject.SetActive(false);
        }

        private void CreateDialogueMenu()
        {
            m_OptionTopPosition = 0;
            var queries = Array.FindAll(m_ActiveDialogue.queries, query => !query.isAsked);

            foreach (var query in queries)
            {
                m_OptionTopPosition += c_DistanceBetweenOption;
                var dialogueoption = CreateDialogueOption(query.text);
                RegisterOptionClickHandler(dialogueoption, query);
            }
        }

        private Button CreateDialogueOption(string optionText)
        {
            Button buttonInstance = Instantiate(dialogueOptionPrefab, dialogueOptionList.transform);
            buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text = optionText;

            RectTransform rt = buttonInstance.GetComponent<RectTransform>();
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, m_OptionTopPosition, rt.rect.height);

            return buttonInstance;
        }

        private void RegisterOptionClickHandler(Button dialogueoption, DialogueQuery query)
        {
            EventTrigger trigger = dialogueoption.gameObject.AddComponent<EventTrigger>();
            var pointerDown = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };

            pointerDown.callback.AddListener((e) =>
            {

                if (!String.IsNullOrEmpty(query.answer.questId))
                {
                    m_Player.GetComponent<QuestLog>().AddQuest(m_Npc.quest);
                }
                if (query.answer.forceDialogueQuit)
                {
                    m_ForceDialogueQuit = true;
                }

                if (!query.isAlwaysAsked)
                {
                    query.isAsked = true;
                }

                ClearDialogueOptioms();
                DisplayAnswerText(query.answer.text);
                TriggerDialogueOptions();
            });

            trigger.triggers.Add(pointerDown);
        }

        private void StopDialogue()
        {
            m_Npc = null;
            m_ActiveDialogue = null;
            m_TimerToShowOptions = 0;
            m_ForceDialogueQuit = false;
            dialogueUI.SetActive(false);
        }

        private void ClearDialogueOptioms()
        {
            foreach (Transform child in dialogueOptionList.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}

