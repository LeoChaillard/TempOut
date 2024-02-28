using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Unit, IEntityBase, IDamageable
{
    // Fields
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private LayerMask _terrainLayer;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _sightRange = 8f;
    [SerializeField] private float _attackRange = 0.8f;

    // Patroling
    private Vector3 _walkPoint;
    private bool _walkPointSet;
    private float _walkPointRange;
    private float _timeBetweenMoves;
    private bool _isMoving;
    private bool _isArrived;

    // Attacking
    private float _timeBetweenAttacks;
    private bool _alreadyAttacked;
    private float _attackHitRange;
    private float _attackPointDistance;

    // States
    private bool _playerInSightRange;
    private bool _playerInAttackRange;

    // Health
    private int _maxHealth = 100;
    private int _currentHealth;

    private Rigidbody _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _currentHealth = _maxHealth;
        _walkPointRange = 5.0f;
        _timeBetweenAttacks = 5f;
        _playerInSightRange = false;
        _playerInAttackRange = false;
        _walkPointSet = false;
        _timeBetweenMoves = 5.0f;
        _isMoving = false;
        _isArrived = false;
        _attackHitRange = 0.5f;
        _attackPointDistance = _attackPoint.localPosition.x;

    }
    private void Update()
    {
        // Check for sight and attack range
       _playerInSightRange = Physics.CheckSphere(transform.parent.position, _sightRange, _playerLayer);
        _playerInAttackRange = Physics.CheckSphere(transform.parent.position, _attackRange, _playerLayer);

        /*if (!_playerInSightRange)
        {
            Movement();
        }*/
        if (_playerInSightRange)
        {
           /* if (_alreadyAttacked)
            {
                Debug.Log("invoke");
                Invoke(nameof(ResetAttack), _timeBetweenAttacks);
            }*/

            float distance = Vector3.Distance(transform.parent.position, _playerTransform.position);
            if (!_alreadyAttacked && distance > 2.0f)
            {                
                _isArrived = false;
                Chase();
            }
            if  (!_alreadyAttacked && _isArrived)
            {
                _alreadyAttacked = true;
                UpdateAttackPointPosition();
                Attack();
                Invoke(nameof(Attack), 0.1f);
                Invoke(nameof(Attack), 0.5f);
                Invoke(nameof(ResetAttack), _timeBetweenAttacks);
            }                   
        }
    }

    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = 0;
        float randomX = 0;

        while (randomX >= -2 && randomX <= 2)
        {
            randomX = Random.Range(-_walkPointRange, _walkPointRange);
        }

        while (randomZ >= -2 && randomZ <= 2)
        {
            randomZ = Random.Range(-_walkPointRange, _walkPointRange);
        }

        _walkPoint = new Vector3(transform.parent.position.x + randomX, transform.parent.position.y, transform.parent.position.z + randomZ);
        Debug.Log(_walkPoint);
        if (!Physics.Raycast(_walkPoint + new Vector3(0, 2f, 0), -transform.up, 2f, _obstacleLayer))
        {
            Debug.Log("ray");
            _walkPointSet = true;
        }
    }

    private void Chase()
    {
        MoveToPosition(_playerTransform.position, _attackRange);
    }

    private void ResetAttack()
    {
        _alreadyAttacked = false;
    }

    private void ResetMove()
    {
        _isMoving = false;
        _walkPointSet = false;
    }

    private void UpdateAttackPointPosition()
    {
        Vector3 xPlus = new Vector3(_attackPointDistance, _attackPoint.transform.localPosition.y, 0);
        Vector3 xMinus= new Vector3(-_attackPointDistance, _attackPoint.transform.localPosition.y, 0);
        Vector3 zPlus = new Vector3(0, _attackPoint.transform.localPosition.y, _attackPointDistance);
        Vector3 zMinus= new Vector3(0, _attackPoint.transform.localPosition.y, -_attackPointDistance);

        Vector3 plusplus = new Vector3(_attackPointDistance * Mathf.Cos(Mathf.Deg2Rad * 45), _attackPoint.transform.localPosition.y, _attackPointDistance * Mathf.Sin(Mathf.Deg2Rad * 45));
        Vector3 minusplus = new Vector3(_attackPointDistance * Mathf.Cos(Mathf.Deg2Rad * 135), _attackPoint.transform.localPosition.y, _attackPointDistance * Mathf.Sin(Mathf.Deg2Rad * 135));
        Vector3 minusminus = new Vector3(_attackPointDistance * Mathf.Cos(Mathf.Deg2Rad * 225), _attackPoint.transform.localPosition.y, _attackPointDistance * Mathf.Sin(Mathf.Deg2Rad * 225));
        Vector3 plusminus = new Vector3(_attackPointDistance * Mathf.Cos(Mathf.Deg2Rad * 315), _attackPoint.transform.localPosition.y, _attackPointDistance * Mathf.Sin(Mathf.Deg2Rad * 315));

        if (CurrentDirection.x >= -1 && CurrentDirection.x <= 0.75)
        {
            _attackPoint.transform.localPosition = xMinus;
        }

        if (CurrentDirection.x >= -0.75 && CurrentDirection.x <= -0.25)
        {
            if (CurrentDirection.z >= 0)
            {
                _attackPoint.transform.localPosition = minusplus;
            }
            else
            {
                _attackPoint.transform.localPosition = minusminus;
            }            
        }

        if (CurrentDirection.x >= -0.25 && CurrentDirection.x <= 0.25)
        {
            if (CurrentDirection.z >= 0)
            {
                _attackPoint.transform.localPosition = zPlus;
            }
            else
            {
                _attackPoint.transform.localPosition = zMinus;
            }

        }

        if (CurrentDirection.x > 0.25 && CurrentDirection.x <= 0.75)
        {
            if (CurrentDirection.z >= 0)
            {
                _attackPoint.transform.localPosition = plusplus;
                Debug.Log(plusplus);
            }
            else
            {
                _attackPoint.transform.localPosition = plusminus;
            }
        }

        if (CurrentDirection.x > 0.75 && CurrentDirection.x <= 1)
        {            
            _attackPoint.transform.localPosition = xPlus;
        }
    }

    protected override void OnMove()
    {
        base.OnMove();

        if (CurrentDirection.x != 0 || CurrentDirection.z != 0)
        {
            _animator.SetBool("isMoving", true);
        }

        if (CurrentDirection.x != 0 && CurrentDirection.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (CurrentDirection.x != 0 && CurrentDirection.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
    }

    protected override void OnArrive()
    {
        base.OnArrive();
        Debug.Log("arrive");
        _isArrived = true;
        _animator.SetBool("isMoving", false);
        Invoke(nameof(ResetMove), _timeBetweenMoves);
    }

    protected override void OnPathNotFound()
    {
        base.OnPathNotFound();
        _animator.SetBool("isMoving", false);
        ResetMove();
    }

    public void Movement()
    {
        if (!_walkPointSet)
        {
            SearchWalkPoint();
        }
        if (_walkPointSet && !_isMoving)
        {
            _isMoving = true;
            MoveToPosition(_walkPoint);
        }
    }

    public void Attack()
    {
        // Make sure enemy doesn't move
        //transform.right = _playerTransform.position - transform.parent.position;
        if (!_animator.GetBool("isAttacking"))
        {
            Debug.Log("attack");
            _animator.SetBool("isAttacking", true);
            Collider[] hitEnemies = Physics.OverlapSphere(_attackPoint.position, _attackHitRange, _playerLayer);
            if (hitEnemies.Length != 0)
            {
                foreach (Collider enemy in hitEnemies)
                {
                    StartCoroutine(enemy.GetComponent<IDamageable>()?.TakeDamage(20));
                }
            }
        }
    }

    public IEnumerator TakeDamage(int damage)
    {
        yield return new WaitForSeconds(0.4f);
        _currentHealth -= damage;
        Debug.Log(_currentHealth);
        _animator.SetTrigger("isHurt");

        if (_currentHealth <= 0)
        {
            _animator.SetTrigger("isDead");
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<BoxCollider>().enabled = false;
            this.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_attackPoint != null)
        {
            Gizmos.DrawWireSphere(_attackPoint.position, _attackHitRange);
        }

        Gizmos.DrawWireSphere(transform.parent.position, _sightRange);
    }
}
