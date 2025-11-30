using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;
    [SerializeField] private AudioClip fireballSound;

    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // แยก Logic การเช็ค Input ออกมาให้อ่านง่ายขึ้น
        if (ShouldAttack())
        {
            PerformAttack();
        }

        cooldownTimer += Time.deltaTime;
    }

    private bool ShouldAttack()
    {
        return Input.GetMouseButton(0)
            && cooldownTimer > attackCooldown
            && playerMovement.CanAttack
            && Time.timeScale > 0;
    }

    private void PerformAttack()
    {
        if (SoundManager.instance != null)
            SoundManager.instance.PlaySound(fireballSound);

        anim.SetTrigger("attack");
        cooldownTimer = 0;

        // Object Pooling Logic
        int fireballIndex = FindFireballIndex();
        GameObject fireball = fireballs[fireballIndex];

        fireball.transform.position = firePoint.position;

        if (fireball.TryGetComponent(out PlayerProjectile projectile))
        {
            projectile.SetDirection(Mathf.Sign(transform.localScale.x));
        }
    }

    private int FindFireballIndex()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}