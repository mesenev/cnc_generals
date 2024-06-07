using Lime;
using Yuzu;

namespace Game.Widgets;

[TangerineRegisterNode]
public class Camera2D : Widget
{
	[YuzuMember]
	public float OrthographicSize { get; set; } = 960.0f;

	[YuzuMember]
	public bool UseCustomAspectRatio { get; set; }

	[YuzuMember]
	public float CustomAspectRatio { get; set; }
}