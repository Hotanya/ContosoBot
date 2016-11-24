///Hotanya Ragtah 2016

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Text;
using ContosoBot.DataModels;
using System.Collections.Generic;

namespace ContosoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        ///
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {



                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                //Setup State Client
                StateClient stateClient = activity.GetStateClient();
                //Grab User Data
                BotData userData = stateClient.BotState.GetPrivateConversationData(activity.ChannelId, activity.Conversation.Id, activity.From.Id);
                // Get BotUserData

                bool isNameAsked = false;
                bool isCurrencyRequest = true;
                string botOutput = "Hello, I am the Contoso Bank Bot! Please enter your password to continue.";

                if (activity.Text.ToLower().Equals("hello"))
                {
                    Activity replyToConversation = activity.CreateReply("Hi There!");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();

                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "https://github.com/Hotanya/ContosoBotLogo/blob/master/ContosoLogoBig.png?raw=true"));

                 

                    ThumbnailCard plCard = new ThumbnailCard()
                    {
                        Title = "I am the Contoso Bank Bot.",
                        Subtitle = "Please enter your password to continue.",
                        Images = cardImages,
                        
                    };

                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    await connector.Conversations.SendToConversationAsync(replyToConversation);

                    return Request.CreateResponse(HttpStatusCode.OK);

                }

                /*if (activity.Text == "hello")
                {
                    Activity infReply = activity.CreateReply(botOutput);
                    await connector.Conversations.ReplyToActivityAsync(infReply);
                }
                */

                if (activity.Text == "*******") // 7 stars
                {
                    string userInput = activity.Text;

                    userData.SetProperty<string>("name", userInput); //use this in nameReply
                    {
                        botOutput = activity.Type;
                        Activity nameReply = activity.CreateReply("Hello" + " " + "Hotanya" + ", how may I help you?");
                        await connector.Conversations.ReplyToActivityAsync(nameReply);
                    }

                }

                

                //database code


                // Save BotUserData
                // stateClient.BotState.SetPrivateConversationData(activity.ChannelId, activity.Conversation.Id, activity.From.Id, userData);

                //Activity infoReply = activity.CreateReply(strReplyMessage.ToString());

                //await connector.Conversations.ReplyToActivityAsync(infoReply);
                // var response = Request.CreateResponse(HttpStatusCode.OK);

                //VIEW FROM DATABASE
                if (activity.Text.ToLower().Equals("view customers"))
                {
                    List<Customer> timelines = await AzureManager.AzureManagerInstance.GetTimelines();
                    botOutput = "";
                    foreach (Customer t in timelines)
                    {
                        botOutput += t.ID + t.firstName + " " + t.lastName + " " + t.dateOfBirth + t.accountType + "\n\n";
                    }
                    isCurrencyRequest = false;

                }

                // ADD TO DATABASE
                if (activity.Text.Length > 16)
                {

                    if (activity.Text.ToLower().Substring(0, 16).Equals("add new customer"))
                    {

                        string subs = activity.Text.Substring(17);
                        string[] split = subs.Split(' ');
                        userData.SetProperty<string>("Id", split[0]);
                        userData.SetProperty<string>("firstName", split[1]);
                        userData.SetProperty<string>("lastName", split[2]);
                        userData.SetProperty<string>("dateOfBirth", split[3]);
                        userData.SetProperty<string>("accountType", split[4]);


                        Customer timeline = new Customer()
                        {
                            ID = userData.GetProperty<string>("Id"),
                            firstName = userData.GetProperty<string>("firstName"),
                            lastName = userData.GetProperty<string>("lastName"),
                            dateOfBirth = userData.GetProperty<string>("dateOfBirth"),
                            accountType = userData.GetProperty<string>("accountType"),

                        };

                        await AzureManager.AzureManagerInstance.AddTimeline(timeline);

                        isCurrencyRequest = false;

                        botOutput = "New timeline added [" + timeline.firstName + timeline.lastName + timeline.dateOfBirth + "]";
                    }
                    isCurrencyRequest = false;

                    //UPDATE DATABASE


                    //DELETE FROM DATABASE
                    if (activity.Text.ToLower().Equals("delete account"))
                    {
                        List<Customer> timelines = await AzureManager.AzureManagerInstance.GetTimelines();
                        string id = userData.GetProperty<string>("Id");
                        foreach (Customer c in timelines)
                        {
                            if (c.ID.Equals(id))
                            {
                                await AzureManager.AzureManagerInstance.DeleteTimeline(c);

                                botOutput = c.firstName + c.lastName + "'s (Id #" + c.ID + ")" + c.accountType + "account" + " was deleted.";

                            }
                        }

                    }
                }

                if (!isCurrencyRequest)
                {
                    // return our reply to the user
                    Activity infoReply = activity.CreateReply(botOutput);


                await connector.Conversations.ReplyToActivityAsync(infoReply);

                }
                else
                {

                    currencyInfo.RootObject rootObject;
                    HttpClient client = new HttpClient();
                    string api = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base=nzd"));

                    rootObject = JsonConvert.DeserializeObject<currencyInfo.RootObject>(api);

                    string date = rootObject.date;
                    double usd = rootObject.rates.USD;
                    double aud = rootObject.rates.AUD;
                    double inr = rootObject.rates.INR;
                    double jpy = rootObject.rates.JPY;
                    double eur = rootObject.rates.EUR;
                    double gbp = rootObject.rates.GBP;

                    if (activity.Text.ToLower().Length == 3)
                    {
                        string title = "";
                    if (activity.Text == "aud")
                    {
                        //return our reply to the user
                        title = $"For 1 New Zealand Dollar, you will get {aud} Australia Dollars";
                       // await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    else if (activity.Text == "AUD")
                    {
                        //return our reply to the user
                        title = $"For 1 New Zealand Dollar, you will get {aud} Australia Dollars";
                        //await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    else if (activity.Text == "inr")
                    {
                        //return our reply to the user
                        title = $"For 1 New Zealand Dollar, you will get {inr} Indian Rupees";
                        //await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    else if (activity.Text == "INR")
                    {
                        //return our reply to the user
                        title = "For 1 New Zealand Dollar, you will get {inr} Indian Rupees";
                        //await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    else if (activity.Text == "jpy")
                    {
                        //return our reply to the user
                        title = $"For 1 New Zealand Dollar, you will get {jpy} Japanese Yen";
                        //await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    else if (activity.Text == "JPY")
                    {
                        //return our reply to the user
                        title = $"For 1 New Zealand Dollar, you will get {jpy} Japanese Yen";
                        //await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    else if (activity.Text == "eur")
                    {
                        //return our reply to the user
                        title = $"For 1 New Zealand Dollar, you will get {eur} Euros";
                        //await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    else if (activity.Text == "EUR")
                    {
                        //return our reply to the user
                        title = $"For 1 New Zealand Dollar, you will get {eur} Euros";
                        //await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    else if (activity.Text == "gbp")
                    {
                        //return our reply to the user
                        title = $"For 1 New Zealand Dollar, you will get {gbp} British Pounds";
                        //await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    else if (activity.Text == "GBP")
                    {
                        //return our reply to the user
                        title = $"For 1 New Zealand Dollar, you will get {gbp} British Pounds";
                        //await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    else if (activity.Text == "usd")
                    {
                        //return our reply to the user
                        title = $"For 1 New Zealand Dollar, you will get {usd} US Dollars";
                        //await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    else if (activity.Text == "USD")
                    {
                        //return our reply to the user
                        title = $"For 1 New Zealand Dollar, you will get {usd} US Dollars";
                        //await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    

                        Activity replyToConversation = activity.CreateReply("Your request:");
                        replyToConversation.Recipient = activity.From;
                        replyToConversation.Type = "message";
                        replyToConversation.Attachments = new List<Attachment>();

                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: "https://cdn2.iconfinder.com/data/icons/finance-26/128/finance_-43-128.png"));

                        List<CardAction> cardButtons = new List<CardAction>();
                        CardAction plButton = new CardAction()
                        {
                            Value = "http://www.xe.com/currency/nzd-new-zealand-dollar",
                            Type = "openUrl",
                            Title = "Currency Info"
                        };
                        cardButtons.Add(plButton);

                        ThumbnailCard plCard = new ThumbnailCard()
                        {
                            Title = title,
                            Subtitle = "For more information, please visit the site below:",
                            Images = cardImages,
                            Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        await connector.Conversations.SendToConversationAsync(replyToConversation);

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            else
            {
                HandleSystemMessage(activity);

            }
                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }



        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
              
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}



