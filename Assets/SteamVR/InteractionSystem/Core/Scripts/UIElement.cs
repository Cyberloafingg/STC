//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: UIElement that responds to VR hands and generates UnityEvents
//
//=============================================================================

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Valve.VR;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	[RequireComponent( typeof( Interactable ) )]
	public class UIElement : MonoBehaviour
	{
		public CustomEvents.UnityEventHand onHandClick;

        protected Hand currentHand;

		//-------------------------------------------------
		protected virtual void Awake()
		{
			Button button = GetComponent<Button>();
			if ( button )
			{
				button.onClick.AddListener( OnButtonClick );
			}
		}


		//-------------------------------------------------
		protected virtual void OnHandHoverBegin( Hand hand )
		{
			currentHand = hand;
			InputModule.instance.HoverBegin(gameObject);
			if(hand.grabGripAction != null && hand.grabGripAction.GetState(hand.handType))
			{
                InputModule.instance.Submit(gameObject);
				hand.hapticAction.Execute(0, 0.05f, 150, 0.75f, hand.handType);
                onHandClick.Invoke(currentHand);
				Debug.Log("Enter");
            }
		}


        //-------------------------------------------------
        protected virtual void OnHandHoverEnd( Hand hand )
		{
			InputModule.instance.HoverEnd(gameObject);
            currentHand = null;
		}


        //-------------------------------------------------
        protected virtual void HandHoverUpdate( Hand hand )
		{
			if (hand.grabGripAction != null && hand.grabGripAction.GetStateDown(hand.handType))
			{
				InputModule.instance.Submit(gameObject);
				//ControllerButtonHints.HideButtonHint(hand, hand.grabGripAction);
			}
		}


        //-------------------------------------------------
        protected virtual void OnButtonClick()
		{
			//onHandClick.Invoke( currentHand );
			//Debug.Log("Click");
		}
	}

#if UNITY_EDITOR
	//-------------------------------------------------------------------------
	[UnityEditor.CustomEditor( typeof( UIElement ) )]
	public class UIElementEditor : UnityEditor.Editor
	{
		//-------------------------------------------------
		// Custom Inspector GUI allows us to click from within the UI
		//-------------------------------------------------
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			UIElement uiElement = (UIElement)target;
			if ( GUILayout.Button( "Click" ) )
			{
				InputModule.instance.Submit( uiElement.gameObject );
			}
		}
	}
#endif
}
