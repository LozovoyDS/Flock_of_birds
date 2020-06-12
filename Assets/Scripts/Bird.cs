using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [HideInInspector] public Transform thisTransform;
    [HideInInspector] public Transform modelBirds;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public float timeUpdateCalculate = 2;

    public LayerMask birdsLayer;
    public float maxSpeed = 15;

    private Vector3 cohesion, separation, alignment;
    private Vector3 vector;
    private float cohesionRadius = 10;
    private float separationDistance = 5;
    private float cohesionCoefficient = 1;
    private float aligmentCoefficient = 4;
    private float separationCoefficient = 10;
    private float vectorLength;
    private int separationCount, i;
    private int maxBirds = 5;
    private Collider thisCollider;
    private Collider[] birds;
    private Bird birdScript;

    private void Awake()
    {
        thisTransform = transform;
        modelBirds = GetComponentInChildren<Transform>();
        velocity = Random.onUnitSphere * maxSpeed;
    }

    private void Start()
    {
        thisCollider = GetComponent<Collider>();
        InvokeRepeating("Сalculate", Random.value * timeUpdateCalculate, timeUpdateCalculate);
        InvokeRepeating("BirdRotation", Random.value, 0.1f);
    }

    private void Update()
    {
        if(transform.position.sqrMagnitude > 25 * 25)
        {
            velocity += -transform.position / 25;
        }

        transform.position += velocity * Time.deltaTime;

        Debug.DrawRay(transform.position, cohesion, Color.red);
        Debug.DrawRay(transform.position, separation, Color.blue);
        Debug.DrawRay(transform.position, alignment, Color.green);
    }

    private void Сalculate()
    {
        cohesion = Vector3.zero;
        velocity = Vector3.zero;
        separation = Vector3.zero;
        alignment = Vector3.zero;
        separationCount = 0;

        birds = Physics.OverlapSphere(thisTransform.position, cohesionRadius, birdsLayer.value); //ищем ближайших птиц
        if (birds.Length < 2) return; //выходим если по близости мало соседей

        //for (i = 0; i < birds.Length; i++)
        for(i = 0; i < birds.Length && i < maxBirds; i++)
        {
            birdScript = birds[i].GetComponent<Bird>();
            vector = thisTransform.position - birdScript.thisTransform.position; // вектор от соседней птицы к нашей
            vectorLength = vector.sqrMagnitude; // длина вектора

            cohesion += birdScript.thisTransform.position; 
            alignment += birdScript.velocity;

            if (vectorLength > 0 && vectorLength < separationDistance * separationDistance)
            {
                separation += vector / vectorLength; //суммма еденичныйх векторов - орт
                separationCount++;
            }
        }

        if (birds.Length > maxBirds) cohesion /= maxBirds; else cohesion /= birds.Length;

        cohesion -= transform.position; // точка центра между текущей и соседними птицами
        cohesion = Vector3.ClampMagnitude(cohesion, maxSpeed);

        if (separationCount > 0) separation /= separationCount; //вектор для направления птиц друг от друга
        
        alignment /= birds.Length; // общее направление движения
        alignment = Vector3.ClampMagnitude(alignment, maxSpeed);


        velocity += cohesion + separation * 10 + alignment * 1.5f; // вектор направления движения птицы
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
    }

    void BirdRotation()
    {
        if(velocity != Vector3.zero && modelBirds.forward != velocity.normalized)
        {
            modelBirds.forward = Vector3.RotateTowards(modelBirds.forward, velocity, 10, 1);
        }
    }
}
