using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isAlive = false;
    public int numNeighbors = 0;
    public string prefab;
    public void SetAlive(bool isAlive)
    {
        this.isAlive = isAlive;
        if(isAlive)
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = false; 
        }
    }
    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
