using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ChunkController : MonoBehaviour
{
    public int chunkNumber;

    public GameObject buildingPrefab;
    public GameObject[] propsPrefab;
    public GameObject enemyPrefab;

    EndAreaCollider endArea;
    
    int propsToSpawn = 5;
    int enemiesToSpawn = 2;

    GameObject building = null;

    // Start is called before the first frame update
    void Start()
    {
        SpawnBuilding();
        SpawnEnemies();
        endArea = GetComponentInChildren<EndAreaCollider>();

        propsToSpawn = 15 - Mathf.RoundToInt(chunkNumber * 0.2f);
        enemiesToSpawn = 1 + Mathf.RoundToInt(chunkNumber * 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnBuilding()
    {

       building = Instantiate(buildingPrefab, transform.position, Quaternion.identity, transform);

        for (int x = propsToSpawn; x > 0; x--)
        {
            bool repeat = true;
            while (repeat)
            {
                float xPos = Random.Range(0f, 30f);
                float zPos = Random.Range(0f, 30f);
                int randomProp = Random.Range(0, propsPrefab.Length);
                BoxCollider box = propsPrefab[randomProp].GetComponent<BoxCollider>();
                Vector3 randomLoc = building.transform.position + new Vector3(xPos, 0, zPos);
                Quaternion randomRot = Quaternion.Euler(new Vector3(0, Random.Range(0f, 180f), 0));

                if (!Physics.CheckBox((box.center) + randomLoc, box.size / 2, randomRot))
                {
                    Instantiate(propsPrefab[randomProp], randomLoc, randomRot, building.transform);
                    repeat = false;
                }
            }
        }
        building.GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
    }

    void SpawnEnemies()
    {
        for (int x = enemiesToSpawn; x > 0; x--)
        {
            bool repeat = true;
            while (repeat)
            {
                float xPos = Random.Range(0f, 30f);
                float zPos = Random.Range(0f, 30f);
                CapsuleCollider capsule = enemyPrefab.GetComponent<CapsuleCollider>();
                Vector3 randomLoc = building.transform.position + new Vector3(xPos, 0, zPos);
                Quaternion randomRot = Quaternion.Euler(new Vector3(0, Random.Range(0f, 180f), 0));

                if (!Physics.CheckBox(randomLoc + capsule.center, Vector3.one/2))
                {
                    Instantiate(enemyPrefab, randomLoc, randomRot, transform);
                    repeat = false;
                }
            }
        }
    }

    public void EndTriggered()
    {
        if (chunkNumber != 0)
        {
            FindObjectOfType<LandGenerator>().NewAreaHit();
        }
    }
}
