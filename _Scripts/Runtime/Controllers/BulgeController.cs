using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Obi;
using Sirenix.OdinInspector;
using UnityEngine;

public class BulgeController : MonoBehaviour
{
    public ObiRope rope;
    public GameObject gun;
    public float delay = 0f;
    public float defaultRadius = 1f;
    public float bulgeScale = 1.2f;
    public float bumpRadius = 1.2f;
    public AnimationCurve curve;
    void Start()
    {
        StopBulgeEffect();
    }

    [Button]
    public void CreateBulgeEffect(float rate)
    {
        gun.transform.DOPunchScale( new Vector3(0.25f, 0.25f, 0.25f), 0.5f, 10, 1f);
        InvokeRepeating("BulgeEffects", 0f, rate);
    }
    [Button]
    public void StopBulgeEffect()
    {
        CancelInvoke();
        StopAllCoroutines();
        for (int j = 0; j < rope.solverIndices.Length; j++) // 10 j = 0
        {
            rope.solver.principalRadii[j] = Vector3.one * defaultRadius;
        }
    }

    void BulgeEffects()
    {
        StartCoroutine(BulgeEffectSingle());
    }

    IEnumerator BulgeEffectSingle()
    {

        for (int i = (int)-bulgeScale; i < rope.solverIndices.Length + bulgeScale; i++)
        {
            for (int j = i; j < i + bulgeScale; j++) 
            {
                if (j < 0 || j >= rope.solverIndices.Length) continue;
                if (Mathf.Abs(i - j) < bulgeScale) 
                {
                    var pow = curve.Evaluate(Mathf.Abs(i - j) / bulgeScale);
                    var powR = 1f - pow;
                    var bumpRadii = defaultRadius + (bumpRadius * powR);

                    rope.solver.principalRadii[j] = Vector3.one * bumpRadii;
                }
            }
            for (int j = i; j > i - bulgeScale; j--) 
            {
                if (j < 0 || j >= rope.solverIndices.Length) continue;

                var pow = curve.Evaluate(Mathf.Abs(i - j) / bulgeScale);
                var powR = 1f - pow;
                var bumpRadii = defaultRadius + (bumpRadius * powR);

                rope.solver.principalRadii[j] = Vector3.one * bumpRadii;
            }

            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(0.001f);
    }

    private void OnDestroy()
    {
        CancelInvoke();
        StopAllCoroutines();
    }
}