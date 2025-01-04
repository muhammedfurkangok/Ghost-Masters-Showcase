using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class EntityProp : MonoBehaviour
{
    [SerializeField] private float pushBackDistance = 2f; 
    [SerializeField] private float pushBackDuration = 1f; 
    [SerializeField] private float rotateDuration = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerIsNearState(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerIsFarState();
        }
    }

    [Button]
    public void PlayerIsNearState(Transform playerTransform)
    {
        Vector3 playerDirection = (transform.position - playerTransform.position).normalized;

        playerDirection.y = 0f;

        if (Mathf.Abs(playerDirection.x) > Mathf.Abs(playerDirection.z))
        {
            float rotateAngleX = playerDirection.x > 0 ? 30f : -30f;
            transform.DOLocalRotate(new Vector3(rotateAngleX, 0f, 0f), rotateDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
        else
        {
            float rotateAngleZ = playerDirection.z > 0 ? 30f : -30f;
            transform.DOLocalRotate(new Vector3(0f, 0f, rotateAngleZ), rotateDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    [Button]
    public void PlayerIsFarState()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() => transform.DOKill());
        sequence.Append(transform.DOLocalRotate(Vector3.zero, rotateDuration).SetEase(Ease.OutQuad));
    }
}