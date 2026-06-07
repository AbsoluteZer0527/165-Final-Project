using GLTFast.Schema;
using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerZone : MonoBehaviour
{
    [SerializeField] private Transform vrPlayer;
    private bool agentInZone = false;
    private List<NPCBehavior> npcsInZone = new();

    private void Update()
    {
        if(agentInZone)
        {
            foreach (var npc in npcsInZone)
            {
                npc.transform.rotation = Quaternion.LookRotation(vrPlayer.position - npc.transform.position);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent(out NPCBehavior npcBehavior);
        if(npcBehavior != null)
        {
            npcBehavior.IsNearPlayer = true;
            npcBehavior.CurrentState = 0;
            Animator animator = npcBehavior.GetComponent<Animator>();
            animator.SetInteger("State", (int)npcBehavior.CurrentState);
            npcsInZone.Add(npcBehavior);
            agentInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.TryGetComponent(out NPCBehavior npcBehavior);
        if(npcBehavior != null)
        {
            npcBehavior.IsNearPlayer = false;
            agentInZone = false;
            npcsInZone.Remove(npcBehavior);
        }
    }
}
