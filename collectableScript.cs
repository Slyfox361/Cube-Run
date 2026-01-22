using UnityEngine;
using System;

public class collectableScript : MonoBehaviour
{
    [SerializeField] private float rotatingSpeed;
    public int points;
    [SerializeField] private Quaternion baseRotation;
    public ParticleSystem miniExplosion;
    public static event Action<int> scoreUpdate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion spin = Quaternion.AngleAxis(Time.time * rotatingSpeed, Vector3.up);
        transform.rotation = spin * baseRotation;
    }

    void FixedUpdate()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        ParticleSystem newParticles = Instantiate(miniExplosion, transform.position, Quaternion.identity);
        scoreUpdate?.Invoke(points);
        Destroy(gameObject);
    }
}
