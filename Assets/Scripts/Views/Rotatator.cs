using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatator : MonoBehaviour {
	[SerializeField] Vector3 rotation;
	[SerializeField] Transform meshObject = null;
	[SerializeField] float rotationSpeed = 0;
	[SerializeField] bool randomize;
	
	[SerializeField] float maxSpeed;
	[SerializeField] float minSpeed;
	
	public bool Randomize 
	
	{
		get {
			return randomize;
		}
	}
	

	void Start (){
		if (randomize)
		{
			rotation = new Vector3(RandFloat(), RandFloat(), RandFloat());
			rotationSpeed = Random.Range(minSpeed,maxSpeed);
		}
	}
	
	float RandFloat()
	{
		return Random.Range(0f,1.01f);
	}
	
	void FixedUpdate()
	{
		if (meshObject != null)
		{
			meshObject.Rotate(rotation, rotationSpeed * Time.deltaTime);
		}
	}
	void Update()
	{
		transform.Rotate(rotation, rotationSpeed * Time.deltaTime);
	}
}
