using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ViligerScrbt : MonoBehaviour
{
    [SerializeField] private GameObject ViligerParent;
    [SerializeField] private Collider2D Player;
    [SerializeField] private List<Transform> WalkToPoints;
    [SerializeField] private GameObject SellUI;
    [SerializeField] private Button CloseSellUI;
    private InputSystem_Actions InputSystem;

    private Dictionary<NavMeshAgent, bool> isWaiting = new Dictionary<NavMeshAgent, bool>();
    void Awake()
    {
        InputSystem = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        InputSystem.Enable(); // Input System aktivieren
    }

    private void OnDisable()
    {
        InputSystem.Disable(); // Input System deaktivieren
        InputSystem.Player.Disable();
    }
    void Start()
    {
        SellUI.SetActive(false);
        CloseSellUI.onClick.AddListener(() =>
        {
            for (int i = 0; i < ViligerParent.transform.childCount; i++)
            {
                NavMeshAgent villager = ViligerParent.transform.GetChild(i).GetComponent<NavMeshAgent>();
                villager.enabled = true;
            }
            SellUI.SetActive(false);
        });
    }

    void Update()
    {



        for (int i = 0; i < ViligerParent.transform.childCount; i++)
        {
            NavMeshAgent villager = ViligerParent.transform.GetChild(i).GetComponent<NavMeshAgent>();

            if (InputSystem.Player.Interact.WasPressedThisFrame() && ViligerParent.transform.GetChild(i).GetComponent<Collider2D>().IsTouching(Player))
            {
                SellUI.SetActive(true);
                villager.enabled = false;
            }


            if (villager == null) continue;

            if (villager.enabled && villager.isOnNavMesh)
            {
                Vector3 velocity = villager.velocity;

                Animator anim = ViligerParent.transform.GetChild(i).GetComponent<Animator>(); ;

                if (velocity.magnitude < 0.1f)
                {
                    // Villager steht still → Idle Animation
                    if (anim != null) anim.Play("Idle");
                }
                else
                {
                    // Bewegungsrichtung prüfen
                    if (velocity.z > 0.1f) // läuft nach oben
                    {
                        if (anim != null) anim.Play("MoveUp");
                    }
                    else if (Mathf.Abs(velocity.x) > 0.1f) // seitlich
                    {
                        if (anim != null) anim.Play("MoveX");
                    }
                    else
                    {
                        if (anim != null) anim.Play("Idle"); // steht still
                    }

                }
                bool arrived = !villager.pathPending && villager.remainingDistance <= villager.stoppingDistance;
                if (arrived && !IsWaiting(villager))
                {
                    StartCoroutine(WaitForPath(villager));
                }

            }

        }

    }

    private void SetNewDestination(NavMeshAgent agent)
    {
        Vector3 newDestination = WalkToPoints[Random.Range(0, WalkToPoints.Count)].position;
        agent.SetDestination(newDestination);
    }

    private bool IsWaiting(NavMeshAgent agent)
    {
        return isWaiting.ContainsKey(agent) && isWaiting[agent];
    }

    IEnumerator WaitForPath(NavMeshAgent navMeshAgent)
    {
        isWaiting[navMeshAgent] = true;

        navMeshAgent.ResetPath();

        yield return new WaitForSeconds(Random.Range(3, 10));

        SetNewDestination(navMeshAgent);
        isWaiting[navMeshAgent] = false;
    }
}
