using Basis;
using Basis.Network.Core;
using UnityEngine;

namespace rikia
{
	[Cilboxable]
	public class NetworkManager : MonoBehaviour
	{
		private const byte MessageState = 0;
		private const byte MessageSlotClick = 1;

		private BasisNetworkShim net;

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start() { }

		// Update is called once per frame
		void Update() { }

		public void _Init()
		{
			net = SafeUtil.MakeNetworkable(this);
			if (net != null)
			{
				// net.NetworkReady += OnNetworkReady;
				net.NetworkMessageReceived += OnNetworkMessage;
				// net.OwnershipTransfer += OnOwnershipTransfer;
				// net.PlayerJoined += OnPlayerJoined;
				// net.PlayerLeft += OnPlayerLeft;
			}
		}

		public void OnSlotClick(int idx) { }

		private void OnNetworkMessage(
			ushort PlayerID,
			byte[] buffer,
			DeliveryMethod DeliveryMethod
		) { }

		private void sendToAll(byte[] buffer)
		{
			if (net == null || !net.HasNetworkID)
				return;
			net.SendCustomNetworkEvent(buffer, DeliveryMethod.ReliableOrdered, null);
		}
	}
}
