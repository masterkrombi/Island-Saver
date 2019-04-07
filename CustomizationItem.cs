using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum ItemType
{
    Accessory,
    Color,
    Prop
}

[System.Serializable]
public enum CreatureType
{
    None,
    Flyer,
    Jumper,
    Swapper
}

[System.Serializable]
public enum AccessoryType
{
    None,
    Hat,
    Glasses
}

[System.Serializable]
public class CustomizationItem : MonoBehaviour
{
    [Header("Set For Shop")]
    public string itemName;
    [TextArea(3, 10)]
    public string itemDescription;
    public int itemPrice;

    [Header("Set item properties")]
    [SerializeField] public ItemType type = ItemType.Accessory;
    [SerializeField] public CreatureType creatureType = CreatureType.Flyer;
    [SerializeField] public AccessoryType accessoryType = AccessoryType.None;
    public Sprite menuImage;
    [SerializeField]
    public List<Material> materials;

    //For ability to drag onto player (Added 12/20)
    //RaycastHit hit;
    //public bool dragging;
    //public LayerMask mask;

    public void ApplyItem(GameObject monster, bool _animation = true)
    {

        if (monster.GetComponent<Creature>().equippedItems.Contains(this))
        {
            _animation = false;
        }
        else
        {
            monster.GetComponent<Creature>().equippedItems.Add(this);
        }

        switch (type)
        {
            case ItemType.Accessory:
                monster.GetComponent<Creature>().ChangeAccessory(this, _animation);
                break;
            case ItemType.Color:
                monster.GetComponent<Creature>().ChangeColor(materials, _animation);
                break;
        }
    }

    
    //Copy constructor
    public void Copy(CustomizationItem original)
    {
        itemName = original.itemName;
        itemDescription = original.itemDescription;
        itemPrice = original.itemPrice;
        type = original.type;
        menuImage = original.menuImage;
        creatureType = original.creatureType;
        accessoryType = original.accessoryType;

        materials = new List<Material>();
        foreach (Material mat in original.materials)
        {
            materials.Add(new Material(mat));
        }
    }

    //TODO: For ability to drag onto player (Added 12/20)

    //Called when selected on customization menu
    //public void OnSelected()
    //{
    //    this.dragging = true;
    //}
    ////Ability to drag item, if selected in cutomization menu
    //void OnMouseDrag()
    //{
    //    if (dragging)
    //    {
    //        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
    //        Vector3 objPosition = transform.position;
    //        Vector3 direction = Camera.main.ScreenToWorldPoint(mousePosition) - Camera.main.transform.position;

    //        BoxCollider box = GetComponent<BoxCollider>();

    //        if (Physics.BoxCast(Camera.main.transform.position, box.size, direction, out hit, box.gameObject.transform.rotation, Mathf.Infinity, mask))// out hit, 100, mask))
    //        {
    //            objPosition = Camera.main.transform.position + direction.normalized * hit.distance;
    //        }

    //        transform.position = objPosition;
    //    }
    //}

    //void OnMouseUp()
    //{
    //    if (dragging)
    //    {
            
    //    }
    //}
}
