using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BoardManager BoardManager;
    public PlayerController PlayerController;
    public TurnManager TurnManager { get; private set; }
    private int m_FoodAmount;
    public UIDocument UIDoc;
    private Label m_FoodLabel;
    private Label m_StrenghLabel;
    private Label m_DefenseLabel;
    private VisualElement m_GameOverPanel;
    private Label m_GameOverMessage;
    private Label m_SummaryMessage;
    private VisualElement m_TutorialPanel;
    private Label m_TutorialMessage;

    public int CurrentLevel { get; private set; }
    public int PlayerStrengh {  get; private set; }
    public int PlayerDefense { get; private set; }
    static public int HalfDamageReductionRequirement = 30;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;
        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        m_StrenghLabel = UIDoc.rootVisualElement.Q<Label>("StrenghLabel");
        m_DefenseLabel = UIDoc.rootVisualElement.Q<Label>("DefenseLabel");
        m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");
        m_SummaryMessage = m_GameOverPanel.Q<Label>("SummaryMessage");
        m_TutorialPanel = UIDoc.rootVisualElement.Q<VisualElement>("TutorialPanel");
        m_TutorialMessage = m_TutorialPanel.Q<Label>("TutorialMessage");
        m_TutorialMessage.text = "Use WASD or Arrows to move around\n\nTo skip a turn press Enter";
        Button continueButton = UIDoc.rootVisualElement.Q<Button>("ContinueButton");
        continueButton.clicked += StartNewGame;
        Button exitButton = UIDoc.rootVisualElement.Q<Button>("ExitButton");
        exitButton.clicked += ExitGame;
        StartNewGame();
    }
    public void NewLevel()
    {
        BoardManager.Clean();
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));

        CurrentLevel++;
    }
    public void StartNewGame()
    {
        m_GameOverPanel.style.visibility = Visibility.Hidden;

        CurrentLevel = 0;
        m_FoodAmount = 30;
        PlayerStrengh = 10;
        PlayerDefense = 0;
        m_FoodLabel.text = "Energy : " + m_FoodAmount;
        m_StrenghLabel.text = "Strengh : " + PlayerStrengh;
        m_DefenseLabel.text = "Defense : " + PlayerDefense;

        NewLevel();
        PlayerController.Init();
        if (m_TutorialPanel.style.visibility != Visibility.Hidden)
            StartCoroutine(FadeTutorialPanel());
    }
    IEnumerator FadeTutorialPanel()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        for (float opacity = 1.0f; opacity >= 0.0f; opacity -= 0.01f)
        {
            m_TutorialPanel.style.opacity = opacity;
            yield return null;
        }
        m_TutorialPanel.style.visibility = Visibility.Hidden;
    }
    void OnTurnHappen()
    {
        ChangeFood(-1);
    }
    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;
        if (m_FoodAmount <= 0)
        {
            m_FoodLabel.text = "Energy : " + 0;
            PlayerController.GameOver();
            m_GameOverPanel.style.visibility = Visibility.Visible;
            m_GameOverMessage.text = "Game Over!";
            m_SummaryMessage.text = "You traveled through " + CurrentLevel + " level(s)";
        }
        else
            m_FoodLabel.text = "Energy : " + m_FoodAmount;
    }
    public void ChangeStrengh(int amount)
    {
        PlayerStrengh += amount;
        m_StrenghLabel.text = "Strengh : " + PlayerStrengh;
    }
    public void ChangeDefense(int amount)
    {
        PlayerDefense += amount;
        m_DefenseLabel.text = "Defense : " + PlayerDefense;
    }
    static public int CalculateDamage(int strengh, int defense)
    {
        return Mathf.RoundToInt(1.0f / (1.0f + (float)defense / (float)HalfDamageReductionRequirement) * (float)strengh / 10.0f);
    }
    private void ExitGame()
    {
        Debug.Log("Exit pressed!");
        Application.Quit();
    }
}