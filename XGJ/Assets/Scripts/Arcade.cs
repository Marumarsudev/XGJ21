using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Arcade : MonoBehaviour
{
    public List<ArcadeMachine> availableMachines;
    public List<ArcadeMachine> occupiedMachines;

    private List<ArcadeMachine> allMachines;

    public List<GameObject> customer;
    public Transform customerSpawn;

    private List<Customer> customersInArcade;

    private float spawnTimer;
    private float spawnTime = 1f;

    public int maxCustomerCount;
    public float entryFee;

    public float rent = 500;
    public float electricity;

    public TextMeshProUGUI infoText;

    void Start()
    {
        allMachines = new List<ArcadeMachine>();
        customersInArcade = new List<Customer>();
        CalculateElectricityPrice();
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (customersInArcade.Count < maxCustomerCount && spawnTimer >= spawnTime && availableMachines.Count + occupiedMachines.Count > 0)
        {
            spawnTimer = 0f;
            SpawnCustomer();
        }
    }

    private void CalculateElectricityPrice()
    {
        electricity = 0;

        allMachines.ForEach(machine => {
            electricity += machine.electricityUsage;
        });

        infoText.text = "Rent: $" + rent.ToString() + "\nElectricity: $" + electricity.ToString();
    }

    private void SpawnCustomer()
    {
        GameObject go = Instantiate(customer[Random.Range(0, customer.Count)], customerSpawn.position, Quaternion.identity);
        customersInArcade.Add(go.GetComponent<Customer>());
    }

    public void RemoveCustomer(Customer customer)
    {
        if (customersInArcade.Contains(customer))
        {
            customersInArcade.Remove(customer);
        }
        else
        {
            Debug.LogWarning("No customer found in customers list! D:::");
        }
    }

    public ArcadeMachine GetRandomOccupiedMachine()
    {
        if (occupiedMachines.Count <= 0) return null;

        ArcadeMachine machine = occupiedMachines[Random.Range(0, occupiedMachines.Count)];

        return machine;
    }

    public ArcadeMachine GetRandomAvailableMachine()
    {
        if (availableMachines.Count <= 0) return null;

        ArcadeMachine machine = availableMachines[Random.Range(0, availableMachines.Count)];

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
            Debug.LogWarning(machine.name + " not found!");
        }
    }

    public void AddArcadeMachine(ArcadeMachine machine)
    {
        availableMachines.Add(machine);
        allMachines.Add(machine);

        CalculateElectricityPrice();
    }

    public void DeleteArcadeMachine(ArcadeMachine machine)
    {
        if (allMachines.Contains(machine))
        {
            allMachines.Remove(machine);
            CalculateElectricityPrice();
        }

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
            Debug.LogWarning("Error trying to delete machine, machine not found!");
        }
    }

}
