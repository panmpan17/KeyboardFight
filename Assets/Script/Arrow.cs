using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotateType {Normal, Slow, Fast}

public class Arrow : MonoBehaviour {

	public RotateType rotateType;
	public int rotateDirection = 1;
	public float rotateSpeed;
	public float slowRate;
	public float fastRate;

	public GameObject bulletPrfab;
	public GameObject hamPrefab;
	public GameObject bombPrefab;
	public GameObject firePrefab;
	public GameObject waterPrefab;
	public GameObject vinePrefab;

	public GameObject spear;
	public GameObject lightBeam;

	bool spearActive = false;

	public float canonRequired;
	float canonTimer;

	public float zuesRequired;
	float zuesTimer;

	public float waterRequired;
	float waterTimer;

	public float vineRequired;
	float vineTimer;

	Dictionary<KeyCode, int> keyPressedTime = new Dictionary<KeyCode, int>();

	void Update () {
		if (waterTimer > 0) {
			waterTimer -= Time.deltaTime;
		}
		if (vineTimer > 0) {
			vineTimer -= Time.deltaTime;
		}

		HandleRotate();
		CheckKeyPressed();
		CheckKeyRealeased();
	}

	void HandleRotate () {
		float zRoation = transform.localRotation.eulerAngles.z;
		if (zRoation < 270 && zRoation > 180 && rotateDirection == -1) {
			rotateDirection = 1;
		}
		else if (zRoation > 90 && zRoation < 180 && rotateDirection == 1) {
			rotateDirection = -1;
		}

		float rotateMultiplier = Time.deltaTime * rotateDirection * rotateSpeed;

		if (rotateType == RotateType.Slow) {
			rotateMultiplier *= slowRate;
		}
		else if (rotateType == RotateType.Fast) {
			rotateMultiplier *= fastRate;
		}

		transform.Rotate(Vector3.forward * rotateMultiplier);
	}

