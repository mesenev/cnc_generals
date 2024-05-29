using Yuzu;

namespace Game;

public class Profile
{
	public static Profile Instance;

	[YuzuAfterDeserialization]
	public void AfterDeserialization()
	{ }
}
