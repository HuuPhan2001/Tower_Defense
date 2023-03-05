using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerManager : Singleton<TowerManager>
{
	//delete tower
	public TowerBtn towerBtnPressed { get; set; }

	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private Button sellButton;
	private List<Tower> towerList = new List<Tower>();
	private List<Collider2D> BuildList = new List<Collider2D>();
	private Collider2D buildTitle;
	private Tower town;
	private static Vector3 bg1;
	//private Collider2D site;
	// Start is called before the first frame update
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		buildTitle = GetComponent<Collider2D>();
		spriteRenderer.enabled = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			DestoyAllTower();
			RenameTagsBuildSites();
		}

		if (Input.GetMouseButtonDown(0))
		{
			Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
			if (hit.collider.tag == "buildSite")
			{
				Debug.Log("build " + hit.transform.position);
				//site = hit.collider;
				buildTitle = hit.collider;
				buildTitle.tag = "buildSiteFull";
				RegisterBuildSite(buildTitle);
				placeTower(hit);// hàm đặt trụ
				sellButton.gameObject.SetActive(false);
			}
			//if (hit.transform.position == bg1)
			//{
			//	sellButton.gameObject.SetActive(true);
			//}
			//if (town == null && hit.collider.tag == "buildSiteFull")
			//{
			//	buildTitle = hit.collider;
			//	hit.collider.tag = "buildSite";
			//	UnRegisterBuildSite(buildTitle);
			//}
			//else if (town != null && hit.collider.tag == "buildSiteFull")
			//{
			//	sellButton.gameObject.SetActive(true);
			//	//town.enableSell();
			//}
			if (hit.collider.tag == "buildSiteFull")
			{
				town = findTower(hit.transform.position);
				getTower(town);
				Debug.Log("check active: " + town);

				if (town.isActiveAndEnabled == false)
				{
					UnRegisterTower(hit.transform);
					placeTower(hit);


				}
				else
				{
					Debug.Log("check town active: " + town);
					sellButton.gameObject.SetActive(true);
					bg1 = hit.transform.position;
					getPos(bg1);
					//if ((colliders = Physics.OverlapSphere(bg1, 1f)).Length > 1)
					//{
					//	foreach (var collider in colliders)
					//	{
					//		var gameObjects = Physics.OverlapSphere(bg1, 1).Except(new[] { collider }).Select(x => x.gameObject).ToArray();
					//	}

					//}
					//for (var go : GameObject in gameObjects)
					//{

					//}
					Debug.Log("full " + hit.transform.position);
					Debug.Log(towerList.Count);

				}
				//	//town = findTower();

				//	//Debug.Log(hit.collider);
			}
		}
		if (spriteRenderer.enabled)
		{
			followMouse();
		}
	}

	public Vector3 getPos(Vector3 pos)
	{
		return pos;
	}
	public Tower getTower(Tower tower)
	{
		return tower;
	}
	public void sellBtnPressed()
	{
		Vector3 pos = getPos(bg1);
		//Tower tow = findTower(pos);
		//Debug.Log("check : " + tow);
		//Debug.Log("town List " + towerList.Select(x=>x.transform.position.normalized).ToString());
		GameObject[] gos = GameObject.FindGameObjectsWithTag("tower");
		var target = gos.OrderBy(go => (go.transform.position).sqrMagnitude == bg1.sqrMagnitude).First();
		Tower tow = getTower(town);
		target.gameObject.SetActiveRecursively(false);
		Debug.Log("check tow: " + tow);
		Debug.Log("check town: " + town);
		//Debug.Log("check money: " + tow.Price);
		//GameManager.Instance.addMoney(tow.Price - tow.Price * 40 / 100);
		//UnRegisterTower((Tower)target);

		//sellButton.gameObject.SetActive(false);
		//town.disableSell();

	}
	public Tower findTower(Vector3 pos)
	{
		if (towerList.Count == 0)
		{
			return null;
		}
		return (Tower)towerList.OrderBy(x => (x.transform.position).sqrMagnitude == pos.sqrMagnitude).First();
	}

	public void RegisterBuildSite(Collider2D buildTag)
	{
		BuildList.Add(buildTag);
	}
	public void UnRegisterBuildSite(Collider2D buildTag)
	{
		BuildList.Remove(buildTag);
	}

	public void RegisterTower(Tower tower)
	{
		towerList.Add(tower);
	}
	public void UnRegisterTower(Transform transforms)
	{
		//Debug.Log(tower.transform.position);
		Tower town = (Tower)towerList.OrderBy(x=>x.transform.position.sqrMagnitude == transform.position.sqrMagnitude).First();
		if (town != null)
		{
			Destroy(town.gameObject);
			GameManager.Instance.addMoney(town.Price - town.Price * 40 / 100);
			towerList.Remove(town);
		}
		else
		{
			return;
		}

		//GameObject gOb = FindObjectOfType<Tower>().transform.position == pos;
		//foreach (Tower town in towerList)
		//{
		//	if(town.transform.position == tower.transform.position)
		//	{
		//		Destroy(town);
		//		towerList.Remove(town);
		//		break;
		//	}
		//}
		//Destroy(tower.gameObject);
		//towerList.Remove(tower);
	}
	public void RenameTagsBuildSites()
	{
		foreach (Collider2D buildTag in BuildList)
		{
			buildTag.tag = "buildSite";
		}
		BuildList.Clear();
	}

	public void DestoyAllTower()
	{
		foreach (Tower tower in towerList)
		{
			Destroy(tower.gameObject);
		}
		towerList.Clear();
	}
	public void placeTower(RaycastHit2D hit)
	{
		if (!EventSystem.current.IsPointerOverGameObject() && towerBtnPressed != null)
		{
			Tower newTower = Instantiate(towerBtnPressed.TowerObject) as Tower;
			newTower.transform.position = hit.transform.position;
			//newTower.disableSell();
			sellButton.onClick.AddListener(() => sellBtnPressed());
			sellButton.gameObject.SetActive(false);
			buyTower(towerBtnPressed.TowerPrice);
			GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.TowerBuilts);
			RegisterTower(newTower);
			disnableDragSprite();
		}
	}

	public void buyTower(int price)
	{
		GameManager.Instance.subtractMoney(price);
	}

	public void selectedTower(TowerBtn towerSelected)
	{
		if (towerSelected.TowerPrice <= GameManager.Instance.TotalMoney)
		{
			towerBtnPressed = towerSelected;
			enableDragSprite(towerBtnPressed.DragSprite);
		}
	}
	public void followMouse()
	{
		transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.position = new Vector2(transform.position.x, transform.position.y);
	}

	public void enableDragSprite(Sprite sprite)
	{
		spriteRenderer.enabled = true;
		spriteRenderer.sprite = sprite;
	}

	public void disnableDragSprite()
	{
		spriteRenderer.enabled = false;

	}
}
