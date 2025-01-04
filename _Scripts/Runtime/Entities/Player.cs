using Cinemachine;
using DG.Tweening;
using FurtleGame.EventSystem;
using FurtleGame.Singleton;
using FurtleGame.TutorialSystem;
using FurtleGame.UpgradeSystem;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

namespace __FurtleAll._FurtleScripts.Controllers
{
    public class Player : SingletonMonoBehaviour<Player>
    {
        [Header("Current Usages")] public Backpack backpack;
        public Animator animator;
        public PlayerMovementController playerMovementController;
        public ExperienceDisplay experienceDisplay;

        [Header("Extensions")] public GameObject maxText;
        public GameObject canvasMax;
        public TutorialArrow tutorialArrow;

        [SerializeField] public int level = 1;
        public Stackpack stackpack;
        public Stackpack bodyBagStack;
        public Cash cashPrefab;
        public bool inFightArea = false;
        public Vector3 fightAreaDirection;

        [Header("IK Constraints")] public TwoBoneIKConstraint leftHoverboardIK;
        public TwoBoneIKConstraint leftCarryIK;

        [Header("Extensions")] public CinemachineVirtualCamera leftCam;
        public ParticleSystem stepParticle;
        public CinemachineVirtualCamera rightCam;
        public CinemachineVirtualCamera forwardCam;
        public CinemachineVirtualCamera panelCam;
        public GameObject hoverboard;
        public GameObject neededLevelPopup;
        public TextMeshProUGUI neededLevelText;
        public TextMeshProUGUI neededLevelDescriptionText;
        public WheelChair wheelChair;
        public GameObject mop;
        public GameObject CircleInTransition;
        public GameObject CircleOutTransition;


        [Header("Upgrades")] public Upgrade bagCapacityUpgrade;
        public Upgrade stackCapacityUpgrade;

        public int Level => level;
        public bool IsStationary => playerMovementController.IsStationary;

        private bool isSpeedBoosted = false;
        private float timer;

        protected override void ChildAwake()
        {
            maxText.SetActive(false);
            stackpack?.OnUpdate.AddListener(OnStackpackUpdate);
            bodyBagStack?.OnUpdate.AddListener(OnBodyBagStackUpdate);

            if (stackpack) stackpack.capacity = 3 + (stackCapacityUpgrade.level - 1) * 1;
        }

        public void OnBackpackUpdate(Storage storage)
        {
            maxText?.SetActive(backpack.IsFull());
        }

        public void OnStackpackUpdate(Storage storage)
        {
            // animator?.SetBool("Carry", !stackpack.IsEmpty());
            leftCarryIK?.DOWeight(stackpack.IsEmpty() ? 0 : 1, 0.25f);
        }

        public void OnStep()
        {
            var emitParams = new ParticleSystem.EmitParams();
            emitParams.position = transform.position;

            stepParticle.Emit(emitParams, 1);
        }

        public void ToggleNeededLevel(bool toggle, int neededLevel)
        {
            neededLevelText.text = neededLevel.ToString();
            neededLevelDescriptionText.text = $"REACH LEVEL {neededLevel} TO UNLOCK";
            neededLevelPopup.SetActive(toggle);
        }

        private void OnEnable()
        {
            // EventManager.StartListening("OnSpeedUpRewarded", OnSpeedBoost);
            // EventManager.StartListening("OnSpeedUpFinished", OnSpeedBoostFinished);
            EventManager.StartListening("OnUpgrade", OnUpgrade);
            EventManager.StartListening("OnPlayerDeath", OnPlayerDeath);
        }

        [Button]
        private void OnPlayerDeath()
        {
            Sequence sequence = DOTween.Sequence();

            sequence.AppendCallback(() => playerMovementController.locked = true);
            sequence.AppendCallback(() => backpack.Clear());
            sequence.AppendCallback (() => BackpackRenderer.Instance.ClearAllItems());
            sequence.AppendCallback(() => EventManager.TriggerEvent("OnResourcesChanged"));
            sequence.AppendCallback(() => EventManager.TriggerEvent("OnInventoryCleared"));
            sequence.AppendCallback(() => EventManager.TriggerEvent("OnInventoryChanged"));
            sequence.Append(transform.DOScale(Vector3.zero, 1).SetEase( Ease.InOutCubic)); // death anim
            sequence.Join(DOVirtual.DelayedCall( 0f, () => CircleInTransition.SetActive(true)));
            sequence.AppendInterval(2.5f);
            sequence.Append(transform.DOMove(Vector3.zero, 0.2f).SetEase( Ease.InOutCubic));
            sequence.AppendCallback(() => transform.GetComponent<NavMeshAgent>().Warp( Vector3.zero));
            sequence.Append(transform.DOScale(Vector3.one, 0.2f).SetEase( Ease.InOutCubic));
            sequence.AppendCallback(() => transform.position = Vector3.zero);
            sequence.AppendCallback(() => CircleInTransition.SetActive(false));
            sequence.AppendCallback(() => CircleOutTransition.SetActive(true));
            sequence.AppendInterval(1f);
            sequence.AppendCallback(() => CircleOutTransition.SetActive(false));
            sequence.AppendCallback(() => playerMovementController.locked = false);
        }

