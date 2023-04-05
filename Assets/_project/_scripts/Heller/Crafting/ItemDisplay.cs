using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDisplay : MonoBehaviour
{
    //Script for ingame objects for collecting items
    [SerializeField] Item item;
    [SerializeField] int itemCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }
void PickupItem()
    {
        ItemsManager.instance.TakeItem(item.name, itemCount);
    }
}
