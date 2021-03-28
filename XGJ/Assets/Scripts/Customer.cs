using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CustomerState
{
    Idle,
    MovingToMachine,
    Playing,
    MovingToExit,
    MovingToEntrance
}

public class Customer : MonoBehaviour
{

    private Player player;
    private Arcade arcade;
    private NavMeshAgent agent;
    private ArcadeMachine targetMachine;
    private CustomerState customerState;

    private Transform agentTarget;

    private float money;

    void Start()
    {
        Invoke("UpdateInfo", 1f);
    }

    private void UpdateInfo() //Vitun purkka paskaa kekw: :D-:,d:D;DD
    {
        customerState = CustomerState.Idle;
        money = Random.Range(0f, 10f) * 10f;
        arcade = GameObject.FindWithTag("Arcade").GetComponent<Arcade>();
        player = GameObject.FindWithTag("MainCamera").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();

        GoToEntrance();
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
                            case CustomerState.Idle:
                                if (agentTarget != null)
                                transform.rotation = agentTarget.rotation;
                                break;
                            case CustomerState.MovingToEntrance:
                                    GotoMachine();
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
        GotoMachine();
        //StartCoroutine(PlayGame(2f));
    }

    public void MoveInQueue(Vector3 point)
    {
        agent.destination = point;
    }

    public void MoveToMachine(ArcadeMachine machine)
    {
        targetMachine = machine;
        agent.isStopped = false;
        customerState = CustomerState.MovingToMachine;
        agentTarget = targetMachine.playingArea;
        agent.destination = agentTarget.position;
    }

    private void GotoMachine()
    {
        if(targetMachine != null)
        {
            //arcade.SetArcadeMachineAvailable(targetMachine);
            targetMachine = null;
        }

        targetMachine = arcade.GetRandomAvailableMachine();
        if (targetMachine != null)
        {
            if (money >= arcade.entryFee)
            {
                money -= arcade.entryFee;
                player.AddMoney(arcade.entryFee);
                agent.isStopped = false;
                if (!targetMachine.SetUser(this))
                {
                    GoToExit();
                }
                customerState = CustomerState.MovingToMachine;
                agentTarget = targetMachine.playingArea;
                agent.destination = agentTarget.position;
            }
            else
            {
                GoToExit();
            }
        }
        else
        {
            targetMachine = arcade.GetRandomOccupiedMachine();
            if (targetMachine != null)
            {
                if (!targetMachine.IsQueueFull())
                {
                    if (money >= arcade.entryFee)
                    {
                        money -= arcade.entryFee;
                        player.AddMoney(arcade.entryFee);
                        agent.isStopped = false;
                        if (!targetMachine.SetUser(this))
                        {
                            GoToExit();
                        }
                        customerState = CustomerState.Idle;
                        agentTarget = targetMachine.playingArea;
                        agent.destination = targetMachine.GetEndOfQueuePoint();
                    }
                    else
                    {
                        GoToExit();
                    }
                }
                else
                {
                    GoToExit();
                }
            }
            else
            {
                GoToExit();
            }
        }
    }

    private void GoToExit()
    {
        agent.isStopped = false;
        arcade.RemoveCustomer(this);
        //arcade.SetArcadeMachineAvailable(targetMachine);
        customerState = CustomerState.MovingToExit;
        agentTarget = GameObject.FindWithTag("Exit").transform;
        agent.destination = agentTarget.position;
    }

    private void GoToEntrance()
    {
        agent.isStopped = false;
        customerState = CustomerState.MovingToEntrance;
        agentTarget = GameObject.FindWithTag("Entrance").transform;
        agent.destination = agentTarget.position;
    }

    private IEnumerator PlayGame(float waitTime)
    {

        yield return new WaitForSeconds(waitTime);

        if (Random.Range(0f, 1f) >= 0.6f)
        {
            if (targetMachine != null)
            {
                targetMachine.DonePlaying();
                targetMachine = null;
            }
            GoToEntrance();
        }
        else
        {
            GoToExit();
            if (targetMachine != null)
            {
                targetMachine.DonePlaying();
                targetMachine = null;
            }
        }
    }
}
