using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using System.Linq;
public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Button closeButton;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActions;
    
    private InputAction toggleMenuAction;
    private bool IsMenuOpen = false;
    
    private Resolution[] availableResolutions;
    private List<Resolution> filteredResolutions;
    private int currentResolutionIndex = 0;
    
    private const string VOLUME_PREF_KEY = "MasterVolume";
    private const string RESOLUTION_PREF_KEY = "ScreenResolutionIndex";
    private const string FULLSCREEN_PREF_KEY = "FullscreenPreference";


    void Awake()
    {
        var uiActionMap = inputActions.FindActionMap("UI");
        if (uiActionMap == null)
        {
            Debug.LogError("Action Map 'UI' not found in the Input Action Asset.");
            return;
        }
        toggleMenuAction = uiActionMap.FindAction("TogglePauseMenu"); // Используйте имя вашего Action
        if (toggleMenuAction == null)
        {
            Debug.LogError("Action 'TogglePauseMenu' not found in the 'UI' Action Map.");
            return;
        }

        toggleMenuAction.performed += OnToggleMenuPerformed;
        
        availableResolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();
        
        var currentRefreshRate = Screen.currentResolution.refreshRateRatio;
        HashSet<string> resolutionStrings = new HashSet<string>();
        
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            // Опционально: фильтр по частоте обновления
            // if (availableResolutions[i].refreshRate == currentRefreshRate)
            // {
            string resString = $"{availableResolutions[i].width} x {availableResolutions[i].height}";
            if (resolutionStrings.Add(resString)) // Добавляем, только если такого разрешения еще не было
            {
                filteredResolutions.Add(availableResolutions[i]);
            }
            // }
        }
        
        List<string> options = filteredResolutions.Select(r => $"{r.width} x {r.height}").ToList();
        resolutionDropdown.AddOptions(options);
        
            // --- Загрузка Настроек ---
            LoadSettings();
            
            // --- Привязка UI событий ---
            volumeSlider.onValueChanged.AddListener(SetVolume);
            resolutionDropdown.onValueChanged.AddListener(SetResolutionDropdownIndex);
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseMenu); // Закрытие по кнопке
            }
            // Скрыть панель при старте
            settingsPanel.SetActive(false);
    }

    void OnEnable()
    {
        toggleMenuAction?.Enable();
    }
    void OnDisable()
    {
        toggleMenuAction?.Disable();
        // Важно отписаться, чтобы избежать утечек памяти
        toggleMenuAction.performed -= OnToggleMenuPerformed;

        // Отписка от UI событий
        volumeSlider.onValueChanged.RemoveListener(SetVolume);
        resolutionDropdown.onValueChanged.RemoveListener(SetResolutionDropdownIndex);
        fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseMenu);
        }
    }
    private void OnToggleMenuPerformed(InputAction.CallbackContext obj)
    {
        ToggleMenu();
    }

    private void ToggleMenu()
    {
        IsMenuOpen = !IsMenuOpen;
        settingsPanel.SetActive(IsMenuOpen);
        
        // Опционально: Пауза игры
       Time.timeScale = IsMenuOpen ? 0f : 1f;
        
       // Опционально: Показать/скрыть курсор
       //Cursor.visible = IsMenuOpen;
       //Cursor.lockState = IsMenuOpen ? CursorLockMode.None : CursorLockMode.Locked; // Или ваш режим по умолчанию

       if (!IsMenuOpen)
       {
           ApplyAndSaveChanges();
       }
       else
       {
           // При открытии обновляем UI до загруженных значений,
           // на случай если что-то изменилось извне
           LoadSettings();
           UpdateUIElements();
       }
    }
    public void CloseMenu()
    {
        if (IsMenuOpen)
        {
            ToggleMenu(); // Используем общую логику переключения
        }
    }
    
    // --- Методы Настроек ---
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume; // Глобальная громкость
        // Или используйте AudioMixer для более тонкой настройки групп звуков
        // audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20); // Пример для AudioMixer
        Debug.Log($"Volume set to: {volume}");
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log($"Fullscreen set to: {isFullscreen}");
        // При изменении fullscreen может потребоваться переустановить разрешение
        ApplyResolution();
    }

    private void ApplyResolution()
    {
        if (currentResolutionIndex >= 0 && currentResolutionIndex < filteredResolutions.Count)
        {
            Resolution resolution = filteredResolutions[currentResolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            Debug.Log($"Resolution applied: {resolution.width}x{resolution.height}, Fullscreen: {Screen.fullScreen}");
        }
        else
        {
            Debug.LogWarning($"Invalid resolution index: {currentResolutionIndex}");
        }
    }

    public void SetResolutionDropdownIndex(int resolutionIndex)
    {
        currentResolutionIndex = resolutionIndex;
        // Не применяем сразу, ждем закрытия меню или нажатия "Применить"
        Debug.Log($"Resolution index changed to: {resolutionIndex}");
    }
    
    
    
    
    // --- Сохранение и Загрузка ---
    private void ApplyAndSaveChanges()
    {
        // Применяем разрешение перед сохранением
        ApplyResolution();
        
        // Сохраняем текущие значения
        PlayerPrefs.SetFloat(VOLUME_PREF_KEY, volumeSlider.value);
        PlayerPrefs.SetInt(RESOLUTION_PREF_KEY, currentResolutionIndex);
        PlayerPrefs.SetInt(FULLSCREEN_PREF_KEY, fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.Save(); // Сохраняем изменения на диск
        Debug.Log("Settings Saved.");
    }

    private void LoadSettings()
    {
        // Загружаем громкость
        float volume = PlayerPrefs.GetFloat(VOLUME_PREF_KEY, 0.75f); // 0.75f - значение по умолчанию
        SetVolume(volume); // Применяем сразу

        // Загружаем полноэкранный режим
        bool isFullscreen = PlayerPrefs.GetInt(FULLSCREEN_PREF_KEY, Screen.fullScreen ? 1 : 0) == 1; // По умолчанию - текущее состояние
        SetFullscreen(isFullscreen); // Применяем сразу

        // Загружаем индекс разрешения
        currentResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_PREF_KEY, GetCurrentResolutionIndex()); // По умолчанию - текущее разрешение

        Debug.Log("Settings Loaded.");

        // Обновляем UI элементы
        UpdateUIElements();
    }
    private void UpdateUIElements()
    {
        // Обновляем UI в соответствии с загруженными/текущими значениями
        volumeSlider.value = AudioListener.volume;
        fullscreenToggle.isOn = Screen.fullScreen;

        // Устанавливаем выбранное разрешение в Dropdown
        if (currentResolutionIndex >= 0 && currentResolutionIndex < resolutionDropdown.options.Count)
        {
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue(); // Обновить отображаемое значение
        }
        else
        {
            resolutionDropdown.value = GetCurrentResolutionIndex(); // Ставим текущее, если сохраненное невалидно
            resolutionDropdown.RefreshShownValue();
        }
    }
    private int GetCurrentResolutionIndex()
    {
        Resolution currentRes = Screen.currentResolution;
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            if (filteredResolutions[i].width == currentRes.width &&
                filteredResolutions[i].height == currentRes.height)
            {
                return i;
            }
        }
        return 0; // Возвращаем 0, если не найдено (или лучше найти ближайшее)
    }
}
