using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;

/// <summary>
/// Class in charge of the game logic.
/// </summary>
public class GameManager : Singleton<GameManager> 
{
	[SerializeField] public List<AAColor> aaColors;

	public delegate void ColorChange (AAColor c);
	/// <summary>
	/// Event triggered when we change the colors of the game
	/// </summary>
	public static event ColorChange OnColorChanged;

	private AAColor _currentAAColor;
	public AAColor currentAAColor
	{
		get
		{
			if(_currentAAColor == null)
				_currentAAColor = aaColors[rand.Next(0,aaColors.Count)];
			
			return _currentAAColor;
		}
		set
		{
			_currentAAColor = value;
			if(OnColorChanged != null)
				OnColorChanged(_currentAAColor);
		}
	}


	AudioSource _audioSourceFX;
	AudioSource audioSourceFX
	{
		get
		{
			if(_audioSourceFX == null)
				_audioSourceFX = GetComponent<AudioSource>();

			return _audioSourceFX;
		}
	}

	public AudioClip soundBeep;
	/// <summary>
	/// Sound played when the player shoots a dot
	/// </summary>
	public void PlaySoundBeep()
	{
		if (PlayerPrefs.GetInt (Constant.SOUND_ON, 1) == 0)
			return;

		audioSourceFX.PlayOneShot(soundBeep);
	}
	public AudioClip soundFail;
	/// <summary>
	/// Sound played when the player hits a dots => game over!
	/// </summary>
	public void PlaySoundFail()
	{
		if (PlayerPrefs.GetInt (Constant.SOUND_ON, 1) == 0)
			return;

		audioSourceFX.PlayOneShot(soundFail);
	}
	public AudioClip soundSuccess;
	/// <summary>
	/// Sound played when the player shoot all the dots => success!
	/// </summary>
	public void PlaySoundSuccess()
	{
		if (PlayerPrefs.GetInt (Constant.SOUND_ON, 1) == 0)
			return;

		audioSourceFX.PlayOneShot(soundSuccess);
	}


	/// <summary>
	/// Reference to the current level played, and modify the textNumLevel
	/// </summary>
	public int Level;
	/// <summary>
	/// true if player sucessfully cleared a level
	/// </summary>
	public bool success;
	/// <summary>
	/// true if level is completed (success or not)
	/// </summary>
	public bool isGameOver;

	public Transform container;
	public GameObject containerParent
	{
		get
		{
			return container.parent.gameObject;
		}
	}

	public SpriteRenderer destroyerCollider;
	public Transform SpriteCircle
	{
		get
		{
			return destroyerCollider.transform.parent;
		}
	}

	public Transform DotPrefab;
	//public ParticleEmitter ExplosionPrefab;
	/// <summary>
	/// list of all the dots the player have to shoot in the level
	/// </summary>
	public List<DotManager> Dots;


	public float speed = 1f;

	public float positionTouchBorder;


	int countDespawner;

	float sizeDot = 0;

	DotManager[] movingDots; 

	/// <summary>
	/// true if player can shoot a dot, false if not
	/// </summary>
	public bool canShoot;
	/// <summary>
	/// list of all the dots the player have to shoot in the level
	/// </summary>
	public List<DotManager> lastShoots;

	/// <summary>
	/// Do it at first. Some configurations.
	/// </summary>
	void Awake()
	{
		Application.targetFrameRate = 60;

		Physics2D.gravity = Vector2.zero;

		currentAAColor = aaColors[rand.Next(0,aaColors.Count)];


		if (!PlayerPrefs.HasKey (Constant.LEVEL_UNLOCKED)) {
			PlayerPrefs.SetInt (Constant.LEVEL_UNLOCKED, 1);
		} 

		if (!PlayerPrefs.HasKey (Constant.LAST_LEVEL_PLAYED)) {
			PlayerPrefs.SetInt (Constant.LAST_LEVEL_PLAYED, 1);
		} 

		InputTouch.OnTouchScreen -= OnTouch;
		InputTouch.OnTouchScreen += OnTouch;
	}

	void OnTouch(Vector2 v)
	{
		if ((Input.mousePosition.y < Screen.height * 0.9f) && (Input.mousePosition.x < Screen.width * 0.9f)) 
		{
			if (!isGameOver && canShoot && Dots.Count > 0)
				ShootDot (Dots [0]);
		}
	}



	public void GameOver()
	{
		if (sequence != null)
			sequence.Kill (false);


		StopAllCoroutines ();
		PlaySoundFail ();
		isGameOver = true;

		CanvasManager.Instance.AnimationCameraGameOver ();
	}


	Sequence sequence;

	void LaunchRotateCircle()
	{
		SequenceLogic();
	}

