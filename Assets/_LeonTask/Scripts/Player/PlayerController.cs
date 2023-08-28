using System;
using UnityEngine;

public class PlayerController : MonoBehaviour,ISensibleCharacter
{
    #region SERIALIZED FIELDS

    [SerializeField] private float enemyScanDistance = 3f;
    [SerializeField] private Transform raycastTransform;
    [SerializeField] private LayerMask enemyLM;
    [SerializeField] private float _damageToEnemyAmount;
    

    #endregion

    #region PRIVATE FIELDS

    private SpriteRenderer characterSR;
    private MoveDirection movementDir=MoveDirection.RIGHT;
    private Animator playerAnim;
    private HealthComponent playerHealth;

    #endregion

    public static Action OnPlayerDead;
    
    #region MONOBEHAVIOUR CALLBACKS

    private void Awake()
    {
        playerHealth = GetComponent<HealthComponent>();
        playerAnim = GetComponent<Animator>();
    }

    private void Start()
    {
        characterSR = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        playerHealth.OnHealthZero += On_PlayerDead;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthZero -= On_PlayerDead;
    }

    // Update is called once per frame
    private void Update()
    {
        if (characterSR.flipX)
            movementDir = MoveDirection.LEFT;
        else
            movementDir = MoveDirection.RIGHT;
    }

    #endregion
    
    #region PUBLIC METHODS
    public void OnPlayerAttack()
    {
        ScanTarget();
    }
    

    public void ScanTarget()
    {
        var hitInfo=Physics2D.Raycast(raycastTransform.position, transform.InverseTransformDirection(transform.right)*(int)movementDir, enemyScanDistance,enemyLM);
        // Debug.DrawRay(raycastTransform.position, (int)movementDir*transform.InverseTransformDirection(transform.right)*enemyScanDistance,Color.magenta,0.5f);
        if (hitInfo.collider!=null &&hitInfo.collider.tag.Equals(Tags.ENEMY))
        {
            hitInfo.collider.GetComponent<Animator>().SetTrigger(EnemyAnimatorParams.ON_PLAYER_ATTACK);
            hitInfo.collider.GetComponent<IDamageable>().TakeDamage(_damageToEnemyAmount);
        }
    }

    public void On_PlayerDead()
    {
        // Debug.Log("Player Is Dead Now");
        playerAnim.SetTrigger(PlayerAnimatorParams.DEATH);
        GetComponent<Collider2D>().enabled = false;
        GameManager.OnGameOver?.Invoke();
        Destroy(gameObject,1f);
    }

    #endregion
}