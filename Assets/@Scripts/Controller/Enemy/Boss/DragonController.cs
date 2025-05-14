using UnityEngine;

public enum AttackType
{
    None = 0,
    ClawAttack = 1,
    FlameAttack = 2,
    FlyFlameAttack = 3
}

public class DragonController : EnemyController
{
    private Animator _animator;
    private Rigidbody _rigidbody;

    private StateMachine<DragonController> _stateMachine;

    [Header("AI 설정")]
    public Transform Target;
    public float MoveSpeed = 5f;

    [Header("Skill")]
    // Flame Attack
    public FlameHitBox FlameHitbox;
    public GameObject FireEffect;
    public GameObject FirePoint;
    // Claw Attack
    public ClawHitBox ClawHitbox;
    // Fire Earth Attack
    public GameObject FireEarthEffect;
    public GameObject LeftFireEarthPos;
    public GameObject RightFireEarthPos;
    // Area Exlposion Attack
    public GameObject BigExplosionEffect;
    public GameObject AreaFireEffect;
    public GameObject AreaExplosionPos;
    // toggle
    private bool _isNextFireEarth = true;

    [Header("전투 거리")]
    public float StartRange = 50f;
    public float NearRange = 25f;

    [Header("HP 상태")]
    public bool IsFlying => _currentHp <= _maxHp * 0.5f;
    public float CurrentHP => _currentHp;
    public float MaxHP => _maxHp;

    public float Speed
    {
        get => _animator.GetFloat("Speed");
        set => _animator.SetFloat("Speed", value);
    }

    public void SetAttackType(AttackType type)
    {
        _animator.SetInteger("AttackType", (int)type);
    }

    public void ClawAttack() => _animator.SetTrigger("ClawAttack");
    public void FlameAttack() => _animator.SetTrigger("FlameAttack");
    public void FlyFlameAttack() => _animator.SetTrigger("FlyFlameAttack");
    public void TakeOff() => _animator.SetTrigger("TakeOff");
    public void GetHit() => _animator.SetTrigger("GetHit");
    public void Die() => _animator.SetTrigger("Die");
    public bool IsScreaming { get; set; } = false;
    public void Scream() => _animator.SetTrigger("Scream");
    public bool IsPlayerNear
    {
        get { return _animator.GetBool("IsPlayerNear"); }
        set { _animator.SetBool("IsPlayerNear", value); }
    }
    public bool IsAttacking
    {
        get { return _animator.GetBool("IsAttacking"); }
        set { _animator.SetBool("IsAttacking", value); }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();

        _stateMachine = new StateMachine<DragonController>(this, new DragonSleepState());
        _stateMachine.AddState(new DragonIdleState());
        _stateMachine.AddState(new DragonTakeOffState());
        _stateMachine.AddState(new DragonFlyFloatState());
        _stateMachine.AddState(new DragonDieState());
        _stateMachine.AddState(new DragonGetHitState());
        _stateMachine.AddState(new DragonChaseState());
        _stateMachine.AddState(new DragonAttackState());

        _maxHp = 500;
        _currentHp = _maxHp;
    }

    private void Start()
    {
        Target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    void Update()
    {
        _stateMachine.OnUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        _stateMachine.OnFixedUpdate(Time.fixedDeltaTime);
    }

    // 이동
    public void MoveTo(Vector3 destination)
    {
        Vector3 dir = (destination - transform.position).normalized;
        _rigidbody.MovePosition(transform.position + dir * MoveSpeed * Time.fixedDeltaTime);
        transform.LookAt(new Vector3(destination.x, transform.position.y, destination.z));
    }

    public void LookAtTarget()
    {
        if (Target == null) return;

        Vector3 lookPos = Target.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
    }

    // 조건 체크
    public bool IsNearRange()
    {
        if (Target == null) return false;
        return Vector3.Distance(transform.position, Target.position) < NearRange;
    }

    public bool IsStartRange()
    {
        if (Target == null) return false;
        return Vector3.Distance(transform.position, Target.position) < StartRange;
    }

    public void Hit(float damage)
    {
        if (_stateMachine.CurrentState is DragonDieState) return;

        _currentHp -= damage;

        if (_currentHp <= 0)
        {
            _stateMachine.ChangeState<DragonDieState>();
        }
        else
        {
            _stateMachine.ChangeState<DragonGetHitState>();
        }
    }

    // 사망 애니메이션 끝에서 호출됨
    public void OnDeathAnimationEnd()
    {
        gameObject.SetActive(false);
        // 예: BossPool.Instance.Despawn(this);
    }

    // 공격 애니메이션 종료 이벤트용 (필요 시 확장)
    public void OnAttackAnimationEnd()
    {
        if (_stateMachine.CurrentState is DragonAttackState)
        {
            IsAttacking = false;
            //Debug.Log("[DragonController] OnAttackAnimationEnd() 이벤트 실행");
            if (IsFlying)
                _stateMachine.ChangeState<DragonFlyFloatState>();
            else
                _stateMachine.ChangeState<DragonIdleState>();
        }
    }

    public override bool AnyDamage(float damage, GameObject damageCauser, Vector2 hitPoint = default)
    {
        if (_stateMachine.CurrentState is DragonDieState)
            return false;

        _currentHp -= damage;

        if (_currentHp <= 0)
        {
            _stateMachine.ChangeState<DragonDieState>();
            return true;
        }

        Target = damageCauser.transform; // 플레이어 추적용

        _stateMachine.ChangeState<DragonGetHitState>();
        Debug.Log($"드래곤 hp : {_currentHp} isFlying : {IsFlying}");
        return true;
    }

    // 이벤트 함수
    public void PlayFireEffect()
    {
        Instantiate(FireEffect, FirePoint.transform.position, FirePoint.transform.rotation, FirePoint.transform);
    }

    // Hitbox 이벤트 함수
    public void EnableClawHitbox() => ClawHitbox.ActivateHitbox();
    public void DisableClawHitbox() => ClawHitbox.DeactivateHitbox();
    public void EnableFlameHitbox() => FlameHitbox.ActivateHitbox();
    public void DisableFlameHitbox() => FlameHitbox.DeactivateHitbox();

    void PlayFireEarthEffect()
    {
        Instantiate(FireEarthEffect, LeftFireEarthPos.transform.position, LeftFireEarthPos.transform.rotation, LeftFireEarthPos.transform);
        Instantiate(FireEarthEffect, RightFireEarthPos.transform.position, RightFireEarthPos.transform.rotation, RightFireEarthPos.transform);
    }

    void PlayAreaExplosionEffect()
    {
        float posZ = Random.Range(25, 89);
        float posX = Random.Range(-175, -138);
        Vector3 randPos = new Vector3(posX, 0, posZ);

        Instantiate(BigExplosionEffect, randPos + new Vector3(0, 1.5f, 0), Quaternion.identity, AreaExplosionPos.transform);
        Instantiate(AreaFireEffect, randPos, Quaternion.identity, AreaExplosionPos.transform);
    }

    public void PlayNextFlyAttackEffect()
    {
        Debug.Log("PlayNextFlyAttackEffect 함수 실행");
        if (_isNextFireEarth)
        {
            PlayFireEarthEffect();
            Debug.Log("FireEarth 이펙트 실행");
        }
        else
        {
            PlayAreaExplosionEffect();
            Debug.Log("AreaExplosion 이펙트 실행");
        }

        _isNextFireEarth = !_isNextFireEarth;
    }
}
