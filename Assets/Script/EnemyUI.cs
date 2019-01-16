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

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponentInParent<EnemyAI>();
        image = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
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
