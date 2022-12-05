using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Class who make the level. Click right on the LevelManager in the editor and select "execute" to generate 1200 levels.
/// </summary>
public class LevelManager : Singleton<LevelManager> 
{
	public List<Level> levels;

	void Awake(){
		int level = PlayerPrefs.GetInt (Constant.LAST_LEVEL_PLAYED);
		if (level > 1200) {
			PlayerPrefs.SetInt (Constant.LAST_LEVEL_PLAYED, 1200);
		}
	}

	#if UNITY_EDITOR
	[ContextMenu("Execute")]
	public virtual void CreateLevels ()
	{
		Debug.Log("execute");


		levels = new List<Level> ();

		for (int i = 1; i <= 1200; i++) 
		{
			Level l = new Level (i);
			levels.Add (l);
		}
	}
	#endif

	public Level GetLevel(int level)
	{
		return levels [level - 1];
	}
}

/// <summary>
/// Level class with all the informations for to create the level in the GameManager
/// </summary>
[Serializable]
public class Level
{
	/// <summary>
	/// Don't create more than 1200 levels
	/// </summary>
	static int maxLevel = 1200;

	[SerializeField] public int levelNumber = 0;
	[SerializeField] public bool smoothDecal = false;
	[SerializeField] public int numberDotsToCreate = 0;
	[SerializeField] public int numberDotsOnCircle = 0;
	[SerializeField] public float sizeRayonRation = 1f;
	[SerializeField] public float rotateDelay = 6f;
	[SerializeField] public Ease rotateEaseType = Ease.InCirc;//Ease.linear;
	[SerializeField] public LoopType rotateLoopType = LoopType.Incremental;

	/// <summary>
	/// Level constructor
	/// </summary>
	public Level (int level)
	{
		levelNumber = level;

		if (level == 1) 
		{
			numberDotsToCreate = 6;
			numberDotsOnCircle = 0;
		} 
		else if (level == 2) 
		{
			numberDotsToCreate = 6;
			numberDotsOnCircle = 2;
		}
		else if (level == 3) 
		{
			numberDotsToCreate = 6;
			numberDotsOnCircle = 3;
		} 
		else if (level == 4) 
		{
			numberDotsToCreate = 6;
			numberDotsOnCircle = 5;

		}
		else 
		{
			numberDotsToCreate = 
				(int)(
					(5 + level % 5)* sizeRayonRation);
			//* sizeRayonRation);

			numberDotsOnCircle = 
				(int)(
					(2 + (4 - level % 5))* sizeRayonRation );

		}
		if (level > 5) 
		{
			sizeRayonRation = 1f - (level % 5f) / 10f;
		}



		sizeRayonRation = 1f - (9 - level % 10) / 10f;

		if (level <= 10) 
		{
			smoothDecal = false;
		} 
		else
		{
			if (level % 5 < 3) {
				smoothDecal = true;
			} else {
				smoothDecal = false;
			}
		}

		if (level > 20 && level < 30) 
		{
			if (level % 2 == 1)
			{
				rotateDelay = 15f - (level % 5);
			} 
			else 
			{
				rotateDelay = 10f + (level % 5);
			}

		} 
		else 
		{

			if (level % 2 == 1) 
			{
				rotateDelay = 15f - (level % 5);
			}
			else
			{
				rotateDelay = 10f + (level % 5);
			}
		}

		int variable = 0;
		if (level <= maxLevel/2) 
		{
			rotateLoopType = LoopType.Incremental;
			variable = 0;
		}
		else
		{
			rotateLoopType = LoopType.Yoyo;
			variable = maxLevel/2;
		}


		if(level > 5 && level%2 == 0)
		{
			this.smoothDecal = true;
		}
		else
		{
			this.smoothDecal = false;
		}

		if (level < 30 + variable) {
			rotateEaseType = Ease.Linear;
		}else if (level < 50 + variable) {
			rotateEaseType = Ease.InCirc; // spring ??
		} else if (level < 70 + variable) {
			rotateEaseType = Ease.InQuad;
		} else if (level < 90 + variable) {
			rotateEaseType = Ease.OutCirc;
		} else if (level < 110 + variable) {
			rotateEaseType = Ease.InOutQuart;
		} else if (level < 130 + variable) {
			rotateEaseType = Ease.InQuint;
		} else if (level < 150 + variable) {
			rotateEaseType = Ease.OutSine;
		} else if (level < 170 + variable) {
			rotateEaseType = Ease.InOutExpo;
		} else if (level < 190 + variable) {
			rotateEaseType = Ease.InCirc;
		} else if (level < 210 + variable) {
			rotateEaseType = Ease.OutBounce;
		} else if (level < 230 + variable) {
			rotateEaseType = Ease.InOutQuint;
		} else if (level < 250 + variable) {
			rotateEaseType = Ease.InExpo;
		} else if (level < 270 + variable) {
			rotateEaseType = Ease.OutQuart;
		} else if (level < 290 + variable) {
			rotateEaseType = Ease.InOutQuad;
		} else if (level < 310 + variable) {
			rotateEaseType = Ease.InSine;
		} else if (level < 330 + variable) {
			rotateEaseType = Ease.OutExpo;
		} else if (level < 350 + variable) {
			rotateEaseType = Ease.InOutCubic;
		} else if (level < 370 + variable) {
			rotateEaseType = Ease.InBounce;
		} else if (level < 390 + variable) {
			rotateEaseType = Ease.OutQuint;
		} else if (level < 410 + variable) {
			rotateEaseType = Ease.InOutSine;
		} else if (level < 430 + variable) {
			rotateEaseType = Ease.InQuart;
		} else if (level < 450 + variable) {
			rotateEaseType = Ease.OutQuad;
		} else if (level < 470 + variable) {
			rotateEaseType = Ease.InOutCirc;
		} else if (level < 490 + variable) {
			rotateEaseType = Ease.InCubic;
		} else if (level < 510 + variable) {
			rotateEaseType = Ease.OutCubic;
		} else if (level < 530 + variable) {
			rotateEaseType = Ease.InOutBounce;
		}
			

		if (level > maxLevel) 
		{
			PlayerPrefs.SetInt (Constant.LEVEL_UNLOCKED, 1200);

			level = 1200;
			PlayerPrefs.SetInt (Constant.LAST_LEVEL_PLAYED, 1200);

			Application.OpenURL ("http://barouch.fr/moregames.php");

		}


	}

}
