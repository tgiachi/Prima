using Prima.UOData.Data.Geometry;

namespace Prima.UOData.Interfaces.Entities;

public interface IEntity
{
    Point3D Location { get; set; }

    bool InRange(Point2D p, int range);

    bool InRange(Point3D p, int range);
}
