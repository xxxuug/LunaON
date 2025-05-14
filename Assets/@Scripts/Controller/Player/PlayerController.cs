using UnityEditor;
using UnityEngine;

public class PlayerController : BaseController
{
    public WeaponHitbox _weaponHitbox;
    public GameObject SwordEffect;

    private Transform _character;
    private Animator _animator;
    private Rigidbody _rigidbody;

    public static float _battleIdleTime = 0f;
    public static float MoveSpeed = 5f;

    private bool _isBattleLayerActive = false;
    private bool _isGround = false;
    //private bool _isStopJump = false;
    private bool _isJumpRequest = false;
    private bool _isDead = false;
    private float _groundDisableTime = 0.75f;
    private float _lastJumpTime;


    [Header("Raycast")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckDistance = 0.3f;
    [SerializeField] private Transform _groundCheckPoint;

    [Header("sound")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _attackClip;
    [SerializeField] private AudioClip _hitClip;
    [SerializeField] private AudioClip _grassStepClip;
    [SerializeField] private AudioClip _jumpClip;

    public float Speed
    {
        get { return _animator.GetFloat(Define.Speed); }
        set { _animator.SetFloat(Define.Speed, value); }
    }

    public int ComboCount
    {
        get { return _animator.GetInteger(Define.ComboCount); }
        set { _animator.SetInteger(Define.ComboCount, value); }
    }

    public bool IsNextCombo
    {
        get { return _animator.GetBool(Define.isNextCombo); }
        set { _animator.SetBool(Define.isNextCombo, value); }
    }

    public bool IsAttacking
    {
        get { return _animator.GetBool(Define.isAttacking); }
        set { _animator.SetBool(Define.isAttacking, value); }
    }

    public bool IsJumping
    {
        get { return _animator.GetBool(Define.Jump); }
        set { _animator.SetTrigger(Define.Jump); }
    }

    public bool IsGround
    {
        get { return _animator.GetBool(Define.Ground); }
        set { _animator.SetTrigger(Define.Ground); }
    }

    public void DefendHit()
    {
        _animator.SetTrigger(Define.DefendHit);
    }

    public void Hit()
    {
        _animator.SetTrigger(Define.Hit);
    }

    public void Die()
    {
        _animator.SetTrigger(Define.Die);
    }

    public bool DieStay
    {
        get { return _animator.GetBool(Define.DieStay); }
        set { _animator.SetBool(Define.DieStay, value); }
    }

    public bool IsDead
    {
        get { return _animator.GetBool(Define.isDead); }
        set { _animator.SetBool(Define.isDead, value); }
    }

    protected override void Initialize()
    {
        _character = transform;
        _animator = GetComponent<Animator>();
        _rigidbody = gameObject.AddComponent<Rigidbody>();
        //_rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
        _rigidbody.freezeRotation = true;

        // 시작할 때 배틀 레이어 비활성화
        _animator.SetLayerWeight(_animator.GetLayerIndex("Battle"), 0);
    }

    private void Update()
    {
        Attack();
        IdleChange();

        if (_isDead) return;
        if (TalkManager.Instance != null && TalkManager.Instance.IsTalking) return;

        if (Input.GetKeyDown(KeyCode.Space) && _isGround && Time.time - _lastJumpTime > _groundDisableTime)
            _isJumpRequest = true;
    }

    void FixedUpdate()
    {
        Move();

        if (_isJumpRequest && _isGround)
        {
            Jump();
            _isJumpRequest = false;
        }

        CheckGround();
    }

    void Attack()
    {
        if (_isDead) return;

        bool isUIClick =
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        bool isLeftMouseDown = Input.GetMouseButtonDown(0);

        if (isLeftMouseDown == false || isUIClick)
            return;

        if (IsAttacking == false)
        {
            // 공격 시작
            IsAttacking = true;

            // 배틀 레이어 활성화
            if (_isBattleLayerActive == false)
            {
                _animator.SetLayerWeight(_animator.GetLayerIndex("Battle"), 0.6f);
                _isBattleLayerActive = true;
            }
        }
        else
        {
            IsNextCombo = true;
        }
    }

    void Move()
    {
        if (_isDead) return;

        if (Input.GetButton(Define.Horizontal) || Input.GetButton(Define.Vertical))
        {
            float h = Input.GetAxis(Define.Horizontal);
            float v = Input.GetAxis(Define.Vertical);

            Vector3 movement = new Vector3(h, 0, v);
            Vector3 localMovement = transform.TransformDirection(movement) * MoveSpeed;
            //_rigidbody.linearVelocity = localMovement.normalized * 10;
            localMovement.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = localMovement;

        }
        else
        {
            //_rigidbody.linearVelocity = new Vector3(0, 0, 0);
            Vector3 velocity = _rigidbody.linearVelocity;
            velocity.x = 0;
            velocity.z = 0;
            _rigidbody.linearVelocity = velocity;
        }
        Speed = _rigidbody.linearVelocity.sqrMagnitude;
    }

    void Jump()
    {
        _rigidbody.AddForce(Vector3.up * 5, ForceMode.Impulse);
        IsJumping = true;
        _isGround = false;
        _lastJumpTime = Time.time;
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        var playerInfo = GameManager.Instance.PlayerInfo;
        GameManager.Instance.ChangeHp(-damage);

        //Debug.Log("TkakeDamage 에서 GameManager 해시: " + GameManager.Instance.GetHashCode());

        BattleLayerIndexUp();
        IsDead = false;
        Hit();
        Invoke(nameof(BattleLayerIndexDown), 0.4f);
        Debug.Log("플레이어 HP : " + playerInfo.CurrentHp);

        if (playerInfo.CurrentHp <= 0)
        {
            _isDead = true;
            BattleLayerIndexUp();
            IsDead = true;
            GetComponent<PlayerController>().enabled = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            Die();
            Debug.Log("Die 애니메이션 실행!");
            DieStay = true;

        }
    }

    void BattleLayerIndexUp()
    {
        if (_isDead) return;
        _animator.SetLayerWeight(_animator.GetLayerIndex("Battle"), 1);
        _animator.SetLayerWeight(_animator.GetLayerIndex("Move"), 0.5f);
    }

    void BattleLayerIndexDown()
    {
        if (_isDead) return;
        _animator.SetLayerWeight(_animator.GetLayerIndex("Battle"), 0.5f);
        _animator.SetLayerWeight(_animator.GetLayerIndex("Move"), 1f);
    }

    void IdleChange()
    {
        // 공격이랑 콤보 모두 비활성화이면 전투 대기 상태라는 뜻
        if (!IsAttacking && !IsNextCombo && _isBattleLayerActive)
        {
            _battleIdleTime += Time.deltaTime;
            // 전투 대기 모션이 5초 이상 지속된다면
            if (_battleIdleTime >= 5f)
            {
                // 배틀 레이어 비활성화
                _animator.SetLayerWeight(_animator.GetLayerIndex("Battle"), 0f);
                _isBattleLayerActive = false;
                _battleIdleTime = 0;
            }
        }
        else
        {
            _battleIdleTime = 0;
        }
    }

    void CheckGround()
    {
        if (_groundCheckPoint == null) return;

        if (Time.time - _lastJumpTime < _groundDisableTime)
        {
            _isGround = false;
            _animator.SetBool(Define.Ground, false);
            return;
        }

        RaycastHit hit;
        _isGround = Physics.Raycast(_groundCheckPoint.position, Vector3.down, out hit, _groundCheckDistance, _groundLayer);
        _animator.SetBool(Define.Ground, _isGround);
    }

    public void PlaySwordEffect()
    {
        // 이펙트 생성
        Quaternion rot = Quaternion.LookRotation(_weaponHitbox.transform.forward);
        Instantiate(SwordEffect, _weaponHitbox.transform.position, rot, transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            int damage = 0;
            CactusController cactus = collision.gameObject.GetComponent<CactusController>();
            if (cactus != null)
                damage = cactus.Damage;
            MushroomController mushroom = collision.gameObject.GetComponent<MushroomController>();
            if (mushroom != null)
                damage = mushroom.Damage;

            if (damage > 0)
                TakeDamage(damage);
        }
    }

    // 애니메이션 이벤트
    //void OnStopJump() => _isStopJump = true;

    void EnableAttack() => _weaponHitbox.EnableAttack();
    void DisableAttack() => _weaponHitbox.DisableAttack();
    void OnEndDieAnimation() => FreezeAnimation();

    void FreezeAnimation()
    {
        _animator.speed = 0;
    }

    public void PlayAttackSound()
    {
        if (_audioSource != null && _attackClip != null)
        {
            _audioSource.PlayOneShot(_attackClip);
        }
    }

    public void PlayHitSound()
    {
        if (_audioSource != null && _hitClip != null)
        {
            _audioSource.PlayOneShot(_hitClip);
        }
    }

    public void PlayGressStepSound()
    {
        if (_audioSource != null && _grassStepClip != null)
        {
            _audioSource.PlayOneShot(_grassStepClip);
        }
    }

    public void PlayJumpSound()
    {
        if (_audioSource != null && _jumpClip != null)
        {
            _audioSource.PlayOneShot(_jumpClip);
        }
    }
}