        private void OnDisable()
        {
            // EventManager.StopListening("OnSpeedUpRewarded", OnSpeedBoost);
            // EventManager.StopListening("OnSpeedUpFinished", OnSpeedBoostFinished);
            EventManager.StopListening("OnUpgrade", OnUpgrade);
            EventManager.StopListening("OnPlayerDeath", OnPlayerDeath);
        }

        private void OnBodyBagStackUpdate(Storage storage)
        {
            ToggleMaxCanvas(bodyBagStack.IsFull());
        }

        public void ToggleMaxCanvas(bool toggle)
        {
            canvasMax?.SetActive(toggle);
        }

        private void OnUpgrade(Upgrade upgrade)
        {
            stackpack.capacity = 3 + (stackCapacityUpgrade.level - 1) * 1;
        }

        public void PlayerAnimationActivate(string key)
        {
            animator.SetBool(key, true);
        }

        public void PlayerAnimationDeactivate(string key)
        {
            animator.SetBool(key, false);
        }

        public void PlayerAnimationBlend(float blend)
        {
            Debug.Log("Blend: " + blend);
            animator.DOBlend("Blend", blend, .25f);
        }

        public void OnSpeedBoost()
        {
            playerMovementController.SetBoostMultiplier(1.5f);
            isSpeedBoosted = true;

            hoverboard.SetActive(true);
            animator.SetBool("OnHoverboard", true);
            leftHoverboardIK.DOWeight(1f, 0.25f);

            DOVirtual.DelayedCall(120f, () =>
            {
                EventManager.TriggerEvent("OnSpeedUpFinished");
                playerMovementController.SetBoostMultiplier(1f);
                hoverboard.SetActive(false);
                animator.SetBool("OnHoverboard", false);
            });
        }

        public void OnSpeedBoostFinished()
        {
            playerMovementController.SetBoostMultiplier(1f);
            isSpeedBoosted = false;

            hoverboard.SetActive(false);
            animator.SetBool("OnHoverboard", false);
            leftHoverboardIK.DOWeight(0f, 0.25f);
        }

        public Tween ToggleWheelChair(bool state)
        {
            if (state && wheelChair.prisoner)
            {
                tutorialArrow.gameObject.SetActive(true);
                tutorialArrow.target = wheelChair.prisoner.roomPart.helpArea.transform;
            }
            else
            {
                tutorialArrow.gameObject.SetActive(false);
                tutorialArrow.target = null;
            }

            return wheelChair.transform.DOScale(state ? Vector3.one : Vector3.zero, 0.2f);
        }


        public void OnCleaning()
        {
            animator.SetBool("Clean", true);
            mop.SetActive(true);
        }

        public void OnCleaningFinished()
        {
            animator.SetBool("Clean", false);
            mop.SetActive(false);
        }


        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other.CompareTag("Room"))
        //     {
        //         var direction = other.transform.position - transform.position;
        //
        //         if (Mathf.Abs(direction.z) > Mathf.Abs(direction.x))
        //         {
        //             forwardCam.gameObject.SetActive(true);
        //         }
        //         else if (direction.x < 0)
        //         {
        //             leftCam.gameObject.SetActive(true);
        //         }
        //         else
        //         {
        //             rightCam.gameObject.SetActive(true);
        //         }
        //     }

        // }
        //
        // private void OnTriggerStay(Collider other)
        // {
        // switch (other.tag)
        // {
        //     case "FightArea":
        //         timer += Time.deltaTime;
        //         if (timer >= 1f)
        //         {
        //             HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
        //             timer = 0f;
        //         }

        //         fightAreaDirection = (other.transform.position - transform.position).normalized;
        //         transform.LookAt(other.transform);
        //         break;
        // }
        // }

        // private void OnTriggerExit(Collider other)
        // {
        //     if (other.CompareTag("Room"))
        //     {
        //         leftCam.gameObject.SetActive(false);
        //         rightCam.gameObject.SetActive(false);
        //         forwardCam.gameObject.SetActive(false);
        //     }
        //     if (other.CompareTag("FightArea"))
        //     {
        //         HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
        //         inFightArea = false;
        //         animator.SetBool("Attack", false);
        //         // other.GetComponentInParent<Cell>().StopTimer();
        //     }
        // }
    }
}