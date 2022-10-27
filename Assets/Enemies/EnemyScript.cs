using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject EnemyGun;
    public Transform PlayerAimTarget;
    public float shootingVelocity;

    private void Start()
    {
        StartCoroutine(ShootProjectile());
    }
    private void Update()
    {
        if (PlayerAimTarget != null)
        {
            transform.LookAt(PlayerAimTarget);
        }
    }

    private IEnumerator ShootProjectile() //shoots a projectile every 4 seconds
    {
        Debug.Log("Euy");
        yield return new WaitForSeconds(4f);
        EnemyGun.GetComponent<EnemyGunScript>().Enemy_ShootProjectile();
        StartCoroutine(ShootProjectile());
    }
}
