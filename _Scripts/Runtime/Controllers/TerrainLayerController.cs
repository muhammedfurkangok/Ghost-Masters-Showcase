using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Runtime.Controllers
{
    public class TerrainLayerController : MonoBehaviour
    {
        public Terrain terrain;
        public TerrainLayer grassLayer;
        public TerrainLayer barrenLayer;
        public float brushSize = 5f;
        public float sphereRadius = 10f;

        [Button]
        private void PaintTerrain()
        {
            Vector3 spherePosition = transform.position;
            TerrainData terrainData = terrain.terrainData;

            int xStart = Mathf.FloorToInt((spherePosition.x / terrainData.size.x) * terrainData.alphamapWidth);
            int zStart = Mathf.FloorToInt((spherePosition.z / terrainData.size.z) * terrainData.alphamapHeight);

            int brushWidth = Mathf.FloorToInt(brushSize * terrainData.alphamapWidth / terrainData.size.x);
            int brushHeight = Mathf.FloorToInt(brushSize * terrainData.alphamapHeight / terrainData.size.z);

            xStart = Mathf.Clamp(xStart, 0, terrainData.alphamapWidth - 1);
            zStart = Mathf.Clamp(zStart, 0, terrainData.alphamapHeight - 1);
            brushWidth = Mathf.Clamp(brushWidth, 1, terrainData.alphamapWidth - xStart);
            brushHeight = Mathf.Clamp(brushHeight, 1, terrainData.alphamapHeight - zStart);

            float[,,] alphaMap = terrainData.GetAlphamaps(xStart, zStart, brushWidth, brushHeight);
            for (int x = 0; x < brushWidth; x++)
            {
                for (int z = 0; z < brushHeight; z++)
                {
                    float worldX = xStart + (x / (float)terrainData.alphamapWidth) * terrainData.size.x;
                    float worldZ = zStart + (z / (float)terrainData.alphamapHeight) * terrainData.size.z;
                    Vector3 worldPos = new Vector3(worldX, 0, worldZ);

                    float distance = Vector3.Distance(new Vector3(spherePosition.x, 0, spherePosition.z), worldPos);

                    if (distance <= sphereRadius)
                    {
                        alphaMap[x, z, 0] = 1f; 
                        alphaMap[x, z, 1] = 0f; 
                    }
                    else
                    {
                        alphaMap[x, z, 0] = 0f;
                        alphaMap[x, z, 1] = 1f; 
                    }
                }
            }

            // Alpha map'i Terrain'e geri yaz
            terrainData.SetAlphamaps(xStart, zStart, alphaMap);

            Debug.Log("Terrain üzerinde katman boyama işlemi yapıldı.");
        }
    }
}
