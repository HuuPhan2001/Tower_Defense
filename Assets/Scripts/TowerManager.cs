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
	private Collider2D site;
	private Vector3 position;
	private Tower town;
	private static Vector3 bg1 = new Vector3((float)-7.66, (float)2.04, 0);
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
			position = hit.transform.position;
			site = hit.collider;
			site.tag = hit.collider.tag;
			if (hit.collider.tag == "buildSite")
			{
				Debug.Log("build " + hit.transform.position);
				//site = hit.collider;
				buildTitle = hit.collider;
				buildTitle.tag = "buildSiteFull";
				RegisterBuildSite(buildTitle);
				placeTower(hit);// hàm đặt trụ
			}
			town = findTower(hit.transform.position);
			Debug.Log("check null: "+town);
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
				
				if (town == null)
				{
					Debug.Log("null");
					buildTitle = hit.collider;
					hit.collider.tag = "buildSite";

				}
				else
				{
					sellButton.gameObject.SetActive(true);

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
	public void sellBtnPressed()
	{
		Debug.Log(bg1);
		Vector3 pos = getPos(bg1);
		Tower tow = findTower(pos);
		Debug.Log("check : " +tow);
		//Debug.Log("check money: " + tow.Price);
		//GameManager.Instance.addMoney(tow.Price - tow.Price * 40 / 100);
		UnRegisterTower(tow);
		
		sellButton.gameObject.SetActive(false);
		//town.disableSell();

	}
	public Tower findTower(Vector3 pos)
	{
		return towerList.FirstOrDefault(x => x.transform.position == pos);
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
	public void UnRegisterTower(Tower tower)
	{
		Destroy(tower.gameObject);
		towerList.Remove(tower);
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
			Tower newTower = Instantiate(towerBtnPressed.TowerObject);
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
