using System;
using Basis;
using Basis.Network.Core;
using Basis.Scripts.BasisSdk.Interactions;
using Basis.Scripts.Device_Management.Devices;
using Basis.Scripts.Networking.Sync;
using Basis.Shims;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace hetarikia.Mahjong
{
	[Cilboxable]
	public class TestButton : MonoBehaviour
	{
		[SerializeField]
		private Transform tilesParent;

		[SerializeField]
		private Transform dummyTilesParent;

		[SerializeField]
		private BasisInteractableButton shuffleButton;

		private BasisTransformSyncShim basisTransformSyncShim;
		private BasisNetworkShim networkShim;
		private int counter = 0;
		private Transform[] tiles;

		void Start()
		{
			networkShim = SafeUtil.MakeNetworkable(this);
			networkShim.NetworkMessageReceived = OnNetworkMessage;
			basisTransformSyncShim = new BasisTransformSyncShim();
			basisTransformSyncShim.Channels = BasisTransformSyncShim.ChannelPose;
			shuffleButton.OnInteractStartEvent.AddListener(OnButtonClick);
			tiles = tilesParent.GetComponentsInChildren<Transform>();
		}

		private void OnButtonClick(BasisInput input)
		{
			networkShim.TakeOwnership();
			Vector3[] newDummyTilePositions = new Vector3[dummyTilesParent.childCount];
			// TODO Add if to check if owner before running below
			for (int i = dummyTilesParent.childCount - 1; i >= 1; i--)
			{
				Transform currentTile = dummyTilesParent.GetChild(i);
				Transform randomTile = dummyTilesParent.GetChild(
					UnityEngine.Random.Range(0, i)
				);
				Vector3 tempTilePosition = currentTile.position;

				currentTile.position = randomTile.position;
				randomTile.position = tempTilePosition;
				newDummyTilePositions[i] = currentTile.position;
			}
			newDummyTilePositions[0] = dummyTilesParent.GetChild(0).position;
			BasisTransformSyncShim.SetValues(
				tiles,
				BasisTransformSyncShim.SpaceWorld,
				newDummyTilePositions,
				null,
				null
			);
			counter++;
			networkShim.SendCustomNetworkEvent(
				BitConverter.GetBytes(counter),
				DeliveryMethod.ReliableOrdered
			);
			OnDeserialization();
		}

		public void OnNetworkMessage(
			ushort PlayerID,
			byte[] buffer,
			DeliveryMethod DeliveryMethod
		)
		{
			if (buffer != null && buffer.Length >= 4)
			{
				counter = BitConverter.ToInt32(buffer, 0);
				OnDeserialization();
			}
		}

		public void OnDeserialization()
		{
			Debug.Log($"Button has been clicked {counter} times");
		}
	}
}
