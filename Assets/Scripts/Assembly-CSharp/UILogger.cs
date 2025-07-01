using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

public class UILogger : MonoBehaviour
{
    private static UILogger _instance;
    public static UILogger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UILogger>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    _instance = obj.AddComponent<UILogger>();
                }
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }
    public Text _uiText;
#if TMP_PRESENT
    private TMP_Text _tmpText;
#endif
    private string _log = "";
    private const string LoggerObjectName = "UILoggerCanvas";
    public ScrollRect _scrollRect;
    public Canvas canvas;
    public InputField input;

    void Awake()
    {
        InitUI();
        Application.logMessageReceived += HandleLog;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            Application.logMessageReceived -= HandleLog;
        }
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string color = "white";
        switch (type)
        {
            case LogType.Warning: color = "yellow"; break;
            case LogType.Error:
            case LogType.Exception: color = "red"; break;
        }
        string msg = $"<color={color}>[{type}] {logString}</color>";
        if (type == LogType.Exception)
            msg += $"\n<color=grey>{stackTrace}</color>";
        AppendLog(msg);
    }
    public GameObject scrollViewObj;
    public GameObject viewportObj;
    public RectTransform viewportRect;
    public GameObject contentObj;
    private void InitUI()
    {
#if TMP_PRESENT
        _tmpText = textObj.AddComponent<TMP_Text>();
        _tmpText.fontSize = 20;
        _tmpText.color = Color.white;
        _tmpText.alignment = TextAlignmentOptions.TopLeft;
        _tmpText.text = "";
        _tmpText.richText = true;
        _tmpText.horizontalAlignment = HorizontalAlignmentOptions.Left;
        _tmpText.verticalAlignment = VerticalAlignmentOptions.Top;
#else
        _uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _uiText.fontSize = 20;
        _uiText.color = Color.white;
        _uiText.alignment = TextAnchor.UpperLeft;
        _uiText.text = "";
        _uiText.supportRichText = true;
        _uiText.horizontalOverflow = HorizontalWrapMode.Wrap;
        _uiText.verticalOverflow = VerticalWrapMode.Overflow;
#endif
        
        _scrollRect.viewport = viewportRect;
        _scrollRect.horizontal = false;
        _scrollRect.vertical = true;
        _scrollRect.scrollSensitivity = 10f;
    }

    public void Log(string message)
    {
        if (_instance == null)
        {
            GameObject loggerObj = new GameObject("UILogger");
            _instance = loggerObj.AddComponent<UILogger>();
        }
        _instance.AppendLog(message);
    }

    private void AppendLog(string message)
    {
        _log += message + "\n";
#if TMP_PRESENT
        if (_tmpText != null) 
        {
            _tmpText.text = _log;
            Canvas.ForceUpdateCanvases();
            if (_scrollRect != null)
                _scrollRect.verticalNormalizedPosition = 0f;
        }
#else
        if (_uiText != null) 
        {
            _uiText.text = _log;
            Canvas.ForceUpdateCanvases();
            if (_scrollRect != null)
                _scrollRect.verticalNormalizedPosition = 0f;
        }
#endif
    }

    public static void Clear()
    {
        if (_instance != null)
        {
            _instance._log = "";
#if TMP_PRESENT
            if (_instance._tmpText != null) _instance._tmpText.text = "";
#else
            if (_instance._uiText != null) _instance._uiText.text = "";
#endif
        }
    }
    public void Show()
    {
        scrollViewObj.SetActive(true);
        input.gameObject.SetActive(false);
    }
    public void Hide() {
        scrollViewObj.SetActive(false);
        input.gameObject.SetActive(true);
        input.text = _uiText.text;
    }
} 