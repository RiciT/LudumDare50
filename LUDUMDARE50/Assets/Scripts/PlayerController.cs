using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public Rigidbody2D rb;
    public GameObject cam;
    public float speed = 2;
    public float extraSpeed = 0;
    public Vector2 spawnPoint;

    public Animator animator;
    public GameObject bodyParts;
    public GameObject bombGO;

    public bool hasBomb;
    public bool canRecieveBomb = true;
    bool isOnCoolDown = false;
    
    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        if (GameLogic.Instance.startGame)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            rb.velocity = new Vector2(x, y).normalized * (speed + extraSpeed);

            if (!isOnCoolDown)
            {
                if (hasBomb)
                    canRecieveBomb = false;
                else
                    canRecieveBomb = true;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("Run", false);
        }
        UpdateAnimation(rb.velocity);
    }

    public void AddSpeed()
    {
        UpgradeManager.Instance.upgrades[1].isBought = true;
        GameLogic.Instance.DecreasePoints(UpgradeManager.Instance.upgrades[1].cost);
        extraSpeed += .4f;
    }

    public void ChangeToGoldenSkin()
    {
        UpgradeManager.Instance.upgrades[2].isBought = true;
        GameLogic.Instance.DecreasePoints(UpgradeManager.Instance.upgrades[2].cost);
        for (int i = 1; i < transform.GetChild(0).transform.childCount; i++)
        {
            transform.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1f, 223f / 255f, 0f);
        }
    }

    public void SpawnPlayer()
    {
        spawnPoint = MapGenerator.Instance.GetSpawnPoint();
        transform.position = spawnPoint;
        cam.transform.position = new Vector3(spawnPoint.x, spawnPoint.y, -10);
    }

    private void UpdateAnimation(Vector2 velocity)
    {
        bombGO.SetActive(hasBomb);
        if (velocity == Vector2.zero)
        {
            animator.SetBool("Run", false);
        }
        else
        {
            animator.SetBool("Run", true);
            if (velocity.y < 0)
            {
                animator.Play("PRunFront");
                bodyParts.transform.localPosition = new Vector3(0, 0, 0);
                bodyParts.transform.localScale = new Vector3(Mathf.Abs(bodyParts.transform.localScale.x), bodyParts.transform.localScale.y, bodyParts.transform.localScale.z);
            }
            else
            {
                if (Mathf.Abs(velocity.x) < Mathf.Abs(velocity.y))
                {
                    animator.Play("PRunBack");
                    bodyParts.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    animator.Play("PRunSide");
                    if (velocity.x < 0)
                    {
                        bodyParts.transform.localPosition = new Vector3(-0.03f, 0, 0);
                        if (bodyParts.transform.localScale.x < 0)
                        {
                            bodyParts.transform.localScale = new Vector3(-bodyParts.transform.localScale.x, bodyParts.transform.localScale.y, bodyParts.transform.localScale.z);
                        }
                    }
                    else
                    {
                        bodyParts.transform.localPosition = new Vector3(0.03f, 0, 0);
                        if (bodyParts.transform.localScale.x > 0)
                        {
                            bodyParts.transform.localScale = new Vector3(-bodyParts.transform.localScale.x, bodyParts.transform.localScale.y, bodyParts.transform.localScale.z);
                        }
                    }
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasBomb == true)
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy)
            {
                if (enemy.hasBomb == false && enemy.canRecieveBomb == true)
                {
                    enemy.hasBomb = true;
                    hasBomb = false;

                    isOnCoolDown = true;
                    StartCoroutine(BombCooldown());
                }
            }
        }
    }

    public IEnumerator BombCooldown()
    {
        yield return new WaitForSeconds(2.0f);
        isOnCoolDown = false;
    }
}
