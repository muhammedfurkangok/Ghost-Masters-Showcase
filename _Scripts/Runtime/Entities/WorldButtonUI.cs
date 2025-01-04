using UnityEngine;

public class WorldButtonUI : MonoBehaviour
{
    public Transform refTransform;
    public RectTransform ButtonRef;
    public Vector3 offset = new Vector3(0, 50, 0);

    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(refTransform.position);
        ButtonRef.position = screenPos + offset;
    }
}