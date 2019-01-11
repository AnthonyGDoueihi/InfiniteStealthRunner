using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ChunkController : MonoBehaviour
{
    public int chunkNumber;

    public GameObject buildingPrefab;
    public GameObject[] props;
    EndAreaCollider endArea;
    
    int propsToSpawn = 5;

    // Start is called before the first frame update
    void Start()
    {
        SpawnProps();
        endArea = GetComponentInChildren<EndAreaCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnProps()
    {

        GameObject building = Instantiate(buildingPrefab, transform.position, Quaternion.identity, transform);

        for (int x = propsToSpawn; x > 0; x--)
        {

            bool repeat = true;
            while (repeat)
            {
                float xPos = Random.Range(0f, 30f);
                float zPos = Random.Range(0f, 30f);
                int randomProp = Random.Range(0, props.Length);
                BoxCollider box = props[randomProp].GetComponent<BoxCollider>();
                Vector3 randomLoc = building.transform.position + new Vector3(xPos, 0, zPos);
                Quaternion randomRot = Quaternion.Euler(new Vector3(0, Random.Range(0f, 180f), 0));

                if (!Physics.CheckBox((box.center) + randomLoc, box.size / 2, randomRot))
                {
                    Instantiate(props[randomProp], randomLoc, randomRot, building.transform);
                    repeat = false;
                }
            }
        }
        building.GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
    }

    public void EndTriggered()
    {
        if (chunkNumber != 0)
        {
            FindObjectOfType<LandGenerator>().NewAreaHit();
        }
    }
}
