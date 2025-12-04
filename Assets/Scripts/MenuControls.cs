using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuControls : MonoBehaviour
{
    public enum Menu { MainMenu, PauseManu };
    public Menu MenuType;

    private UIDocument m_UIDoc;
    private VisualElement m_MenuPanel;
    private Label m_TitleLabel;
    private Button m_PlayButton;
    private Button m_ExitButton;

    public AudioMixer AudioMixer;
    private Slider m_AudioSlider;

    public event System.Action OnResume;
    private void Awake()
    {
        m_UIDoc = GetComponent<UIDocument>();
    }
    private void OnEnable()
    {
        m_MenuPanel = m_UIDoc.rootVisualElement.Q<VisualElement>("MenuPanel");
        m_TitleLabel = m_UIDoc.rootVisualElement.Q<Label>("TitleLabel");
        m_PlayButton = m_UIDoc.rootVisualElement.Q<Button>("PlayButton");
        m_ExitButton = m_UIDoc.rootVisualElement.Q<Button>("ExitButton");
        m_AudioSlider = m_UIDoc.rootVisualElement.Q<Slider>("AudioSlider");
        m_ExitButton.clicked += OnExitClicked;
        m_AudioSlider.RegisterValueChangedCallback(OnAudioChanged);
        switch(MenuType)
        {
            case Menu.MainMenu:
                m_TitleLabel.text = "Rogue Shapes";
                m_PlayButton.text = "Play";
                m_PlayButton.clicked += OnPlayClicked;
                break;
            case Menu.PauseManu:
                m_TitleLabel.text = "Pause";
                m_PlayButton.text = "Resume";
                m_PlayButton.clicked += OnResumeClicked;
                HideMenu();
                break;
            default:
                break;
        }
    }
    private void OnPlayClicked()
    {
        SceneManager.LoadScene("Main");
    }
    private void OnResumeClicked()
    {
        OnResume?.Invoke();
        HideMenu();
    }
    private void OnExitClicked()
    {
        Debug.Log("Exit pressed!");
        Application.Quit();
    }
    private void OnAudioChanged(ChangeEvent<float> evt)
    {
        AudioMixer.SetFloat("masterVolume", evt.newValue);
    }
    public void ShowMenu()
    {
        m_MenuPanel.style.visibility = Visibility.Visible;
    }
    public void HideMenu()
    {
        m_MenuPanel.style.visibility = Visibility.Hidden;
    }
}