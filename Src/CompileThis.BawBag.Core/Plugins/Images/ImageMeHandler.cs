namespace CompileThis.BawBag.Plugins.Images
{
	using System;
	using System.Configuration;
	using System.Linq;
	using System.Net;
	using System.Text.RegularExpressions;

	using CompileThis.BawBag.Extensibility;

	using NLog;

	internal class ImageMeHandler : MessageHandlerPluginBase
	{
		private const string BingApiUri = "https://api.datamarket.azure.com/Bing/Search";

		private static readonly Regex ImageMeExpression = new Regex(@"^\s*image me\s+(?<query>.*?)\s*$", RegexOptions.IgnoreCase);

		private static readonly string BingApi = ConfigurationManager.AppSettings["BawBag/BingApi"];
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		public ImageMeHandler()
			: base("ImageMe", PluginPriority.Normal, continueProcessing: false, mustBeAddressed: false)
		{ }

		protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
		{
			if (message.Type != MessageType.Default)
			{
				return NotHandled();
			}

			var match = ImageMeExpression.Match(message.Text);
			if (match.Success)
			{
				var query = match.Groups["query"].Value;
				return GetImage(query, context);
			}

			return NotHandled();
		}

		private MessageHandlerResult GetImage(string query, IPluginContext context)
		{
			try
			{
				var bingContainer = new Bing.BingSearchContainer(new Uri(BingApiUri))
					                    {
						                    Credentials =
							                    new NetworkCredential(BingApi, BingApi)
					                    };

				var imageQuery = bingContainer.Image(query, null, null, null, null, null, null);
				var imageResults = imageQuery.Execute();

				if (imageResults != null)
				{
					var imageList = imageResults.ToList();
					if (imageList.Count > 0)
					{
						var index = context.RandomProvider.Next(imageList.Count);
						var url = imageList[index].MediaUrl;

						return this.Handled(this.Message(url));
					}
				}

				var response = context.TextProcessor.FormatPluginResponse("Really? Do you kiss ${SOMEONE}'s mum with that mouth?", context);
				return this.Handled(this.Message(response));
			}
			catch (Exception ex)
			{
				Log.ErrorException("Unable to search for images on bing.", ex);
				return this.Handled(this.Message("Uh Oh..."));
			}
		}
	}
}
