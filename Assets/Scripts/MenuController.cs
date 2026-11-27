using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public sealed class MenuController : MonoBehaviour
{
    [Header("Paneles: 0 Inicio, 1 Ajustes, 2 Puntuaciones, 3 Creditos")]
    public GameObject[] panels;

    [Header("Botones: 0 Jugar, 1 Ajustes, 2 Puntuaciones, 3 Creditos, 4 Salir")]
    public Button[] menuButtons;

    [Header("Botones Volver")]
    public Button[] backButtons;

    [Header("Controles de Ajustes")]
    public TMP_InputField playerNameField;
    public TMP_Dropdown qualityDropdown;
    public Slider brightnessSlider;
    public Toggle fullscreenToggle;
    public Image brightnessOverlay;

    [Header("Escena de juego")]
    public string gameSceneName = "GameScene";

    private static readonly string[] QualityNames = { "Low", "Medium", "High", "Ultra" };

    private void Awake()
    {
        ConnectMenuButtons();
        ConnectBackButtons();
        ConnectSettingsControls();
        ShowPanel(0);
    }

    private void ConnectMenuButtons()
    {
        if (menuButtons == null)
            return;

        if (menuButtons.Length > 0 && menuButtons[0] != null)
            menuButtons[0].onClick.AddListener(LoadGameScene);

        for (int index = 1; index <= 3 && index < menuButtons.Length; index++)
        {
            if (menuButtons[index] == null)
                continue;

            int panelIndex = index;
            menuButtons[index].onClick.AddListener(() => ShowPanel(panelIndex));
        }

        if (menuButtons.Length > 4 && menuButtons[4] != null)
            menuButtons[4].onClick.AddListener(QuitGame);
    }

    private void ConnectBackButtons()
    {
        if (backButtons == null)
            return;

        foreach (Button backButton in backButtons)
        {
            if (backButton != null)
                backButton.onClick.AddListener(() => ShowPanel(0));
        }
    }

    private void ConnectSettingsControls()
    {
        if (qualityDropdown != null)
        {
            qualityDropdown.onValueChanged.AddListener(ApplyQuality);
            ApplyQuality(qualityDropdown.value);
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.AddListener(ApplyBrightness);
            ApplyBrightness(brightnessSlider.value);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(ApplyFullscreen);
            ApplyFullscreen(fullscreenToggle.isOn);
        }
    }

    public void ShowPanel(int panelIndex)
    {
        if (panels == null || panels.Length == 0)
            return;

        for (int index = 0; index < panels.Length; index++)
        {
            if (panels[index] != null)
                panels[index].SetActive(index == panelIndex);
        }
    }

    public void ApplyQuality(int dropdownValue)
    {
        if (QualitySettings.names == null || QualitySettings.names.Length == 0)
            return;

        int option = Mathf.Clamp(dropdownValue, 0, QualityNames.Length - 1);
        int qualityLevel = Array.FindIndex(
            QualitySettings.names,
            name => string.Equals(name, QualityNames[option], StringComparison.OrdinalIgnoreCase));

        if (qualityLevel < 0)
        {
            float normalizedValue = option / (float)(QualityNames.Length - 1);
            qualityLevel = Mathf.RoundToInt(normalizedValue * (QualitySettings.names.Length - 1));
        }

        QualitySettings.SetQualityLevel(qualityLevel, true);
    }

    public void ApplyBrightness(float brightness)
    {
        if (brightnessOverlay == null)
            return;

        Color overlayColor = brightnessOverlay.color;
        overlayColor.r = 0f;
        overlayColor.g = 0f;
        overlayColor.b = 0f;
        overlayColor.a = 1f - Mathf.Clamp01(brightness);
        brightnessOverlay.color = overlayColor;
    }

    public void ApplyFullscreen(bool isFullscreen)
    {
        Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.fullScreen = isFullscreen;
    }

    private void LoadGameScene()
    {
        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogError("MenuController: asigna el nombre de la escena de juego.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            Debug.LogError("MenuController: la escena configurada no está incluida en Build Settings: " + gameSceneName);
            return;
        }

        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    private static void QuitGame()
    {
        Application.Quit();
    }
}
