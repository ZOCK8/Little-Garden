using UnityEngine;
using UnityEngine.UI;

public enum ItemTypeEnum
{
    Tools,
    Plant,
    Buildings
}

[CreateAssetMenu(fileName = "Items", menuName = "Scriptable Objects/Items")]
public class Items : ScriptableObject
{
    public string ItemName;
    public ItemTypeEnum ItemType;
    public GameObject ObjectInHand;
    public int SellPrice;
    public int BuyPrice;
    public Sprite ShowcaseImage;
    public Sprite GrownItem;
    public bool IsDoneGrowing = false;
    
}
