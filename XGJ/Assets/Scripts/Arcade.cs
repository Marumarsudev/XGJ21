using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arcade : MonoBehaviour
{
    public List<ArcadeMachine> availableMachines;
    public List<ArcadeMachine> occupiedMachines;

    public List<GameObject> customer;
    public Transform customerSpawn;

    private float spawnTimer;
    private float spawnTime = 2f;

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (availableMachines.Count >= 1 && spawnTimer >= spawnTime)
        {
            spawnTimer = 0f;
            SpawnCustomer();
        }
    }

    private void SpawnCustomer()
    {
        Instantiate(customer[Random.Range(0, customer.Count)], customerSpawn.position, Quaternion.identity);
    }

    public ArcadeMachine GetRandomAvailableMachine()
    {
        ArcadeMachine machine = availableMachines[Random.Range(0, availableMachines.Count - 1)];

        availableMachines.Remove(machine);
        occupiedMachines.Add(machine);

        return machine;
    }

    public void SetArcadeMachineAvailable(ArcadeMachine machine)
    {
        if (occupiedMachines.Contains(machine))
        {
            occupiedMachines.Remove(machine);
            availableMachines.Add(machine);
            Debug.Log(machine.name + " added to available machines!");
        }
        else
        {
            Debug.LogError(machine.name + " not found!");
        }
    }

    public void AddArcadeMachine(ArcadeMachine machine)
    {
        availableMachines.Add(machine);
    }

    public void DeleteArcadeMachine(ArcadeMachine machine)
    {
        if (availableMachines.Contains(machine))
        {
            availableMachines.Remove(machine);
        }
        else if (occupiedMachines.Contains(machine))
        {
            occupiedMachines.Remove(machine);
        }
        else
        {
            Debug.LogError("Error trying to delete machine, machine not found!");
        }
    }

}
