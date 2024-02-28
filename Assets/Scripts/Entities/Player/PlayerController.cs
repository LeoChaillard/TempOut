using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IEntityBase, IDamageable
{
    // Fields
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] float groundDist;
    [SerializeField] LayerMask terrainLayer;
    [SerializeField] Transform _groundRayTransform;
    [SerializeField] private int _jumpForce;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private LayerMask _ennemyLayers;

    // Components
    private Animator _animator;
    private Rigidbody _physics;
    private SpriteRenderer _spriteRenderer;

    // Controls
    private Vector2 _movementInput;
    public Vector3 FacingDirection { get; private set; }
    private float _currentSpeed;

    // Jump
    private int _numJumps = 1;
    private int _jumpsLeft;
    private float _jumpTimer;
    private float _fallMultiplier = 2.5f;

    // Attack
    private float _attackRange = 0.5f;
    private float _attackPointDistance;

    // Health
    private int _maxHealth = 100;
    private int _currentHealth;


    // Delegates
    public event Action OnSwitchCharacter;

    private void Awake()
    {
        _currentSpeed = _moveSpeed;
        _currentHealth = _maxHealth;
        _physics = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _attackPointDistance = _attackPoint.localPosition.x;
    }

    private void InitializeComponents()
    {
        _currentSpeed = _moveSpeed;
        _physics = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        Movement();
        Jump();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_jumpsLeft > 0)
            {
                _animator.SetBool("isJumping", true);
                _physics.AddForce(new Vector3(0, _jumpForce, 0), ForceMode.Impulse);
                _jumpsLeft--;
                _jumpTimer = 0;
            }
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            OnSwitchCharacter?.Invoke();
        }
    }

    private void Jump()
    {
        if (_physics.velocity.y < 0)
        {
            _physics.velocity += (Physics.gravity.y * (_fallMultiplier - 1) * Time.deltaTime) * Vector3.up;
        }
                
        if (IsGrounded() && _jumpTimer >= 0.5)
        {
            _animator.SetBool("isJumping", false);
            _jumpsLeft = _numJumps;
        }
        _jumpTimer += Time.fixedDeltaTime;
    }

    private void OnMove(InputValue inputValue)
    {        
        _movementInput = inputValue.Get<Vector2>();
        if (_movementInput.x != 0 && _movementInput.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (_movementInput.x != 0 && _movementInput.x > 0)
        {
            _spriteRenderer.flipX = false;
        }

        if ( _movementInput.x != 0 || _movementInput.y != 0)
        {
            _animator.SetBool("isMoving", true);
        }
        else
        {
            _animator.SetBool("isMoving", false);
        }        
    }

    public void Movement()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Transition1") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Transition2") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Transition3"))
        {
            _currentSpeed = _moveSpeed - 4;
        }
        else if (_movementInput.y != 0)
        {
            _currentSpeed = _moveSpeed - 1;
        }
        else
        {
            _currentSpeed = _moveSpeed;
        }

        Vector3 dir = new Vector3(_movementInput.x, 0, _movementInput.y);
        if (dir != Vector3.zero)
        {
            FacingDirection = dir;
        }
        _physics.velocity = (dir * _currentSpeed) + new Vector3(0, _physics.velocity.y, 0);
    }

    private void OnFire()
    {
        if (!_animator.GetBool("isAttacking"))
        {
            _animator.SetBool("isAttacking", true);
            UpdateAttackPointPosition();
            Attack();
        }
    }

    public void Attack()
    {
        Collider [] hitEnemies = Physics.OverlapSphere(_attackPoint.position, _attackRange, _ennemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            StartCoroutine(enemy.GetComponent<IDamageable>()?.TakeDamage(20));
        }
    }

    private void UpdateAttackPointPosition()
    {
        Vector3 xPlus = new Vector3(_attackPointDistance, _attackPoint.transform.localPosition.y, 0);
        Vector3 xMinus = new Vector3(-_attackPointDistance, _attackPoint.transform.localPosition.y, 0);
        Vector3 zPlus = new Vector3(0, _attackPoint.transform.localPosition.y, _attackPointDistance);
        Vector3 zMinus = new Vector3(0, _attackPoint.transform.localPosition.y, -_attackPointDistance);

        Vector3 plusplus = new Vector3(_attackPointDistance * Mathf.Cos(Mathf.Deg2Rad * 45), _attackPoint.transform.localPosition.y, _attackPointDistance * Mathf.Sin(Mathf.Deg2Rad * 45));
        Vector3 minusplus = new Vector3(_attackPointDistance * Mathf.Cos(Mathf.Deg2Rad * 135), _attackPoint.transform.localPosition.y, _attackPointDistance * Mathf.Sin(Mathf.Deg2Rad * 135));
        Vector3 minusminus = new Vector3(_attackPointDistance * Mathf.Cos(Mathf.Deg2Rad * 225), _attackPoint.transform.localPosition.y, _attackPointDistance * Mathf.Sin(Mathf.Deg2Rad * 225));
        Vector3 plusminus = new Vector3(_attackPointDistance * Mathf.Cos(Mathf.Deg2Rad * 315), _attackPoint.transform.localPosition.y, _attackPointDistance * Mathf.Sin(Mathf.Deg2Rad * 315));

        if (FacingDirection.x >= -1 && FacingDirection.x <= 0.75)
        {
            _attackPoint.transform.localPosition = xMinus;
        }

        if (FacingDirection.x >= -0.75 && FacingDirection.x <= -0.25)
        {
            if (FacingDirection.z >= 0)
            {
                _attackPoint.transform.localPosition = minusplus;
            }
            else
            {
                _attackPoint.transform.localPosition = minusminus;
            }
        }

        if (FacingDirection.x >= -0.25 && FacingDirection.x <= 0.25)
        {
            if (FacingDirection.z >= 0)
            {
                _attackPoint.transform.localPosition = zPlus;
            }
            else
            {
                _attackPoint.transform.localPosition = zMinus;
            }

        }

        if (FacingDirection.x > 0.25 && FacingDirection.x <= 0.75)
        {
            if (FacingDirection.z >= 0)
            {
                _attackPoint.transform.localPosition = plusplus;
            }
            else
            {
                _attackPoint.transform.localPosition = plusminus;
            }
        }

        if (FacingDirection.x > 0.75 && FacingDirection.x <= 1)
        {
            _attackPoint.transform.localPosition = xPlus;
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
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Linecast(transform.position, _groundRayTransform.position, terrainLayer);
    }
}
