using System.Collections;
using System.Collections.Generic;
using Core.Input;
using UnityEngine;
using UnityInput = UnityEngine.Input;

[RequireComponent(typeof(PlacementManager))]
	public class TowerDefenseKeyboardMouseInput : KeyboardMouseInput
	{
		/// <summary>
		/// Cached eference to gameUI
		/// </summary>
		PlacementManager placeManager;

		public PlacementItem[] testTower;
		/// <summary>
		/// Register input events
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			
			placeManager = GetComponent<PlacementManager>();

			if (InputController.InstanceExist)
			{
				InputController controller = InputController.Instance;

				controller.tapped += OnTap;
				controller.mouseMoved += OnMouseMoved;
			}
		}

		/// <summary>
		/// Deregister input events
		/// </summary>
		protected override void OnDisable()
		{
			if (!InputController.InstanceExist)
			{
				return;
			}

			InputController controller = InputController.Instance;

			controller.tapped -= OnTap;
			controller.mouseMoved -= OnMouseMoved;
		}

		/// <summary>
		/// Handle camera panning behaviour
		/// </summary>
		protected override void Update()
		{
			base.Update();
			
			// Escape handling
			if (UnityInput.GetKeyDown(KeyCode.Escape))
			{
				switch (placeManager.buildState)
				{
					case PlacementManager.BuildState.Normal:
						if (placeManager.isTowerSelected)
						{
							placeManager.DeselectItem();
						}
						else
						{
							placeManager.Pause();
						}
						break;
					case PlacementManager.BuildState.Paused:
						placeManager.Unpause();
						break;
					case PlacementManager.BuildState.BuildingWithDrag:
					case PlacementManager.BuildState.Building:
						placeManager.CancelGhostPlacement();
						break;
					
				}
			}
		
			// place towers with keyboard numbers
			int towerLibraryCount = testTower.Length;

			// find the lowest value between 9 (keyboard numbers)
			// and the amount of towers in the library
			int count = Mathf.Min(9, towerLibraryCount);
			KeyCode highestKey = KeyCode.Alpha1 + count;

			for (var key = KeyCode.Alpha1; key < highestKey; key++)
			{
				// add offset for the KeyCode Alpha 1 index to find correct keycodes
				if (UnityInput.GetKeyDown(key))
				{
					PlacementItem controller = testTower[key - KeyCode.Alpha1];
					if (placeManager.isBuilding)
					{
						placeManager.CancelGhostPlacement();
					}

					PlacementManager.Instance.SetToBuildMode(controller);
					PlacementManager.Instance.TryMoveGhost(InputController.Instance.basicMouseInfo);
					break;
				}
			}
		}

		/// <summary>
		/// Ghost follows pointer
		/// </summary>
		void OnMouseMoved(PointerInfo pointer)
		{
			Debug.Log("OnMouseMoved");
			// We only respond to mouse info
			var mouseInfo = pointer as MouseCursorInfo;

			if ((mouseInfo != null) && (placeManager.isBuilding))
			{
				placeManager.TryMoveGhost(pointer, false);
			}
		}

		/// <summary>
		/// Select towers or position ghosts
		/// </summary>
		void OnTap(PointerActionInfo pointer)
		{
			// We only respond to mouse info
			var mouseInfo = pointer as MouseButtonInfo;

			if (mouseInfo != null && !mouseInfo.startedOverUI)
			{
				if (placeManager.isBuilding)
				{
					if (mouseInfo.mouseButtonId == 0) // LMB confirms
					{
						placeManager.TryPlaceTower(pointer);
					}
					else // RMB cancels
					{
						placeManager.CancelGhostPlacement();
					}
				}
				else
				{
					if (mouseInfo.mouseButtonId == 0)
					{
						// select towers
						placeManager.TrySelectItem(pointer);
					}
				}
			}
		}
	}