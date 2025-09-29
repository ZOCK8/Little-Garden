using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines.Interpolators;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class NightDay : MonoBehaviour
{
    public int day;
    [SerializeField] private RuleTile DirtTile;
    [SerializeField] private GameObject NightUI;
    [SerializeField] private GameObject PlantsContainer;
    [SerializeField] private Tilemap GroundTileMap;
    [SerializeField] private Button EndDayButton;
    [SerializeField] private GameObject BuildingsContainer;
    [SerializeField][Range(0, 100)] private int CrowEvent;
    [SerializeField][Range(0, 100)] private int DesertEvent;
    [SerializeField][Range(0, 100)] private int ThiefEvent;
    [SerializeField] private TextMeshProUGUI DayText;
    [SerializeField] private TextMeshProUGUI MoneyEarnedText;
    [SerializeField] private TextMeshProUGUI PlantsHarvestedText;

    [SerializeField] private Light2D GlobalLight;
    [SerializeField] private List<Light2D> Lights;
    [Range(0, 1)] public float TimeLight;
    private bool DayIsOn;
    [SerializeField] public int Day;
    [SerializeField] public int MoneyEarned;
    [SerializeField] public int PlantsHarvested;
    [SerializeField] private GameObject SoundParent;
    [SerializeField] DialogText dialogText;

    private bool EndDay;
    private float t;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CrowEvent = 100;
        DesertEvent = 100;
        ThiefEvent = 100;

        Day = 0;

        DayIsOn = true;

        NightUI.SetActive(false);
        StartCoroutine(NightCooldown());
        EndDayButton.onClick.AddListener(() =>
        {
            EndDay = true;
        });
        StartCoroutine(TimeColodown());
        float t = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        GlobalLight.intensity = TimeLight;

        for (int i = 0; i < Lights.Count; i++)
        {
            Lights[i].intensity = TimeLight;
        }

    }
    IEnumerator NightCooldown()
    {
        while (true)
        {
            NightUI.SetActive(false);
            EndDay = false;


            yield return new WaitForSeconds(300); // Nachtdauer
            /////////////////////////
            /// Night time
            ///////////////////////// 
            DayIsOn = false;
            Day += 1;
            DayText.text = Day.ToString();
            MoneyEarnedText.text = MoneyEarned.ToString();
            PlantsHarvestedText.text = PlantsHarvested.ToString();

            NightUI.SetActive(true);
            int Highest;
            // Berechne Event-Wahrscheinlichkeiten basierend auf Vogelscheuchen
            // Berechne Event-Wahrscheinlichkeiten basierend auf Vogelscheuchen
            int scareCrows = 0;
            for (int i = 0; i < BuildingsContainer.transform.childCount; i++)
            {
                if (BuildingsContainer.transform.GetChild(i).name == "Scare Crow")
                    scareCrows++;
            }

            // Basiswerte
            int baseCrow = 50;      // Startchance für Crow
            int baseRain = 50;      // Startchance für Rain
            int baseThief = 30;     // Chance für Thief

            // Anpassung durch Vogelscheuchen
            int crowChance = Mathf.Max(0, baseCrow - scareCrows * 5);  // Crow wird unwahrscheinlicher
            int rainChance = baseRain + scareCrows * 10;               // Regen wird wahrscheinlicher
            int thiefChance = baseThief;                               // konstant

            // Gesamtchance
            int total = crowChance + rainChance + thiefChance;

            // Zufallsauswahl
            int roll = Random.Range(0, total);

            if (roll < crowChance)
            {
                UpdatePlants();
                Debug.Log("Starting Crow Event");
                dialogText.TextToDisplay = "Crows have destroyed some of your plants";
                dialogText.Text.color = Color.black;
                StartCoroutine(dialogText.ShowText());
                SoundParent.transform.GetChild(1).GetComponent<AudioSource>().Play();
                for (int i = 0; i < Random.Range(1, PlantsContainer.transform.childCount / 2); i++)
                {
                    Destroy(PlantsContainer.transform.GetChild(i).gameObject);
                }
            }
            else if (roll < crowChance + rainChance)
            {
                Debug.Log("Starting Rain Event");
                SoundParent.transform.GetChild(0).GetComponent<AudioSource>().Play();
                dialogText.TextToDisplay = "Your plants have been refreshed by the rain";
                dialogText.Text.color = Color.black;
                StartCoroutine(dialogText.ShowText());
                for (int i = 0; i < Random.Range(1, PlantsContainer.transform.childCount / 2); i++)
                {
                    for (int s = 0; s < PlantsContainer.transform.childCount; s++)
                    {
                        var plant = PlantsContainer.transform.GetChild(s).GetComponent<Plants>();
                        Vector3Int TilePos = GroundTileMap.WorldToCell(plant.transform.position - new Vector3(0.5f, 0.7f, 0));
                        GroundTileMap.SetColor(TilePos, Color.navajoWhite);
                        plant.GrowStatus += 1;
                        plant.WasWatered = true;
                    }
                }
        }
            else
        {
            Debug.Log("Nothing");
            UpdatePlants();
        }




        yield return new WaitUntil(() => EndDay); // wartet auf Button
        PlantsHarvested = 0;
        MoneyEarned = 0;
        TimeLight = 0;
        CrowEvent = 100;
        DesertEvent = 100;
        ThiefEvent = 100;
        DayIsOn = true;
        StartCoroutine(TimeColodown());
    }
}

private void UpdatePlants()
{
    for (int i = 0; i < PlantsContainer.transform.childCount; i++)
    {
        var plant = PlantsContainer.transform.GetChild(i).GetComponent<Plants>();
        Vector3Int TilePos = GroundTileMap.WorldToCell(plant.transform.position - new Vector3(0.5f, 0.7f, 0));
        day += 1;
        GroundTileMap.SetColor(TilePos, Color.white);
        plant.GrowStatus += plant.WasWatered ? 1 : -1;
        plant.WasWatered = false;
    }
}
IEnumerator TimeColodown()
{
    bool countingUp = true; // Hoch- oder Runterzählen
    float step = 1f / 300f; // Jede Sekunde

    while (DayIsOn)
    {
        yield return new WaitForSeconds(0.5f);

        if (countingUp)
        {
            TimeLight += step;

            if (TimeLight >= 1f)
            {
                TimeLight = 1f; // Überlauf vermeiden
                countingUp = false; // Wechsel auf Runterzählen
            }
        }
        else
        {
            TimeLight -= step;

            if (TimeLight <= 0f)
            {
                TimeLight = 0f; // Unterlauf vermeiden
                countingUp = true; // Wechsel auf Hochzählen
            }
        }
    }
}
}
