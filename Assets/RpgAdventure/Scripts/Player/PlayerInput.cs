using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RpgAdventure
{
    public class PlayerInput : MonoBehaviour
    {
        public static PlayerInput Instance { get { return s_Instance; } }
        public float distanceToInteractWithNpc = 2.0f;
        public bool isPlayerControllerInputBlocked;
        public static PlayerInput s_Instance;

        private Vector3 m_movement;
        private bool m_Isattack;

        private Collider m_OptionClickTarget;

        public Collider OptionClickTrager { get { return m_OptionClickTarget; } }
        public Vector3 MoveInput 
        { 
            get 
            { 
                if (isPlayerControllerInputBlocked)
                {
                    return Vector3.zero;
                }
                return m_movement; 
            } 
        }
        public bool IsAttack 
        { 
            get 
            { 
                return !isPlayerControllerInputBlocked && m_Isattack; 
            } 
        }
        public bool IsMoveInput { get { return !Mathf.Approximately(MoveInput.magnitude, 0); } }
        


        private void Awake()
        {
            s_Instance = this;
        }
        void Update()
        {
            m_movement.Set(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical")
            );

            bool isLeftMouseClick = Input.GetMouseButtonDown(0);
            bool isRightMouseClick = Input.GetMouseButtonDown(1);

            if (isLeftMouseClick)
            {
                HandleLeftMouseBtnDown();
            }

            if (isRightMouseClick)
            {
                HandleRightMouseBtnDown();
            }
        }

        private void HandleLeftMouseBtnDown()
        {
            if (!IsAttack && !IsPointerOverUiElement())
            {     
                StartCoroutine(TriggerAttack());
            }
        }

        private bool IsPointerOverUiElement()
        {
            var eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            return results.Count > 0;
        }

        private void HandleRightMouseBtnDown()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hasHit = Physics.Raycast(ray, out RaycastHit hit);

            if (hasHit)
            {
                StartCoroutine(TriggerOptionTarget(hit.collider));
            }
        }
        private IEnumerator TriggerOptionTarget(Collider other)
        {
            m_OptionClickTarget = other;
            yield return new WaitForSeconds(0.03f);
            m_OptionClickTarget = null;

        }
        private IEnumerator TriggerAttack()
        {
            m_Isattack = true;
            yield return new WaitForSeconds(0.03f);
            m_Isattack = false;

        }
    }
}
