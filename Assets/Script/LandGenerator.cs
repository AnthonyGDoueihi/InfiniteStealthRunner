using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandGenerator : MonoBehaviour
{
    public GameObject chunkConPrefab;
    public GameObject emptyChunk;
    public GameObject backWall;

    int chunkNumber = 0;

    List<GameObject> chunksLoaded = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        SpawnStartingChunk();
        SpawnNextChunk();
        SpawnNextChunk();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NewAreaHit()
    {
        DestroyLastChunk();
        SpawnNextChunk();
    }

    void DestroyLastChunk()
    {
        Destroy(chunksLoaded[0]);
        backWall.transform.position += new Vector3(0, 0, 30);
        chunksLoaded.RemoveAt(0);
    }

    void SpawnNextChunk()
    {
        Vector3 chunkPosition = new Vector3(0, 0, chunkNumber * 30);

        GameObject chunk = Instantiate(chunkConPrefab, chunkPosition, Quaternion.identity);
        chunk.GetComponent<ChunkController>().chunkNumber = chunkNumber;
        chunksLoaded.Add(chunk);
        chunkNumber++;
    }

    void SpawnStartingChunk()
    {
        Vector3 chunkPosition = new Vector3(0, 0, 0);

        GameObject chunk = Instantiate(emptyChunk, chunkPosition, Quaternion.identity);
        chunksLoaded.Add(chunk);
        chunkNumber++;
    }    


}
