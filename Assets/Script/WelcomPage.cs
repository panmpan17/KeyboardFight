using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WelcomPage : MonoBehaviour {
	bool transitioning = false;
	Animator anim;

	void Awake () {
		anim = GetComponent<Animator>();
	}

	public void Fight () {
		if (!transitioning) {
			anim.SetTrigger("FadeOut");
			bool transitioning = true;
		}
	}

	public void FinishedFadeOut () {
		SceneManager.LoadScene("Game");
	}
}
