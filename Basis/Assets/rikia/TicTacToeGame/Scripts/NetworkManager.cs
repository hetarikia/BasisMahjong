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
		private GameManager gameManager;

		public void _Init(GameManager gameManager)
		{
			this.gameManager = gameManager;
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

		public void OnSlotClick(int idx)
		{
			net.TakeOwnership();
			net.SendCustomNetworkEvent(
				new byte[] { MessageSlotClick, (byte)idx },
				DeliveryMethod.ReliableOrdered
			);
		}

		private void OnNetworkMessage(
			ushort PlayerID,
			byte[] buffer,
			DeliveryMethod DeliveryMethod
		)
		{
			if (buffer == null || buffer.Length == 0)
				return;
			byte Prefix = buffer[0];
			if (Prefix == MessageSlotClick)
			{
				gameManager.toggleSlot(buffer[1]);
			}
		}

		private void sendToAll(byte[] buffer)
		{
			if (net == null || !net.HasNetworkID)
				return;
			net.SendCustomNetworkEvent(buffer, DeliveryMethod.ReliableOrdered, null);
		}
	}
}
