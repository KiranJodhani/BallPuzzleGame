using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// In Charge to the logic of the dots.
/// </summary>
public class DotManager : MonoBehaviour 
{
	public SpriteRenderer DotSprite;

	public bool isOnCircle;

	public bool isMoving;

	void Awake()
	{
		Reset ();
	}

	void OnSpawned()
	{
		isOnCircle = false;
		Reset ();
	}

	void OnDespawned()
	{
		StopAllCoroutines ();
		Reset ();
	}

	void Reset()
	{
		isMoving = false;

		DotSprite.color = GameManager.Instance.currentAAColor.colorDark;

		transform.position = new Vector3 (transform.position.x, transform.position.y, -0.1f);
		StopAllCoroutines ();

		if (GetComponent<Rigidbody2D>() == null) 
			gameObject.AddComponent<Rigidbody2D> ();

		GetComponent<Rigidbody2D>().velocity = Vector3.zero;

		GetComponent<Rigidbody2D> ().isKinematic = false;

		GetComponent<Collider2D>().enabled = false;
	}

	public void Replace()
	{
		isMoving = false;
		GetComponent<Rigidbody2D>().velocity = Vector3.zero;

		GetComponent<Collider2D>().enabled = false;
	}

	public void Shoot()
	{
		GetComponent<Collider2D>().enabled = true;

		GetComponent<Rigidbody2D>().AddForce (Vector2.up * 50, ForceMode2D.Impulse);
	}

	public void ActivateTrail()
	{
		GetComponent<Collider2D>().enabled = true;

		transform.position = new Vector3 (transform.position.x, transform.position.y, 0f);

		GetComponent<Rigidbody2D> ().isKinematic = true;
	}

	/// <summary>
	/// Analyze the collision paraleters. If dot colide with an other dot => Game Over
	/// </summary>
	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.name.Contains("DestroyerCollider")) 
		{
			GameManager.Instance.OnDespawnedDotOnTarget (transform);
			GameManager.Instance.lastShoots.Remove(this);
		}
		else
		{
			GameOverLogic (col.gameObject);
		}
	}
	/// <summary>
	/// Send to the GameManager a Game Over "message"
	/// </summary>
	void GameOverLogic(GameObject obj)
	{
		if( !GameManager.Instance.isGameOver && !isOnCircle)
		{
			GetComponent<Rigidbody2D>().velocity = Vector3.zero;

			obj.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

			GameManager.Instance.GameOver ();
		}
	}
}
