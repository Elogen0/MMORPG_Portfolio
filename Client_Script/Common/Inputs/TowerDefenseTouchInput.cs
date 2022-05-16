using Core.Input;
using UnityEngine;
using UnityEngine.UI;
using UnityInput = UnityEngine.Input;

namespace TowerDefense.Input
{
	[RequireComponent(typeof(PlacementManager))]
	public class TowerDefenseTouchInput : TouchInput
	{
		/// <summary>
		/// A percentage of the screen where panning occurs while dragging
		/// </summary>
		[Range(0, 0.5f)]
		public float panAreaScreenPercentage = 0.2f;

		/// <summary>
		/// The object that holds the confirmation buttons
		/// </summary>
		public Button confirmationButtons;

		/// <summary>
		/// The object that holds the invalid selection
		/// </summary>
		public Button invalidButtons;

		/// <summary>
		/// The attached Game UI object
		/// </summary>
		PlacementManager mPlacePlacementManager;

		/// <summary>
		/// Keeps track of whether or not the ghost tower is selected
		/// </summary>
		bool m_IsGhostSelected;

		/// <summary>
		/// The pointer at the edge of the screen
		/// </summary>
		TouchInfo m_DragPointer;

		/// <summary>
		/// Called by the confirm button on the UI
		/// </summary>
		public void OnTowerPlacementConfirmation()
		{
			confirmationButtons.enabled = false;
			if (!mPlacePlacementManager.IsGhostAtValidPosition())
			{
				return;
			}
			mPlacePlacementManager.PutItem();
		}

		/// <summary>
		/// Called by the close button on the UI
		/// </summary>
		public void Cancel()
		{
			PlacementManager.Instance.CancelGhostPlacement();
			confirmationButtons.enabled = false;
			invalidButtons.enabled = false;
		}

		/// <summary>
		/// Register input events
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			
			mPlacePlacementManager = GetComponent<PlacementManager>();
			
			mPlacePlacementManager.stateChanged += OnStateChanged;
			mPlacePlacementManager.ghostBecameValid += OnGhostBecameValid;

			// Register tap event
			if (InputController.InstanceExist)
			{
				InputController.Instance.tapped += OnTap;
				InputController.Instance.startedDrag += OnStartDrag;
			}

			// disable pop ups
			confirmationButtons.enabled = false;
			invalidButtons.enabled = false;
		}

