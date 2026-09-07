using System;
using Unity.XR.CoreUtils;
using UnityEngine;

public class TestButton : MonoBehaviour
{
	[SerializeField]
	private Transform tiles;

	public void OnButtonClick()
	{
		for (int i = tiles.childCount - 1; i >= 1; i--)
		{
			Transform randomTile = tiles.GetChild(UnityEngine.Random.Range(0, i));
			Vector3 tempTilePosition = tiles.GetChild(i).position;
			tiles.GetChild(i).position = randomTile.position;
			randomTile.position = tempTilePosition;
		}
	}
}
