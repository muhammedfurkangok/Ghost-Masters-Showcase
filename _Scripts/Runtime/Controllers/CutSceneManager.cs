using UnityEngine;
using __FurtleAll._FurtleScripts.Controllers;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class CutSceneManager : MonoBehaviour
{
    [Header("Camera Settings")] public GameObject lineRenderer;
    public GameObject cinemachineTutorialVirtualCamera;
    public Renderer objectRenderer;
    public Renderer fovSystemMat;
    public GameObject UIRoot;
    public GameObject carObject;
    public Animator carObjectAnimator;
     public GameObject playerObject;
    public GameObject canvasInAnim;

    [Header("Tutorial Settings")] private bool inputDetected = false;
    private bool tutorialCompleted = false;
    [SerializeField] float fraction = 0.65f;


    private void Start()
    {
        TutorialStartAnim();
        Screen.SetResolution((int)(Screen.currentResolution.width * fraction),
            (int)(Screen.currentResolution.height * fraction), true);
    }

    private void TutorialStartAnim()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => fovSystemMat.enabled = false);
        sequence.AppendCallback(() => objectRenderer.enabled = false);
        sequence.AppendCallback(() => UIRoot.SetActive(false));
        sequence.AppendCallback(() => playerObject.SetActive(false));

        sequence.Append(carObject.transform.DOMove(new Vector3(-5, 0, 2), 3f).SetEase(Ease.InOutQuad));
        sequence.AppendCallback(() => playerObject.SetActive(true));
        sequence.AppendCallback(() => playerObject.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.5f, 10, 1f));

        sequence.AppendCallback(() => objectRenderer.enabled = true);
        sequence.AppendCallback(() => cinemachineTutorialVirtualCamera.SetActive(false));
        sequence.AppendCallback(() => UIRoot.SetActive(true));
        sequence.AppendCallback(() => fovSystemMat.enabled = true);
    }

    [Button]
    public void EndAnim()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() => fovSystemMat.enabled = false);
        sequence.AppendCallback(() => UIRoot.SetActive(false));
        sequence.Append(Player.Instance.transform.DOMove(new Vector3(-3.5f, 0, -0.2f), 0.5f).SetEase(Ease.InOutQuad));
        sequence.Join(Player.Instance.transform.DORotate(new Vector3(0, -90, 0), 0.5f).SetEase(Ease.InOutQuad));
        sequence.Append(Player.Instance.transform.DOJump(new Vector3(-5, 0, 0), 1, 1, 0.25f).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                playerObject.SetActive(false);
                carObject.transform.DOPunchScale(new Vector3(10, 10, 10), 0.5f, 10, 1f);
            }));
        sequence.AppendCallback(() => carObjectAnimator.enabled = true);
        sequence.AppendCallback(() => carObjectAnimator.SetTrigger("carOut"));
        sequence.AppendCallback(() => DOVirtual.DelayedCall(1.5f, () => canvasInAnim.SetActive(true)));
    }
}