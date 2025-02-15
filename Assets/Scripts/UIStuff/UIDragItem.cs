using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// Class responsible for dragging itself.
/// Using EventSystems interfaces.
/// </summary>
public class UIDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	// Reference to current item slot.
	public UIDropSlot currentSlot;
	// Reference to the canvas.
	[HideInInspector]
	public Canvas canvas;
	// Reference to UI raycaster.
	[HideInInspector]
	public GraphicRaycaster graphicRaycaster;

	/// <summary>
	/// IBeginDragHandler
	/// Method called on drag begin.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnBeginDrag(PointerEventData eventData) {
		// Start moving object from the beginning!
		// transform.localPosition += new Vector3(eventData.delta.x, eventData.delta.y, 0); 
		// Thanks to the canvas scaler we need to devide pointer delta by canvas scale to match pointer movement.
		// We need a few references from UI.
		if (!canvas) {
			canvas = GetComponentInParent<Canvas>();
			graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
		}
		// Change parent of our item to the canvas.
		transform.SetParent(canvas.transform, true);
		// And set it as last child to be rendered on top of UI.
		transform.SetAsLastSibling();
	}
	/// <summary>
	/// IDragHandler
	/// Method called on drag continuously.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnDrag(PointerEventData eventData) {
		// Continue moving object around screen.
		transform.localPosition += new Vector3(eventData.delta.x, eventData.delta.y, 0); // Thanks to the canvas scaler we need to devide pointer delta by canvas scale to match pointer movement.
	}
	/// <summary>
	/// IEndDragHandler
	/// Method called on drag end.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public virtual void OnEndDrag(PointerEventData eventData) {
		// On end we need to test if we can drop item into new slot.
		var results = new List<RaycastResult>();
		graphicRaycaster.Raycast(eventData, results);
		// Check all hits.
		foreach (var hit in results) {
			// If we found slot.
			var slot = hit.gameObject.GetComponent<UIDropSlot>();
			if (slot) {
				// We should check if we can place ourselves​ there.
				if (slot.SlotFilled) {
					var otherItem = slot.currentItem;
					otherItem.currentSlot = currentSlot;
					currentSlot.currentItem = otherItem;
					otherItem.transform.SetParent(currentSlot.transform);
					otherItem.transform.localPosition = Vector3.zero;
				} else if (currentSlot) currentSlot.currentItem = null;
				currentSlot = slot;
				slot.currentItem = this;
				// In either cases we should break check loop.
				break;
			}
		}
		// Changing parent back to slot.
		transform.SetParent(currentSlot.transform);
		// And centering item position.
		transform.localPosition = Vector3.zero;
	}

	public void Link(UIDropSlot slot) {
		if (slot.currentItem) slot.currentItem.currentSlot = null;
		slot.currentItem = this;
		if (this.currentSlot) this.currentSlot.currentItem = null;
		this.currentSlot = slot;
	}
}