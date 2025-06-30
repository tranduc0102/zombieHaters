using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Survivors
{
	[Serializable]
	public struct SurvivorLevels
	{
		public float damage;

		public int cost;

		public float power;

		public float hp;
	}

	public LanguageKeysEnum name;

	public LanguageKeysEnum fullname;

	public SaveData.HeroData.HeroType heroType;

	public Sprite icon;

	public HeroOpenType heroOpenType;

	public int requiredLevelToOpen = 1;

	public int daysToOpen;

	public LanguageKeysEnum heroStory;

	public LanguageKeysEnum shortDescriptionKey;

	[HideInInspector]
	public List<SurvivorLevels> levels;

	public SurvivorHuman survivorPrefab;
}
