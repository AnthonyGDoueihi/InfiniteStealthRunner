using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ChunkController : MonoBehaviour
{
    public int chunkNumber; //To Control Spawn Location

    // All objects to Spawn Declaration
    public GameObject buildingPrefab;
    public GameObject[] propsPrefab;
    public GameObject enemyPrefab;
    public GameObject centerWallsPrefab;


    //Number of objects to Instantiate     
    int propsToSpawn;
    int enemiesToSpawn;
    float centreWallPercentageChance = 20f;

    GameObject centerWalls = null;
    GameObject building = null;

    // Start is called before the first frame update
    void Start()
    {
        enemiesToSpawn = 1 + Mathf.RoundToInt(0.05f * chunkNumber);
        propsToSpawn = 10 - Mathf.RoundToInt(chunkNumber * 0.2f * Mathf.PerlinNoise(chunkNumber, enemiesToSpawn));


        SpawnBuilding();
        SpawnEnemies();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnBuilding()
    {

        building = Instantiate(buildingPrefab, transform.position, Quaternion.identity, transform);
        centerWalls = Instantiate(centerWallsPrefab, transform.position, Quaternion.identity, transform);

        //Spawn Walls in the Centre
        foreach (Transform c in centerWalls.transform)
        {
            if(Random.Range(0, 100) > centreWallPercentageChance)
            {
                Destroy(c.gameObject);
            }
        }

        //Spawn Props
        for (int x = propsToSpawn; x > 0; x--)
        {
            bool repeat = true;  //Continue looping until it will not spawn ontop of another object

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

        //To make the NavMesh Agents see the objects created
        building.GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
    }

    void SpawnEnemies()
    {
        for (int x = enemiesToSpawn; x > 0; x--)
        {
            bool repeat = true; //Continue looping until it will not spawn ontop of another object
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
            FindObjectOfType<LandGenerator>().NewAreaHit(); //Passes detection to game controller
        }
    }
}
