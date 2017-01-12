using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour {

	public float speed;
	public float healthSpawnTime;
	public Transform[] spawnLocations;

	private bool alive;
	private float healthTimer;

	// Use this for initialization
	void Start () {
		
		healthTimer = healthSpawnTime;
		Hide ();
		Spawn ();
		
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.Rotate (Vector3.up, speed * Time.deltaTime);

		if (alive == false) {
			healthTimer -= Time.deltaTime;
		}
		if (healthTimer <= 0) {
			alive = true;
			healthTimer = healthSpawnTime;
			Spawn ();
		}
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer ("Players")) {

			TankHealth tankHealth = other.GetComponent<TankHealth> ();

			tankHealth.HealthPack ();
			alive = false;
			Hide ();
			//Destroy(gameObject);
		}

	}

	private void Spawn()
	{
		int possibilities = spawnLocations.Length;

		//Debug.Log ("Possibilities: " + possibilities);

		int choice = Random.Range (0, possibilities);

		//Debug.Log("Choice: " + choice);

		transform.position = spawnLocations [choice].position;

		alive = true;
	}

	private void Hide()
	{
		transform.position = new Vector3 (0.0f, 100.0f, 0.0f);

	}
}
