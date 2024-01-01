using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace RpgAdventure
{
    public class BanditBehaviour : MonoBehaviour, IMessageReceiver, IAttackAnimListener
    {
        public PlayerScanner playScanner;
        public float timeToStopPursuit = 2.0f;
        public float timeToWaitOnPursuit = 2.0f;
        public float attackDistance = 1.1f;
        public MeleeWeapon meleeWeapon;
        public bool HasFollowTarget
        {
            get
            {
                return m_Followtarget != null;
            }
        }

        private PlayerController m_Followtarget;
        private NavMeshAgent m_NavMeshAgent;
        private float m_TimeSinceLostTarget = 0f;
        private Vector3 m_OriginPosition;
        private Quaternion m_OriginRotation;
        private EnemyController m_EnemyController;

        private readonly int m_HashInPursuit = Animator.StringToHash("InPusuit");
        private readonly int m_HashNearBase = Animator.StringToHash("NearBase");
        private readonly int m_HashAttack = Animator.StringToHash("Attack");
        private readonly int m_HashHurt = Animator.StringToHash("Hurt");
        private readonly int m_HashDead = Animator.StringToHash("Dead");

        private void Awake()
        {
            m_EnemyController = GetComponent<EnemyController>();
            m_OriginPosition = transform.position;
            m_OriginRotation = transform.rotation;
            meleeWeapon.SetOwner(gameObject);
            meleeWeapon.SetTargetLayer(1 << PlayerController.Instance.gameObject.layer);
        }

        private void Update()
        {
            if (PlayerController.Instance.IsRespawning)
            {
                GoToOriginSpot();
                CheckIfNearBase();
                return;
            }
            GuardPosition();
        }

        private void GuardPosition()
        {
            var detectedtarget = playScanner.Detect(transform);
            bool hasDetectedTarget = detectedtarget != null;

            if (hasDetectedTarget) { m_Followtarget = detectedtarget; }

            if (HasFollowTarget)
            {
                AttackOrFollowTarget();

                if (hasDetectedTarget)
                {
                    m_TimeSinceLostTarget = 0f;
                }
                else
                {
                    StopPursuit();
                }
            }
            CheckIfNearBase();
        }

        void IMessageReceiver.OnReceiveMessage(MessageType type, object sender, object message)
        {
            switch (type)
            {
                case MessageType.DEAD:
                    OnDead();
                    break;
                case MessageType.DAMAGED:
                    OnReceiveDamage();
                    break;
                default:
                    break;

            }
        }

        public void MeleeAttackStart()
        {
            meleeWeapon.BeginAttack();
        }

        public void MeleeAttackEnd()
        {
            meleeWeapon.EndAttack();
        }

        private void OnDead()
        {
            m_EnemyController.StopFollowTarget();
            m_EnemyController.Animator.SetTrigger(m_HashDead);
        }

        private void OnReceiveDamage()
        {
            m_EnemyController.Animator.SetTrigger(m_HashHurt);
        }

        private void AttackOrFollowTarget()
        {
            Vector3 toTarget = m_Followtarget.transform.position - transform.position;
            if (toTarget.magnitude <= attackDistance)
            {
                AttackTarget(toTarget);
            }
            else
            {
                FollowTarget();
            }
        }

        private void GoToOriginSpot()
        {
            m_Followtarget = null;
            m_EnemyController.Animator.SetBool(m_HashInPursuit, false);
            m_EnemyController.FollowTarget(m_OriginPosition);
        }

        private void StopPursuit()
        {

            m_TimeSinceLostTarget += Time.deltaTime;
            if (m_TimeSinceLostTarget >= timeToStopPursuit)
            {
                m_Followtarget = null;
                m_EnemyController.Animator.SetBool(m_HashInPursuit, false);
                StartCoroutine(WaitBeforeReturn());
            }
        }

        private void AttackTarget(Vector3 toTarget)
        {
            var toTargetRotation = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.RotateTowards
            (
                transform.rotation,
                toTargetRotation,
                360 * Time.deltaTime
            );
            m_EnemyController.StopFollowTarget();
            m_EnemyController.Animator.SetTrigger(m_HashAttack);
        }

        private void FollowTarget()
        {
            m_EnemyController.Animator.SetBool(m_HashInPursuit, true);
            m_EnemyController.FollowTarget(m_Followtarget.transform.position);
        }

        private void CheckIfNearBase()
        {
            Vector3 toBase = m_OriginPosition - transform.position;
            toBase.y = 0;

            bool nearBase = toBase.magnitude < 0.01f;
            m_EnemyController.Animator.SetBool(m_HashNearBase, nearBase);
            if (nearBase)
            {
                Quaternion targetRotation = Quaternion.RotateTowards
                (
                    transform.rotation,
                    m_OriginRotation,
                    360 * Time.deltaTime
                );
                transform.rotation = targetRotation;
            }
        }

        private IEnumerator WaitBeforeReturn()
        {
            yield return new WaitForSeconds(timeToWaitOnPursuit);
            m_EnemyController.FollowTarget(m_OriginPosition);

        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Color c = new Color(0.8f, 0, 0, 0.4f);
            UnityEditor.Handles.color = c;

            Vector3 rotateForward = Quaternion.Euler(0, -playScanner.detectionAngle * 0.5f, 0) * transform.forward;

            UnityEditor.Handles.DrawSolidArc(
                transform.position,
                Vector3.up,
                rotateForward,
                playScanner.detectionAngle,
                playScanner.detectionRadius);


            UnityEditor.Handles.DrawSolidArc(
                transform.position,
                Vector3.up,
                rotateForward,
                360,
                playScanner.meleeDetectionRadius);
        }
#endif
    }

}
