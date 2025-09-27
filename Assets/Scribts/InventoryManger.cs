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
    public int Coins;

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
        ItemParent.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = CurrentItem.ShowcaseImage;
        UpdateShowcase();
    }

    void LateUpdate()
    {
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
        bool FoundItem = false;
        for (int i = 0; i < ItemsInInv.Count; i++)
        {
            if (ItemsInInv[i].name == CurrentItem.name && !FoundItem)
            {
                FoundItem = true;
                ItemsInInv.Remove(ItemsInInv[i]);
            }
        }
        LastItem();
    }
}
