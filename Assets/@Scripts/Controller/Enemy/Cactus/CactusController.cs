using UnityEngine;

public class CactusController : EnemyController
{
    private Animator _animator;
    private Rigidbody _rigidbody;

    private StateMachine<CactusController> _stateMachine;

    public string MonsterName = "선인장";

    [Header("AI 관련 설정")]
    public Transform Target;
    public float MoveSpeed = 2f;
    public float AttackRange = 1.5f;
    public float ChaseRange = 4f;
    public float ReturnRange = 6f;

    [Header("Idle 이동 범위")]
    public float MinX = 16f;
    public float MaxX = 25f;
    public float MinZ = 55f;
    public float MaxZ = 64f;

    [Header("Status")]
    public int Damage = 10;
    public GameObject HitEffect;

    public Vector3 SpawnPoint { get; private set; }
    public Vector3 IdleTarget { get; private set; }

    // 애니메이터 파라미터 제어
    public float Speed
    {
        get => _animator.GetFloat("Speed");
        set => _animator.SetFloat("Speed", value);
    }

    public bool IsAttacking
    {
        get => _animator.GetBool("isAttacking");
        set => _animator.SetBool("isAttacking", value);
    }

    // Animator 트리거 제어
    public void Attack()
    {
        _animator.SetTrigger("Attack");
    }

    public void Die()
    {
        _animator.SetTrigger("Die");
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Target = GameObject.FindGameObjectWithTag("Player")?.transform;

        _stateMachine = new StateMachine<CactusController>(this, new CactusIdleState());
        _stateMachine.AddState(new CactusPatrolState());
        _stateMachine.AddState(new CactusChaseState());
        _stateMachine.AddState(new CactusAttackState());
        _stateMachine.AddState(new CactusCooldownState());
        _stateMachine.AddState(new CactusReturnState());
        _stateMachine.AddState(new CactusDeadState());
        _stateMachine.AddState(new CactusHitState());

        SpawnPoint = transform.position;
        SetRandomIdleTarget();

        _currentHp = _maxHp;
    }

    private void Update()
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

    // 상태 조건
    public bool ArrivedTo(Vector3 point)
    {
        return Vector3.Distance(transform.position, point) < 0.5f;
    }

    public bool IsAttackRange()
    {
        if (Target == null) return false;
        return Vector3.Distance(transform.position, Target.position) < AttackRange;
    }

    public bool IsMoveRange()
    {
        if (Target == null) return false;
        return Vector3.Distance(transform.position, Target.position) < ChaseRange;
    }

    public bool IsReturnToSpawn()
    {
        if (Target == null) return false;
        return Vector3.Distance(transform.position, Target.position) > ReturnRange;
    }

    // 랜덤 이동
    public void SetRandomIdleTarget()
    {
        float x = Random.Range(MinX, MaxX);
        float z = Random.Range(MinZ, MaxZ);

        Vector3 origin = new Vector3(x, 20f, z); // 위에서 쏘는 Ray

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 50f, LayerMask.GetMask("Ground")))
        {
            IdleTarget = hit.point + Vector3.up * 0.1f;
        }
        else
        {
            IdleTarget = transform.position; // fallback
        }
    }

    // 애니메이션 이벤트로 호출됨
    public void OnAttackAnimationEnd()
    {
        if (_stateMachine != null && _stateMachine.CurrentState is CactusAttackState)
            _stateMachine.ChangeState<CactusCooldownState>();
    }

    public void OnDeathAnimationEnd()
    {
        gameObject.SetActive(false);
        SpawningPool.Instance.CactusDie(this);
    }

    public override void SetSpawnPoint(Vector3 spawnPos)
    {
        transform.position = spawnPos;
        SpawnPoint = spawnPos;
    }

    public override bool AnyDamage(float damage, GameObject damageCauser, Vector2 hitPoint = default)
    {
        if (_stateMachine.CurrentState is CactusDeadState)
            return false;

        _currentHp -= damage;
        Instantiate(HitEffect, transform.position, Quaternion.identity, transform);

        if (_currentHp <= 0)
        {
            _stateMachine.ChangeState<CactusDeadState>();
            GameManager.Instance.AddExp(30);
            QuestManager.Instance.AddKillCount(MonsterName);
            return true;
        }

        Target = damageCauser.transform; // 플레이어 추적용

        _stateMachine.ChangeState<CactusHitState>();

        return true;
    }
}
