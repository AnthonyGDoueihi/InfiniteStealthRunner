using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAreaCollider : MonoBehaviour
{
    bool isTriggered;

    Material mat;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.color = new Color(1, 0, 0, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (!isTriggered)
            {
                mat.color = new Color(0, 1, 0, 0.3f);
                if (GetComponentInParent<ChunkController>())
                {
                    GetComponentInParent<ChunkController>().EndTriggered();
                }
                isTriggered = true;
                other.GetComponent<PlayerController>().areasPassed ++;
            }
        }
    }
}
