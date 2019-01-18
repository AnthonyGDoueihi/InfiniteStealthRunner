using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    //To create and visualise all scores kept by the player

    public Image lifeImage;
    public Text areaNumber;
    public GameObject gameOverFolder;
    PlayerController player;
    int lifeCount;

    Image[] livesOnScreen;
    LevelManager levelMan;

    // Start is called before the first frame update
    void Start()
    {
        gameOverFolder.SetActive(false);
        player = FindObjectOfType<PlayerController>();
        lifeCount = player.life;

        livesOnScreen = new Image[lifeCount];

        for (int x = 0; x < lifeCount; x++)
        {
            Vector3 position = new Vector3(-856 + (x * 125), 461, 0);
            livesOnScreen[x] = Instantiate(lifeImage, transform);
            livesOnScreen[x].transform.localPosition = position;
        }

        levelMan = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        areaNumber.text = "Area: " + player.areasPassed;
    }

    public void HitByBullet()
    {
        lifeCount--;
        Destroy(livesOnScreen[lifeCount]);
    }

    public void GameOver()
    {
        gameOverFolder.SetActive(true);
    }
}
