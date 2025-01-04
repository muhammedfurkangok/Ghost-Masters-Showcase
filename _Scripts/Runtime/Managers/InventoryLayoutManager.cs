using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryLayoutManager : MonoBehaviour
{
    public RectTransform capacityTextRect;
    public GameObject itemPrefab;
    public float itemSpacing = 10f;
    public float itemWidth = 50f;
    public float itemHeight = 50f;

    private List<GameObject> instantiatedItems = new List<GameObject>();
    public List<ItemData> itemList = new List<ItemData>();

    public void AddItemToLayout(Sprite itemSprite, string itemName, int itemAmount)
    {
        GameObject newItem = Instantiate(itemPrefab, transform);
        newItem.GetComponent<Image>().sprite = itemSprite;

        RectTransform itemRect = newItem.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(itemWidth, itemHeight);

        instantiatedItems.Add(newItem);

        itemList.Add(new ItemData
        {
            itemName = itemName,
            itemSprite = itemSprite,
            itemAmount = itemAmount
        });

        UpdateItemPositions();
    }

    private void UpdateItemPositions()
    {
        float totalWidth = instantiatedItems.Count * itemWidth + (instantiatedItems.Count - 1) * itemSpacing;

        float startX = -totalWidth / 2f;

        for (int i = 0; i < instantiatedItems.Count; i++)
        {
            RectTransform itemRect = instantiatedItems[i].GetComponent<RectTransform>();

            float xPos = startX + i * (itemWidth + itemSpacing);
            itemRect.anchoredPosition = new Vector2(xPos, -itemHeight / 2f);
        }
    }

    public void ClearLayout()
    {
        foreach (var item in instantiatedItems)
        {
            Destroy(item);
        }

        instantiatedItems.Clear();
        itemList.Clear();
    }

    public struct ItemData
    {
        public string itemName;
        public Sprite itemSprite;
        public int itemAmount;
    }
}