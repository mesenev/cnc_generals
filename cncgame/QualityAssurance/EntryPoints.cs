using QualityAssurance.Tests;
using RemoteScripting;
using RemoteScriptingClient;

namespace QualityAssurance
{
	public static class EntryPoints
	{
		[PortableEntryPoint("Stop all tests", -3000)]
		public static void StopAllTests() => Test.StopAll();

		[PortableEntryPoint("Dummy test", -2000)]
		public static void DummyTest() => RunTest(new DummyTest());

		[PortableEntryPoint("HitBox Visualization Tool", -1000)]
		public static void HitBoxVisualizationTool() => RunTest(new HitBoxVisualizationTool());

		[PortableEntryPoint("Example Test")]
		public static void ExampleTest() => RunTest(new ExampleTest());

		private static void RunTest(Test test) => test.Run(RemoteScriptingManager.Instance.Environment.TasksTag);
	}
}
