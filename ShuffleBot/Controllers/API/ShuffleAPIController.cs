using System.Collections.Generic;
using SlackBotMessages;
using System.Web.Http;
using SlackBotMessages.Models;

namespace ShuffleBot.Controllers.API
{
    public class ShuffleAPIController : ApiController
    {
        public IHttpActionResult Shuffle()
        {
            var WebHookUrl = "https://hooks.slack.com/services/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";

            var client = new SbmClient(WebHookUrl);

            var list = "Jack, Ryan, Tin, Mike, Rui, Josh";

            var reminder = "@channel Please do not forget to update your remaining points on the card that you are working on.";

			var message = new Message("");
            message.AddAttachment(new Attachment()
                .AddField("Date", System.DateTime.Now.Date.ToString("MMMM dd"), true)
                .AddField("Standup Order", list, true)
                .AddField("Reminder", reminder)
            );

            client.Send(message);
            return Ok();
        }

    }
}