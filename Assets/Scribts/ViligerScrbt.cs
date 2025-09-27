using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.AI;

public class ViligerScrbt : MonoBehaviour
{
    [SerializeField] private GameObject ViligerMenPrefab;
    [SerializeField] private GameObject ViligerFemPrefab;
    [SerializeField] private GameObject ViligerParent;
    [SerializeField] private int MaxViligers;
    [SerializeField] private List<Transform> WalkToPoints;
    void Start()
    {
        for (int i = 0; i < MaxViligers; i++)
        {
            int X = Random.Range(0, 2);
            switch (X)
            {
                case 0:
                    GameObject M = Instantiate(ViligerMenPrefab);
                    M.transform.position = WalkToPoints[Random.Range(0, WalkToPoints.Count)].position;
                    M.transform.SetParent(ViligerParent.transform);
                    break;
                case 1:
                    GameObject F = Instantiate(ViligerFemPrefab);
                    F.transform.position = WalkToPoints[Random.Range(0, WalkToPoints.Count)].position;
                    F.transform.SetParent(ViligerParent.transform);
                    break;
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < ViligerParent.transform.childCount; i++)
        {
            NavMeshAgent villager = ViligerParent.transform.GetChild(i).GetComponent<NavMeshAgent>();

            if (villager.enabled && villager.isOnNavMesh)
            {
                if (!villager.pathPending && villager.remainingDistance <= villager.stoppingDistance)
                {
                    Vector3 newDestination = WalkToPoints[Random.Range(0, WalkToPoints.Count)].position;
                    villager.SetDestination(newDestination);
                }
            }
        }
    }

}
