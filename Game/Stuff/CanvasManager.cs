using System.Collections.Generic;
using Lime;

namespace Game.Stuff
{
	public class CanvasManager
	{
		public static readonly CanvasManager Instance = new();
		
		private Dictionary<int, Frame> canvases = new();

		public void InitLayers(Node parent)
		{
			CreateNewCanvas(Layers.Background, parent);
			CreateNewCanvas(Layers.HexMap, canvases[Layers.Background]);
			CreateNewCanvas(Layers.Entities, canvases[Layers.HexMap]);
		}

		public void CreateNewCanvas(int layer, Node parent)
		{
			if (canvases.ContainsKey(layer)) return;
			var newCanvas = new Frame { Id = layer.ToString() };
			parent.PushNode(newCanvas);
			canvases.Add(layer, newCanvas);
		}

		public Frame GetCanvas(int layer)
		{
			Frame frame;
			canvases.TryGetValue(layer, out frame);
			return frame;
		}
	}
}
