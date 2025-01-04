using UnityEngine;
using DG.Tweening; 

public class LineRendererAnimation : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform startPoint;
    public Transform endPoint;  
    public float drawDuration = 2f; 

    private void Start()
    {
        AnimateLine();
    }

    void AnimateLine()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint.position); 
        lineRenderer.SetPosition(1, startPoint.position);

        DOTween.To(() => 0f, UpdateLine, 1f, drawDuration).SetEase(Ease.Linear);
    }

    void UpdateLine(float value)
    {
        Vector3 currentPosition = Vector3.Lerp(startPoint.position, endPoint.position, value);

        lineRenderer.SetPosition(1, currentPosition);
    }
}