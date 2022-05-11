using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage = 1;
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        if(collision.transform.tag == "Enemy")
        {
            collision.transform.GetComponent<EnemyHealth>().Attacked(damage);
        }
    }
}
