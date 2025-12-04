using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    private BoardManager m_Board;
    private Vector2Int m_CellPosition;
    public Vector2Int Cell;
    public InputAction MoveAction;
    public InputAction StartNewGameAction;
    public InputAction NextTurnAction;
    public InputAction PauseGameAction;
    readonly float inputDelay = 0.5f;
    float inputDelayTimer = 0.0f;

    private bool m_IsGameOver;

    private Animator m_Animator;
    private bool m_IsMoving;
    private Vector3 m_MoveTarget;
    private Vector2 moveDirection = new(0, -1);
    public float MoveSpeed = 1.0f;
    private int m_MovingHash;
    private int m_AttackingHash;
    private int m_BeingDamagedHash;

    private AudioSource m_AudioSource;
    public AudioClip attackClip;

    public MenuControls PauseMenu;
    private bool m_IsGamePaused;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_MovingHash = Animator.StringToHash("Moving");
        m_AttackingHash = Animator.StringToHash("Attacking");
        m_BeingDamagedHash = Animator.StringToHash("BeingDamaged");
        m_AudioSource = GetComponent<AudioSource>();
        PauseMenu.OnResume += UnpauseGame;
    }
    public void Init()
    {
        m_IsGameOver = false;
        m_IsGamePaused = false;
    }
    void Start()
    {
        MoveAction.Enable();
        StartNewGameAction.Enable();
        NextTurnAction.Enable();
        PauseGameAction.Enable();
    }
    public void GameOver()
    {
        m_IsGameOver = true;
    }
   void Update()
    {
        if (m_IsGameOver)
            if (StartNewGameAction.WasPressedThisFrame())
                GameManager.Instance.StartNewGame();
            else return;
        if (m_IsGamePaused)
        {
            if (PauseGameAction.WasPressedThisFrame())
            {
                m_IsGamePaused = false;
                PauseMenu.HideMenu();
            }
            return;
        }
        if (PauseGameAction.WasPressedThisFrame())
            {
                m_IsGamePaused = true;
                PauseMenu.ShowMenu();
                return;
            }
        if (m_IsMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_MoveTarget, MoveSpeed * Time.deltaTime);

                if (transform.position == m_MoveTarget)
                {
                    m_IsMoving = false;
                    m_Animator.SetBool(m_MovingHash, false);
                    m_AudioSource.Stop();
                    var cellData = m_Board.GetCellData(m_CellPosition);
                    if (cellData.ContainedObject != null)
                        cellData.ContainedObject.PlayerEntered();
                    NextTurn();
                }
                return;
            }
        if (NextTurnAction.WasPressedThisFrame())
        {
            NextTurn();
            return;
        }
        Vector2Int move = Vector2Int.CeilToInt(MoveAction.ReadValue<Vector2>());
        if (move.magnitude > 0)
        {
            if (inputDelayTimer > 0)
            {
                inputDelayTimer -= Time.deltaTime;
                return;
            }
            Vector2Int newCellTarget = m_CellPosition + move;
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);
            if (cellData != null && cellData.Passable)
            {
                if (cellData.ContainedObject == null || cellData.ContainedObject.PlayerWantsToEnter())
                {
                    moveDirection.Set(move.x, move.y);
                    moveDirection.Normalize();
                    MoveTo(newCellTarget, false);
                }
                else
                    NextTurn();
            }
            inputDelayTimer = inputDelay;
            return;
        }
        inputDelayTimer = 0.0f;
    }

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        MoveTo(cell, true);
    }
    public void MoveTo(Vector2Int cell, bool immediate)
    {
        m_CellPosition = cell;
        Cell = cell;

        if (immediate)
        {
            m_IsMoving = false;
            transform.position = m_Board.CellToWorld(m_CellPosition);
        }
        else
        {
            m_IsMoving = true;
            m_Animator.SetFloat("Look X", moveDirection.x);
            m_Animator.SetFloat("Look Y", moveDirection.y);
            m_AudioSource.Play();
            m_MoveTarget = m_Board.CellToWorld(m_CellPosition);
        }

        m_Animator.SetBool(m_MovingHash, m_IsMoving);
    }

    public void GetDamaged()
    {
        m_Animator.SetTrigger(m_BeingDamagedHash);
    }
    public void PlayAttack()
    {
        m_Animator.SetTrigger(m_AttackingHash);
        PlaySound(attackClip);
    }
    public void NextTurn()
    {
        GameManager.Instance.TurnManager.Tick();
    }
    public void PlaySound(AudioClip clip)
    {
        m_AudioSource.PlayOneShot(clip);
    }
    private void UnpauseGame()
    {
        m_IsGamePaused = false;
    }
}