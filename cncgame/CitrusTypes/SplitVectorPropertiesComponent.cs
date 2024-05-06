using Lime;

namespace CitrusTypes;

[TangerineRegisterComponent]
[AllowedComponentOwnerTypes(typeof(Widget))]
public class SplitVectorPropertiesComponent : NodeComponent
{
	private Widget OwnerWidget => (Widget)Owner;

	[TangerineInspect]
	public float PositionX
	{
		get
		{
			return OwnerWidget.Position.X;
		}
		set
		{
			OwnerWidget.Position = new Vector2(value, OwnerWidget.Position.Y);
		}
	}

	[TangerineInspect]
	public float PositionY
	{
		get
		{
			return OwnerWidget.Position.Y;
		}
		set
		{
			OwnerWidget.Position = new Vector2(OwnerWidget.Position.X, value);
		}
	}
}
