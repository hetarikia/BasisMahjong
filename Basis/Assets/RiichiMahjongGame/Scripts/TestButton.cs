using System;
using Basis;
using Basis.Scripts.BasisSdk.Interactions;
using Basis.Scripts.Device_Management.Devices;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace hetarikia.Mahjong
{
	[Cilboxable]
	public class TestButton : BasisNetworkShim
	{
		[SerializeField]
		private Transform tiles;
		private BasisInteractableButton shuffleButton;

		public override void Start()
		{
			shuffleButton = GetComponent<BasisInteractableButton>();
			shuffleButton.OnInteractStartEvent.AddListener(OnButtonClick);
			base.Start();
		}

		private void OnButtonClick(BasisInput input)
		{
			TakeOwnership();
			for (int i = tiles.childCount - 1; i >= 1; i--)
			{
				Transform randomTile = tiles.GetChild(UnityEngine.Random.Range(0, i));
				Vector3 tempTilePosition = tiles.GetChild(i).position;
				tiles.GetChild(i).position = randomTile.position;
				randomTile.position = tempTilePosition;
			}
		}
	}
}
