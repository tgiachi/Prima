using System.ComponentModel;
using System.Runtime.CompilerServices;
using Prima.UOData.Data.Geometry;
using Prima.UOData.Id;
using Prima.UOData.Interfaces.Entities;
using Prima.UOData.Interfaces.Persistence.Entities;


namespace Prima.UOData.Entities.Base;

public class BaseWorldEntity : IHaveSerial, IEntity, ISerializableEntity, INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;

    public Serial Id { get; set; }  
    public Point3D Location { get; set; }

    public int MapIndex { get; set; }

    public bool Deleted { get; set; }

    public bool InRange(Point2D p, int range)
    {
        return Math.Abs(p.X - Location.X) <= range && Math.Abs(p.Y - Location.Y) <= range;
    }

    public bool InRange(Point3D p, int range)
    {
        return Math.Abs(p.X - Location.X) <= range && Math.Abs(p.Y - Location.Y) <= range &&
               Math.Abs(p.Z - Location.Z) <= range;
    }
}
