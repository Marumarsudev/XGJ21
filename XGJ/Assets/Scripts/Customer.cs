using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CustomerState
{
    Idle,
    MovingToMachine,
    Playing,
    MovingToExit
}

public class Customer : MonoBehaviour
{

    private Player player;
    private Arcade arcade;
    private NavMeshAgent agent;
    private ArcadeMachine targetMachine;
    private CustomerState customerState;

    private Transform agentTarget;
    void Start()
    {
        Invoke("UpdateInfo", 1f);
    }

    private void UpdateInfo() //Vitun purkka paskaa kekw: :D-:,d:D;DD
    {
        customerState = CustomerState.Idle;
        arcade = GameObject.FindWithTag("Arcade").GetComponent<Arcade>();
        player = GameObject.FindWithTag("MainCamera").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
        GotoMachine();
    }

    void Update()
    {
        // Check if got to location.
        if (agent != null)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        switch(customerState)
                        {
                            case CustomerState.MovingToMachine:
                                player.AddMoney(targetMachine.machinePrice);
                                customerState = CustomerState.Playing;
                                transform.rotation = agentTarget.rotation;
                                StartCoroutine(PlayGame(targetMachine.machineGameTime));
                                break;
                            case CustomerState.MovingToExit:
                                Destroy(this.gameObject); // haha commit sudoku
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }

    public void ResetCustomer()
    {
        agent.isStopped = true;
        customerState = CustomerState.Idle;
        StartCoroutine(PlayGame(2f));
    }

    private void GotoMachine()
    {
        if(targetMachine != null)
        {
            arcade.SetArcadeMachineAvailable(targetMachine);
            targetMachine = null;
        }

        targetMachine = arcade.GetRandomAvailableMachine();
        if (targetMachine != null)
        {
            agent.isStopped = false;
            targetMachine.SetUser(this);
            customerState = CustomerState.MovingToMachine;
            agentTarget = targetMachine.playingArea;
            agent.destination = agentTarget.position;
        }
        else
        {
            customerState = CustomerState.Idle;
            StartCoroutine(PlayGame(2f));
        }
    }

    private void GoToExit()
    {
        agent.isStopped = false;
        arcade.SetArcadeMachineAvailable(targetMachine);
        targetMachine = null;
        customerState = CustomerState.MovingToExit;
        agentTarget = GameObject.FindWithTag("Exit").transform;
        agent.destination = agentTarget.position;
    }

    private IEnumerator PlayGame(float waitTime)
    {

        yield return new WaitForSeconds(waitTime);

        if (Random.Range(0f, 1f) >= 0.6f)
        {
            GotoMachine();
        }
        else
        {
            GoToExit();
        }
    }
}
