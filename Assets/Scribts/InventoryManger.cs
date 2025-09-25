using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class InventoryManger : MonoBehaviour
{
    [SerializeField] public List<Items> ItemsInInv;

    public Items CurrentItem;
    private int CurrentItemInt;
    [SerializeField] private GameObject ItemParent;
    [SerializeField] private Transform PlayerHand;
    private GameObject CurrentItemObject;
    void Start()
    {
        CheckItem();
    }
    public void NextItem()
    {
        CurrentItemInt += 1;
        if (CurrentItemInt >= ItemsInInv.Count)
        {
            CurrentItemInt = 0;
        }
        CurrentItem = ItemsInInv[CurrentItemInt];
        CheckItem();
    }
    public void LastItem()
    {
        CurrentItemInt -= 1;
        if (CurrentItemInt < 0)
        {
            CurrentItemInt = ItemsInInv.Count - 1;
        }
        CurrentItem = ItemsInInv[CurrentItemInt];
        CheckItem();
    }

    void CheckItem()
    {
        for (int i = 0; i < ItemParent.transform.childCount; i++)
        {
            Destroy(ItemParent.transform.GetChild(0).gameObject);
        }
        GameObject Item = Instantiate(CurrentItem.ObjectInHand);
        Item.transform.parent = ItemParent.transform;
        Item.name = CurrentItem.ItemName;
        Item.transform.position = PlayerHand.position;
        CurrentItemObject = Item;
    }

    void LateUpdate()
    {
      CurrentItemObject.transform.position = PlayerHand.position;
    }
}