	void CheckKeyPressed () {
		// Bullet always
		if (Input.GetKeyDown(KeyCode.A)) {
			Pressed(KeyCode.A);
			ShootBullet();
		}
		
		// Key that need to be pressed multiple time
		if (Input.GetKeyDown(KeyCode.B)) {
			Pressed(KeyCode.B);

			if (keyPressedTime[KeyCode.B] % 5 == 0) {
				ShootBomb();
			}
		}
		else if (Input.GetKeyDown(KeyCode.F)) {
			Pressed(KeyCode.F);

			if (keyPressedTime[KeyCode.F] % 10 == 0) {
				GameController.controller.ForcePush();
			}
		}
		if (Input.GetKeyDown(KeyCode.D)) {
			Pressed(KeyCode.D);
			
			if (keyPressedTime[KeyCode.D] % 10 == 0) {
				GameController.controller.SpawnSmallNormalMonster();
			}
		}
		else if (Input.GetKeyDown(KeyCode.Y)) {
			Pressed(KeyCode.Y);
			
			if (keyPressedTime[KeyCode.Y] % 50 == 0) {
				GameController.controller.SpawnSmallNormalMonster();
			}
		}
		if (Input.GetKeyDown(KeyCode.R)) {
			Pressed(KeyCode.R);
			
			if (keyPressedTime[KeyCode.R] >= 100) {
				GameController.controller.GameOver();
			}
		}
		else if (Input.GetKeyDown(KeyCode.N)) {
			Pressed(KeyCode.N);
			if (keyPressedTime[KeyCode.N] % 4 == 0) {
				Debug.Log("no");
				GameController.controller.No();
			}
		}
		

		if (Input.GetKeyDown(KeyCode.G)) {
			Pressed(KeyCode.G);
			GrowPlant();
		}
		else if (Input.GetKeyDown(KeyCode.H)) {
			Pressed(KeyCode.H);
			ShootHam();
		}
		else if (Input.GetKeyDown(KeyCode.I)) {
			Pressed(KeyCode.I);
			ShootFire();
		}
		else if (Input.GetKeyDown(KeyCode.J)) {
			Pressed(KeyCode.J);
			Debug.Log(keyPressedTime[KeyCode.J]);
			// TODO: Joke
		}
		else if (Input.GetKeyDown(KeyCode.K)) {
			Pressed(KeyCode.K);
			Debug.Log(keyPressedTime[KeyCode.K]);
			// TODO: Kill Random
		}
		else if (Input.GetKey(KeyCode.L)) {
			Pressed(KeyCode.L);
			lightBeam.SetActive(true);
			lightBeam.transform.rotation = transform.rotation;
		}
		else if (Input.GetKeyDown(KeyCode.O)) {
			Pressed(KeyCode.O);
			Debug.Log(keyPressedTime[KeyCode.O]);
			// TODO: Omelet
		}
		else if (Input.GetKeyDown(KeyCode.P)) {
			Pressed(KeyCode.P);
			StartCoroutine(ShowSpear());
		}
		else if (Input.GetKey(KeyCode.T)) {
			Pressed(KeyCode.T);
			Monster.speedMultiplier = 1.2f;
		}
		else if (Input.GetKeyDown(KeyCode.V)) {
			Pressed(KeyCode.V);
			rotateDirection *= -1;
		}
		else if (Input.GetKey(KeyCode.W)) {
			Pressed(KeyCode.W);
			ShootWater();
		}

		// Canon key.
		if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.M)) {
			Pressed(KeyCode.C);
			canonTimer += Time.deltaTime;

			if (canonTimer >= canonRequired) {
				canonTimer = 0;

				// TODO: canon
			}
		}

		// Lightning key.
		if (Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.U) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.S)) {
			Pressed(KeyCode.Z);
			zuesTimer += Time.deltaTime;

			if (zuesTimer >= zuesRequired) {
				zuesTimer = 0;

				GameController.controller.Zues();
			}
		}

		// Change arrow speed key.
		if (Input.GetKey(KeyCode.Q)) {
			Pressed(KeyCode.Q);
			rotateType = RotateType.Fast;
		}
		else if (Input.GetKey(KeyCode.X)) {
			Pressed(KeyCode.X);
			rotateType = RotateType.Slow;
		}
		else {
			rotateType = RotateType.Normal;
		}
	}

	void CheckKeyRealeased () {
		if (Input.GetKeyUp(KeyCode.L)) {
			lightBeam.SetActive(false);
		}
		if (Input.GetKeyUp(KeyCode.T)) {
			Monster.speedMultiplier = 1f;
		}
		if (Input.GetKeyUp(KeyCode.W)) {}
		if (Input.GetKeyUp(KeyCode.C) && Input.GetKeyUp(KeyCode.M)) {
			canonTimer = 0;
		}
		if (Input.GetKeyUp(KeyCode.Z) && Input.GetKeyUp(KeyCode.U) && Input.GetKeyUp(KeyCode.E) && Input.GetKeyUp(KeyCode.S)) {
			zuesTimer = 0;
		}
	}

	void Pressed (KeyCode code) {
		int times;
		keyPressedTime.TryGetValue(code, out times);

		if (times == 0)
			keyPressedTime.Add(code, 1);
		else
			keyPressedTime[code] =  times + 1;
	}

	void ShootBullet () {
		GameObject bullet = Instantiate(bulletPrfab, transform.position, transform.rotation);
		bullet.GetComponent<Rigidbody2D>().AddForce(transform.up * 400);
		bullet.transform.SetParent(GameController.controller.weaponesSet);
	}

	void ShootHam () {
		GameObject ham = Instantiate(hamPrefab, transform.position, transform.rotation);
		ham.GetComponent<Rigidbody2D>().AddForce(transform.up * 1500);
		ham.transform.SetParent(GameController.controller.weaponesSet);
	}

	void ShootBomb () {
		GameObject bomb = Instantiate(bombPrefab, transform.position, transform.rotation);
		bomb.GetComponent<Rigidbody2D>().AddForce(transform.up * 1000);
		bomb.transform.SetParent(GameController.controller.weaponesSet);
	}

	void ShootFire () {
		GameObject fire = Instantiate(firePrefab, transform.position, Quaternion.identity);
		fire.GetComponent<Rigidbody2D>().AddForce(transform.up * 150);
		fire.transform.SetParent(GameController.controller.weaponesSet);
	}

	void ShootWater () {
		if (waterTimer <= 0) {
			waterTimer = waterRequired;

			GameObject water = Instantiate(waterPrefab, transform.position, transform.rotation);
			water.GetComponent<Rigidbody2D>().AddForce(transform.up * 300);
			water.transform.SetParent(GameController.controller.weaponesSet);
		}
	}

	void GrowPlant () {
		if (vineTimer <= 0) {
			vineTimer = vineRequired;

			GameObject vine = Instantiate(vinePrefab, transform.position, transform.rotation);
			vine.GetComponent<ActionCtl>().RunActionForever(ActionCtl.SizeBy(0.1f, 0, 0.4f));
			vine.transform.SetParent(GameController.controller.weaponesSet);
		}
	}

	IEnumerator ShowSpear () {
		if (!spearActive) {
			spearActive = true;
			spear.SetActive(true);
			spear.transform.rotation = transform.rotation;

			yield return new WaitForSeconds(0.2f);

			spearActive = false;
			spear.SetActive(false);
		}
	}
}
