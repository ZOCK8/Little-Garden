using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plants : MonoBehaviour
{
    private GameObject CurrentPlant;
    public int GrowStatus = 0;
    [SerializeField] private int StartGrow;
    private Animator PlantStageAnimator;

    private SpriteRenderer PlantSprrite;
    private BoxCollider2D PlayerBox;
    private BoxCollider2D PlantBox;
    private InputSystem_Actions InputSystem;
    public bool IsDone;
    public bool WasWatered;
    public Items items;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IsDone = false;
        InputSystem = new InputSystem_Actions();
        PlantStageAnimator = this.gameObject.GetComponent<Animator>();
        CurrentPlant = this.gameObject;
        PlantSprrite = CurrentPlant.GetComponent<SpriteRenderer>();
        PlantBox = CurrentPlant.GetComponent<BoxCollider2D>();
        WasWatered = true;
    }

    void Update()
    {
        if (GrowStatus < 0)
        {
            Debug.Log("Plant " + CurrentPlant.name + " is to low destroying");
            Destroy(CurrentPlant);
        }

        if (GrowStatus <= PlantStageAnimator.parameterCount - 1)
        {
            for (int i = 0; i < PlantStageAnimator.parameterCount - 1; i++)
            {
                PlantStageAnimator.SetBool("Stage" + i, false);
            }

            PlantStageAnimator.SetBool("Stage" + GrowStatus, true);
        }
        else
        {
            for (int i = 0; i < PlantStageAnimator.parameterCount - 1; i++)
            {
                PlantStageAnimator.SetBool("Stage" + i, false);
            }
            int s = PlantStageAnimator.parameterCount - 1;
            PlantStageAnimator.SetBool("Stage" + s, true);
            IsDone = true;
        }
    }
}
