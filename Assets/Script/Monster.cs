using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType {SmallNormal, BigNormal, SmallFire, BigFire, Void, Plant, Stone}

public enum CompareType {Bigger, Smaller, Equal, Other}

[Serializable]
public class TransformCondiction {
	public string name;
	public GameObject turnTo;
	public CompareType compareType;
	public int condictionHealth;
}

public class Monster : MonoBehaviour {

	public static float speedMultiplier = 1;

	public MonsterType type;
	public int health;
	public int maxHealth;
	public float speed = 1;
	public TransformCondiction[] transformCondictions;

	bool transformed = false;

	void Update () {
		Vector3 diff = Player.player.transform.position - transform.position;
		float dis = Mathf.Sqrt((diff.x * diff.x) + (diff.y * diff.y));
		transform.Translate(diff * (speed * speedMultiplier / dis) * Time.deltaTime);
	}

	void HandleDeath () {
		Destroy(gameObject);
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.tag == "Weapone") {
			Weapone weapone = coll.gameObject.GetComponent<Weapone>();

			int damage = weapone.damage;
			bool destoryWeapone = true;

			if (weapone.type == WeaponeType.Bomb) {return;}
			else if (weapone.type == WeaponeType.Bullet) {
				switch (type) {
					case MonsterType.SmallFire:
					case MonsterType.BigFire:
					case MonsterType.Void:
						return; // Bullet passed through these type
					default:
						break; // Bullet affect these type
				}
			}
			else if (weapone.type == WeaponeType.Ham) {
				switch (type) {
					case MonsterType.SmallNormal:
					case MonsterType.BigNormal:
						break; // Ham affect these type
					case MonsterType.Plant:
						damage = 0; // Ham don't affect these type
						break;
					case MonsterType.Stone:
						destoryWeapone = false; // Ham don't affect these type, will bounce off
						break;
					default:
						return;
				}
			}
			else if (weapone.type == WeaponeType.Fire) {
				switch (type) {
					case MonsterType.SmallNormal:
					case MonsterType.BigNormal:
						break;
					case MonsterType.Plant:
						damage *= 8;
						break;
					case MonsterType.SmallFire:
					case MonsterType.BigFire:
						damage *= -1;
						break;
					case MonsterType.Stone:
						damage /= 2;
						break;
					default:
						return;
				}
			}
			else if (weapone.type == WeaponeType.Spear) {
				switch (type) {
					case MonsterType.SmallFire:
					case MonsterType.BigFire:
					case MonsterType.Void:
						return;
					default:
						break;
				}

				destoryWeapone = false;
			}
			else if (weapone.type == WeaponeType.Water) {
				switch (type) {
					case MonsterType.SmallFire:
					case MonsterType.BigFire:
						break;
					case MonsterType.Stone:
						damage /= 2;
						break;
					case MonsterType.Plant:
						damage *= -1;
						break;
					case MonsterType.Void:
						return;
					default:
						damage = 0;
						break;
				}
			}
			else if (weapone.type == WeaponeType.Vine) {
				switch (type) {
					case MonsterType.SmallNormal:
					case MonsterType.BigNormal:
					case MonsterType.Stone:
						destoryWeapone = false;
						break;
					case MonsterType.SmallFire:
					case MonsterType.BigFire:
						damage *= -1;
						break;
					case MonsterType.Plant:
						damage *= -1;
						destoryWeapone = false;
						break;
					default:
						return;
				}
			}

			Damage(damage);

			if (destoryWeapone) {
				weapone.Destroy();
			}
		}
	}

	public void Damage (int amount) {
		health -= amount;

		if (health <= 0) {
			HandleDeath();
			return;
		}
		else if (health > maxHealth) {
			health = maxHealth;
		}

		CheckTransform();
	}

	void CheckTransform () {
		if (transformed) {return;}

		foreach (TransformCondiction tcon in transformCondictions) {
			if (tcon.compareType == CompareType.Bigger && health > tcon.condictionHealth) {
				TransformTo(tcon.turnTo);
				return;
			}
			else if (tcon.compareType == CompareType.Smaller && health < tcon.condictionHealth)  {
				TransformTo(tcon.turnTo);
				return;
			}
			else if (tcon.compareType == CompareType.Equal && health == tcon.condictionHealth) {
				TransformTo(tcon.turnTo);
				return;
			}
		}
	}

	void TransformTo (GameObject prefab) {
		transformed = true;
		GameObject newMonster = Instantiate(prefab, transform.position, Quaternion.identity);
		newMonster.transform.SetParent(GameController.controller.monstersSet);

		HandleDeath();
	}
}
