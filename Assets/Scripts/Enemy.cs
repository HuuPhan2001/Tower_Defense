using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
	[SerializeField]
	private Transform exitPoint;
	[SerializeField]
	private Transform[] wayPoints;//check point
	[SerializeField]
	private float navigationUpdate;
	[SerializeField]
	private int healthPoints;
	[SerializeField]
	private int rewardAmt;
	[SerializeField]
	private Image heathBar;
	[SerializeField]
	private TextMeshProUGUI health;

	private int target = 0;
	private Transform enemy;
	private Collider2D enemyCollider;
	private Animator anim;
	private float navigationTime = 0;
	private bool isDead = false;
	private float heathStart;

	public bool IsDead { get => isDead; }
	public int HealthPoints { get => healthPoints;}
	// Start is called before the first frame update
	void Start()
	{
		heathStart = healthPoints;
		health.text = heathStart.ToString();
		enemy = GetComponent<Transform>();
		enemyCollider = GetComponent<Collider2D>();
		anim = GetComponent<Animator>();
		GameManager.Instance.RegisterEnemy(this);
	}

	// Update is called once per frame
	void Update()
	{
		if (wayPoints != null && !isDead)
		{
			navigationTime += Time.deltaTime;
			if (navigationTime > navigationUpdate)
			{
				if (target < wayPoints.Length)
				{
					enemy.position = Vector2.MoveTowards(enemy.position, wayPoints[target].position, navigationTime);
				}
				else
				{
					enemy.position = Vector2.MoveTowards(enemy.position, exitPoint.position, navigationTime);
				}
				navigationTime = 0;
			}
		}
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "checkpoint")
			target += 1;
		else if (collision.tag == "Finish")
		{
			GameManager.Instance.RoundEscaped += 1;
			GameManager.Instance.TotalEscaped += 1;
			GameManager.Instance.UnregisterEnemy(this);
			GameManager.Instance.isWaveOver();
			/*            GameManager.Instance.removeEnemyFromScreen();
                        Destroy(gameObject);*/
		}
		else if (collision.tag == "projectile")
		{
			Projectile newP = collision.gameObject.GetComponent<Projectile>();
			enemyHit(newP.AttackStrength);
			Destroy(collision.gameObject);
		}

	}

	public void enemyHit(int hitpoints)
	{
		if (healthPoints - hitpoints > 0)
		{
			// hurt animation
			healthPoints -= hitpoints;
			health.text = healthPoints.ToString();
			heathBar.fillAmount = healthPoints / heathStart;
			GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Hit);
			anim.Play("Hurt");

		}
		else
		{
			//enemy die aniamtion
			die();
			enemy.GetChild(0).gameObject.SetActive(false);
			health.text = "0";
		}
	}
	public void die()
	{
		isDead = true;
		anim.SetTrigger("didDie");
		enemyCollider.enabled = false;
		GameManager.Instance.TotalKilled += 1;
		GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Death);
		GameManager.Instance.addMoney(rewardAmt);
		GameManager.Instance.isWaveOver();
	}

}
