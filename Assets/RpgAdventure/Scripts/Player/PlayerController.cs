using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RpgAdventure
{

    public class PlayerController : MonoBehaviour, IAttackAnimListener, IMessageReceiver
    {
        public static PlayerController Instance { get { return s_Instance; } }
        public bool IsRespawning { get { return m_IsRespawning; } }
        public MeleeWeapon meleeWeapon;
        public float maxForwardSpeed = 8.0f;
        public float rotaionSpeed;
        public float m_MaxRotationSpeed = 1200;
        public float m_MinRotationSpeed = 800;
        public float gravity = 20.0f;
        public Transform attackHand;
        public RandomAudioPlayer sprintAudio;
        


        private static PlayerController s_Instance;
        private PlayerInput m_PlayerInput;
        private Damageable m_Damageable;
        private CameraController m_CameraController;
        private Vector3 m_movement;
        private CharacterController m_ChController;
        private Quaternion m_rotation;
        private Animator m_animator;
        private float m_desiredForwardSpeed;
        private float m_ForwardSpeed;
        private Quaternion m_targetRotation;
        private float m_VerticalSpeed;
        private HudManager m_HUDManager;

        private AnimatorStateInfo m_CurrentStateInfo;
        private AnimatorStateInfo m_NextStateInfo;
        private bool m_IsAnimatorTransitioning;
        private bool m_IsRespawning;

        // Animator Parameter Hashes
        private readonly int m_HashForwarSpeed = Animator.StringToHash("ForwardSpeed");
        private readonly int m_HashMeleeAttack = Animator.StringToHash("MeleeAttack");
        private readonly int m_HashDeath = Animator.StringToHash("Death");
        private readonly int m_HashFootFall = Animator.StringToHash("FootFall");

        // Animator Tags Hashes
        private readonly int m_HashBlockInput = Animator.StringToHash("BlockInput");



        const float k_Acceleration = 20f;
        const float k_Dcceleration = 200f;

        void Awake()
        {
            m_ChController = GetComponent<CharacterController>();
            m_PlayerInput = GetComponent<PlayerInput>();
            m_animator = GetComponent<Animator>();
            m_Damageable = GetComponent<Damageable>();
            m_CameraController = Camera.main.GetComponent<CameraController>();
            m_HUDManager = FindObjectOfType<HudManager>();
            m_Damageable = GetComponent<Damageable>();
            s_Instance = this;

            m_HUDManager.SetMaxHealth(m_Damageable.maxHitPoints);

        }
        private void FixedUpdate()
        {
            CacheAnimationState();
            UpdateInputBlocking();
            ComputeForwardMovment();
            ComputeVerticalMovment();
            ComputeRotaion();

            if (m_PlayerInput.IsMoveInput)
            {
                float rotaionSpeed = Mathf.Lerp(m_MaxRotationSpeed, m_MinRotationSpeed, m_ForwardSpeed / m_desiredForwardSpeed);
                m_targetRotation = Quaternion.RotateTowards(
                    transform.rotation,
                    m_targetRotation,
                    rotaionSpeed * Time.fixedDeltaTime
                );
                transform.rotation = m_targetRotation;
            }
            m_animator.ResetTrigger(m_HashMeleeAttack);
            if (m_PlayerInput.IsAttack)
            {
                m_animator.SetTrigger(m_HashMeleeAttack);

            }

            PlayerSprintAudio();

        }

        private void OnAnimatorMove()
        {
            if (m_IsRespawning) { return; }

            Vector3 movement = m_animator.deltaPosition;
            movement += m_VerticalSpeed * Vector3.up * Time.fixedDeltaTime;
            m_ChController.Move(movement);
        }

        public void OnReceiveMessage(MessageType type, object sender, object msg)
        {
            if (type == MessageType.DAMAGED)
            {
                m_HUDManager.SetHealth((sender as Damageable).CurrentHitPoints);
            }

            if (type == MessageType.DEAD)
            {
                m_IsRespawning = true;
                m_animator.SetTrigger(m_HashDeath);
                m_HUDManager.SetHealth(0);
            }
        }

        public void MeleeAttackStart()
        {
            if (meleeWeapon != null)
            {
                meleeWeapon.BeginAttack();
            }
            
        }

        public void MeleeAttackEnd()
        {
            if (meleeWeapon != null) 
            { 
                meleeWeapon.EndAttack();
            }
                
        }

        public void StartRespawn()
        {
            transform.position = Vector3.zero;
            m_HUDManager.SetHealth(m_Damageable.maxHitPoints);
            m_Damageable.SetInitialHealth();

        }

        public void FinishRespawn()
        {
            m_IsRespawning = false;
        }

        public void UseItemFrom(InventorySlot Slot)
        {
            if (meleeWeapon != null)
            {
                if (Slot.itemPrefab.name == meleeWeapon.name)
                {
                    return;
                }
                else
                {
                    Destroy(meleeWeapon.gameObject);
                    
                }
            }
            meleeWeapon = Instantiate(Slot.itemPrefab, transform)
            .GetComponent<MeleeWeapon>();
            meleeWeapon.GetComponent<FixedUpdateFollow>().SetFollowee(attackHand);
            meleeWeapon.name = Slot.itemPrefab.name;
            meleeWeapon.SetOwner(gameObject);
        }

        private void OnCollisionExit(Collision collision)
        {

        }

        private void OnBeforeTransformParentChanged()
        {

        }

        private void OnAudioFilterRead(float[] data, int channels)
        {

        }

        private void OnApplicationPause(bool pause)
        {

        }

        private void OnAnimatorIK(int layerIndex)
        {

        }

        private void LateUpdate()
        {

        }

        private void ComputeVerticalMovment()
        {
            m_VerticalSpeed = -gravity;
        }

        private void ComputeForwardMovment()
        {
            Vector3 moveInput = m_PlayerInput.MoveInput.normalized;
            m_desiredForwardSpeed = moveInput.magnitude * maxForwardSpeed;

            float Acceleration = m_PlayerInput.IsMoveInput ? k_Acceleration : k_Dcceleration;

            m_ForwardSpeed = Mathf.MoveTowards(
                m_ForwardSpeed,
                m_desiredForwardSpeed,
                Time.fixedDeltaTime * Acceleration);

            m_animator.SetFloat(m_HashForwarSpeed, m_ForwardSpeed);
        }


        private void ComputeRotaion()
        {
            Vector3 moveInput = m_PlayerInput.MoveInput.normalized;

            Vector3 cameraDirection = Quaternion.Euler(
                0,
                m_CameraController.PlayerCam.m_XAxis.Value,
                0) * Vector3.forward;
            Quaternion targetRotation;

            if (Mathf.Approximately(Vector3.Dot(moveInput, Vector3.forward), -1.0f))
            {
                targetRotation = Quaternion.LookRotation(-cameraDirection);
            }
            else
            {
                Quaternion movermentRotation = Quaternion.FromToRotation(Vector3.forward, moveInput);
                targetRotation = Quaternion.LookRotation(movermentRotation * cameraDirection);

            }
            m_targetRotation = targetRotation;
        }

        private void CacheAnimationState()
        {
            m_CurrentStateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
            m_NextStateInfo = m_animator.GetNextAnimatorStateInfo(0);
            m_IsAnimatorTransitioning = m_animator.IsInTransition(0);

        }

        private void UpdateInputBlocking()
        {
            bool inputBlocked = m_CurrentStateInfo.tagHash == m_HashBlockInput && !m_IsAnimatorTransitioning;
            inputBlocked |= m_NextStateInfo.tagHash == m_HashBlockInput;
            m_PlayerInput.isPlayerControllerInputBlocked = inputBlocked;
        }

        private void PlayerSprintAudio()
        {
            float footFallCurve = m_animator.GetFloat(m_HashFootFall);

            if (footFallCurve > 0.001f && !sprintAudio.isPlaying && sprintAudio.canPlay) 
            {
                sprintAudio.isPlaying = true;
                sprintAudio.canPlay = false;
                sprintAudio.PlayRandomClip();
            }
            else if (sprintAudio.isPlaying) 
            {
                sprintAudio.isPlaying = false;
            }
            else if (footFallCurve < 0.01f && !sprintAudio.canPlay)
            {
                 sprintAudio.canPlay = true;
            }
        }

    }


}


