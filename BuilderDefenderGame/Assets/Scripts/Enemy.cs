using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb2;
    private Transform targetTransform;
    private HealthSystem healthSystem;
    private float lookForTargetTimer;
    private float lookForTargetTimerMax = 0.2f;

    public static Enemy Create(Vector3 position)
    {
        Transform pfEnemy = Resources.Load<Transform>("pfEnemy");

        Transform enemyTransform = Instantiate(pfEnemy, position, Quaternion.identity);

        Enemy enemy = enemyTransform.GetComponent<Enemy>();

        return enemy;
    }

    private void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();

        if (BuildingManager.Instance.GetHQBuilding() != null)
        {
            targetTransform = BuildingManager.Instance.GetHQBuilding().transform;
        }

        healthSystem = GetComponent<HealthSystem>();

        healthSystem.OnDamaged += HealthSystem_OnDamaged;

        healthSystem.OnDied += HealthSystem_OnDied;

        lookForTargetTimer = Random.Range(0f, lookForTargetTimerMax);
    }

    private void HealthSystem_OnDamaged(object sender, System.EventArgs e)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyHit);

        CinemachineShake.Instance.ShakeCamera(1.5f, .1f);

        ChromaticAberrationEffect.Instance.SetWeigth(.5f);
    }

    private void HealthSystem_OnDied(object sender, System.EventArgs e)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyDie);

        CinemachineShake.Instance.ShakeCamera(7f, .15f);

        ChromaticAberrationEffect.Instance.SetWeigth(.5f);

        Instantiate(Resources.Load<Transform>("pfEnemyDieParticles"), transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void Update()
    {
        HandleMovement();

        HandleTrageting();
    }

    private void OnCollisionEnter2D(Collision2D otherCollision)
    {
        Building building = otherCollision.gameObject.GetComponent<Building>();

        if (building != null)
        {
            // Collided with a building!
            HealthSystem healthSystem = building.GetComponent<HealthSystem>();

            healthSystem.Damage(10);

            this.healthSystem.Damage(999);
        }
    }

    private void HandleMovement()
    {
        if (targetTransform != null)
        {
            Vector3 moveDir = (targetTransform.position - transform.position).normalized;

            float moveSpeed = 6f;
            rb2.velocity = moveDir * moveSpeed;
        }
        else
        {
            rb2.velocity = Vector2.zero;
        }
    }

    private void HandleTrageting()
    {
        lookForTargetTimer -= Time.deltaTime;

        if (lookForTargetTimer < 0f)
        {
            lookForTargetTimer += lookForTargetTimerMax;

            LookForTragets();
        }
    }

    private void LookForTragets()
    {
        float targetMaxRadius = 10f;

        Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(transform.position, targetMaxRadius);

        foreach (Collider2D collider2D in collider2DArray)
        {
            Building building = collider2D.GetComponent<Building>();

            if (building != null)
            {
                // Is a building!

                if (targetTransform == null)
                {
                    targetTransform = building.transform;
                }
                else
                {
                    if (Vector3.Distance(transform.position, building.transform.position) < 
                        Vector3.Distance(transform.position, targetTransform.position))
                    {
                        // Closer!
                        targetTransform = building.transform;
                    }
                }
            }
        }

        if (targetTransform == null)
        {
            // Found no targets within range!
            if (BuildingManager.Instance.GetHQBuilding() != null)
            {
                targetTransform = BuildingManager.Instance.GetHQBuilding().transform;
            }
        }
    }
}