		/// <summary>
		/// Deregister input events
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			
			if (confirmationButtons != null)
			{
				confirmationButtons.enabled = false;
			}
			if (invalidButtons != null)
			{
				invalidButtons.enabled = false;
			}
			if (InputController.InstanceExist)
			{
				InputController.Instance.tapped -= OnTap;
				InputController.Instance.startedDrag -= OnStartDrag;
			}
			if (mPlacePlacementManager != null)
			{
				mPlacePlacementManager.stateChanged -= OnStateChanged;
				mPlacePlacementManager.ghostBecameValid -= OnGhostBecameValid;
			}
		}

		/// <summary>
		/// Hide UI 
		/// </summary>
		protected virtual void Awake()
		{
			if (confirmationButtons != null)
			{
				confirmationButtons.enabled = false;
			}
			if (invalidButtons != null)
			{
				invalidButtons.enabled = false;
			}
		}

		/// <summary>
		/// Decay flick
		/// </summary>
		protected override void Update()
		{
			base.Update();

			// Edge pan
			if (m_DragPointer != null)
			{
				EdgePan();
			}

			if (UnityInput.GetKeyDown(KeyCode.Escape))
			{
				switch (mPlacePlacementManager.buildState)
				{
					case PlacementManager.BuildState.Normal:
						if (mPlacePlacementManager.isTowerSelected)
						{
							mPlacePlacementManager.DeselectItem();
						}
						else
						{
							mPlacePlacementManager.Pause();
						}
						break;
					case PlacementManager.BuildState.Building:
						mPlacePlacementManager.CancelGhostPlacement();
						break;
				}
			}
		}

		/// <summary>
		/// Called on input press
		/// </summary>
		protected override void OnPress(PointerActionInfo pointer)
		{
			base.OnPress(pointer);
			var touchInfo = pointer as TouchInfo;
			// Press starts on a ghost? Then we can pick it up
			if (touchInfo != null)
			{
				if (mPlacePlacementManager.buildState == PlacementManager.BuildState.Building)
				{
					m_IsGhostSelected = mPlacePlacementManager.IsPointerOverGhost(pointer);
					if (m_IsGhostSelected)
					{
						m_DragPointer = touchInfo;
					}
				}				
			}
		}

		/// <summary>
		/// Called on input release, for flicks
		/// </summary>
		protected override void OnRelease(PointerActionInfo pointer)
		{
			// Override normal behaviour. We only want to do flicks if there's no ghost selected
			// For this reason, we intentionally do not call base
			var touchInfo = pointer as TouchInfo;

			if (touchInfo != null)
			{
				// Show UI on release
				if (mPlacePlacementManager.isBuilding)
				{
					Vector2 screenPoint = cameraRig.cachedCamera.WorldToScreenPoint(mPlacePlacementManager.GetGhostPosition());
					if (mPlacePlacementManager.IsGhostAtValidPosition())
					{
						confirmationButtons.enabled = true;
						invalidButtons.enabled = false;
						TryMove(confirmationButtons.GetComponent<RectTransform>(), screenPoint); 
					}
					else
					{
						invalidButtons.enabled = true;
						confirmationButtons.enabled = false;
						TryMove(confirmationButtons.GetComponent<RectTransform>(), screenPoint);
					}
					if (m_IsGhostSelected)
					{
						mPlacePlacementManager.ReturnToBuildMode();
					}
				}
				if (!m_IsGhostSelected && cameraRig != null)
				{
					// Do normal base behaviour here
					DoReleaseFlick(pointer);
				}
				
				m_IsGhostSelected = false;

				// Reset m_DragPointer if released
				if (m_DragPointer != null && m_DragPointer.touchId == touchInfo.touchId)
				{
					m_DragPointer = null;
				}
			}
		}
		
		public void TryMove(RectTransform rectTransform, Vector3 position)
		{
			Rect rect = rectTransform.rect;
			position += (Vector3) Vector3.up;
			rect.position = position;

			if (rect.xMin < rect.width * 0.5f)
			{
				position.x = rect.width * 0.5f;
			}
			if (rect.xMax > Screen.width - rect.width * 0.5f)
			{
				position.x = Screen.width - rect.width * 0.5f;
			}
			if (rect.yMin < rect.height * 0.5f)
			{
				position.y = rect.height * 0.5f;
			}
			if (rect.yMax > Screen.height - rect.height * 0.5f)
			{
				position.y = Screen.height - rect.height * 0.5f;
			}
			transform.position = position;
		}

		/// <summary>
		/// Called on tap,
		/// calls confirmation of tower placement
		/// </summary>
		protected virtual void OnTap(PointerActionInfo pointerActionInfo)
		{
			var touchInfo = pointerActionInfo as TouchInfo;
			if (touchInfo != null)
			{
				if (mPlacePlacementManager.buildState == PlacementManager.BuildState.Normal && !touchInfo.startedOverUI)
				{
					mPlacePlacementManager.TrySelectItem(touchInfo);
				}
				else if (mPlacePlacementManager.buildState == PlacementManager.BuildState.Building && !touchInfo.startedOverUI)
				{
					mPlacePlacementManager.TryMoveGhost(touchInfo, false);
					if (mPlacePlacementManager.IsGhostAtValidPosition())
					{
						confirmationButtons.enabled = true;
						invalidButtons.enabled = false;
						TryMove(confirmationButtons.GetComponent<RectTransform>(), touchInfo.currentPosition);
					}
					else
					{
						invalidButtons.enabled = true;
						TryMove(invalidButtons.GetComponent<RectTransform>(), touchInfo.currentPosition);
						confirmationButtons.enabled = false;
					}
				}
			}
		}

		/// <summary>
		/// Assigns the drag pointer and sets the UI into drag mode
		/// </summary>
		/// <param name="pointer"></param>
		protected virtual void OnStartDrag(PointerActionInfo pointer)
		{
			var touchInfo = pointer as TouchInfo;
			if (touchInfo != null)
			{
				if (m_IsGhostSelected)
				{
					mPlacePlacementManager.ChangeToDragMode();
					m_DragPointer = touchInfo;
				}
			}
		}
		

		/// <summary>
		/// Called when we drag
		/// </summary>
		protected override void OnDrag(PointerActionInfo pointer)
		{
			// Override normal behaviour. We only want to pan if there's no ghost selected
			// For this reason, we intentionally do not call base
			var touchInfo = pointer as TouchInfo;
			if (touchInfo != null)
			{
				// Try to pick up the tower if it was dragged off
				if (m_IsGhostSelected)
				{
					mPlacePlacementManager.TryMoveGhost(pointer, false);
				}
				
				if (mPlacePlacementManager.buildState == PlacementManager.BuildState.BuildingWithDrag)
				{
					DragGhost(touchInfo);
				}
				else
				{
					// Do normal base behaviour only if no ghost selected
					if (cameraRig != null)
					{
						DoDragPan(pointer);

						if (invalidButtons.enabled)
						{
							TryMove(invalidButtons.GetComponent<RectTransform>(), cameraRig.cachedCamera.WorldToScreenPoint(mPlacePlacementManager.GetGhostPosition()));
						}
						if (confirmationButtons.enabled)
						{
							TryMove(confirmationButtons.GetComponent<RectTransform>(), cameraRig.cachedCamera.WorldToScreenPoint(mPlacePlacementManager.GetGhostPosition()));
						}
					}
				}
			}
		}

		/// <summary>
		/// Drags the ghost
		/// </summary>
		void DragGhost(TouchInfo touchInfo)
		{
			if (touchInfo.touchId == m_DragPointer.touchId)
			{
				mPlacePlacementManager.TryMoveGhost(touchInfo, false);

				if (invalidButtons.enabled)
				{
					invalidButtons.enabled = false;
				}
				if (confirmationButtons.enabled)
				{
					confirmationButtons.enabled = false;
				}
			}
		}

		/// <summary>
		/// pans at the edge of the screen
		/// </summary>
		void EdgePan()
		{
			float edgeWidth = panAreaScreenPercentage * Screen.width;
			PanWithScreenCoordinates(m_DragPointer.currentPosition, edgeWidth, panSpeed);
		}
		

		/// <summary>
		/// If the new state is <see cref="GameUI.State.Building"/> then move the ghost to the center of the screen
		/// </summary>
		/// <param name="previousState">
		/// The previous the GameUI was is in
		/// </param>
		/// <param name="currentState">
		/// The new state the GameUI is in
		/// </param>
		void OnStateChanged(PlacementManager.BuildState previousState, PlacementManager.BuildState currentState)
		{
			// Early return for two reasons
			// 1. We are not moving into Build Mode
			// 2. We are not actually touching
			if (UnityInput.touchCount == 0)
			{
				return;
			}
			if (currentState == PlacementManager.BuildState.Building && previousState != PlacementManager.BuildState.BuildingWithDrag)
			{
				mPlacePlacementManager.MoveGhostToCenter();
				confirmationButtons.enabled = false;
				invalidButtons.enabled = false;
			}
			if (currentState == PlacementManager.BuildState.BuildingWithDrag)
			{
				m_IsGhostSelected = true;
			}
		}

		/// <summary>
		/// Displays the correct confirmation buttons when the tower has become valid
		/// </summary>
		void OnGhostBecameValid()
		{
			// this only needs to be done if the invalid buttons are already on screen
			if (!invalidButtons.enabled)
			{
				return;
			}
			Vector2 screenPoint = cameraRig.cachedCamera.WorldToScreenPoint(mPlacePlacementManager.GetGhostPosition());
			if (!confirmationButtons.enabled)
			{
				confirmationButtons.enabled = true;
				invalidButtons.enabled = false;
				TryMove(confirmationButtons.GetComponent<RectTransform>(), screenPoint);
			}
		}
	}
}