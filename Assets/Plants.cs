using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plants : MonoBehaviour
{
    private GameObject CurrentPlant;
    [SerializeField] public int GrowStatus;
    private int day;
    [SerializeField] private int StartGrow;
    private Animator PlantStageAnimator;

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
        PlantStageAnimator = this.gameObject.GetComponent<Animator>();
        CurrentPlant = this.gameObject;
        GrowStatus = 0;
        StartGrow = day;
        PlantSprrite = CurrentPlant.GetComponent<SpriteRenderer>();
        PlantBox = CurrentPlant.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        GrowStatus = day - StartGrow;

        if (GrowStatus <= PlantStageAnimator.parameterCount - 1)
        {
            for (int i = 0; i < PlantStageAnimator.parameterCount -1; i++)
            {
                PlantStageAnimator.SetBool("Stage" + i, false);
            }
            
            PlantStageAnimator.SetBool("Stage" + GrowStatus, true);
        }
        else
        {
            for (int i = 0; i < PlantStageAnimator.parameterCount -1; i++)
            {
                PlantStageAnimator.SetBool("Stage" + i, false);
            }
            int s = PlantStageAnimator.parameterCount - 1;
            PlantStageAnimator.SetBool("Stage" +  s, true);
            IsDone = true;
        }
    }
}
