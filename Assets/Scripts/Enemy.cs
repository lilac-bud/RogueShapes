using UnityEngine;

public class Enemy : CellObject
{
    public int MaxHealth = 2;
    private int m_HealthPoint;

    private bool m_IsMoving;
    private Vector3 m_MoveTarget;
    public float MoveSpeed = 1.0f;

    private Animator m_Animator;
    private int m_AttackHash;
    private int m_MovingHash;
    private int m_DamagedHash;

    public int StartingStrengh = 10;
    public int StartingDefense = 0;
    private int m_StrenghPoint;
    private int m_DefensePoint;

    private AudioSource m_AudioSource;
    public AudioClip attackClip;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_MovingHash = Animator.StringToHash("Moving");
        m_AttackHash = Animator.StringToHash("Attack");
        m_DamagedHash = Animator.StringToHash("Damaged");
        m_AudioSource = GetComponent<AudioSource>();
        GameManager.Instance.TurnManager.OnTick += TurnHappened;
    }
    void Update()
    {
        if (m_IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_MoveTarget, MoveSpeed * Time.deltaTime);

            if (transform.position == m_MoveTarget)
            {
                m_IsMoving = false;
                m_Animator.SetBool(m_MovingHash, false);
                //m_AudioSource.Stop();
            }
        }
    }
    private void OnDestroy()
    {
        GameManager.Instance.TurnManager.OnTick -= TurnHappened;
    }
    public override void Init(Vector2Int cell)
    {
        base.Init(cell);
        if ((GameManager.Instance.CurrentLevel + 1) % 5 == 0)
        {
            MaxHealth++;
            StartingStrengh += 10;
            StartingDefense += 10;
        }
        m_HealthPoint = MaxHealth;
        m_StrenghPoint = StartingStrengh;
        m_DefensePoint = StartingDefense;
    }
    public override bool PlayerWantsToEnter()
    {
        GameManager.Instance.PlayerController.PlayAttack();
        m_Animator.SetTrigger(m_DamagedHash);
        int damage = GameManager.CalculateDamage(GameManager.Instance.PlayerStrengh, m_DefensePoint);
        m_HealthPoint -= damage;
        if (m_HealthPoint <= 0)
        {
            Destroy(gameObject);
        }
        return false;
    }
    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.BoardManager;
        var targetCell = board.GetCellData(coord);

        if (targetCell == null
            || !targetCell.Passable
            || targetCell.ContainedObject != null)
        {
            return false;
        }

        var currentCell = board.GetCellData(m_Cell);
        currentCell.ContainedObject = null;

        targetCell.ContainedObject = this;
        m_Cell = coord;
        m_IsMoving = true;
        m_MoveTarget = board.CellToWorld(coord);
        m_Animator.SetBool(m_MovingHash, m_IsMoving);
        //m_AudioSource.Play();

        return true;
    }
    void TurnHappened()
    {
        var playerCell = GameManager.Instance.PlayerController.Cell;

        int xDist = playerCell.x - m_Cell.x;
        int yDist = playerCell.y - m_Cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        if ((xDist == 0 && absYDist == 1)
            || (yDist == 0 && absXDist == 1))
        {
            m_Animator.SetTrigger(m_AttackHash);
            m_AudioSource.PlayOneShot(attackClip);
            int damage = GameManager.CalculateDamage(m_StrenghPoint, GameManager.Instance.PlayerDefense);
            GameManager.Instance.ChangeFood(-damage);
            GameManager.Instance.PlayerController.GetDamaged();
        }
        else
        {
            Vector2Int[] directions = new Vector2Int[4];
            int x_index, y_index;
            x_index = absXDist > absYDist ? 0 : 1;
            y_index = 1 - x_index;
            if (xDist > 0)
            {
                directions[x_index] = Vector2Int.right;
                directions[x_index + 2] = Vector2Int.left;
            }
            else
            {
                directions[x_index] = Vector2Int.left;
                directions[x_index + 2] = Vector2Int.right;
            }
            if (yDist > 0)
            {
                directions[y_index] = Vector2Int.up;
                directions[y_index + 2] = Vector2Int.down;
            }
            else
            {
                directions[y_index] = Vector2Int.down;
                directions[y_index + 2] = Vector2Int.up;
            }
            foreach (var direction in directions)
                if (MoveTo(m_Cell + direction))
                    break;         
        }
    }
}