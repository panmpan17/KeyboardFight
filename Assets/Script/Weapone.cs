using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponeType {Bullet, Ham, Bomb, Fire, Spear, LightBeam, Water, Vine}

public class Weapone : MonoBehaviour {

	public WeaponeType type;
	public int damage = 1;

	public float countdown;
	public Collider2D effectArea;

	void Update () {
		if (type == WeaponeType.Ham) {
			transform.Rotate(new Vector3(0, 0, 2));
		}
		else if (type == WeaponeType.Bomb) {
			if (countdown <= 0) {return;}

			countdown -= Time.deltaTime;

			if (countdown <= 0) {
				StartCoroutine(BlowUp());
			}
		}
	}

	public void Destroy () {
		Destroy(gameObject);
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.tag == "Wall") {
			switch (type) {
				case WeaponeType.Bullet:
				case WeaponeType.Ham:
				case WeaponeType.Fire:
				case WeaponeType.Water:
				case WeaponeType.Vine:
					Destroy(gameObject);
					break;
				default:
					break;
			}
		}
	}

	IEnumerator BlowUp () {
		Collider2D[] colliders = new Collider2D[2];
		ContactFilter2D contactFilter = new ContactFilter2D();
		contactFilter.SetLayerMask(LayerMask.GetMask("NormalMonster"));

		int count = effectArea.OverlapCollider(contactFilter, colliders);
		for (int index=0;index < count;index++) {
			colliders[index].gameObject.GetComponent<Monster>().Damage(damage);
		}

		contactFilter.SetLayerMask(LayerMask.GetMask("PlantMonster"));
		count = effectArea.OverlapCollider(contactFilter, colliders);
		for (int index=0;index < count;index++) {
			colliders[index].gameObject.GetComponent<Monster>().Damage(damage);
		}

		contactFilter.SetLayerMask(LayerMask.GetMask("StoneMonster"));
		count = effectArea.OverlapCollider(contactFilter, colliders);
		for (int index=0;index < count;index++) {
			colliders[index].gameObject.GetComponent<Monster>().Damage(damage);
		}

		yield return new WaitForSeconds(0.2f);
		Destroy(gameObject);
	}
}
