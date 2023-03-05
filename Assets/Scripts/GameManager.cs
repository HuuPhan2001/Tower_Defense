using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum gameStatus
{
	next, play, gameover, win
};
public class GameManager : Singleton<GameManager>
{
	[SerializeField]
	private int totalWaves = 10;
	[SerializeField]
	//private TextMeshProUGUI totalMoneyLbl;
	private Text totalMoneyLbl;
	[SerializeField]
	//private TextMeshProUGUI curretWaveLbl;
	private Text curretWaveLbl;
	[SerializeField]
	//private TextMeshProUGUI totalEscapedLbl;
	private Text totalEscapedLbl;
	[SerializeField]
	private GameObject spawnPoint;
	[SerializeField]
	private Enemy[] enemies;
	[SerializeField]
	private int maxEnemiesOnScreen;
	[SerializeField]
	private int totalEnemies;
	[SerializeField]
	private int enemiesPerSpawn;
	[SerializeField]
	private TextMeshProUGUI playBtnLbl;
	[SerializeField]
	private Button playBtn;

	private int waveNumber = 0;
	private int totalMoney = 10;
	private int totalEscaped = 0;
	private int roundEscaped = 0;
	private int totalKilled = 0;
	private int enemiesToSpawn = 0;
	private gameStatus currentState = gameStatus.play;
	private AudioSource audioSource;

	public List<Enemy> EnemyList = new List<Enemy>();

	/*    private int enemiesOnScreen = 0;*/
	const float spawDelay = 1f;

	public gameStatus CurrentStates { get => currentState; }
	public int WaveNumber { get => waveNumber; }
	public int TotalEscaped
	{
		get => totalEscaped;
		set => totalEscaped = value;
	}

	public int RoundEscaped
	{
		get => roundEscaped;
		set => roundEscaped = value;
	}

	public int TotalKilled
	{
		get => totalKilled;
		set => totalKilled = value;
	}
	public int TotalMoney
	{
		get => totalMoney;
		set
		{
			totalMoney = value;
			totalMoneyLbl.text = totalMoney.ToString();
		}
	}

	public AudioSource AudioSource { get => audioSource; }

	// Start is called before the first frame update
	void Start()
	{
		/*  StartCoroutine(spawn());*/
		playBtn.gameObject.SetActive(false);
		audioSource = GetComponent<AudioSource>();
		showMenu();
	}
	void Update()
	{
		// spawn();
		handleEscape();
	}


	IEnumerator spawn()
	{
		if (enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies)
		{
			for (int i = 0; i < enemiesPerSpawn; i++)
			{
				//enemiesOnScreen
				if (EnemyList.Count < maxEnemiesOnScreen)
				{
					Enemy newEnemy = Instantiate(enemies[Random.Range(0, enemiesToSpawn)]) as Enemy;
					newEnemy.transform.position = spawnPoint.transform.position;
					/*enemiesOnScreen += 1;*/
				}
			}
			yield return new WaitForSeconds(spawDelay);
			StartCoroutine(spawn());
		}

	}

	public void RegisterEnemy(Enemy enemy)
	{
		EnemyList.Add(enemy);
	}
	public void UnregisterEnemy(Enemy enemy)
	{
		EnemyList.Remove(enemy);
		Destroy(enemy.gameObject);
		isWaveOver();
	}

	public void DestroyAllEnemies()
	{
		foreach (Enemy enemy in EnemyList)
		{
			Destroy(enemy.gameObject);
		}
		EnemyList.Clear();
	}
	/*    public  void removeEnemyFromScreen()
		{
			if (enemiesOnScreen > 0)
				enemiesOnScreen -= 1;
		}*/

	public void addMoney(int amount)
	{
		TotalMoney += amount;
	}
	public void subtractMoney(int amount)
	{
		TotalMoney -= amount;
	}

	public void isWaveOver()
	{
		totalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";
		if ((TotalKilled + TotalEscaped) == totalEnemies)
		{
			if (waveNumber <= enemies.Length)
			{
				enemiesToSpawn = waveNumber;
			}
			setCurrentGameState();
			showMenu();

		}
	}

	public void setCurrentGameState()
	{
		if (TotalEscaped >= 10)
		{
			currentState = gameStatus.gameover;

		}
		else if (waveNumber == 0 && (TotalKilled + RoundEscaped) == 0)
		{
			currentState = gameStatus.play;
		}
		else if (waveNumber >= totalWaves)
		{
			currentState = gameStatus.win;
		}
		else
		{
			currentState = gameStatus.next;
		}
	}

	public void showMenu()
	{
		playBtn.gameObject.SetActive(true);
		switch (currentState)
		{
			case gameStatus.gameover:
				playBtnLbl.text = "Play Again";
				AudioSource.PlayOneShot(SoundManager.Instance.GameOver);
				break;
			case gameStatus.next:
				playBtnLbl.text = "Next Wave";
				break;
			case gameStatus.win:
				playBtnLbl.text = "Play";
				break;
			case gameStatus.play:
				playBtnLbl.text = "Play";
				break;
		}
		

	}

	public void playBtnPressed()
	{
		switch (currentState)
		{
			case gameStatus.next:
				waveNumber += 1;
				totalEnemies += waveNumber;
				break;
			default:
				totalEnemies = 3;
				TotalEscaped = 0;
				waveNumber = 0;
				TotalMoney = 1000;
				enemiesToSpawn = 0;
				TowerManager.Instance.DestoyAllTower();
				TowerManager.Instance.RenameTagsBuildSites();
				totalMoneyLbl.text = TotalMoney.ToString();
				totalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";
				audioSource.PlayOneShot(SoundManager.Instance.NewGame);
				break;
		}
		DestroyAllEnemies();
		TotalKilled = 0;
		RoundEscaped = 0;
		curretWaveLbl.text = "Wave " + (waveNumber + 1);
		StartCoroutine(spawn());
		playBtn.gameObject.SetActive(false);
	}
	//delete tower
	private void handleEscape()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TowerManager.Instance.disnableDragSprite();
			TowerManager.Instance.towerBtnPressed = null;
		}
	}
}
