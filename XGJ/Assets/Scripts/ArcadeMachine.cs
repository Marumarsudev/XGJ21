using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeMachine : MonoBehaviour
{
    private Arcade arcade;

    private Customer user;

    void Start() 
    {
        //UpdateInfo();
    }

    public void SetUser(Customer customer)
    {
        user = customer;
    }

    public void ResetUser()
    {
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

    public float machineGameTime;
    public float machineDifficulty;
    public float machinePrice;
}
