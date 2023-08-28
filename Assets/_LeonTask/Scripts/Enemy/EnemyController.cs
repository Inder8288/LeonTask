using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour,ISensibleCharacter
{
    #region SERIALIZED FIELDS

    [Header("Controller Options")] 
    [SerializeField] private float walkSpeed = 1f;
    [Header("Player Related Settings")]
    [SerializeField] private float playerScanRadius = 1.25f;
    [SerializeField] private LayerMask playersLM;
    [SerializeField] private float damageToPlayerAmount = 10f;

    #endregion
    
    #region PRIVATE FIELDS

    private HealthComponent _healthComponent;
    private Animator enemyAnim;
    private GameObject playerTarget;
    private MoveDirection movementDir = MoveDirection.RIGHT;
    private Coroutine repeatedChangeDirectionCoroutine,changeDirectionWait;
    private bool canChangeDirection = true,isDead;

    #endregion

    #region MONOBEHAVIOUR CALLBACKS

    private void Awake()
    {
        _healthComponent = GetComponent<HealthComponent>();
        enemyAnim=GetComponent<Animator>();
    }

    void Start()
    {
        // InvokeRepeating(nameof(ChangeDirection),0,5f);
        repeatedChangeDirectionCoroutine = StartCoroutine(RepeatChangeDirection(Random.Range(3f,8f)));
    }

    private void OnEnable()
    {
        _healthComponent.OnHealthZero += MakeEnemyDie;
    }

    private void OnDisable()
    {
        _healthComponent.OnHealthZero -= MakeEnemyDie;
    }

    // Update is called once per frame
    void Update()
    {
        
        RandomWandering();
        ScanTarget();
    }

    #endregion

    #region PRIVATE METHODS
    private void RandomWandering()
    {
        if(playerTarget==null && !isDead)
        {
            UpdateMovementDirection();
            transform.Translate(new Vector3((int)movementDir * walkSpeed * Time.deltaTime, 0, 0));
            enemyAnim.SetBool("Walk", true);
        }
    }
       private void MakeEnemyDie()
    {
        isDead = true;
        enemyAnim.SetTrigger("Death");
        GameManager.OnEnemyDead?.Invoke(gameObject);
        GameManager.Instance.enemySpawner.ReturnObjectToPool(gameObject,2f);
    }

    /// <summary>
    /// Updates the current DIRECTION of this enemy object based on Scale.
    /// </summary>
    /// <returns></returns>
    private bool UpdateMovementDirection()
    {
        if (transform.localScale.x < 0)
        {
            movementDir = MoveDirection.LEFT;
            return false;
        }
        else
        {
            movementDir = MoveDirection.RIGHT;
            return true;
        }
    }

    private void ChangeDirection(bool random=true,MoveDirection dirIfRandom=MoveDirection.RIGHT)
    {
        int newMoveDir=random ? Random.Range(0, 1) : (int)dirIfRandom;
        bool moveLeft= newMoveDir==0;
        if (moveLeft)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        UpdateMovementDirection();
    }
    IEnumerator RepeatChangeDirection(float repeatRate)
    {
        while (true)
        {
            yield return new WaitForSeconds(repeatRate);
            ChangeDirection(); 
        }
    }

    IEnumerator ChangeDirWait()
    {
        canChangeDirection = false;
        yield return new WaitForSeconds(3f);
        canChangeDirection = true;
    }
    #endregion
    
    #region PUBLIC METHODS

    public void ScanTarget()
    {
        var target=Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.right)*(int)movementDir,playerScanRadius,playersLM);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right)*(int)movementDir*playerScanRadius,Color.cyan,0.5f);

        if (target.collider != null)
        {
            if(target.collider.tag.Equals(Tags.PLAYER)) 
            {
                StopCoroutine(repeatedChangeDirectionCoroutine);
                playerTarget = target.collider.gameObject;
                enemyAnim.SetTrigger(EnemyAnimatorParams.ATTACK_THE_PLAYER);
                enemyAnim.SetBool(EnemyAnimatorParams.WALKING, false);
            }
            else if (canChangeDirection||target.collider.tag.Equals("Scene"))
            {
                Debug.Log("<color=red>Direction Change</color>");
                ChangeDirection(false,UpdateMovementDirection()?MoveDirection.LEFT:MoveDirection.RIGHT);
                if(changeDirectionWait!=null)
                    StopCoroutine(changeDirectionWait);
               
                changeDirectionWait=StartCoroutine(ChangeDirWait());
            }
        }
    }

    public void DamagePlayer()
    {
        if(playerTarget!=null)
        {
            playerTarget.GetComponent<Animator>().SetTrigger(PlayerAnimatorParams.HURT);
            playerTarget.GetComponent<IDamageable>().TakeDamage(damageToPlayerAmount);
            playerTarget = null;
        }
    }

    #endregion
}
