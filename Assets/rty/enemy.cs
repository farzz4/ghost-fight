using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    private AudioSource attackAudioSource;

    public AudioClip attackSound;
    public Transform checkPoint;
    public float distance = 1f;
    public LayerMask layerMask;
    public bool facingLeft = true;

    public GameObject Player;
    public float attackRange = 2f;
    public int damage = 1;
    private bool isPlayerInRange = false;

    private Animator animator;
    private bool isDead = false;
    private bool isAttacking = false;

    public int health = 3;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        CheckPlayerDistance();

        if (isPlayerInRange)
        {
            Attack();
        }
        else
        {
            Move();
        }
    }

    private void Move()
    {
        if (Player == null) return;

        float directionToPlayer = Player.transform.position.x - transform.position.x;

        if (directionToPlayer < 0 && !facingLeft)
        {
            FlipDirection(true);
        }
        else if (directionToPlayer > 0 && facingLeft)
        {
            FlipDirection(false);
        }

       
        if (isPlayerInRange)
        {
            if (facingLeft)
            {
                transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);
            }
            else
            {
                 transform.Translate(Vector2.right * Time.deltaTime * moveSpeed);
            }
        }
        else
        {
            animator.SetBool("isrunning", false);
        }

        RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, distance, layerMask);

        if (!hit && facingLeft)
        {
            FlipDirection(false);
        }
        else if (!hit && !facingLeft)
        {
            FlipDirection(true);
        }
    }

    private void FlipDirection(bool toLeft)
    {
        facingLeft = toLeft;
        transform.eulerAngles = toLeft ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
    }

    private void CheckPlayerDistance()
    {
        if (Player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            isPlayerInRange = true;
        }
        else
        {
            isPlayerInRange = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;  // L'ennemi ne peut pas prendre de dégâts s'il est déjà mort

        health -= damage;
        Debug.Log("Enemy took " + damage + " damage. Remaining health: " + health);

        // Animation de blessure
        animator.SetTrigger("TakeDamage");

        if (health <= 0)
        {
            Die();
        }
    }
    private void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        animator.SetBool("isAttacking", true);

        if (Player != null)
        {
            Player playerScript = Player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
            }
        }

        Invoke(nameof(StopAttack), 1f);
        if (attackAudioSource != null && attackSound != null)
        {
            attackAudioSource.PlayOneShot(attackSound);
        }
        else
        {
            Debug.LogWarning("AudioSource or AttackSound is missing!");
        }
    }

    private void StopAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Enemy died!");

        animator.SetTrigger("Death");
        this.enabled = false;
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (checkPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(checkPoint.position, Vector2.down * distance);
        }
    }
}