	void SequenceLogic()
	{
		if (sequence != null)
			sequence.Kill (false);

		sequence = DOTween.Sequence ();


		if (loopType == LoopType.Incremental) {
			sequence.Append (container.DORotate (rotateVector * UnityEngine.Random.Range (360, 520), rotateCircleDelay, RotateMode.FastBeyond360).SetEase (easeType));
			sequence.SetLoops (1, loopType);
		} else {
			sequence.Append (container.DORotate (rotateVector * UnityEngine.Random.Range (360, 520), rotateCircleDelay, RotateMode.FastBeyond360).SetEase (easeType));
			sequence.SetLoops (2, loopType);
		}



		sequence.OnStepComplete (() => {
			Debug.Log("step complete");
			SequenceLogic();
		});

		sequence.Play ();
	}

	/// <summary>
	/// Keep a reference of the LEVEL use to generate the level
	/// </summary>
	Level LEVEL;
	/// <summary>
	/// Rotation direction
	/// </summary>
	private Vector3 rotateVector
	{
		get
		{
			Vector3 v = new Vector3 (0, 0, 1);
			if (this.LEVEL.levelNumber % 2 == 0)
			{
				v = new Vector3 (0, 0, -1);
			}

			return v;
		}
	}
	float positionCircle = 1f / 4f;
	private Ease easeType{
		get
		{
			return this.LEVEL.rotateEaseType;
		}
	}
	private LoopType loopType
	{
		get
		{
			return this.LEVEL.rotateLoopType;
		}
	}
	private float rotateCircleDelay
	{
		get
		{
			return this.LEVEL.rotateDelay;
		}
	}
	private int numberDotsToCreate
	{
		get
		{
			return this.LEVEL.numberDotsToCreate;
		}
	}
	private int numberDotsOnCircle
	{
		get
		{
			return this.LEVEL.numberDotsOnCircle;
		}
	}

	System.Random rand = new System.Random();

	public void CreateGame(int level, bool lastWasSuccess)
	{
        foreach (var d in FindObjectsOfType<DotManager>())
		{
			Destroy(d.gameObject);
		}


		Level = level;

		canShoot = false;
		isGameOver = false;
		success = false;

		float height = 2f * Camera.main.orthographicSize;
		float width = height * Camera.main.aspect;

		isGameOver = true;

		Camera.main.transform.position = new Vector3 (0, 0, -10);

		countDespawner = 0;

		StopAllCoroutines ();

		isGameOver = true;

		Dots = new List<DotManager>();
		Level = PlayerPrefs.GetInt (Constant.LAST_LEVEL_PLAYED);
		this.LEVEL = LevelManager.Instance.GetLevel (Level);

		containerParent.transform.rotation = Quaternion.Euler (Vector3.zero);

		containerParent.transform.position = Vector3.zero;

		positionTouchBorder = Mathf.Min (width, height) * 0.3f;

		Camera.main.transform.position = new Vector3 (0, Camera.main.transform.position.y, Camera.main.transform.position.z);

		SpriteCircle.transform.position = new Vector3 (0, height/2 +  destroyerCollider.GetComponent<SpriteRenderer> ().bounds.size.x*0.5f + 1 , 0);

		container.localScale = Vector3.one;

		CreateDotOnCircle ();
		CreateListDots ();
		LaunchRotateCircle ();

		DOVirtual.DelayedCall (0.1f, () => {
			canShoot = true;
			isGameOver = false;
			success = false;
		});

		if(lastWasSuccess)
			ForceChangeColors();
	}

	public void RefreshColors()
	{
		currentAAColor = currentAAColor;
	}

	void ForceChangeColors()
	{
		currentAAColor = aaColors[rand.Next(0,aaColors.Count)];
	}

	void ChangeColors()
	{
		if(Time.realtimeSinceStartup > 0.1f)
		{
			currentAAColor = aaColors[rand.Next(0,aaColors.Count)];
		}
	}

	void CreateDotOnCircle()
	{
		canShoot = false;

		Transform prefab = this.DotPrefab;
		Vector3 pos = new Vector3 (0, -positionTouchBorder, -2);
		Quaternion rot = prefab.rotation;
		Transform parent = container;


		for (int i = 0; i < numberDotsOnCircle ; i++)
		{
			containerParent.transform.rotation = Quaternion.Euler( new Vector3 (0, 0, ((float)i) * 360f / numberDotsOnCircle) );

			Transform t = Instantiate (prefab) as Transform;
			t.position = pos;
			t.rotation = rot;
			t.parent = parent;

			DotManager dm = t.GetComponent<DotManager> ();

			dm.ActivateTrail ();
		}

		SmoothDecal () ;
	}

	void PreparePoolForAnimation()
	{
		canShoot = false;
	
		var dotPool = FindObjectsOfType<DotManager>();

		foreach(var d in dotPool ) 
		{
			var t = d.transform;
			t.parent = container;
		}
	}


	private float GetPositionYTarget()
	{

		float height = 2f * Camera.main.orthographicSize;
		float width = height * Camera.main.aspect;

		return Mathf.Min(width,height) * positionCircle*0.5f + positionTouchBorder*1f ;
	}


