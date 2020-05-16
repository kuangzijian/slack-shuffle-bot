using System.Collections.Generic;
using SlackBotMessages;
using System.Web.Http;
using SlackBotMessages.Models;
using System.Linq;
using System;

namespace ShuffleBot.Controllers.API
{
    public class ShuffleAPIController : ApiController
    {
        public IHttpActionResult Shuffle()
        {
            // to-do: need to move the configuration to a secrete configuration file.
            var WebHookUrl = "https://hooks.slack.com/services/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";

            var client = new SbmClient(WebHookUrl);

            // to-do: need to figure out the format of the payload send from slack command.
            var shuffleList = ("Jack,Ryan,Tin,Josh").Split(',').ToList().OrderBy(a => Guid.NewGuid()).ToList();

            // to-do: need to figure out if there is a way to integrate with attandance bot and grab the day off information from there.
            var dayOffList = "Mike, Rui";
            var reminder = "@channel Please do not forget to update your remaining points on the card that you are working on.";

			var message = new Message(System.DateTime.Now.Date.ToString("MMMM dd"));
            message.AddAttachment(new Attachment()
                .AddField("Day-off Today:", dayOffList, true)
                .AddField("Standup Order", String.Join(", ", shuffleList), true)
                .AddField("Reminder", reminder)
            );

            client.Send(message);
            return Ok();
        }



    }
}