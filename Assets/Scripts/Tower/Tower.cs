using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
	[SerializeField]
	private float timeBetweenAttacks;
	[SerializeField]
	private float attackRadius;
	[SerializeField]
	private int price;
	[SerializeField]
	private Projectile projecttile;
	//[SerializeField]
	//private Button sellButton;
	private Enemy targetEnemy = null;
	private float attackCounter;
	private bool isAttacking = false;
	private AudioSource audioSource;
	public int Price { get => price;}
	// Start is called before the first frame update
	void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
		attackCounter -= Time.deltaTime;
		if (targetEnemy == null || targetEnemy.IsDead)
		{
			Enemy nearestEnemy = GetNearestEnemyInRange();
			if (nearestEnemy != null && Vector2.Distance(transform.position, nearestEnemy.transform.position) <= attackRadius)
			{
				targetEnemy = nearestEnemy;
			}
		}
		else
		{
			if (attackCounter <= 0)
			{
				isAttacking = true;
				//reset attack counter
				attackCounter = timeBetweenAttacks;
			}
			else
			{
				isAttacking = false;
			}
			if (Vector2.Distance(transform.localPosition, targetEnemy.transform.localPosition) > attackRadius)
			{
				targetEnemy = null;
			}
		}
	}

	//public void enableSell()
	//{
	//	sellButton.gameObject.SetActive(true);
	//}
	//public void disableSell()
	//{
	//	sellButton.gameObject.SetActive(false);
	//}
	void FixedUpdate()
	{
		if (isAttacking)
			Attack();
	}
	IEnumerator MoveProjectile(Projectile projectile)
	{
		while (getTargetDistance(targetEnemy) > 0.20f &&
			projectile != null && targetEnemy != null)
		{
			var dir = targetEnemy.transform.localPosition - transform.localPosition;
			var angleDirection = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
			projectile.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);
			projectile.transform.localPosition = Vector2.MoveTowards(projectile.transform.localPosition, targetEnemy.transform.localPosition, 5f * Time.deltaTime);
			yield return null;
		}
		if (projectile != null || targetEnemy == null)
		{
			Destroy(projectile);
		}
	}

	private float getTargetDistance(Enemy thisEnemy)
	{
		if (thisEnemy == null)
		{
			thisEnemy = GetNearestEnemyInRange();
			if (thisEnemy == null)
			{
				return 0f;
			}
		}
		return Mathf.Abs(Vector2.Distance(transform.localPosition, thisEnemy.transform.localPosition));
	}
	public void Attack()
	{
		isAttacking = false;
		Projectile newProjectile = Instantiate(projecttile) as Projectile;
		newProjectile.transform.localPosition = transform.localPosition;
		if (newProjectile.ProjectileType == proType.arrow)
		{
			GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Arrow);
		}
		else if (newProjectile.ProjectileType == proType.fireball)
		{
			GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.FireBall);
		}
		else if (newProjectile.ProjectileType == proType.rock)
		{
			GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Rock);
		}
		if (targetEnemy == null)
		{
			Destroy(newProjectile);
		}
		else
		{
			//move projectile to enemy
			StartCoroutine(MoveProjectile(newProjectile));
		}
	}

	private List<Enemy> GetEnemiesInRange()
	{
		List<Enemy> enemiesInRange = new List<Enemy>();
		foreach (Enemy enemy in GameManager.Instance.EnemyList)
		{
			if (Vector2.Distance(transform.localPosition, enemy.transform.localPosition) <= attackRadius && !enemy.IsDead)
			{
				enemiesInRange.Add(enemy);
			}
		}
		return enemiesInRange;
	}
	private Enemy GetNearestEnemyInRange()
	{
		Enemy nearestEnemy = null;
		float smallestDistance = float.PositiveInfinity;
		foreach (Enemy enemy in GetEnemiesInRange())
		{
			if (Vector2.Distance(transform.localPosition, enemy.transform.localPosition) < smallestDistance)
			{
				smallestDistance = Vector2.Distance(transform.localPosition, enemy.transform.localPosition);
				nearestEnemy = enemy;
			}
		}
		return nearestEnemy;
	}
}
