using Basis;
using Basis.Network.Core;
using Basis.Scripts.Networking.NetworkedAvatar;
using UnityEngine;

namespace rikia
{
	[Cilboxable]
	public class NetworkManager : MonoBehaviour
	{
		private const byte MessageState = 0;
		private const byte MessageSlotClick = 1;
		private const byte MessageResetGame = 2;

		/**
			1 byte - Turn count
			9 bytes - Each slot state (0: empty, 1: X, 2: O)
		*/
		private byte[] networkGameState = new byte[10];

		private BasisNetworkShim net;
		private GameManager gameManager;

		public void _Init(GameManager gameManager)
		{
			this.gameManager = gameManager;
			net = SafeUtil.MakeNetworkable(this);
			if (net != null)
			{
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
				DeliveryMethod.ReliableOrdered,
				null
			);
		}

		public bool IsLocalOwner()
		{
			return net != null && net.IsLocalOwner();
		}

		private void OnNetworkMessage(
			ushort PlayerID,
			byte[] buffer,
			DeliveryMethod DeliveryMethod
		)
		{
			Debug.Log("network event received");
			if (buffer == null || buffer.Length == 0)
				return;
			byte Prefix = buffer[0];
			if (Prefix == MessageSlotClick)
			{
				gameManager.ToggleSlot(buffer[1]);
			}
			else if (Prefix == MessageState)
			{
				// #NOTE need to update network sync state before doing this
				// byte[] sync
			}
		}

		private void OnPlayerJoined(BasisNetworkPlayer player)
		{
			if (player == null || !IsLocalOwner())
				return;
			net.SendCustomNetworkEvent(
				GetStateMessage(),
				DeliveryMethod.ReliableOrdered,
				new ushort[] { player.playerId }
			);
		}

		private byte[] GetStateMessage()
		{
			byte[] message = new byte[10];
			message[0] = MessageState;
			for (int i = 1; i < 11; i++)
			{
				message[i] = networkGameState[i - 1];
			}
			return message;
		}

		private void SendToAll(byte[] buffer)
		{
			if (net == null || !net.HasNetworkID)
				return;
			net.SendCustomNetworkEvent(buffer, DeliveryMethod.ReliableOrdered, null);
		}
	}
}
