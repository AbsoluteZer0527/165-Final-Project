using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SelectorManipulator : MonoBehaviour
{
    public Material selectedMaterial;
    private List<Material> origMaterial = new();
    public InteractorScaleModule interactorScaleModule;

    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        //SkinnedMeshRenderer meshRenderer = args.interactableObject.transform.GetComponent<SkinnedMeshRenderer>(); 
        MeshRenderer[] meshRenderers = args.interactableObject.transform.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            origMaterial.Add(meshRenderers[i].material);
            meshRenderers[i].material = selectedMaterial;
        }
    }

    public void OnHoverExit(HoverExitEventArgs args)
    {
        //args.interactableObject.transform.GetComponent<SkinnedMeshRenderer>().material = origMaterial;
        MeshRenderer[] meshRenderers = args.interactableObject.transform.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = origMaterial[i];
        }
        origMaterial.Clear();
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
