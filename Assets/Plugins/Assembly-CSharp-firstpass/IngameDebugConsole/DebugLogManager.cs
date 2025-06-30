using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IngameDebugConsole
{
	public class DebugLogManager : MonoBehaviour
	{
		[Header("Properties")]
		private float minimumHeight = 200f;

		[SerializeField]
		private bool startInPopupMode = true;

		[Header("Visuals")]
		[SerializeField]
		private DebugLogItem logItemPrefab;

		[SerializeField]
		private Sprite infoLog;

		[SerializeField]
		private Sprite warningLog;

		[SerializeField]
		private Sprite errorLog;

		private Dictionary<LogType, Sprite> logSpriteRepresentations;

		private Color collapseButtonNormalColor = new Color(27f / 85f, 27f / 85f, 27f / 85f, 1f);

		private Color collapseButtonSelectedColor = new Color(0.4392157f, 37f / 85f, 37f / 85f, 1f);

		private Color filterButtonsNormalColor = new Color(27f / 85f, 27f / 85f, 27f / 85f, 1f);

		private Color filterButtonsSelectedColor = new Color(0.4392157f, 37f / 85f, 37f / 85f, 1f);

		[Header("Internal References")]
		public RectTransform canvasTR;

		[SerializeField]
		private RectTransform logWindowTR;

		[SerializeField]
		private RectTransform logItemsContainer;

		[SerializeField]
		private Image collapseButton;

		[SerializeField]
		private Image filterInfoButton;

		[SerializeField]
		private Image filterWarningButton;

		[SerializeField]
		private Image filterErrorButton;

		[SerializeField]
		private Text infoEntryCountText;

		[SerializeField]
		private Text warningEntryCountText;

		[SerializeField]
		private Text errorEntryCountText;

		[SerializeField]
		private GameObject snapToBottomButton;

		private int infoEntryCount;

		private int warningEntryCount;

		private int errorEntryCount;

		[SerializeField]
		private CanvasGroup logWindowCanvasGroup;

		private bool isLogWindowVisible = true;

		private bool screenDimensionsChanged;

		[SerializeField]
		private ScrollRect logItemsScrollRect;

		[SerializeField]
		private DebugLogRecycledListView recycledListView;

		private bool isCollapseOn;

		private DebugLogFilter logFilter = DebugLogFilter.All;

		private bool snapToBottom = true;

		private List<DebugLogEntry> collapsedLogEntries;

		private Dictionary<DebugLogEntry, int> collapsedLogEntriesMap;

		private DebugLogIndexList uncollapsedLogEntriesIndices;

		private DebugLogIndexList indicesOfListEntriesToShow;

		private List<DebugLogItem> pooledLogItems;

		private PointerEventData nullPointerEventData;

		public static DebugLogManager Instance { get; private set; }

		private void Awake()
		{
			Instance = this;
		}

		private void OnEnable()
		{
			pooledLogItems = new List<DebugLogItem>();
			logSpriteRepresentations = new Dictionary<LogType, Sprite>
			{
				{
					LogType.Log,
					infoLog
				},
				{
					LogType.Warning,
					warningLog
				},
				{
					LogType.Error,
					errorLog
				},
				{
					LogType.Exception,
					errorLog
				},
				{
					LogType.Assert,
					errorLog
				}
			};
			logSpriteRepresentations = new Dictionary<LogType, Sprite>
			{
				{
					LogType.Log,
					infoLog
				},
				{
					LogType.Warning,
					warningLog
				},
				{
					LogType.Error,
					errorLog
				},
				{
					LogType.Exception,
					errorLog
				},
				{
					LogType.Assert,
					errorLog
				}
			};
			filterInfoButton.color = filterButtonsSelectedColor;
			filterWarningButton.color = filterButtonsSelectedColor;
			filterErrorButton.color = filterButtonsSelectedColor;
			collapsedLogEntries = new List<DebugLogEntry>(128);
			collapsedLogEntriesMap = new Dictionary<DebugLogEntry, int>(128);
			uncollapsedLogEntriesIndices = new DebugLogIndexList();
			indicesOfListEntriesToShow = new DebugLogIndexList();
			recycledListView.Initialize(this, collapsedLogEntries, indicesOfListEntriesToShow, logItemPrefab.Transform.sizeDelta.y);
			recycledListView.UpdateItemsInTheList(true);
			nullPointerEventData = new PointerEventData(null);
			Application.logMessageReceived -= ReceivedLog;
			Application.logMessageReceived += ReceivedLog;
			if (minimumHeight < 200f)
			{
				minimumHeight = 200f;
			}
		}

		private void OnDisable()
		{
			Application.logMessageReceived -= ReceivedLog;
		}

		private void OnRectTransformDimensionsChange()
		{
			screenDimensionsChanged = true;
		}

		private void LateUpdate()
		{
			if (screenDimensionsChanged)
			{
				if (isLogWindowVisible)
				{
					recycledListView.OnViewportDimensionsChanged();
				}
				screenDimensionsChanged = false;
			}
			if (snapToBottom)
			{
				logItemsScrollRect.verticalNormalizedPosition = 0f;
			}
		}

		private void Update()
		{
			snapToBottom = false;
		}

		public char OnValidateCommand(string text, int charIndex, char addedChar)
		{
			if (addedChar == '\n')
			{
				if (text.Length > 0)
				{
					DebugLogConsole.ExecuteCommand(text);
					SetSnapToBottom(true);
				}
				return '\0';
			}
			return addedChar;
		}

		private void ReceivedLog(string logString, string stackTrace, LogType logType)
		{
			DebugLogEntry debugLogEntry = new DebugLogEntry(logString, stackTrace, null);
			int value;
			bool flag = collapsedLogEntriesMap.TryGetValue(debugLogEntry, out value);
			if (!flag)
			{
				debugLogEntry.logTypeSpriteRepresentation = logSpriteRepresentations[logType];
				value = collapsedLogEntries.Count;
				collapsedLogEntries.Add(debugLogEntry);
				collapsedLogEntriesMap[debugLogEntry] = value;
			}
			else
			{
				debugLogEntry = collapsedLogEntries[value];
				debugLogEntry.count++;
			}
			uncollapsedLogEntriesIndices.Add(value);
			Sprite logTypeSpriteRepresentation = debugLogEntry.logTypeSpriteRepresentation;
			if (isCollapseOn && flag)
			{
				if (isLogWindowVisible)
				{
					recycledListView.OnCollapsedLogEntryAtIndexUpdated(value);
				}
			}
			else if (logFilter == DebugLogFilter.All || (logTypeSpriteRepresentation == infoLog && (logFilter & DebugLogFilter.Info) == DebugLogFilter.Info) || (logTypeSpriteRepresentation == warningLog && (logFilter & DebugLogFilter.Warning) == DebugLogFilter.Warning) || (logTypeSpriteRepresentation == errorLog && (logFilter & DebugLogFilter.Error) == DebugLogFilter.Error))
			{
				indicesOfListEntriesToShow.Add(value);
				if (isLogWindowVisible)
				{
					recycledListView.OnLogEntriesUpdated(false);
				}
			}
			switch (logType)
			{
			case LogType.Log:
				infoEntryCount++;
				infoEntryCountText.text = infoEntryCount.ToString();
				break;
			case LogType.Warning:
				warningEntryCount++;
				warningEntryCountText.text = warningEntryCount.ToString();
				break;
			default:
				errorEntryCount++;
				errorEntryCountText.text = errorEntryCount.ToString();
				break;
			}
		}

		public void SetSnapToBottom(bool snapToBottom)
		{
			this.snapToBottom = snapToBottom;
		}

		public void ValidateScrollPosition()
		{
			logItemsScrollRect.OnScroll(nullPointerEventData);
		}

		public void Show()
		{
			recycledListView.OnLogEntriesUpdated(true);
			logWindowCanvasGroup.interactable = true;
			logWindowCanvasGroup.blocksRaycasts = true;
			logWindowCanvasGroup.alpha = 1f;
			isLogWindowVisible = true;
		}

		public void Hide()
		{
			logWindowCanvasGroup.interactable = false;
			logWindowCanvasGroup.blocksRaycasts = false;
			logWindowCanvasGroup.alpha = 0f;
			isLogWindowVisible = false;
		}

		public void ClearButtonPressed()
		{
			snapToBottom = true;
			infoEntryCount = 0;
			warningEntryCount = 0;
			errorEntryCount = 0;
			infoEntryCountText.text = "0";
			warningEntryCountText.text = "0";
			errorEntryCountText.text = "0";
			collapsedLogEntries.Clear();
			collapsedLogEntriesMap.Clear();
			uncollapsedLogEntriesIndices.Clear();
			indicesOfListEntriesToShow.Clear();
			recycledListView.DeselectSelectedLogItem();
			recycledListView.OnLogEntriesUpdated(true);
		}

		public void CollapseButtonPressed()
		{
			isCollapseOn = !isCollapseOn;
			snapToBottom = true;
			collapseButton.color = ((!isCollapseOn) ? collapseButtonNormalColor : collapseButtonSelectedColor);
			recycledListView.SetCollapseMode(isCollapseOn);
			FilterLogs();
		}

		public void FilterLogButtonPressed()
		{
			logFilter ^= DebugLogFilter.Info;
			if ((logFilter & DebugLogFilter.Info) == DebugLogFilter.Info)
			{
				filterInfoButton.color = filterButtonsSelectedColor;
			}
			else
			{
				filterInfoButton.color = filterButtonsNormalColor;
			}
			FilterLogs();
		}

		public void FilterWarningButtonPressed()
		{
			logFilter ^= DebugLogFilter.Warning;
			if ((logFilter & DebugLogFilter.Warning) == DebugLogFilter.Warning)
			{
				filterWarningButton.color = filterButtonsSelectedColor;
			}
			else
			{
				filterWarningButton.color = filterButtonsNormalColor;
			}
			FilterLogs();
		}

		public void FilterErrorButtonPressed()
		{
			logFilter ^= DebugLogFilter.Error;
			if ((logFilter & DebugLogFilter.Error) == DebugLogFilter.Error)
			{
				filterErrorButton.color = filterButtonsSelectedColor;
			}
			else
			{
				filterErrorButton.color = filterButtonsNormalColor;
			}
			FilterLogs();
		}

		private void FilterLogs()
		{
			if (logFilter == DebugLogFilter.None)
			{
				indicesOfListEntriesToShow.Clear();
			}
			else if (logFilter == DebugLogFilter.All)
			{
				if (isCollapseOn)
				{
					indicesOfListEntriesToShow.Clear();
					for (int i = 0; i < collapsedLogEntries.Count; i++)
					{
						indicesOfListEntriesToShow.Add(i);
					}
				}
				else
				{
					indicesOfListEntriesToShow.Clear();
					for (int j = 0; j < uncollapsedLogEntriesIndices.Count; j++)
					{
						indicesOfListEntriesToShow.Add(uncollapsedLogEntriesIndices[j]);
					}
				}
			}
			else
			{
				bool flag = (logFilter & DebugLogFilter.Info) == DebugLogFilter.Info;
				bool flag2 = (logFilter & DebugLogFilter.Warning) == DebugLogFilter.Warning;
				bool flag3 = (logFilter & DebugLogFilter.Error) == DebugLogFilter.Error;
				if (isCollapseOn)
				{
					indicesOfListEntriesToShow.Clear();
					for (int k = 0; k < collapsedLogEntries.Count; k++)
					{
						DebugLogEntry debugLogEntry = collapsedLogEntries[k];
						if (debugLogEntry.logTypeSpriteRepresentation == infoLog && flag)
						{
							indicesOfListEntriesToShow.Add(k);
						}
						else if (debugLogEntry.logTypeSpriteRepresentation == warningLog && flag2)
						{
							indicesOfListEntriesToShow.Add(k);
						}
						else if (debugLogEntry.logTypeSpriteRepresentation == errorLog && flag3)
						{
							indicesOfListEntriesToShow.Add(k);
						}
					}
				}
				else
				{
					indicesOfListEntriesToShow.Clear();
					for (int l = 0; l < uncollapsedLogEntriesIndices.Count; l++)
					{
						DebugLogEntry debugLogEntry2 = collapsedLogEntries[uncollapsedLogEntriesIndices[l]];
						if (debugLogEntry2.logTypeSpriteRepresentation == infoLog && flag)
						{
							indicesOfListEntriesToShow.Add(uncollapsedLogEntriesIndices[l]);
						}
						else if (debugLogEntry2.logTypeSpriteRepresentation == warningLog && flag2)
						{
							indicesOfListEntriesToShow.Add(uncollapsedLogEntriesIndices[l]);
						}
						else if (debugLogEntry2.logTypeSpriteRepresentation == errorLog && flag3)
						{
							indicesOfListEntriesToShow.Add(uncollapsedLogEntriesIndices[l]);
						}
					}
				}
			}
			recycledListView.DeselectSelectedLogItem();
			recycledListView.OnLogEntriesUpdated(true);
			ValidateScrollPosition();
		}

		public void PoolLogItem(DebugLogItem logItem)
		{
			logItem.gameObject.SetActive(false);
			pooledLogItems.Add(logItem);
		}

		public DebugLogItem PopLogItem()
		{
			DebugLogItem debugLogItem;
			if (pooledLogItems.Count > 0)
			{
				debugLogItem = pooledLogItems[pooledLogItems.Count - 1];
				pooledLogItems.RemoveAt(pooledLogItems.Count - 1);
				debugLogItem.gameObject.SetActive(true);
			}
			else
			{
				debugLogItem = Object.Instantiate(logItemPrefab, logItemsContainer, false);
				debugLogItem.Initialize(recycledListView);
			}
			return debugLogItem;
		}
	}
}
