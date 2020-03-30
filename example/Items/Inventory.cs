using System.Collections.Generic;

namespace BasicTutorial.Items
{
    internal class Inventory
    {
        public enum ActionResult
        {
            Failure,
            Success
        }

        private readonly List<Item> _carriedItems = new List<Item>();

        public const int MaxCarriedItems = 11;

        public IEnumerable<Item> CarriedItems => _carriedItems;

        public Item Head { get; private set; }
        public Item LeftHand { get; private set; }
        public Item RightHand { get; private set; }
        public Item Feet { get; private set; }
        public Item Body { get; private set; }

        public ActionResult AddItem(Item item, bool carried)
        {
            if (carried || item.Spot == InventorySpot.None)
            {
                if (_carriedItems.Count == MaxCarriedItems)
                    return ActionResult.Failure;

                _carriedItems.Add(item);
                return ActionResult.Success;
            }

            switch (item.Spot)
            {
                case InventorySpot.Head:
                    Head = item;
                    break;
                case InventorySpot.LHand:
                    LeftHand = item;
                    break;
                case InventorySpot.RHand:
                    RightHand = item;
                    break;
                case InventorySpot.Body:
                    Body = item;
                    break;
                case InventorySpot.Feet:
                    Feet = item;
                    break;
                default:
                    break;
            }
            return ActionResult.Success;
        }

        public ActionResult RemoveItem(Item item)
        {
            if (_carriedItems.Contains(item))  // Drop item
                _carriedItems.Remove(item);

            else
            {
                switch (item.Spot)
                {
                    case InventorySpot.Head:
                        if (Head == item)
                            Head = null;

                        break;
                    case InventorySpot.LHand:
                        if (LeftHand == item)
                            LeftHand = null;

                        break;
                    case InventorySpot.RHand:
                        if (RightHand == item)
                            RightHand = null;

                        break;
                    case InventorySpot.Body:
                        if (Body == item)
                            Body = null;

                        break;
                    case InventorySpot.Feet:
                        if (Feet == item)
                            Feet = null;

                        break;
                    default:
                        break;
                }
            }

            return ActionResult.Success;
        }

        public bool IsSlotEquipped(InventorySpot spot)
        {
            switch (spot)
            {
                case InventorySpot.Head:
                    return Head != null;
                case InventorySpot.LHand:
                    return LeftHand != null;
                case InventorySpot.RHand:
                    return RightHand != null;
                case InventorySpot.Body:
                    return Body != null;
                case InventorySpot.Feet:
                    return Feet != null;
                default:
                    return false;
            }
        }

        public bool IsInventoryFull() => _carriedItems.Count == MaxCarriedItems;
        public Item GetItem(InventorySpot spot)
        {
            switch (spot)
            {
                case InventorySpot.Head:
                    return Head;
                case InventorySpot.LHand:
                    return LeftHand;
                case InventorySpot.RHand:
                    return RightHand;
                case InventorySpot.Body:
                    return Body;
                case InventorySpot.Feet:
                    return Feet;
                default:
                    return null;
            }
        }

        public IEnumerable<Item> GetEquippedItems()
        {
            List<Item> items = new List<Item>(5);

            if (Head != null)
                items.Add(Head);

            if (LeftHand != null)
                items.Add(LeftHand);

            if (RightHand != null)
                items.Add(RightHand);

            if (Body != null)
                items.Add(Body);

            if (Feet != null)
                items.Add(Feet);

            return items;
        }
    }
}
