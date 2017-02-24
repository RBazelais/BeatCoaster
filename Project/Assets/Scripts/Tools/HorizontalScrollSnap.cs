/// Credit BinaryX 
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1945602
/// Updated by ddreaper - removed dependency on a custom ScrollRect script. Now implements drag interfaces and standard Scroll Rect.

using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Horizontal Scroll Snap")]
	public class HorizontalScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		private int _screens = 1;

		private bool _fastSwipeTimer = false;
		private int _fastSwipeCounter = 0;
		private int _fastSwipeTarget = 30;


		private System.Collections.Generic.List<Vector3> _positions;
		private Vector3 _lerp_target;
		private bool _lerp;

		private int _containerSize;
		private int _step;
		private RectTransform _pagesContainerRectTransform;

		public Transform PagesContainer;
		public GameObject NextButton;

		public Boolean UseFastSwipe = true;
		public int FastSwipeThreshold = 100;

		private bool _startDrag = true;
		private Vector3 _startPosition = new Vector3();
		private int _currentScreen;

		// Use this for initialization
		void Start()
		{
			_pagesContainerRectTransform = PagesContainer.GetComponent<RectTransform>();
			_step = (int)_pagesContainerRectTransform.GetWidth();

			DistributePages();

			_screens = PagesContainer.childCount;

			_lerp = false;

			_positions = new System.Collections.Generic.List<Vector3>();

			if (_screens > 0)
			{
				for (int i = 0; i < _screens; ++i)
				{
					Vector3 pos = new Vector3(-(i * _step), 0, 0);// / MainCanvas.canvas.scaleFactor;
					_positions.Add(pos);
				}
			}

			_containerSize = (int)PagesContainer.gameObject.GetComponent<RectTransform>().offsetMax.x;

			if (NextButton)
				NextButton.GetComponent<Button>().onClick.AddListener(() => { NextScreen(); });
		}

		//used for changing between screen resolutions
		private void DistributePages()
		{
			int currentXPosition = 0;
			int pageCount = PagesContainer.childCount;

			for (int i = 0; i < pageCount; i++)
			{
				RectTransform child = PagesContainer.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
				currentXPosition = i * _step;
				child.anchoredPosition = new Vector2(currentXPosition, 0f);
//				child.SetWidth(_pagesContainerRectTransform.GetWidth());
//				child.SetHeight(_pagesContainerRectTransform.GetHeight());
			}

			float entireWidth = pageCount * _step;
//			Debug.Log(_pagesContainerRectTransform.GetWidth() + ", " + _pagesContainerRectTransform.GetHeight());
			PagesContainer.GetComponent<RectTransform>().SetWidth(entireWidth);
		}

		void Update()
		{
			if (_lerp)
			{
				PagesContainer.localPosition = Vector3.Lerp(PagesContainer.localPosition, _lerp_target, 7.5f * Time.deltaTime);
				if (Vector3.Distance(PagesContainer.localPosition, _lerp_target) < 0.005f)
				{
					_lerp = false;
				}
			}

			if (_fastSwipeTimer)
			{
				_fastSwipeCounter++;
			}

		}

		private bool fastSwipe = false; //to determine if a fast swipe was performed


		//Function for switching screens with buttons
		public void NextScreen()
		{
			if (CurrentScreen() < _screens - 1)
			{
				_lerp = true;
				_lerp_target = _positions[CurrentScreen() + 1];
			}
		}

		//Function for switching screens with buttons
		public void PreviousScreen()
		{
			if (CurrentScreen() > 0)
			{
				_lerp = true;
				_lerp_target = _positions[CurrentScreen() - 1];
			}
		}

		//Because the CurrentScreen function is not so reliable, these are the functions used for swipes
		private void NextScreenCommand()
		{
			if (_currentScreen < _screens - 1)
			{
				_lerp = true;
				_lerp_target = _positions[_currentScreen + 1];
			}
		}

		//Because the CurrentScreen function is not so reliable, these are the functions used for swipes
		private void PrevScreenCommand()
		{
			if (_currentScreen > 0)
			{
				_lerp = true;
				_lerp_target = _positions[_currentScreen - 1];
			}
		}


		//find the closest registered point to the releasing point
		private Vector3 FindClosestFrom(Vector3 start, System.Collections.Generic.List<Vector3> positions)
		{
			Vector3 closest = Vector3.zero;
			float distance = Mathf.Infinity;

			foreach (Vector3 position in _positions)
			{
				if (Vector3.Distance(start, position) < distance)
				{
					distance = Vector3.Distance(start, position);
					closest = position;
				}
			}

			return closest;
		}


		//returns the current screen that the is seeing
		public int CurrentScreen()
		{
			float absPoz = Math.Abs(PagesContainer.gameObject.GetComponent<RectTransform>().offsetMin.x);

			absPoz = Mathf.Clamp(absPoz, 1, _containerSize - 1);

			float calc = (absPoz / _containerSize) * _screens;

			return (int)calc;
		}

		#region Interfaces
		public void OnBeginDrag(PointerEventData eventData)
		{
			_startPosition = PagesContainer.localPosition;
			_fastSwipeCounter = 0;
			_fastSwipeTimer = true;
			_currentScreen = CurrentScreen();
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			_startDrag = true;
			if (UseFastSwipe)
			{
				fastSwipe = false;
				_fastSwipeTimer = false;
				if (_fastSwipeCounter <= _fastSwipeTarget)
				{
					if (Math.Abs(_startPosition.x - PagesContainer.localPosition.x) > FastSwipeThreshold)
					{
						fastSwipe = true;
					}
				}
				if (fastSwipe)
				{
					if (_startPosition.x - PagesContainer.localPosition.x > 0)
					{
						NextScreenCommand();
					}
					else
					{
						PrevScreenCommand();
					}
				}
				else
				{
					_lerp = true;
					_lerp_target = FindClosestFrom(PagesContainer.localPosition, _positions);
				}
			}
			else
			{
				_lerp = true;
				_lerp_target = FindClosestFrom(PagesContainer.localPosition, _positions);
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			_lerp = false;
			if (_startDrag)
			{
				OnBeginDrag(eventData);
				_startDrag = false;
			}
		}
		#endregion
	}
}