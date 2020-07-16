using System.Collections.Generic;
using SlackBotMessages;
using Newtonsoft.Json;
using SlackBotMessages.Models;
using System.Linq;
using System;
using System.Net;
using System.IO;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ShuffleBotLambdaFunction
{
    public class Function
    {
        public class AttandanceRecordList
        {
            public List<AttandanceRecord> data { get; set; }
        }
        public class AttandanceRecord
        {
            public string name { get; set; }
            public string duration { get; set; }
            public string reason { get; set; }
        }
        /// <summary>
		///     Get attandance summary list for a specific team on today's date
		/// </summary>
		/// <param name="dateToday">Today's Date in MM/dd/yyyy format</param>
		/// <param name="teamID">Team ID</param>
        public AttandanceRecordList GetAttandanceList(string dateToday, string teamID)
        {
            var requestURL = $"https://www.attendancebot.com/api/report/leave/?team=TCRAG4HNE&auth=9a5ffe4629&from={dateToday}&to={dateToday}&department_id={teamID}";

            WebRequest request = WebRequest.Create(requestURL);
            WebResponse response = request.GetResponse();
            var content = new StreamReader(response.GetResponseStream()).ReadToEnd();
            content = content.Replace("Employee Name", "name")
                .Replace("Leave Duration", "duration")
                .Replace("Leave Type", "reason");
        
            AttandanceRecordList attandanceRecordList = JsonConvert.DeserializeObject<AttandanceRecordList>(content);

            return attandanceRecordList;
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string reminder, ILambdaContext context)
        {
            var WebHookUrl = "https://hooks.slack.com/services/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";
            var client = new SbmClient(WebHookUrl);
            var staffList = "Ryan Meria,Michael Williams,Rui Chapouto,Jack Kuang,Tin Hoang,Josh Deng";
            var shuffleList = staffList.Split(',').ToList().OrderBy(a => Guid.NewGuid()).ToList();
            var dateToday = System.DateTime.Now.Date.ToString("MM/dd/yyyy");
            var teamID = "4226";

            AttandanceRecordList attandanceRecordList = GetAttandanceList(dateToday, teamID);

            var dayOffList = "";
            var index = 1;

            if (attandanceRecordList.data != null)
            {
                foreach (var item in attandanceRecordList.data)
                {
                    if ((item.duration.ToLower() == "full day" && item.reason.ToLower() != "tech investigation") || item.duration.ToLower() == "7.5 hours")
                    {
                        dayOffList += item.name;
                        if (index < attandanceRecordList.data.Count()) dayOffList += (", ");
                    }
                    index++;
                }
            }
            var shuffleListNoDayOff = shuffleList.Except(dayOffList.Replace(", ", ",").Split(',').ToList()).ToList();
            reminder = reminder?? "@channel Please don't forget to update your remaining points on the card that you are working on.";

            var message = new Message(System.DateTime.Now.Date.ToString("MMMM dd"));
            message.AddAttachment(new Attachment()
                .AddField("Day-off Today:", dayOffList, true)
                .AddField("Standup Order", shuffleListNoDayOff != null ? String.Join(", ", shuffleListNoDayOff) : "", true)
                .AddField("Reminder", reminder)
            );

            client.Send(message);
            return reminder;
        }
    }
}
