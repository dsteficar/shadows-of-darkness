using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HolderScaling : MonoBehaviour
{
    public GameObject handField;
    public GameObject slotPrefab;

    public List<GameObject> slotList = new List<GameObject>();

    public int numberOfSlots = 1;

    public float fieldSizeExpander = 0.1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (Transform child in handField.transform)
            {
                Destroy(child.gameObject);
            }
            slotList.Clear();
            
            ScaleHand();
        }
       
    }

    private void ScaleHand()
    {

        GameObject slot;
        int halfNumberOfCards = Mathf.CeilToInt((float)numberOfSlots / 2);
        float zOffset = -1;
        if (numberOfSlots % 2 == 0)
        {
            zOffset = -(fieldSizeExpander * ((Mathf.CeilToInt(((float)numberOfSlots + 1) / 2))) - (fieldSizeExpander/2));
        }
        else {
            zOffset = -(fieldSizeExpander * ((numberOfSlots + 1) / 2));

        }
        for (int currentSlot = 1; currentSlot <= numberOfSlots; currentSlot++)
        {
            slot = Instantiate(slotPrefab, handField.transform.position, handField.transform.rotation);
            slot.transform.parent = handField.transform;

            zOffset = zOffset + fieldSizeExpander;
            Vector3 slotPosition = new Vector3(0, 0, 0);
            slotPosition.z = zOffset;
            slot.transform.localPosition = slotPosition;

            slotList.Add(slot);
        }
    }

}
