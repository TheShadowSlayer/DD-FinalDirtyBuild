using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thuleanx.Utils;

public class UnitManager : MonoBehaviour
{
    static public List<UnitData> CurrentUnits = new List<UnitData>();

	public Transform UnitParent;
	public List<UnitTemplate> Templates; //possible unit types
	public GameObject UnitPrefab;

	private void Awake()
	{
		for (int i = 0; i < 10; i++) Generate(); //on startup, give player 10 random units;
	}

	private void Generate()
	{
		//get random unit
		int r = Calc.RandomRange(0, Templates.Count);
		UnitTemplate template = Templates[r];

		//generate unit data and add it to your overall units list
		UnitData data = template.GenerateData();
		CurrentUnits.Add(data);

		//spawn unit AI object
		GameObject obj = GameObject.Instantiate(
			UnitPrefab,
			Vector3.zero,
			Quaternion.identity,
			UnitParent
		);
		obj.transform.localPosition = new Vector3(Random.Range(-5.5f, 6f), Random.Range(-2f, -4.5f), 0); // <-hard-coded boundaries of igloo floor (in terms of canvas)
		UnitTempDisplay Display = obj.GetComponent<UnitTempDisplay>();
		Display.SetData(data);
	}
}
