using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Text;
using Lime;
namespace Game
{
	public class ReceiveCommandsFromCommandRecognitionModule : IProcessor
	{
		private SpeechToCommand.SpeechToCommandClient client;
		private AsyncServerStreamingCall<global::Game.DummyCommand> callForReadComms;
		private PlayerComponent player;

		public ReceiveCommandsFromCommandRecognitionModule(PlayerComponent playerComponent)
		{
			player = playerComponent;
			var channel = GrpcChannel.ForAddress("http://localhost:5175");
			client = new SpeechToCommand.SpeechToCommandClient(channel);
			callForReadComms = client.TextToCommand(new InputText());
		}

		public void Update(float delta, Game game)
		{
			try{ 
				if (callForReadComms.ResponseStream.MoveNext().Result) {
					Console.WriteLine(callForReadComms.ResponseStream.Current);
					if (callForReadComms.ResponseStream.Current.Direction.ToLower() == "вверх")
					{
						player.Position += new Vector2(0, callForReadComms.ResponseStream.Current.Offset);	
					}
					if (callForReadComms.ResponseStream.Current.Direction.ToLower() == "вниз")
					{
						player.Position -= new Vector2(0, callForReadComms.ResponseStream.Current.Offset);	
					}

					if (callForReadComms.ResponseStream.Current.Direction.ToLower() == "вправо") {
						player.Position += new Vector2(callForReadComms.ResponseStream.Current.Offset, 0);
					}
					if (callForReadComms.ResponseStream.Current.Direction.ToLower() == "влево") {
						player.Position -= new Vector2(callForReadComms.ResponseStream.Current.Offset, 0);
					}
				}
				Console.WriteLine(player.Position);
			}
			catch (Exception error){
				return;
			}
		}
	}
}
