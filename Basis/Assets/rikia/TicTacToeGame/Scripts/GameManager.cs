using UnityEngine;

namespace rikia
{
	[Cilboxable]
	public class GameManager : MonoBehaviour
	{
		public NetworkManager networkManager;
		public GameObject[] gamePieces;
		public Slot[] slots;

		/**
			1 byte - Turn count
			9 bytes - Each slot state (0: empty, 1: X, 2: O)
		*/
		private byte[] localGameState = new byte[11];

		void Start()
		{
			networkManager._Init(this);
			for (int i = 0; i < slots.Length; i++)
			{
				slots[i]._Init(this, i);
			}
		}

		public void TriggerToggleSlotState(int idx)
		{
			if (localGameState[0] < 9 && !IsSlotOccupied(idx))
			{
				ToggleSlot(idx);
				networkManager.OnSlotClick(idx);
			}
		}

		public void ToggleSlot(int idx)
		{
			Debug.Log($"turnCount: {(int)localGameState[0]}");
			Debug.Log($"Slot: {idx} was clicked!");
			gamePieces[GetSlotNumber(idx)].SetActive(true);
			localGameState[0]++;
		}

		private int GetSlotNumber(int idx)
		{
			return IsEven(localGameState[0]) ? idx * 2 : (idx * 2) + 1;
		}

		private bool IsEven(int num)
		{
			return num % 2 == 0;
		}

		private bool IsSlotOccupied(int idx)
		{
			return gamePieces[idx * 2].activeSelf
				|| gamePieces[(idx * 2) + 1].activeSelf;
		}
	}
}
