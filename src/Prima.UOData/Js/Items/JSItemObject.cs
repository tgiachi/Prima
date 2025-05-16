using Prima.UOData.Types;

namespace Prima.UOData.Js.Items;

public class JSItemObject
{
    public int ItemId { get; set; }

    public string Name { get; set; }

    public int GraphicId { get; set; }

    public double Weight { get; set; }

    public Layer Layer { get; set; }

    public int Amount { get; set; }

    public JsDamageType Damage { get; set; }
}
