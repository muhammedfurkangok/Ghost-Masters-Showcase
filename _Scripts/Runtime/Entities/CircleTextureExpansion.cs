using FurtleGame.Singleton;
using UnityEngine;

public class CircleTextureExpansion : SingletonMonoBehaviour<CircleTextureExpansion>
{
    public Material circleMaterial;  
    private Vector2 originalTextureScale;  
    private bool isScaled = false; 

    void Start()
    {
        originalTextureScale = circleMaterial.GetTextureScale("_MainTex");
    }

    public void ExpandTexture(float generalScale, float expansionSpeed)
    {
        if (!isScaled)
        {
            Vector2 newTextureScale = originalTextureScale + Vector2.one * generalScale * expansionSpeed;
            
            circleMaterial.SetTextureScale("_MainTex", newTextureScale);
            isScaled = true; 
        }
    }

    public void ResetTexture()
    {
        if (isScaled)
        {
            circleMaterial.SetTextureScale("_MainTex", originalTextureScale);
            isScaled = false; 
        }
    }
}