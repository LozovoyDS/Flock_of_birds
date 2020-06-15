using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBirds : MonoBehaviour
{
    public Transform birdPrefab;
    public int birdsCount;

    void Start()
    {
        for(int i = 0; i < birdsCount; i++)
        {
            Transform bird = Instantiate(birdPrefab, Random.insideUnitSphere * 25, Quaternion.identity);
            bird.parent = transform; // назначаем текущий объект родителем птицы для групироваки в иерархии
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
