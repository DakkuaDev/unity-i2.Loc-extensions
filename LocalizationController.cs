using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

/// <summary>
/// Controller for handling I2 Localization functionality across the application.
/// Provides static methods for language switching and term translation.
/// </summary>
public class LocalizationController : MonoBehaviour
{
    private static LocalizationController _instance;

    [SerializeField] private string defaultLanguage = "English";
    [SerializeField] private bool loadLanguageFromPlayerPrefs = true;
    [SerializeField] private string playerPrefsKey = "SelectedLanguage";

    public static event System.Action<string> OnLanguageChanged;

    public static LocalizationController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<LocalizationController>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject("LocalizationController");
                    _instance = obj.AddComponent<LocalizationController>();
                    DontDestroyOnLoad(obj);
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize localization
        Initialize();
    }

    private void Initialize()
    {
        if (loadLanguageFromPlayerPrefs && PlayerPrefs.HasKey(playerPrefsKey))
        {
            string savedLanguage = PlayerPrefs.GetString(playerPrefsKey);
            SetLanguage(savedLanguage);
        }
        else
        {
            SetLanguage(defaultLanguage);
        }
    }

    /// <summary>
    /// Changes the current language and saves it to PlayerPrefs
    /// </summary>
    /// <param name="languageCode">The language code or name to switch to</param>
    /// <returns>True if language was changed successfully</returns>
    public bool SetLanguage(string languageCode)
    {
        if (string.IsNullOrEmpty(languageCode) || !IsLanguageSupported(languageCode))
        {
            Debug.LogWarning($"Language '{languageCode}' is not supported or invalid");
            return false;
        }

        LocalizationManager.CurrentLanguage = languageCode;

        // Save to PlayerPrefs
        PlayerPrefs.SetString(Instance.playerPrefsKey, languageCode);
        PlayerPrefs.Save();

        Debug.Log($"Language changed to: {languageCode}");

        // Trigger event
        OnLanguageChanged?.Invoke(languageCode);


        return true;
    }

    /// <summary>
    /// Gets the current language code
    /// </summary>
    /// <returns>Current language code</returns>
    public string GetCurrentLanguage()
    {
        return LocalizationManager.CurrentLanguage;
    }

    /// <summary>
    /// Checks if a language is supported by the I2 Localization system
    /// </summary>
    /// <param name="languageCode">Language code or name to check</param>
    /// <returns>True if language is supported</returns>
    public bool IsLanguageSupported(string languageCode)
    {
        return LocalizationManager.GetAllLanguages().Contains(languageCode);
    }

    /// <summary>
    /// Get all available languages
    /// </summary>
    /// <returns>List of available language codes</returns>
    public List<string> GetAvailableLanguages()
    {
        return LocalizationManager.GetAllLanguages();
    }

    /// <summary>
    /// Translates a term using I2 Localization
    /// </summary>
    /// <param name="term">The term key to translate</param>
    /// <returns>Translated text</returns>
    public object Translate(string term)
    {
        // Validate term
        if (string.IsNullOrEmpty(term))
        {
            Debug.LogWarning($"Translation term is null or empty: {term}");
            return string.Empty;
        }

        // Check if term exists
        if (!LocalizationManager.TryGetTranslation(term, out var translation))
        {
            Debug.LogWarning($"Translation term not found: {term}");
            return term;
        }

        return translation;
    }

    private void Update()
    {
#if UNITY_EDITOR
        // Si pulso F12 llamo a ChangeLanguage

        if (Input.GetKeyDown(KeyCode.F12))
        {
            ChangeLanguage();
        }
#endif
    }

    // TODO: This need to be called for a GUI or UI
    // Now it is just a debug method to change language in the editor 

    public void ChangeLanguage()
    {

        var languages = GetAvailableLanguages();
        if (languages.Count == 0)
        {
            Debug.LogWarning("No languages available in I2 Localization.");
            return;
        }

        foreach (var language in languages)
        {
            if (language == GetCurrentLanguage())
            {
                int nextIndex = (languages.IndexOf(language) + 1) % languages.Count;
                SetLanguage(languages[nextIndex]);

                return;
            }
        }
    }
}

