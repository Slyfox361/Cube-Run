using UnityEngine;
using System;

public class enemyScript : MonoBehaviour
{
    public ParticleSystem explosionParticles;
    public static event Action<int> scoreUpdate;
    public int points;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "playerAttack")
        {
            //Debug.Log(gameObject.name + " HIT");
            scoreUpdate?.Invoke(points);
            die();
        }
    }

    void die()
    {
        ParticleSystem newParticles = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
