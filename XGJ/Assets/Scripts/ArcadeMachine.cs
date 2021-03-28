using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeMachine : MonoBehaviour
{
    private Arcade arcade;

    private Customer user;
    private List<Customer> queue;

    private int maxQueueLength;
    private bool isOccupied;

    void Start() 
    {
        maxQueueLength = 3;
        isOccupied = false;
        queue = new List<Customer>();
        //UpdateInfo();
    }

    public bool IsQueueFull()
    {
        return queue.Count >= maxQueueLength;
    }

    public bool SetUser(Customer customer)
    {
        if (!isOccupied)
        {
            isOccupied = true;
            user = customer;
            return true;
        }
        else if (queue.Count < maxQueueLength)
        {
            queue.Add(customer);
            return true;
        }

        return false;
    }

    public Vector3 GetEndOfQueuePoint()
    {
        if (queue.Count == 0) return playingArea.transform.position;

        Vector3 p = playingArea.transform.position;
        Vector3 d = (-playingArea.transform.forward * 1f) * queue.Count;

        return new Vector3(p.x + d.x, p.y + d.y, p.z + d.z);
    }

    public void DonePlaying()
    {
        Debug.Log("DONE");
        if (queue.Count <= 1)
        {
            Debug.Log("Machiin avava");
            arcade.SetArcadeMachineAvailable(this);
            isOccupied = false;
        }
        else
        {
            Customer c = queue[0];
            queue.Remove(c);
            c.MoveToMachine(this);
            user = c;

            int spot = 1;
            queue.ForEach(customer => {
                Vector3 p = playingArea.transform.position;
                Vector3 d = (-playingArea.transform.forward * 1f) * spot;
                customer.MoveInQueue(new Vector3(p.x + d.x, p.y + d.y, p.z + d.z));
                spot++;
            });
        }
    }

    public void ResetUser()
    {
        queue.ForEach(c => {
            c.StopAllCoroutines();
            c.ResetCustomer();
        });

        queue.Clear();

        if (user == null) return;

        user.StopAllCoroutines();
        user.ResetCustomer();
        user = null;
    }

    public void UpdateInfo() //Vitun purkka paskaa kekw: :D-:,d:D;DD
    {
        if (arcade == null)
        {
            arcade = GameObject.FindWithTag("Arcade").GetComponent<Arcade>();
        }
        arcade.AddArcadeMachine(this);
    }

    public Transform playingArea;

    public string machineName;

    public float electricityUsage;
    public float machineGameTime;
    public float machineDifficulty;
    public float machinePrice;
    public float machinePurchasePrice;
}
