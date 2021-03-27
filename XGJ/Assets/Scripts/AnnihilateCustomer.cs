using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnihilateCustomer : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.GetComponent<Customer>())
        {
            Destroy(other.gameObject);
        }
    }
}
