using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SelectorManipulator : MonoBehaviour
{
    public Material selectedMaterial;
    private Material origMaterial;
    public InteractorScaleModule interactorScaleModule;

    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        MeshRenderer meshRenderer = args.interactableObject.transform.GetComponent<MeshRenderer>(); 
        origMaterial = meshRenderer.material;
        meshRenderer.material = selectedMaterial;
    }

    public void OnHoverExit(HoverExitEventArgs args)
    {
        args.interactableObject.transform.GetComponent<MeshRenderer>().material = origMaterial;
    }

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        args.interactableObject.transform.GetComponent<XRGrabInteractable>().trackScale = false;
        interactorScaleModule.selectedObject = args.interactableObject.transform;
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        args.interactableObject.transform.GetComponent<XRGrabInteractable>().trackScale = true;
        interactorScaleModule.selectedObject = null;
    }
}
