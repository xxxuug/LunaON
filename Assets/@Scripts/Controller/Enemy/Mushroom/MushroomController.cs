using UnityEngine;

public class MushroomController : EnemyController
{
    private Animator _animator;
    private Rigidbody _rigidbody;

    private StateMachine<MushroomController> _stateMachine;

    [Header("AI ���� ����")]
    public Transform Target;
    public float MoveSpeed = 2f;
    public float AttackRange = 1.5f;
    public float ChaseRange = 4f;
    public float ReturnRange = 6f;

    [Header("Idle �̵� ����")]
    public float MinX = 8f;
    public float MaxX = 30f;
    public float MinZ = 70f;
    public float MaxZ = 80f;

    [Header("Status")]
    public int Damage = 10;
    public GameObject HitEffect;

    public Vector3 SpawnPoint { get; private set; }
    public Vector3 IdleTarget { get; private set; }

    // �ִϸ����� �Ķ���� ����
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

    // Animator Ʈ���� ����
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

        _stateMachine = new StateMachine<MushroomController>(this, new MushroomIdleState());
        _stateMachine.AddState(new MushroomPatrolState());
        _stateMachine.AddState(new MushroomChaseState());
        _stateMachine.AddState(new MushroomAttackState());
        _stateMachine.AddState(new MushroomCooldownState());
        _stateMachine.AddState(new MushroomReturnState());
        _stateMachine.AddState(new MushroomDeadState());
        _stateMachine.AddState(new MushroomHitState());

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

    // �̵�
    public void MoveTo(Vector3 destination)
    {
        Vector3 dir = (destination - transform.position).normalized;
        _rigidbody.MovePosition(transform.position + dir * MoveSpeed * Time.fixedDeltaTime);
        transform.LookAt(new Vector3(destination.x, transform.position.y, destination.z));
    }

    // ���� ����
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

    // ���� �̵�
    public void SetRandomIdleTarget()
    {
        float x = Random.Range(MinX, MaxX);
        float z = Random.Range(MinZ, MaxZ);

        Vector3 origin = new Vector3(x, 20f, z); // ������ ��� Ray

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 50f, LayerMask.GetMask("Ground")))
        {
            IdleTarget = hit.point + Vector3.up * 0.1f;
        }
        else
        {
            IdleTarget = transform.position; // fallback
        }
    }

    // �ִϸ��̼� �̺�Ʈ�� ȣ���
    public void OnAttackAnimationEnd()
    {
        if (_stateMachine != null && _stateMachine.CurrentState is MushroomAttackState)
            _stateMachine.ChangeState<MushroomCooldownState>();
    }

    public void OnDeathAnimationEnd()
    {
        gameObject.SetActive(false);
        SpawningPool.Instance.MushroomDie(this);
    }

    public override void SetSpawnPoint(Vector3 spawnPos)
    {
        transform.position = spawnPos;
        SpawnPoint = spawnPos;
    }

    public override bool AnyDamage(float damage, GameObject damageCauser, Vector2 hitPoint = default)
    {
        if (_stateMachine.CurrentState is MushroomDeadState)
            return false;

        _currentHp -= damage;
        Instantiate(HitEffect, transform.position, Quaternion.identity, transform);

        if (_currentHp <= 0)
        {
            _stateMachine.ChangeState<MushroomDeadState>();
            GameManager.Instance.AddExp(30);
            return true;
        }

        Target = damageCauser.transform; // �÷��̾� ������

        _stateMachine.ChangeState<MushroomHitState>();

        return true;
    }
}
