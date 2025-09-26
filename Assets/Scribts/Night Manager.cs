using System.Collections;
using System.Data;
using UnityEngine;
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
    private bool EndDay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NightUI.SetActive(false);
        StartCoroutine(NightCooldown());
        EndDayButton.onClick.AddListener(() =>
        {
            EndDay = true;
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator NightCooldown()
    {
        while (true)
        {
            NightUI.SetActive(false);
            EndDay = false;

            yield return new WaitForSeconds(300); // Nachtdauer

            NightUI.SetActive(true);
            UpdatePlants();

            yield return new WaitUntil(() => EndDay); // wartet auf Button
        }
    }

    private void UpdatePlants()
    {
        for (int i = 0; i < PlantsContainer.transform.childCount; i++)
        {
            var plant = PlantsContainer.transform.GetChild(i).GetComponent<Plants>();
            Vector3Int TilePos = GroundTileMap.WorldToCell(plant.transform.position);
            day += 1;
            GroundTileMap.SetTile(TilePos, DirtTile);
            GroundTileMap.SetColor(TilePos , Color.white);

            plant.GrowStatus += plant.WasWatered ? 1 : -1;
        }
    }

}
