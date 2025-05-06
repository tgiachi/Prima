using Prima.UOData.Types;

namespace Prima.UOData.Maps.Data;

public class MapDefinition
{
    public int Index { get; set; }


    public int Id { get; set; }


    public int FileIndex { get; set; }


    public string Name { get; set; }


    public int Width { get; set; }


    public int Height { get; set; }


    public int Season { get; set; }


    public MapRules Rules { get; set; }
}
