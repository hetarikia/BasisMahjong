using UnityEngine;

namespace rikia
{
	[Cilboxable]
	public class GameManager : MonoBehaviour
	{
		public NetworkManager networkManager;
		public GameObject[] gamePieces;
		public Slot[] slots;

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			for (int i = 0; i < slots.Length; i++)
			{
				slots[i]._Init(this, i);
			}
		}

		// // Update is called once per frame
		// void Update() { }

		public void toggleSlotState(int idx)
		{
			Debug.Log($"Slot: {idx} was clicked!");
			gamePieces[idx * 2].SetActive(true);
		}
	}
}
