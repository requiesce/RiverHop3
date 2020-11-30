using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGUI : MonoBehaviour
{
    public List<GameObject> items;

    // Start is called before the first frame update
    void Start()
    {
        ItemCollect.ItemCollected += IncrementItem;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    void IncrementItem(Item.VegetableType ItemType)
    {
        CountGUI cg = items[(int)ItemType].GetComponent<CountGUI>();
        cg.UpdateCount();
    }
}
