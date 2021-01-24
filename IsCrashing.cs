using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsCrashing : MonoBehaviour
{
    //脚本绑在碎片预制体上
    Rigidbody[] rigs;
    Transform player;
    IEnumerator Start()
    {
        player = transform.Find("/Player");
        rigs = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rigs.Length; i++)
        {
            rigs[i].AddExplosionForce(200, transform.position + Vector3.Normalize(player.position - transform.position) - Vector3.up * 0.3f, 5);
        }
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }
}
