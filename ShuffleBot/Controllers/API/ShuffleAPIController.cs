using System.Collections.Generic;
using SlackBotMessages;
using System.Web.Http;
using SlackBotMessages.Models;
using System.Linq;
using System;
using System.Web.Configuration;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace ShuffleBot.Controllers.API
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


    public class ShuffleAPIController : ApiController
    {
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


            JavaScriptSerializer js = new JavaScriptSerializer();
            AttandanceRecordList attandanceRecordList = js.Deserialize<AttandanceRecordList>(content);

            return attandanceRecordList;
        }

        /// <summary>
		///     Send Shuffle list along with attandance summary and reminder message to slack channel
		/// </summary>

        public IHttpActionResult Shuffle()
        {
            var WebHookUrl =  WebConfigurationManager.AppSettings["WebHookUrlProd"];            
            var client = new SbmClient(WebHookUrl);
            var staffList = WebConfigurationManager.AppSettings["StaffList"];
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
                    if (item.duration.ToLower() == "full day" && item.reason.ToLower() != "tech investigation")
                    {
                        dayOffList += item.name;
                        if (index < attandanceRecordList.data.Count()) dayOffList += (", ");
                    }
                    index++;
                }
            }
            var shuffleListNoDayOff = shuffleList.Except(dayOffList.Replace(", ", ",").Split(',').ToList()).ToList();
            var reminder = "@channel Please don't forget to update your remaining points on the card that you are working on.";

			var message = new Message(System.DateTime.Now.Date.ToString("MMMM dd"));
            message.AddAttachment(new Attachment()
                .AddField("Day-off Today:", dayOffList, true)
                .AddField("Standup Order", shuffleListNoDayOff!=null? String.Join(", ", shuffleListNoDayOff) : "", true)
                .AddField("Reminder", reminder)
            );

            client.Send(message);
            return Ok();
        }



    }
}