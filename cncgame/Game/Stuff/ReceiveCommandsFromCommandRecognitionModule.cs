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
		private GrpcChannel channel;

		public ReceiveCommandsFromCommandRecognitionModule(PlayerComponent playerComponent)
		{
			player = playerComponent;
			channel = GrpcChannel.ForAddress("http://localhost:5175");
			client = new SpeechToCommand.SpeechToCommandClient(channel);
			callForReadComms = client.TextToCommand(new InputText());
		}

		public void Update(float delta, Game game)
		{
			if (channel.State != ConnectivityState.Ready) {
				Console.WriteLine("RECREATE CHANNEL");
				channel.Dispose();
				channel = GrpcChannel.ForAddress("http://localhost:5175");
				
				client = new SpeechToCommand.SpeechToCommandClient(channel);
				
				callForReadComms.Dispose();
				callForReadComms = client.TextToCommand(new InputText());
			}
			try {
				Console.WriteLine(channel.State);
				if (callForReadComms.ResponseStream.Current.Direction.ToLower() == "вверх") {
					Console.WriteLine("T1");
					player.Position += new Vector2(0, callForReadComms.ResponseStream.Current.Offset);
				}

				if (callForReadComms.ResponseStream.Current.Direction.ToLower() == "вниз") {
					Console.WriteLine("T2");
					player.Position -= new Vector2(0, callForReadComms.ResponseStream.Current.Offset);
				}

				if (callForReadComms.ResponseStream.Current.Direction.ToLower() == "вправо") {
					Console.WriteLine("T3");
					player.Position += new Vector2(callForReadComms.ResponseStream.Current.Offset, 0);
				}

				if (callForReadComms.ResponseStream.Current.Direction.ToLower() == "влево") {
					Console.WriteLine("T4");
					player.Position -= new Vector2(callForReadComms.ResponseStream.Current.Offset, 0);
				}
			} catch (Exception e) {
				Console.WriteLine(e);
			}

			//} else {
				//Console.WriteLine("SERVER RECONNET TO CRM");
				//callForReadComms.Dispose();
				//callForReadComms = client.TextToCommand(new InputText());
			//}
			callForReadComms.ResponseStream.MoveNext();
			
			//}
			// catch (Exception error){
			//
			// 	//Console.WriteLine("ERROR DURING RECEIVING VOICE COMMANDS, TRYING TO RECONNET TO CR MODULE");
			// 	// callForReadComms.Dispose();
			// 	//callForReadComms = client.TextToCommand(new InputText());
			// }
		}
	}
}
