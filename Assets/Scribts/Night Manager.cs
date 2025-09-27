using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    [SerializeField] private Light2D GlobalLight;
    [SerializeField] private List<Light2D> Lights;
    [Range(0, 1)] public float TimeLight;
    private bool DayIsOn;

    private bool EndDay;
    private float t;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CrowEvent = 100;
        DesertEvent = 100;
        ThiefEvent = 100;

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
        GlobalLight.intensity = TimeLight / 2;

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
            NightUI.SetActive(true);
            UpdatePlants();
            int Highest;
            for (int i = 0; i < BuildingsContainer.transform.childCount; i++)
            {
                var Obj = BuildingsContainer.transform.GetChild(i);
                if (Obj.name == "Scare Crow")
                {
                    CrowEvent -= 10;
                }
                if (Obj.name == "???")
                {
                    DesertEvent -= 10;
                }
                if (Obj.name == "???")
                {
                    ThiefEvent -= 10;
                }
            }
            Highest = Mathf.Max(CrowEvent, DesertEvent, ThiefEvent);

            if (Highest == CrowEvent)
            {
                Debug.Log("Starting Crow Event");

            }
            if (Highest == DesertEvent)
            {
                Debug.Log("Starting Desert Event");
            }
            if (Highest == ThiefEvent)
            {
                Debug.Log("Starting Thief Event");
            }

            yield return new WaitUntil(() => EndDay); // wartet auf Button
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
            Vector3Int TilePos = GroundTileMap.WorldToCell(plant.transform.position);
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
