using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public int damageAmount = 5;

    int randomizedDamge = 5;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "Enemy")
        {
            Destroy(gameObject);
            if (collision.transform.tag == "Player")
            {
                randomizedDamge = Random.Range(randomizedDamge - 2, randomizedDamge + 2);
                randomizedDamge = Random.Range(0, 5) > 4 ? randomizedDamge * 2 : randomizedDamge;

                PlayerManager.PlayerHurt(randomizedDamge);
            }
        }
    }
}
