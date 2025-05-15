using Orion.Core.Server.Attributes.Scripts;
using Prima.UOData.Js.Items;
using Prima.UOData.Mul;

namespace Prima.UOData.Modules.Scripts;

[ScriptModule("items")]
public class ItemsScriptModule
{
    [ScriptFunction("Register new item object")]
    public void Register(string id, JSItemObject itemObject)
    {
        var item = TileData.ItemTable[itemObject.ItemId];

        if (item.Name == null)
        {
            throw new ArgumentException($"Item with ID {itemObject.ItemId} does not exist.");
        }
    }
}
