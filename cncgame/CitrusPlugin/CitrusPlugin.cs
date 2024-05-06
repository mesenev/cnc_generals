using System;
using System.IO;
using System.Linq;
using System.ComponentModel.Composition;

using Orange;
using Orange.Schema;

namespace CitrusPlugin;

public static class CitrusPlugin
{
	private static ConfigWindow Window;

	/// <summary>
	/// Installs pre-commit git hook which will run PngOptimizerCL.exe for every .png file in the project.
	/// </summary>
	private static void InstallHooks()
	{
		var gitRootPath = The.Workspace.ProjectDirectory;
		do {
			if (Directory.Exists(Path.Combine(gitRootPath, ".git"))) {
				gitRootPath = Path.Combine(gitRootPath, ".git");
				break;
			}
			gitRootPath = Path.GetDirectoryName(gitRootPath);
		} while (Directory.Exists(gitRootPath));
		if (Directory.Exists(gitRootPath)) {
			var installedHookPath = Path.Combine(gitRootPath, "hooks", "pre-commit");
			var distrHookPath = Path.Combine(The.Workspace.ProjectDirectory, "hooks", "pre-commit");
			var fiInstalledHook = new System.IO.FileInfo(installedHookPath);
			var fiDistrHook = new System.IO.FileInfo(distrHookPath);
			if (!fiDistrHook.Exists) {
				return;
			}
			if (!fiInstalledHook.Exists || fiInstalledHook.LastWriteTime != fiDistrHook.LastWriteTime) {
				System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(installedHookPath));
				var citrusDirectory = Orange.Toolbox.FindCitrusDirectory();
				var hookText = File.ReadAllText(distrHookPath)
					.Replace(
						oldValue: "$(CITRUS_DIRECTORY)",
						newValue: Path.GetRelativePath(
							Path.Combine(gitRootPath, ".."),
							path: citrusDirectory
						).Replace("\\", "/")
					);
				File.WriteAllText(installedHookPath, hookText);
				File.SetLastWriteTime(installedHookPath, fiDistrHook.LastWriteTime);
				if (!fiInstalledHook.Exists) {
					Console.WriteLine("Installed git hook.");
				} else if (fiInstalledHook.LastWriteTime != fiDistrHook.LastWriteTime) {
					Console.WriteLine("Updated git hook.");
				}
			}
		}
	}

	[Export(nameof(Orange.OrangePlugin.Initialize))]
	public static void DoInitialization()
	{
		InstallHooks();
		var lpc = Orange.Migrations.LimeMigratingPersistenceContext.Instance;
		Lime.Application.RegisterDataLayers(new[] {
			(CitrusTypes.DataLayer.Name, CitrusTypes.DataLayer.Version)
		});
		lpc.RegisterMigrationLayer(
			CitrusTypes.DataLayer.Name,
			CitrusTypes.DataLayer.Version,
			new Type[] { },
			(version) => {
				var schema = new Lime.Schema.SchemaStorage();
				var schemaDirectory = Path.Combine(
					The.Workspace.ProjectDirectory,
					$"Generated",
					"Schema"
				);
				foreach (var file in new Orange.FileEnumerator(schemaDirectory).Enumerate()) {
					var path = Path.Combine(schemaDirectory, file);
					var meta = lpc.ReadMetadataFromFile(path);
					if (
						meta.Any(
							m => m is Lime.Schema.LayerMetadata lm
							&& lm.Layers.Any(
								l => l.Layer == CitrusTypes.DataLayer.Name && l.Version == version
							)
						)
					) {
						return schema.LoadFromFile(path);
					}
				}
				return null;
			}
		);
		Console.WriteLine("Orange plugin initialized.");
	}

	[Export(nameof(Orange.OrangePlugin.BuildUI))]
	public static void BuildUI(IPluginUIBuilder builder)
	{
		Window = new ConfigWindow(builder);
	}

	[Export(nameof(Orange.OrangePlugin.Finalize))]
	public static void DoFinalization()
	{
		Window = null;
		Console.WriteLine("Orange plugin finalized.");
	}

	[Export(nameof(Orange.OrangePlugin.CommandLineArguments))]
	public static string GetCommandLineArguments()
	{
		return Window.GetCommandLineArguments();
	}

	[Export(nameof(Orange.OrangePlugin.MenuItems))]
	[ExportMetadata(nameof(IMenuItemMetadata.Label), "Cook Selected Bundles")]
	public static void CookSelectedBundles()
	{
		var target = The.UI.GetActiveTarget();
		if (!AssetCooker.CookForTarget(
			target,
			Orange.Toolbox.GetCommandLineArg("--bundles").Split(',').Select(s => s.Trim()).ToList(),
			out var errorMessage
		)) {
			Console.WriteLine("Error cooking selected bundles: " + errorMessage);
		}
	}

	[Export(nameof(OrangePlugin.MenuItems))]
	[ExportMetadata("Label", "Generate DemoServer Persistence Schema")]
	[ExportMetadata("Priority", 5)]
	public static void GenerateProjectSchema()
	{
		Orange.Actions.GenerateProjectSchema(
			new[] { (CitrusTypes.DataLayer.Name, CitrusTypes.DataLayer.Version) }
		);
	}
}
