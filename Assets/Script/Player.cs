using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public static Player player;

	public GameObject arrow;

	void Awake () {
		if (Player.player == null) {
			Player.player = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.tag == "Monster") {
			GameController.controller.GameOver();
		}
	}
}
