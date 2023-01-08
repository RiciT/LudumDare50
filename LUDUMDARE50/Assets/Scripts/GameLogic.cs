using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;
    public TMP_Text timeText;
    public TMP_Text enemyText;
    public TMP_Text bombText;
    public TMP_Text pointText;
    public GameObject roundText;

    public int remainingTime = 20;
    public int waitBetweenRound = 1;
    public int nORounds = 1;
    public bool startGame = false;
    public bool victory = false;


    public int lives = 1;
    public int currentLives = 1;
    public int points = 0;

    private int enemyCount = 63;
    private int numberOfEnemies;
    public int bombCount;
    public GameObject enemyPrefab;
    public GameObject enemyContainer;
    public GameObject player;
    public GameObject explosionPrefab;

    private GameObject[] enemies;
    private GameObject[] players;

    public CameraFollow cameraFollow;
    public CameraShake cameraShake;

    private void Start()
    {
        Instance = this;
        numberOfEnemies = enemyCount;
    }

    public void EndRound()
    {
        points += (nORounds - 1) * 15;
        pointText.text = "Points: " + points;
        nORounds++;

        int count = 0;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].GetComponent<EnemyController>().hasBomb == false)
            {
                count++;
            }
            else
            {
                GameObject explosion = Instantiate(explosionPrefab, enemies[i].transform.position, Quaternion.identity);
                Destroy(explosion, 1);

                Destroy(enemies[i]);
            }
        }
        victory = count == 0;
        if (PlayerController.Instance.hasBomb)
        {
            currentLives--;

            if (currentLives <= 0)
            {
                GameObject explosion = Instantiate(explosionPrefab, player.transform.position, Quaternion.identity);
                Destroy(explosion, 1);

                player.SetActive(false);
            }
        }

        numberOfEnemies = count;

        StartCoroutine(StartRound());
    }
    public void EndGame()
    {
        startGame = false;
        for (int i = 0; i < enemyContainer.transform.childCount; i++)
        {
            Destroy(enemyContainer.transform.GetChild(i).gameObject);
        }
        nORounds = 1;
        numberOfEnemies = enemyCount;

        currentLives = lives;
        MapGenerator.Instance.ClearMap();
        player.SetActive(true);
        victory = false;
    }

    public void AddPlusLife()
    {
        UpgradeManager.Instance.upgrades[0].isBought = true;
        DecreasePoints(UpgradeManager.Instance.upgrades[0].cost);
        lives++;
        currentLives = lives;
    }

    public IEnumerator StartRound()
    {

        yield return new WaitForSeconds(waitBetweenRound);

        cameraFollow.follow = true;
        cameraFollow.zoom = cameraFollow.defaultZoom;

        pointText.gameObject.transform.parent.gameObject.SetActive(true);
        bombText.gameObject.transform.parent.gameObject.SetActive(true);
        timeText.gameObject.transform.parent.gameObject.SetActive(true);
        enemyText.gameObject.transform.parent.gameObject.SetActive(true);

        yield return new WaitForSeconds(waitBetweenRound);

        if (PlayerController.Instance.hasBomb && currentLives <= 0)
        {
            MenuManager.Instance.OpenEndScreen(victory);
            EndGame();
        }
        else
        {
            GenerateEnemies();
            PlayerController.Instance.SpawnPlayer();
            timeText.text = "Remaining Time: " + (victory ? 5 : remainingTime);
            StartCoroutine(DisplayRoundNumber());

            yield return new WaitForSeconds(waitBetweenRound);

            startGame = true;
            StartCoroutine(StartCountdown());
        }
    }
    public IEnumerator StartCountdown()
    {
        int countDown = victory ? 5 : remainingTime;
        while (countDown > 0)
        {
            countDown -= 1;
            timeText.text = "Remaining Time: " + countDown;
            yield return new WaitForSeconds(1);
        }
        StartCoroutine(ZoomOut());
    }
    public IEnumerator ZoomOut()
    {
        startGame = false;
        cameraFollow.follow = false;
        cameraFollow.zoom = MapGenerator.height / 2;

        pointText.gameObject.transform.parent.gameObject.SetActive(false);
        bombText.gameObject.transform.parent.gameObject.SetActive(false);
        timeText.gameObject.transform.parent.gameObject.SetActive(false);
        enemyText.gameObject.transform.parent.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);

        StartCoroutine(cameraShake.Shake(0.5f, 0.5f));
        EndRound();
    }

    public IEnumerator DisplayRoundNumber()
    {
        roundText.GetComponent<TMP_Text>().text = "Round " + nORounds;
        roundText.SetActive(true);
        yield return new WaitForSeconds(waitBetweenRound);
        roundText.SetActive(false);
    }

    public int GetPoints()
    {
        return points;
    }

    public void DecreasePoints(int value)
    {
        points -= value;
    }

    public void GenerateEnemies()
    {
        for (int i = 0; i < enemyContainer.transform.childCount; i++)
        {
            Destroy(enemyContainer.transform.GetChild(i).gameObject);
        }

        List<Vector2> emptyCells = new List<Vector2>(MapGenerator.Instance.emptyCells);
        bombCount = (numberOfEnemies + 1) / 2 == 0 ? 1 : (numberOfEnemies + 1) / 2;
        player.GetComponent<PlayerController>().hasBomb = false;

        enemies = new GameObject[numberOfEnemies];
        players = new GameObject[numberOfEnemies + 1];

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int posIndex = Random.Range(0, emptyCells.Count);
            Vector2 spawnPos = new Vector2(emptyCells[posIndex].y, emptyCells[posIndex].x) - new Vector2(MapGenerator.width / 2, MapGenerator.height / 2);

            enemies[i] = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, enemyContainer.transform);
            emptyCells.RemoveAt(posIndex);
        }

        enemies.CopyTo(players, 0);
        players[numberOfEnemies] = player;

        enemyText.text = "Number of Players: " + (numberOfEnemies + 1);
        bombText.text = "Number of Bombs: " + bombCount;

        while (bombCount > 0)
        {
            int r = Random.Range(0, players.Length);
            EnemyController enemy = players[r].GetComponent<EnemyController>();
            PlayerController player = players[r].GetComponent<PlayerController>();

            if (enemy)
            {
                if (enemy.hasBomb == false)
                {
                    enemy.hasBomb = true;
                    bombCount--;
                }
            }
            else if (player)
            {
                if (player.hasBomb == false)
                {
                    player.hasBomb = true;
                    bombCount--;
                }
            }
        }
    }

    public GameObject[] GetEnemyArray()
    {
        return enemies;
    }

    public GameObject[] GetPlayerArray()
    {
        return players;
    }
}
