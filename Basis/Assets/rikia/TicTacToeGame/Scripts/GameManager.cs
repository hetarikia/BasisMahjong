using Basis;
using Basis.Network.Core;
using Basis.Scripts.BasisSdk.Interactions;
using Basis.Scripts.Device_Management.Devices;
using Basis.Scripts.Networking.NetworkedAvatar;
using UnityEngine;

[Cilboxable]
public class GameManager : MonoBehaviour
{
	public BasisInteractableButton toggleButton;

	// This is used to send data through the network
	private BasisNetworkShim network;
	public GameObject cube;
	public GameObject sphere;

	// This is the game state
	private bool state = true;

	void Start()
	{
		// First we need to instiate the nework shim.
		network = SafeUtil.MakeNetworkable(this);
		// Next we need to set up the various callbacks for the network events.
		// These are required to handle the actual networking part of the game!
		// The purpose of these functions are explained more below.
		network.NetworkMessageReceived = OnNetworkMessage;
		network.PlayerJoined = OnPlayerJoined;
		network.PlayerLeft = OnPlayerLeft;

		// Lastly we add the event listener for our button. This is what causes the objects to be toggled and what
		// sends out the game state network message!
		toggleButton.OnInteractStartEvent.AddListener(OnButtonClick);
	}

	// This function is only called by the player that clicked the button.
	// Used to trigger a state change for the game
	private void OnButtonClick(BasisInput input)
	{
		// First take ownership of the game so this player is now considered the source of truth for the game state.
		network.TakeOwnership();
		// Flip the state.
		state = !state;
		// Then toggle the objects for this player.
		ToggleObjects();

		network.SendCustomNetworkEvent(
			new byte[] { state ? (byte)1 : (byte)0 },
			DeliveryMethod.ReliableOrdered
		);
	}

	private void ToggleObjects()
	{
		cube.SetActive(state);
		sphere.SetActive(!state);
	}

	// When a player receives a message over the network, this function is called
	// This is what is used to sync the game state with the rest of the players
	private void OnNetworkMessage(
		ushort playerID,
		byte[] buffer,
		DeliveryMethod deliveryMethod
	)
	{
		state = buffer[0] == 1 ? true : false;
		ToggleObjects(); // Toggle the object
	}

	// When a new player joins they need to be sent the game state so they're synced with everyone else.
	// This function is called for every player in the instance, but only sends the game state if that
	// player is the game owner. This ensure the state stays consistent for all players.
	private void OnPlayerJoined(BasisNetworkPlayer player)
	{
		// Only send the state if the calling player is the network owner of the game
		if (network.IsLocalOwner())
		{
			network.SendCustomNetworkEvent(
				new byte[] { state ? (byte)1 : (byte)0 },
				DeliveryMethod.ReliableOrdered,
				new ushort[] { player.playerId }
			);
		}
	}

	// When a player leaves, if that player was the game owner we need to assign a new owner.
	// So every player attempts to request to be the owner if there is none until one of them are the new owner.
	private void OnPlayerLeft(BasisNetworkPlayer player)
	{
		network.RequestOwnershipIfNone();
	}
}
