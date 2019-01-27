using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public static GameController controller;

	public float monsterSpawnY;
	public float monsterSpawnMinX;
	public float monsterSpawnMaxX;

	public GameObject[] monsterPrefabs;
	public float minMonsterSpawnPeriod;
	public float maxMonsterSpawnPeriod;
	public float monsterSpawnTimer;

	public Transform monstersSet;
	public Transform weaponesSet;

	public ActionCtl NoText;
	public bool NoShowed;

	public GameObject[] lightnings;
	public int lightningDamage;
	public float lightningColdDown;
	float lightningTimer;

	void Awake () {
		if (controller == null)
			controller = this;
		else {
			Destroy(gameObject);
			return;
		}

		monsterSpawnTimer = Random.Range(minMonsterSpawnPeriod, maxMonsterSpawnPeriod);
	}

	void Update () {
		monsterSpawnTimer -= Time.deltaTime;

		if (monsterSpawnTimer <= 0) {
			monsterSpawnTimer = Random.Range(minMonsterSpawnPeriod, maxMonsterSpawnPeriod);
			SpawnRandomMonster();
		}
		if (lightningTimer > 0) {
			lightningTimer -= Time.deltaTime;
		}
	}

	void SpawnRandomMonster () {
		int index;
		if (monsterPrefabs.Length == 1) {
			index = 0;
		}
		else {
			index = Random.Range(0, monsterPrefabs.Length);
		}

		GameObject monster = Instantiate(monsterPrefabs[index],
			new Vector3(Random.Range(monsterSpawnMinX, monsterSpawnMaxX), monsterSpawnY, 0), Quaternion.identity);
		monster.transform.SetParent(monstersSet);
	}

	public void SpawnSmallNormalMonster () {
		GameObject monster = Instantiate(monsterPrefabs[0],
			new Vector3(Random.Range(monsterSpawnMinX, monsterSpawnMaxX), monsterSpawnY, 0), Quaternion.identity);
		monster.transform.SetParent(monstersSet);
	}

	public void SpawnBigNormalMonster () {
		GameObject monster = Instantiate(monsterPrefabs[1],
			new Vector3(Random.Range(monsterSpawnMinX, monsterSpawnMaxX), monsterSpawnY, 0), Quaternion.identity);
		monster.transform.SetParent(monstersSet);
	}

	public void ForcePush () {
		Vector3 playerPos = Player.player.transform.position;
		foreach (Rigidbody2D rg2d in monstersSet.GetComponentsInChildren<Rigidbody2D>()) {
			rg2d.AddForce((rg2d.transform.position - playerPos) * 500);
		}
	}

	public void No () {
		if (!NoShowed) {
			NoShowed = true;
			NoText.gameObject.SetActive(true);
			NoText.transform.localScale = new Vector3(0.1f, 0.1f, 1);
			NoText.RunAction(ActionCtl.ScaleTo(0.3f, 0.3f, 0.3f),
				ActionCtl.CallFunc(() => {
					NoShowed = false;
					NoText.gameObject.SetActive(false);
					return true;
					}));
		}
	}

	public void GameOver () {
		Debug.Log("Game Over");
	}

	public void Zues () {
		if (lightningTimer > 0) {return;}
		lightningTimer = lightningColdDown;

		foreach (Monster monster in monstersSet.GetComponentsInChildren<Monster>()) {
			monster.Damage(lightningDamage);
		}
		
		foreach (GameObject lightning in lightnings) {
			lightning.SetActive(true);
			StartCoroutine(HideLightning(lightning, Random.Range(0.1f, 0.2f)));
		}
	}

	IEnumerator HideLightning (GameObject lightning, float time) {
		yield return new WaitForSeconds(time);
		lightning.SetActive(false);
	}
}
