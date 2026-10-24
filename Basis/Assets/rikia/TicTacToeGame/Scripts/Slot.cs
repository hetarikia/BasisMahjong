using System;
using Basis.Scripts.BasisSdk.Interactions;
using Basis.Scripts.Device_Management.Devices;
using UnityEngine;

namespace rikia
{
	[Cilboxable]
	[RequireComponent(typeof(BasisInteractableButton))]
	public class Slot : MonoBehaviour
	{
		[NonSerialized]
		public int idx; // This is the slot being clicked on
		private GameManager gameManager; // Used to manage the blendshape of the slot, depending on which player clicked on it
		private BasisInteractableButton interactableButton;

		public void _Init(GameManager gameManager, int idx)
		{
			this.gameManager = gameManager;
			this.idx = idx;

			interactableButton = GetComponent<BasisInteractableButton>();
			interactableButton.OnInteractStartEvent.AddListener(OnInteractStart);
		}

		public void OnInteractStart(BasisInput Input)
		{
			gameManager.sendToggleSlotState(idx);
		}
	}
}
