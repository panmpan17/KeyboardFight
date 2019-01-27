using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActionType {Fade, Func, Delay, ImageFill, Scale, Size}

public class ActionCtl : MonoBehaviour {
	public bool sizeUpdatebc = false;
	BoxCollider2D bc2d;
	Vector2 pivot;
	public bool running = false;

	ComponentType componentType;
	SpriteRenderer spriteRenderer;
	Image image;

	List<Action> actionsList = new List<Action> {};
	int actionIndex;
	float actionTime;

	bool forever = false;

	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		image = GetComponent<Image>();
		bc2d = GetComponent<BoxCollider2D>();

		if (spriteRenderer != null) {
			componentType = ComponentType.SpriteRenderer;
			pivot = new Vector2(((spriteRenderer.sprite.rect.width / 2) - spriteRenderer.sprite.pivot.x) / spriteRenderer.sprite.rect.width,
								((spriteRenderer.sprite.rect.height / 2) - spriteRenderer.sprite.pivot.y) / spriteRenderer.sprite.rect.height);
		}
		else if (image != null) {
			componentType = ComponentType.Image;
		}
	}

	// Update is called once per frame
	void Update () {
		if (running) {
			Action action = actionsList[actionIndex];
			actionTime += Time.unscaledDeltaTime;

			if (action.type == ActionType.Fade) {
				FadeAction faction = (FadeAction) action;

				if (!faction.started) {
					faction.SetupColor(GetColor());
				}

				if (actionTime >= action.time) {
					faction.started = false;
					SetColor(faction.targetColor);
					nextAction();
					return;
				}

				SetColor(Color.Lerp(faction.originalColor, faction.targetColor, actionTime / action.time));
			}
			else if (action.type == ActionType.Func) {
				((FuncAction) action).function();
				nextAction();
			}
			else if (action.type == ActionType.Delay && actionTime >= action.time) {
				nextAction();
			}
			else if (action.type == ActionType.ImageFill) {
				ImageFillAction iaction = (ImageFillAction) action;

				if (!iaction.started) {
					iaction.SetupFill(GetFillAmount());
				}

				if (actionTime >= action.time) {
					iaction.started = false;
					SetFillAmount(iaction.targetFill);
					nextAction();
					return;
				}

				SetFillAmount(Mathf.Lerp(iaction.originalFill, iaction.targetFill, actionTime / action.time));
			}
			else if (action.type == ActionType.Scale) {
				ScaleAction saction = (ScaleAction) action;

				if (!saction.started) {
					saction.SetupScale(transform.localScale);
				}

				if (actionTime >= action.time) {
					saction.started = false;
					transform.localScale = saction.targetScale;
					nextAction();
					return;
				}

				transform.localScale = Vector3.Lerp(saction.originalScale, saction.targetScale, actionTime / action.time);
			}
			else if (action.type == ActionType.Size) {
				SizeAction saction = (SizeAction) action;

				if (!saction.started) {
					saction.SetupSize(GetSize());
				}

				if (actionTime >= action.time) {
					saction.started = false;
					SetSize(saction.targetSize);
					nextAction();
					return;
				}

				Vector2 size = Vector2.Lerp(saction.originalSize, saction.targetSize, actionTime / action.time);
				SetSize(size);

				if (sizeUpdatebc && bc2d != null) {
					bc2d.size = size;
					bc2d.offset = new Vector2(size.x * pivot.x, size.y * pivot.y);
				}
			}
		}
	}

	public void Show () {
		if (componentType == ComponentType.SpriteRenderer) {
			spriteRenderer.color = new Color(1, 1, 1, 1);
		}
		else if (componentType == ComponentType.Image) {
			image.color = new Color(1, 1, 1, 1);
		}
	}

	public void Hide () {
		if (componentType == ComponentType.SpriteRenderer) {
			spriteRenderer.color = new Color(1, 1, 1, 0);
		}
		else if (componentType == ComponentType.Image) {
			image.color = new Color(1, 1, 1, 0);
		}
	}

	public void RunAction (Action action) {
		if (!running) {
			running = true;
			actionsList.Clear();
			actionsList.Add(action);
			actionIndex = 0;
			actionTime = 0;
		}
	}

	public void RunAction (params Action[] actionCollection) {
		if (!running) {
			running = true;
			actionsList.Clear();

			foreach (Action action in actionCollection) {
				actionsList.Add(action);
			}

			actionIndex = 0;
			actionTime = 0;
		}
	}

	public void RunActionForever (Action action) {
		forever = true;
		RunAction(action);
	}

	public void RunActionForever (params Action[] actionCollection) {
		forever = true;
		RunAction(actionCollection);
	}

	public void StopAction () {
		running = false;
		forever = false;
		actionsList.Clear();
		actionIndex = 0;
		actionTime = 0;
	}

	public void GracefulStopForeverAction () {
		forever = false;
	}

	void nextAction () {
		actionTime = 0;
		actionIndex++;

		if (actionIndex >= actionsList.Count) {
			if (forever) {
				actionIndex = 0;
			}
			else {
				running = false;
			}
		}
	}

	Color GetColor () {
		if (componentType == ComponentType.SpriteRenderer) {
			return spriteRenderer.color;
		}
		else if (componentType == ComponentType.Image) {
			return image.color;
		}
		return new Color(0f, 0f, 0f, 0f);
	}

	void SetColor (Color color) {
		if (componentType == ComponentType.SpriteRenderer) {
			spriteRenderer.color = color;
		}
		else if (componentType == ComponentType.Image) {
			image.color = color;
		}
	}

	Vector2 GetSize () {
		return spriteRenderer.size;
	}

	void SetSize (Vector2 size) {
		spriteRenderer.size = size;
	}

	float GetFillAmount () {
		if (image != null) {
			if (image.type == Image.Type.Filled) {
				return image.fillAmount;
			}
		}
		return 0f;
	}

	void SetFillAmount (float fillAmount) {
		if (image != null) {
			if (image.type == Image.Type.Filled) {
				image.fillAmount = fillAmount;
			}
		}
	}

	public static FadeAction FadeIn (float time) {
		return new FadeAction(time, 1);
	}

	public static FadeAction FadeOut (float time) {
		return new FadeAction(time, 0);
	}

	public static FadeAction FadeTo(float time, float opacity) {
		return new FadeAction(time, opacity);
	}

	public static FuncAction CallFunc (Func<bool> func) {
		return new FuncAction(func);
	}

	public static DelayAction DelayTime (float time) {
		return new DelayAction(time);
	}

	public static ImageFillAction FillFull (float time) {
		return new ImageFillAction(time, 1);
	}
	public static ImageFillAction FillEmpty (float time) {
		return new ImageFillAction(time, 0);
	}
	public static ImageFillAction FillTo (float time, float fillAmount) {
		return new ImageFillAction(time, fillAmount);
	}

	public static ScaleAction ScaleTo (float time, float x=1f, float y=1, float z=1) {
		return new ScaleAction(time, new Vector3(x, y, z));
	}

	public static ScaleAction ScaleBy (float time, float x=0f, float y=0f, float z=0) {
		return new ScaleAction(time, new Vector3(x, y, z), true);
	}

	public static SizeAction SizeTo (float time, float x=1f, float y=1f) {
		return new SizeAction(time, new Vector2(x, y));
	}

	public static SizeAction SizeBy (float time, float x=0f, float y=0f) {
		return new SizeAction(time, new Vector2(x, y), true);
	}

	enum ComponentType {SpriteRenderer, Image}

	public class Action {
		public ActionType type;
		public float time;

		public Action () {}

		public Action (ActionType actionType, float actionTime) {
			type = actionType;
			time = actionTime;
		}
	}

	public class FadeAction : Action {
		public bool started = false;

		public Color originalColor;
		public Color targetColor;
		float fadeAlphaTo;

		public FadeAction (float actionTime, float alphaTo) : base(ActionType.Fade, actionTime) {
			fadeAlphaTo = alphaTo;
		}

		public void SetupColor (Color oldColor) {
			started = true;

			originalColor = oldColor;
			targetColor = oldColor;
			targetColor.a = fadeAlphaTo;
		}
	}

	public class FuncAction : Action {
		public Func<bool> function;

		public FuncAction (Func<bool> func) : base(ActionType.Func, 0) {
			function = func;
		}
	}

	public class DelayAction : Action {
		public DelayAction (float actionTime) : base(ActionType.Delay, actionTime) {}
	}

	public class ImageFillAction : Action {
		public bool started = false;

		public float originalFill;
		public float targetFill;

		public ImageFillAction (float actionTime, float fillTo) : base(ActionType.ImageFill, actionTime) {
			targetFill = fillTo;
		}

		public void SetupFill (float fillAmount) {
			started = true;
			originalFill = fillAmount;
		}
	}

	public class ScaleAction : Action {
		public bool started = false;

		public Vector3 originalScale;
		public Vector3 targetScale;
		public bool deltaMode;
		public Vector3 deltaScale;

		public ScaleAction (float actionTime, Vector3 scale, bool delta=false) : base(ActionType.Scale, actionTime) {
			deltaMode = delta;

			if (deltaMode) {
				deltaScale = scale;
			}
			else {
				targetScale = scale;
			}
		}

		public void SetupScale (Vector3 scale) {
			started = true;

			originalScale = scale;

			if (deltaMode) {
				targetScale = scale + deltaScale;
			}
		}
	}

	public class SizeAction : Action {
		public bool started = false;

		public Vector2 originalSize;
		public Vector2 targetSize;
		public bool deltaMode;
		public Vector2 deltaSize;

		public SizeAction (float actionTime, Vector2 size, bool delta=false) : base(ActionType.Size, actionTime) {
			deltaMode = delta;

			if (deltaMode) {
				deltaSize = size;
			}
			else {
				targetSize = size;
			}
		}

		public void SetupSize (Vector2 size) {
			started = true;

			originalSize = size;

			if (deltaMode) {
				targetSize = size + deltaSize;
			}
		}
	}
}
