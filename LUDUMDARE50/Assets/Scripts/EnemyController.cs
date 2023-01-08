using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject bodyParts;

    private GameObject[] enemies;

    public bool hasBomb = false;
    public bool canRecieveBomb = true;
    private bool hasTarget = false;
    bool isOnCoolDown = false;

    public Transform target;
    private Vector3 targetPosition;
    private float speed;

    void Start()
    {
        enemies = GameLogic.Instance.GetPlayerArray();
        speed = PlayerController.Instance.speed;
        hasTarget = false;
    }

    void Update()
    {
        if (GameLogic.Instance.startGame)
        {
            if (!hasTarget)
            {
                hasTarget = true;
                StartCoroutine(FindTarget());
            }
            targetPosition = hasBomb == true ? target.position : 2 * transform.position - target.position;
            rb.velocity = (targetPosition - transform.position).normalized * speed;

            if (!isOnCoolDown)
            {
                if (hasBomb)
                    canRecieveBomb = false;
                else
                    canRecieveBomb = true;
            }

            UpdateAnimation(rb.velocity);

            SpriteRenderer chestSprite = bodyParts.transform.GetChild(1).GetComponent<SpriteRenderer>();
            if (hasBomb)
            {
                chestSprite.color = Color.red;
            }
            else
            {
                if (isOnCoolDown)
                {
                    chestSprite.color = new Color(1, 0.5f, 0, 1);
                }
                else
                {
                    chestSprite.color = Color.green;
                }
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("Run", false);
        }
    }

    private IEnumerator FindTarget()
    {
        target = GetClosestEnemy(enemies, transform);
        yield return new WaitForSeconds(Random.Range(2, 5));

        hasTarget = false;
    }

    private void UpdateAnimation(Vector2 velocity)
    {
        if (velocity == Vector2.zero)
        {
            animator.SetBool("Run", false);
        }
        else
        {
            animator.SetBool("Run", true);
            if (velocity.y < 0)
            {
                animator.Play("ERunFront");
                bodyParts.transform.localPosition = new Vector3(0, 0, 0);
                bodyParts.transform.localScale = new Vector3(Mathf.Abs(bodyParts.transform.localScale.x), bodyParts.transform.localScale.y, bodyParts.transform.localScale.z);
            }
            else
            {
                if (Mathf.Abs(velocity.x) < Mathf.Abs(velocity.y))
                {
                    animator.Play("ERunBack");
                    bodyParts.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    animator.Play("ERunSide");
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
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
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
            else if (player)
            {
                if (player.hasBomb == false && player.canRecieveBomb == true)
                {
                    player.hasBomb = true;
                    hasBomb = false;

                    isOnCoolDown = true;
                    StartCoroutine(BombCooldown());
                }
            }
        }
    }

    public IEnumerator BombCooldown()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        isOnCoolDown = false;
    }

    public Transform GetClosestEnemy(GameObject[] enemies, Transform myself)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject potentialTarget in enemies)
        {
            EnemyController enemy = potentialTarget.gameObject.GetComponent<EnemyController>();
            PlayerController player = potentialTarget.gameObject.GetComponent<PlayerController>();

            if (potentialTarget.transform == myself)
            {
                continue;
            }
            if (hasBomb)
            {
                if (target != null)
                {
                    if (potentialTarget.transform == target.transform)
                    {
                        continue;
                    }
                }
                if (enemy)
                {
                    if (enemy.canRecieveBomb == false)
                    {
                        continue;
                    }
                }
                if (player)
                {
                    if (player.canRecieveBomb == false)
                    {
                        continue;
                    }
                }
            }

            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }

        if (bestTarget == null)
        {
            return target;
        }
        return bestTarget;
    }
}
