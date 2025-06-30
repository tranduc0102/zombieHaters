using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EPPZ.Cloud.Scenes
{
	public class Controller : MonoBehaviour
	{
		[Serializable]
		public class Elements
		{
			public InputField nameLabel;

			public Animation nameLabelAnimation;

			[Space]
			public Toggle soundToggle;

			public Animation soundToggleAnimation;

			[Space]
			public Slider volumeSlider;

			public Animation volumeSliderAnimation;

			[Space]
			public Dropdown levelDropdown;

			public Animation levelDropdownAnimation;

			[Space]
			public Toggle firstTrophyToggle;

			public Animation firstTrophyToggleAnimation;

			[Space]
			public Toggle secondTrophyToggle;

			public Animation secondTrophyToggleAnimation;

			[Space]
			public Toggle thirdTrophyToggle;

			public Animation thirdTrophyToggleAnimation;
		}

		public Elements elements;

		private void Start()
		{
			Cloud.onCloudChange = (Cloud.OnCloudChange)Delegate.Combine(Cloud.onCloudChange, new Cloud.OnCloudChange(OnCloudChange));
			AddElementUpdatingActions();
			PopulateElementsFromCloud();
		}

		private void OnDestroy()
		{
			Cloud.onCloudChange = (Cloud.OnCloudChange)Delegate.Remove(Cloud.onCloudChange, new Cloud.OnCloudChange(OnCloudChange));
		}

		private void PopulateElementsFromCloud()
		{
			elements.nameLabel.text = Cloud.StringForKey("name");
			elements.soundToggle.isOn = Cloud.BoolForKey("sound");
			elements.volumeSlider.value = Cloud.FloatForKey("volume");
			elements.levelDropdown.value = Cloud.IntForKey("level");
			elements.firstTrophyToggle.isOn = Cloud.BoolForKey("firstTrophy");
			elements.secondTrophyToggle.isOn = Cloud.BoolForKey("secondTrophy");
			elements.thirdTrophyToggle.isOn = Cloud.BoolForKey("thirdTrophy");
		}

		private Cloud.Should OnCloudChange(string[] changedKeys, ChangeReason changeReason)
		{
			switch (changeReason)
			{
			case ChangeReason.InitialSyncChange:
				PopulateElementsFromCloud();
				return Cloud.Should.StopUpdateKeys;
			case ChangeReason.QuotaViolationChange:
				return Cloud.Should.StopUpdateKeys;
			case ChangeReason.AccountChange:
				PopulateElementsFromCloud();
				return Cloud.Should.StopUpdateKeys;
			default:
				return Cloud.Should.UpdateKeys;
			}
		}

		public void OnNameInputFieldEndEdit(string text)
		{
			Cloud.SetStringForKey(text, "name");
			Cloud.Synchrnonize();
		}

		public void OnSoundToggleValueChanged(bool isOn)
		{
			Cloud.SetBoolForKey(isOn, "sound");
			Cloud.Synchrnonize();
		}

		public void OnVolumeSliderEndDrag(BaseEventData eventData)
		{
			Cloud.SetFloatForKey(elements.volumeSlider.value, "volume");
			Cloud.Synchrnonize();
		}

		public void OnLevelDropDownValueChanged(int value)
		{
			Cloud.SetIntForKey(value, "level");
			Cloud.Synchrnonize();
		}

		public void OnFirstTrophyToggleValueChanged(bool isOn)
		{
			Cloud.SetBoolForKey(isOn, "firstTrophy");
			Cloud.Synchrnonize();
		}

		public void OnSecondTrophyToggleValueChanged(bool isOn)
		{
			Cloud.SetBoolForKey(isOn, "secondTrophy");
			Cloud.Synchrnonize();
		}

		public void OnThirdTrophyToggleValueChanged(bool isOn)
		{
			Cloud.SetBoolForKey(isOn, "thirdTrophy");
			Cloud.Synchrnonize();
		}

		public void OnConflictResolutionToggleValueChanged(bool isOn)
		{
			if (isOn)
			{
				AddConflictResolvingActions();
			}
			else
			{
				RemoveConflictResolvingActions();
			}
		}

		private void AddElementUpdatingActions()
		{
			Cloud.OnKeyChange("name", delegate(string value)
			{
				elements.nameLabel.text = value;
				elements.nameLabelAnimation.Play("Blink");
			}, 2);
			Cloud.OnKeyChange("sound", delegate(bool value)
			{
				elements.soundToggle.isOn = value;
				elements.soundToggleAnimation.Play("Blink");
			}, 2);
			Cloud.OnKeyChange("volume", delegate(float value)
			{
				elements.volumeSlider.value = value;
				elements.volumeSliderAnimation.Play("Blink");
			}, 2);
			Cloud.OnKeyChange("level", delegate(int value)
			{
				elements.levelDropdown.value = value;
				elements.levelDropdownAnimation.Play("Blink");
			}, 2);
			Cloud.OnKeyChange("firstTrophy", delegate(bool value)
			{
				elements.firstTrophyToggle.isOn = value;
				elements.firstTrophyToggleAnimation.Play("Blink");
			}, 2);
			Cloud.OnKeyChange("secondTrophy", delegate(bool value)
			{
				elements.secondTrophyToggle.isOn = value;
				elements.secondTrophyToggleAnimation.Play("Blink");
			}, 2);
			Cloud.OnKeyChange("thirdTrophy", delegate(bool value)
			{
				elements.thirdTrophyToggle.isOn = value;
				elements.thirdTrophyToggleAnimation.Play("Blink");
			}, 2);
		}

		private void AddConflictResolvingActions()
		{
			Cloud.OnKeyChange("level", ResolveConflictForLevel, 1);
			Cloud.OnKeyChange("firstTrophy", ResolveConflictForFirstTrophy, 1);
			Cloud.OnKeyChange("secondTrophy", ResolveConflictForSecondTrophy, 1);
			Cloud.OnKeyChange("thirdTrophy", ResolveConflictForThirdTrophy, 1);
		}

		private void RemoveConflictResolvingActions()
		{
			Cloud.RemoveOnKeyChangeAction("level", ResolveConflictForLevel);
			Cloud.RemoveOnKeyChangeAction("firstTrophy", ResolveConflictForFirstTrophy);
			Cloud.RemoveOnKeyChangeAction("secondTrophy", ResolveConflictForSecondTrophy);
			Cloud.RemoveOnKeyChangeAction("thirdTrophy", ResolveConflictForThirdTrophy);
		}

		private void ResolveConflictForLevel(int value)
		{
			Debug.Log("ResolveConflictForLevel(" + value + ")");
			if (elements.levelDropdown.value != value)
			{
				elements.levelDropdown.value = Math.Max(elements.levelDropdown.value, value);
				OnLevelDropDownValueChanged(elements.levelDropdown.value);
			}
		}

		private void ResolveConflictForFirstTrophy(bool value)
		{
			Debug.Log("ResolveConflictForFirstTrophy(" + value + ")");
			if (elements.firstTrophyToggle.isOn != value)
			{
				elements.firstTrophyToggle.isOn = elements.firstTrophyToggle.isOn || value;
				OnFirstTrophyToggleValueChanged(elements.firstTrophyToggle.isOn);
			}
		}

		private void ResolveConflictForSecondTrophy(bool value)
		{
			Debug.Log("ResolveConflictForSecondTrophy(" + value + ")");
			if (elements.secondTrophyToggle.isOn != value)
			{
				elements.secondTrophyToggle.isOn = elements.secondTrophyToggle.isOn || value;
				OnSecondTrophyToggleValueChanged(elements.secondTrophyToggle.isOn);
			}
		}

		private void ResolveConflictForThirdTrophy(bool value)
		{
			Debug.Log("ResolveConflictForThirdTrophy(" + value + ")");
			if (elements.thirdTrophyToggle.isOn != value)
			{
				elements.thirdTrophyToggle.isOn = elements.thirdTrophyToggle.isOn || value;
				OnThirdTrophyToggleValueChanged(elements.thirdTrophyToggle.isOn);
			}
		}
	}
}
