using System.Collections.Generic;
using SlackBotMessages;
using System.Web.Http;
using SlackBotMessages.Models;
using System.Linq;
using System;
using System.Web.Configuration;

namespace ShuffleBot.Controllers.API
{
    public class ShuffleAPIController : ApiController
    {
        public IHttpActionResult Shuffle()
        {
            var WebHookUrl =  WebConfigurationManager.AppSettings["WebHookUrlTest"];
            
            var client = new SbmClient(WebHookUrl);

            var staffList = WebConfigurationManager.AppSettings["StaffList"];

            var shuffleList = staffList.Split(',').ToList().OrderBy(a => Guid.NewGuid()).ToList();

            // to-do: need to figure out if there is a way to integrate with attandance bot and grab the day off information from there.
            var dayOffList = "";
            var reminder = "@channel Please don't forget to update your remaining points on the card that you are working on.";

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