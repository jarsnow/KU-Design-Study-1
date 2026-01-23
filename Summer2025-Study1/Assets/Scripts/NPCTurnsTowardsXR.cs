using UnityEngine;

public class NPCTurnsTowardsXR : MonoBehaviour
{
    public GameObject NPC;
    public GameObject XR_User;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TurnNPCToXR();
    }

    void TurnNPCToXR()
    {
        Debug.Log(NPC.transform.rotation.eulerAngles);
        float angle = Quaternion.Angle(NPC.transform.rotation, XR_User.transform.rotation);

        VectorToPlayer = Player.transform.position - transform.position;
        AngleToPlayer = Mathf.DeltaAngle(transform.localRotation.eulerAngles.z, Mathf.Atan2(VectorToPlayer.y, VectorToPlayer.x) * Mathf.Rad2Deg - 90);
    }
}
