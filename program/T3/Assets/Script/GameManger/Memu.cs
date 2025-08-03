using System;
using UnityEngine;


public class Memu : MonoBehaviour
{
    public GameObject[] gameObjects;
    int RandomNum;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RandomNum = UnityEngine.Random.Range(0, 8);

        Instantiate(gameObjects[RandomNum], this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
