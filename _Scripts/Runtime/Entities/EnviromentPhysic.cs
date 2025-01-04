using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnviromentPhysic : MonoBehaviour
{
    [SerializeField] private GameObject deadVisual;
    [SerializeField] private GameObject liveVisual;
    [SerializeField] private ParticleSystem revealParticle;
    
    
    [SerializeField] private float scaleDownDuration;
    [SerializeField] private float scaleUpDuration;
    
    [SerializeField] private Ease scaleDownEase;
    [SerializeField] private Ease scaleUpEase;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            ChangeVisual();
        }
    }

    private void ChangeVisual()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(deadVisual.transform.DOScale(Vector3.zero, scaleDownDuration).SetEase(scaleDownEase).OnComplete( () => deadVisual.SetActive(false)));
        sequence.AppendCallback(() => revealParticle.Play());
        sequence.Append(liveVisual.transform.DOScale(Vector3.one, scaleUpDuration).SetEase(scaleUpEase));
        sequence.AppendCallback(() => this.enabled = false);
    }
}
