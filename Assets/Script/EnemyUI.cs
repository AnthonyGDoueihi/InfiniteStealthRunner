using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{

    public Sprite heard;
    public Sprite seen;
    public Sprite neither;

    EnemyAI enemy;
    Image image;

    //All to visualise awareness

    void Start()
    {
        enemy = GetComponentInParent<EnemyAI>();
        image = GetComponentInChildren<Image>();
    }

    void Update()
    {
        if (enemy.seePlayer)
        {
            image.sprite = seen;
        }
        else if (enemy.hearPlayer)
        {
            image.sprite = heard;
        }
        else
        {
            image.sprite = neither;
        }

        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
