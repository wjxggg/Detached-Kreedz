using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaveing : MonoBehaviour
{
    private float maxTime = 0.5f;
    private float timer = 0.5f;
    private bool inCD = false;
    public GameObject cleavingCollider;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && !inCD)
        {
            inCD = true;
            cleavingCollider.SetActive(true);
        }
        else if (inCD)
        {
            timer -= Time.deltaTime;
            if (timer < 0.2f && timer > 0)
            {
                cleavingCollider.SetActive(false);
            }
            else if(timer < 0)
            {
                inCD = false;
                timer = maxTime;
            }
        }
    }
}
