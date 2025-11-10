using UnityEngine;
using System.Collections;
using System.IO;
using I2.Loc;

namespace INVELON
{
    /// <summary>
    /// Downloads a CSV file from a remote URL at runtime and imports its content into the specified I2 Localization asset.
    /// Supports configurable update modes and CSV separators.
    /// </summary>
    public class LocalizationCSVLoader : MonoBehaviour
    {
        [Header("Target I2 Localization Asset")]
        [Tooltip("Reference to the LanguageSourceAsset you want to update.")]
        public LanguageSourceAsset targetSourceAsset;

        [Header("CSV Download Settings")]
        [Tooltip("URL of the CSV file to download.")]
         private string csvUrl = "https://docs.google.com/spreadsheets/u/1/d/${id}/export?format=csv&id=${id}&gid=${gid}\r\n";
        [SerializeField] private string spreadsheetID;
        [SerializeField] private string spreadsheetGID;

        [Tooltip("Filename to save the downloaded CSV locally.")]
        [SerializeField] private string localFileName = "localization.csv";

        [Header("CSV Import Settings")]
        [Tooltip("Separator character used in the CSV file.")]
        [SerializeField] private char csvSeparator = ',';

        [Tooltip("Update mode for importing the CSV data.")]
        [SerializeField] private eSpreadsheetUpdateMode updateMode = eSpreadsheetUpdateMode.Replace;

        [Header("Debug Settings")]
        [Tooltip("Enable detailed debug logging.")]
        [SerializeField] private bool enableDebugLogging = true;


        public static LocalizationCSVLoader Instance { get; private set; }

        private string LocalFilePath => Path.Combine(Application.persistentDataPath, localFileName);

        private void Awake()
        {

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (targetSourceAsset == null || targetSourceAsset.mSource == null)
            {
                Debug.LogError("[LocalizationCSVLoader] LanguageSourceAsset is not assigned or invalid.");
                return;
            }

            StartCoroutine(DownloadAndImportCSV());
        }

        /// <summary>
        /// Downloads the CSV file and imports its content into the assigned LanguageSource.
        /// </summary>
        private IEnumerator DownloadAndImportCSV()
        {
            csvUrl = csvUrl.Replace("${id}", spreadsheetID).Replace("${gid}", spreadsheetGID);


            if (string.IsNullOrEmpty(csvUrl))
            {
                Debug.LogWarning("[LocalizationCSVLoader] CSV URL is not specified.");
                yield break;
            }

            if (enableDebugLogging)
            {
                Debug.Log($"[LocalizationCSVLoader] Starting download from: {csvUrl}");
            }

            yield return new WaitForSeconds(1f); // delay to ensure everything is initialized (from Unity side)

            using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(csvUrl))
            {
                var asynccall= www.SendWebRequest();

                while (!asynccall.isDone)
                {
                    yield return null;
                }

                if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[LocalizationCSVLoader] Error downloading CSV: {www.error}");
                    yield break;
                }

                string csvData = www.downloadHandler.text;

                try
                {
                    File.WriteAllText(LocalFilePath, csvData);
                    if (enableDebugLogging)
                        Debug.Log($"[LocalizationCSVLoader] CSV saved to: {LocalFilePath}");
                }
                catch (IOException ex)
                {
                    Debug.LogWarning($"[LocalizationCSVLoader] Failed to save CSV: {ex.Message}");
                }

                ImportCSVData(csvData);
            }
        }

        /// <summary>
        /// Imports CSV content into the selected LanguageSource.
        /// </summary>
        private void ImportCSVData(string csvData)
        {
            if (string.IsNullOrEmpty(csvData))
            {
                Debug.LogWarning("[LocalizationCSVLoader] Empty CSV data.");
                return;
            }

            if (targetSourceAsset?.mSource == null)
            {
                Debug.LogWarning("[LocalizationCSVLoader] Target source asset is not assigned.");
                return;
            }

            try
            {
                targetSourceAsset.mSource.Import_CSV(string.Empty, csvData, updateMode, csvSeparator);
                LocalizationManager.LocalizeAll();
                if (enableDebugLogging)
                    Debug.Log("[LocalizationCSVLoader] CSV imported successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[LocalizationCSVLoader] Failed to import CSV: {ex.Message}");
            }
        }

        /// <summary>
        /// Manually load CSV from persistent path if already downloaded.
        /// </summary>
        public void LoadCSVFromLocalFile()
        {
            if (!File.Exists(LocalFilePath))
            {
                Debug.LogWarning($"[LocalizationCSVLoader] Local CSV not found at: {LocalFilePath}");
                return;
            }

            try
            {
                string csvData = File.ReadAllText(LocalFilePath);
                ImportCSVData(csvData);
            }
            catch (IOException ex)
            {
                Debug.LogWarning($"[LocalizationCSVLoader] Error reading local CSV: {ex.Message}");
            }
        }
    }
}