	void CreateListDots()
	{

		Dots = new List<DotManager>();

		for (int i = 0; i < numberDotsToCreate; i++)
		{


			var go = Instantiate(this.DotPrefab) as Transform;
			go.parent = transform;

			DotManager dm = go.GetComponent<DotManager> ();


			if (sizeDot == 0) 
				sizeDot = this.DotPrefab.GetComponent<DotManager> ().DotSprite.bounds.size.x * 1.1f;


			Vector3 target = new Vector3 (0,-GetPositionYTarget() + (-i-1)*sizeDot, dm.transform.position.z);
			dm.transform.position = target;

			Dots.Add (dm);

		}
		 
	}
	/// <summary>
	/// Method to shoot the first dot and moving the other. This method check if the list of dots to shoot is empty or not. If the list is empty, this method triggered the success for this level.
	/// </summary>
	void ShootDot(DotManager d){

		StopCoroutine("PositioningDots");
		StopCoroutine("MoveStartPositionDot");


		for (int i = 0; i < Dots.Count; i++) 
		{
			Dots [i].transform.DOKill();
		}


		for (int i = 0; i < Dots.Count; i++) {

			if ( !Dots[i].isMoving ){
				Dots [i].transform.position = new Vector3 (0, -GetPositionYTarget() + (-i-1)*sizeDot, Dots [i].transform.position.z);
			}

		}

		PlaySoundBeep ();

		Dots.Remove (d);

		if (this.lastShoots == null)
			this.lastShoots = new List<DotManager> ();

		this.lastShoots.Add(d);


		Vector3 target = new Vector3 (d.transform.position.x, SpriteCircle.position.y - 1, d.transform.position.z);

		d.transform.position = new Vector3 (0, -GetPositionYTarget () + (-0 - 1) * sizeDot, d.transform.position.z);

		d.isMoving = true;




		d.Shoot ();


		for (int i = 0; i < Dots.Count; i++) 
		{

			if (sizeDot == 0)
			{
				Debug.Log ("!!!!!! GET SIZEDOT ( must be done just one time only)");
				sizeDot = Dots [0].DotSprite.bounds.size.x * 1.1f;
			}


			if ( !Dots[i].isMoving )
			{
				Dots [i].transform.DOMoveY(-GetPositionYTarget() + (-i-1)*sizeDot,0.3f)
					.OnComplete(() => {

					});
			}
		}
	}
	/// <summary>
	/// Destroy the dot who is hiting the top of the screen
	/// </summary>
	public void OnDespawnedDotOnTarget(Transform d)
	{
		countDespawner++;
		//var t = Instantiate(ExplosionPrefab.transform) as Transform;
		//t.position = new Vector3(d.position.x,d.position.y,-1);
		//t.rotation = d.rotation;
		//t.gameObject.SetActive(true);
		//t.GetComponent<ParticleExplosionLogic>().Do();

		Destroy(d.gameObject);


		if (numberDotsToCreate == countDespawner && !isGameOver)
		{
			success = true;
		}

		if (success && !isGameOver) 
		{
			PlaySoundSuccess();
			isGameOver = true;
			CanvasManager.Instance.AnimationCameraSuccess();
		}
	}
	/// <summary>
	/// To anim in and out the rotating dots
	/// </summary>
	void SmoothDecal()
	{
		if ( this.LEVEL.smoothDecal )
		{
			movingDots = container.GetComponentsInChildren<DotManager>();

			foreach(DotManager d in movingDots)
			{
				StartCoroutine(SmoothDecalRoutine(d));
			}

		}

	}	
	/// <summary>
	/// To anim in and out the rotating dots
	/// </summary>
	IEnumerator SmoothDecalRoutine(DotManager d)
	{
		bool goIn = true;

		float distance = Vector3.Distance(d.transform.position, containerParent.transform.position);

		while (true) 
		{
			if(d == null)
				break;

			if ( goIn ){
				d.transform.localPosition *= 0.99f;
			}else{
				d.transform.localPosition *= 1.01f;
			}


			if (goIn && Vector3.Distance(d.transform.position, containerParent.transform.position) < distance/2f) {
				goIn = false;
				yield return new WaitForSeconds(((10-rotateCircleDelay) + numberDotsOnCircle)/2);
			}
			if (!goIn && Vector3.Distance(d.transform.position, containerParent.transform.position) > distance) {
				goIn = true;
				yield return new WaitForSeconds(((10-rotateCircleDelay) + numberDotsOnCircle)/4);
			}

			yield return new WaitForEndOfFrame ();

			if(d == null)
				break;
		}
	}
	/// <summary>
	/// Clean memory if the game is pausing
	/// </summary>
	public void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			Application.targetFrameRate = 60;
			Resources.UnloadUnusedAssets ();
			Time.timeScale = 1.0f;
		}
		else 
		{
			Application.targetFrameRate = 60;
			Resources.UnloadUnusedAssets ();
			Time.timeScale = 0.0f;
		}
	}  
	/// <summary>
	/// Save the PlayerPrefs when the game is quiting
	/// </summary>
	void OnApplicationQuit()
	{
		PlayerPrefs.Save();
	}
}



