using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	private Unit source;
	private Unit target;
	private Vector3 sourcePos;
	private Vector3 targetPos;
	private float startTime;
	private float duration;

	// Update is called once per frame
	void Update () {
		float time = Time.time;

		if (target != null) { 
			gameObject.transform.position = Vector3.Lerp (
				sourcePos, target.gameObject.transform.position, 
				(time - startTime) / duration);
		} else {
			gameObject.transform.position = Vector3.Lerp (
				sourcePos, targetPos, 
				(time - startTime) / duration);
		}
	}

	void OnTriggerEnter(Collider collider) {
		Debug.Log ("Colliding with " + collider.ToString());
		target = null;
		sourcePos = targetPos = gameObject.transform.position;
	}

	public void Fire (Unit source, Unit target, float projectileSpeed) {
		this.source = source;
		this.target = target;
		this.sourcePos = source.gameObject.transform.position;
		this.targetPos = Vector3.zero;
		this.startTime = Time.time;
		this.duration = (target.gameObject.transform.position - sourcePos).magnitude / projectileSpeed;
		//this.gameObject.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
		this.gameObject.transform.LookAt (target.gameObject.transform.position);
		this.gameObject.transform.Rotate(new Vector3(90, 0, 0));
	}

	public void Fire (Unit source, Vector3 targetPos, float projectileSpeed) {
		this.source = source;
		this.target = null;
		this.sourcePos = source.gameObject.transform.position;
		this.targetPos = targetPos;
		this.startTime = Time.time;
		this.duration = (targetPos - sourcePos).magnitude / projectileSpeed;
		//this.gameObject.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
		this.gameObject.transform.LookAt (targetPos);
		this.gameObject.transform.Rotate(new Vector3(0, 0, 90));
	}
}
