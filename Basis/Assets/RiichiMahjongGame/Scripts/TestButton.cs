using System;
using Basis;
using Basis.Network.Core;
using Basis.Scripts.BasisSdk.Interactions;
using Basis.Scripts.Device_Management.Devices;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace hetarikia.Mahjong
{
	[Cilboxable]
	[RequireComponent(typeof(BasisInteractableButton))]
	public class TestButton : MonoBehaviour
	{
		[SerializeField]
		private Transform tiles;
		private BasisInteractableButton shuffleButton;
		private BasisNetworkShim networkShim;
		private int counter = 0;

		void Start()
		{
			networkShim = SafeUtil.MakeNetworkable(this);
			Debug.Log(networkShim);
			networkShim.NetworkMessageReceived = OnNetworkMessage;
			if (
				TryGetComponent<BasisInteractableButton>(
					out BasisInteractableButton button
				)
			)
			{
				shuffleButton = button;
				shuffleButton.OnInteractStartEvent.AddListener(OnButtonClick);
			}
		}

		private void OnButtonClick(BasisInput input)
		{
			Debug.Log("shuffle button clicked");
			networkShim.TakeOwnership();
			// TODO Add if to check if owner before running below
			for (int i = tiles.childCount - 1; i >= 1; i--)
			{
				Transform randomTile = tiles.GetChild(UnityEngine.Random.Range(0, i));
				Vector3 tempTilePosition = tiles.GetChild(i).position;
				tiles.GetChild(i).position = randomTile.position;
				randomTile.position = tempTilePosition;
			}
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
