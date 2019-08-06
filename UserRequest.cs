using System;

public class UserRequest
{
    public bool isjob = false;
    public bool is1z = false;
    public String z { get; set; }
    private bool order { get; set; }
    public bool cancel { get; set; }
    private bool inventory { get; set; }
    public String trackingNumber { get; set; }
    public String UPSTrackingNumber { get; set; }
    public String orderNumber { get; set; }
    public String SKUNumber { get; set; }
    public bool isTrackingNumber = false; 
    public bool isUPSTrackingNumber = false;
    public bool isOrderNumber = false;
    public bool isSKUNumber = false;
    public String purchaseOrderNumber { get; set; }
    public bool ispurchaseOrderNumber = false;
    public bool yes = false;
    public bool restart = false;

    public void setOrder(bool value)
    {
        if (value)
            order = true;
        else
            order = false;
    }

    public void setInventory(bool value)
    {
        if (value)
            inventory = true;
        else
            inventory = false;
    }
    

    public bool isOrder()
    {
        if (order)
            return true;
        return false;
    }//isOrder

    public bool isInventory()
    {
        if (inventory)
            return true;
        return false;
    }

}
