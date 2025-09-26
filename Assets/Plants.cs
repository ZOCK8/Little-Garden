using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plants : MonoBehaviour
{
    private GameObject CurrentPlant;
    [SerializeField] private int GrowStatus;
    private int day;
    [SerializeField] private int StartGrow;
    [SerializeField] private List<Sprite> PlantStageImages;

     private SpriteRenderer PlantSprrite;
    private BoxCollider2D PlayerBox;
    private BoxCollider2D PlantBox;
    private InputSystem_Actions InputSystem;
    public bool IsDone;
    public Items items;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IsDone = false;
        PlayerData data = SaveSystem.Load();
        if (data != null)
        {
            day = data.Day;
        }

        InputSystem = new InputSystem_Actions();
        CurrentPlant = this.gameObject;
        GrowStatus = 0;
        StartGrow = day;
        PlantSprrite = CurrentPlant.GetComponent<SpriteRenderer>();
        PlantBox = CurrentPlant.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        int GrowStatus = day - StartGrow;

        if (GrowStatus < PlantStageImages.Count - 1)
        {
            PlantSprrite.sprite = PlantStageImages[GrowStatus];
        }
        else
        {
            GrowStatus = PlantStageImages.Count - 1;
            PlantSprrite.sprite = PlantStageImages[PlantStageImages.Count - 1];
            IsDone = true;
        }
    }
}
