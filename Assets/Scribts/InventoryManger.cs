using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManger : MonoBehaviour
{
    [SerializeField] public List<Items> ItemsInInv;

    public Items CurrentItem;
    private int CurrentItemInt;
    [SerializeField] private GameObject ItemParent;
    [SerializeField] private Transform PlayerHand;
    [SerializeField] private UnityEngine.UI.Image ItemShowcaseLeft;
    [SerializeField] private UnityEngine.UI.Image ItemShowcaseMiddle;
    [SerializeField] private UnityEngine.UI.Image ItemShowcaseRight;

    private GameObject CurrentItemObject;
    void Start()
    {
        CheckItem();
    }
    void Update()
    {
        if (CurrentItem == null)
        {
            CurrentItem = ItemsInInv[0];
            CheckItem();
        }

    }
    public void NextItem()
    {
        CurrentItemInt += 1;
        if (CurrentItemInt >= ItemsInInv.Count)
        {
            CurrentItemInt = 0;
        }
        CurrentItem = ItemsInInv[CurrentItemInt];
        UpdateShowcase();
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
        UpdateShowcase();
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
        UpdateShowcase();
    }

    void LateUpdate()
    {
        CurrentItemObject.transform.position = PlayerHand.position;
    }
    public void UpdateShowcase()
    {
        Debug.Log("ddsdasd");
        // left item
        int Leftint = CurrentItemInt - 1;
        if (Leftint < 0)
        {
            Leftint = ItemsInInv.Count - 1;
        }
        // right item
        int Rightint = CurrentItemInt + 1;
        if (Rightint >= ItemsInInv.Count)
        {
            Rightint = 0;
        }

        ItemShowcaseLeft.sprite = ItemsInInv[Leftint].ShowcaseImage;
        ItemShowcaseMiddle.sprite = ItemsInInv[CurrentItemInt].ShowcaseImage;
        ItemShowcaseRight.sprite = ItemsInInv[Rightint].ShowcaseImage;

    }
    public void ClearCurrentItem()
    {
        for (int i = 0; i < ItemsInInv.Count; i++)
        {
            if (ItemsInInv[i].name == CurrentItem.name)
            {
                ItemsInInv.Remove(ItemsInInv[i]);
                CurrentItem = ItemsInInv[i - 1];
                CheckItem();
            }
        }
    }
}
