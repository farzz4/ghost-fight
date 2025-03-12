using UnityEngine;
using UnityEngine.UI; // N'oubliez pas d'ajouter  ette ligne pour accéder aux éléments UI

public class Player : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private AudioSource attackAudioSource;

    public int health = 5;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    public float attackRange = 1f;
    public int attackDamage = 10;
    public LayerMask enemyLayers;
    public AudioClip attackSound;

    private bool isGrounded = true;
    private bool facingRight = true;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        attackAudioSource = GetComponent<AudioSource>();

        if (attackAudioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the Player!");
        }


    }

    void Update()
    {
        if (health <= 0)
        {
            Die();
        }

        // Mouvement horizontal
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        // Animation de course et d'idle
        animator.SetFloat("Run", Mathf.Abs(move));

        // Flip du personnage
        if (move > 0 && !facingRight)
        {
            Flip();
        }
        else if (move < 0 && facingRight)
        {
            Flip();
        }

        // Saut
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool("Jump", true);
            isGrounded = false;
        }

        // Attaque
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Jump", false);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Player took " + damage + " damage. Remaining health: " + health);

        animator.SetTrigger("Hurt");

        if (health <= 0)
        {
            Die();
        }

    }

    private void Die()
    {
        Debug.Log("Player died!");
        animator.SetTrigger("Death");
        this.enabled = false;
        Destroy(gameObject, 3f);
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");

        if (attackAudioSource != null && attackSound != null)
        {
            attackAudioSource.PlayOneShot(attackSound);
        }
        else
        {
            Debug.LogWarning("AudioSource or AttackSound is missing!");
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage); // Appelle la méthode TakeDamage sur l'ennemi
        }

    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
