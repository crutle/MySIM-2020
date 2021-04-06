using MySIM.Models;
using MySIM.Views;
using MySIM.Views.Students;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Syncfusion.XForms.Buttons;
using Syncfusion.XForms.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;


namespace MySIM.ViewModels
{
    //Reference: https://github.com/SyncfusionExamples/Chat-GettingStarted-in-Xamarin-Forms
    public class SfChatViewModel : INotifyPropertyChanged
    {
        //Collection of messages in conversation.
        private ObservableCollection<object> messages;
        private Author currentUser;
        private readonly DatabaseController db = new DatabaseController();
        private readonly UserSettingsController userData = new UserSettingsController();
        private readonly Users stud = new Users();
        private static string name, POSTResult, userInput;
        private readonly List<string> qnAMakerLinks = new List<string>();

        public INavigation Navigation { get; set; }


        public SfChatViewModel(INavigation nav)
        {
            Navigation = nav;
            stud = db.GetOneStudent(userData.UserRecordID);
            name = stud.User_LastName + " " + stud.User_FirstName;
          
            this.messages = new ObservableCollection<object>();
            this.currentUser = new Author() { Name = name };
            this.GenerateDefaultMessage();
        }
        
        private void GenerateDefaultMessage()
        {
            this.messages.Add(new TextMessage()
            {
                Author = new Author() { Name = "MySIM Bot"},
                Text = "Hello! How can I help you today?",
            });
        }

        public async void SendMessage(object sender, SendMessageEventArgs e)
        {
            try
            {
                userInput = e.Message.Text.Trim();

                if(userInput.ToLower().Equals("y") || userInput.ToLower().Equals("yes") || userInput.ToLower().Equals("n") || userInput.ToLower().Equals("no"))
                {
                    //Check previous message.
                    if (messages[messages.Count - 1].GetType() == typeof(TextMessage))
                    {
                        TextMessage lastMessageObj = new TextMessage();
                        lastMessageObj = (TextMessage)messages[messages.Count - 1];
                        if(lastMessageObj.Text.Equals("Would you like to email to student services? (Y/N)"))
                        {
                            //Yes reply.
                            if(userInput.ToLower().Equals("y") || userInput.ToLower().Equals("yes"))
                            {
                                await Navigation.PushAsync(new ContactUs("student services"));
                            }
                            //No reply.
                            else
                            {
                                await Task.Run(() => AddTextMessage("How can I help you?"));
                            }
                        }
                        else
                        {
                            await PostMessageToQnAKBAsync(userInput);
                            FormatResponseString();
                        }
                    }
                }
                else
                {
                    await PostMessageToQnAKBAsync(userInput);
                    FormatResponseString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send message and get a response: " + ex.Message + " (Contact Administrator)");
            }
        }
       
        private void FormatResponseString()
        {
            int endIndex, listCount;
          
            try
            {
                if (POSTResult != null)
                {
                    JObject postResObj = JObject.Parse(POSTResult);
                    string qnAAns = (string)postResObj["answers"][0]["answer"];
                    string tempStr = qnAAns;

                    qnAMakerLinks.Clear();

                    //Put strings (sentences & paragraphs) into list.
                    while (!string.IsNullOrEmpty(tempStr))
                    {
                        endIndex = tempStr.IndexOf("\n");

                        if (endIndex == 0)
                        {
                            //Check if there is a "\n" in the rest of string.
                            Match match = Regex.Matches(tempStr, Regex.Escape("\n"))
                                               .Cast<Match>()
                                               .Skip(1)
                                               .FirstOrDefault();

                            //"\n" exists and "\n" is right after.
                            if (match != null && match.Index == 1)
                            {
                                //Check if last string.
                                Match match2 = CheckIfLastStringDoubleNewLine(tempStr);

                                //Not last string.
                                if (match2 != null)
                                {
                                    qnAMakerLinks.Add(tempStr.Substring(0, match2.Index));
                                    tempStr = tempStr.Substring(match2.Index);
                                }
                                //Last string.
                                else
                                {
                                    qnAMakerLinks.Add(tempStr.Substring(0));
                                    tempStr = "";
                                }
                            }
                            //"\n" exists and "\n" is not right after.
                            else if (match != null && match.Index != 1)
                            {
                                //Check if last string.
                                Match match2 = CheckIfLastStringSingleNewLine(tempStr);

                                //Not last string.
                                if (match2 != null)
                                {
                                    qnAMakerLinks.Add(tempStr.Substring(0, match2.Index));
                                    tempStr = tempStr.Substring(match2.Index);
                                }
                                //Last string.
                                else
                                {
                                    qnAMakerLinks.Add(tempStr.Substring(0));
                                    tempStr = "";
                                }

                            }
                            //Last string.
                            else if (match == null)
                            {
                                qnAMakerLinks.Add(tempStr.Substring(0));
                                tempStr = "";
                            }
                        }
                        else if (endIndex != -1)
                        {
                            qnAMakerLinks.Add(tempStr.Substring(0, endIndex));
                            tempStr = tempStr.Substring(endIndex);
                        }
                        //endIndex = -1 = does not exists.
                        else
                        {
                            qnAMakerLinks.Add(tempStr);
                            tempStr = "";
                        }
                    }

                    listCount = qnAMakerLinks.Count;
                    DisplayResponses(listCount);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to format response string: " + ex.Message + " (Contact Administrator)");
            }
        }

        private void DisplayResponses(int listCount)
        {
            string notHyperLinkText = "";
            int count = 0;

            try
            {
                if (listCount != 1)
                {
                    //Decide to add as hyperlink message or text message.
                    while (count < listCount)
                    {
                        //First string in list.
                        if (count == 0)
                        {
                            //Hyperlink
                            if (qnAMakerLinks[count].Contains("https"))
                            {
                                AddHyperlinkMessage(count);
                            }
                            //Text Message
                            else
                            {
                                notHyperLinkText = String.Join("", qnAMakerLinks[count].Split('#', '*'));
                            }

                        }
                        //Last string in list.
                        else if (count == listCount - 1)
                        {
                            //Hyperlink
                            if (qnAMakerLinks[count].Contains("https"))
                            {
                                AddHyperlinkMessage(count);
                            }
                            //Text Message
                            else
                            {
                                notHyperLinkText += String.Join("", qnAMakerLinks[count].Split('#', '*'));
                                AddTextMessage(notHyperLinkText);
                                notHyperLinkText = "";
                            }
                        }
                        //Not first or last string in list.
                        else
                        {
                            //Prev Hyperlink, Current Hyperlink
                            if (qnAMakerLinks[count - 1].Contains("https") && qnAMakerLinks[count].Contains("https"))
                            {
                                AddHyperlinkMessage(count);
                            }
                            //Prev Text, Current Hyperlink - Add Text Message & Hyperlink.
                            else if (!qnAMakerLinks[count - 1].Contains("https") && qnAMakerLinks[count].Contains("https"))
                            {
                                //Previous text message not added.
                                if (!string.IsNullOrEmpty(notHyperLinkText))
                                {
                                    AddTextMessage(notHyperLinkText);
                                    //Reset string for text message.
                                    notHyperLinkText = "";
                                }
                                AddHyperlinkMessage(count);
                            }
                            //Prev Hyperlink, Current Text
                            else if (qnAMakerLinks[count - 1].Contains("https") && !qnAMakerLinks[count].Contains("https"))
                            {
                                notHyperLinkText += String.Join("", qnAMakerLinks[count].Split('#', '*'));
                            }
                            //Prev Text, Current Text
                            else
                            {
                                notHyperLinkText += String.Join("", qnAMakerLinks[count].Split('#', '*'));
                            }
                        }
                        count++;
                    }
                }
                else
                {
                    notHyperLinkText = "";

                    if (qnAMakerLinks[0].Equals("Please use the \"Contact Us\" function / tab to contact SIM."))
                    {
                        AddTextMessage("Would you like to email to student services? (Y/N)");
                    }
                    else
                    {
                        AddTextMessage(qnAMakerLinks[0]);
                    }                   
                    
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to display responses: " + ex.Message + " (Contact Administrator)");
            }            
        }

        private void AddHyperlinkMessage(int listIndex)
        {
            string[] str = (qnAMakerLinks[listIndex]).Split(new string[] { "](" }, StringSplitOptions.None);

            string hyperlinkTitle = String.Join("", str[0].Split('[', ']', '\n')); 
            string hyperlink = String.Join("", str[1].Split('(', ')'));  

            messages.Add(new HyperlinkMessage()
            {
                Author = new Author() { Name = "MySIM Bot" },
                Text = hyperlinkTitle,
                Url = hyperlink
            });
        }

        private void AddTextMessage(string content)
        {
            messages.Add(new TextMessage()
            {
                Author = new Author() { Name = "MySIM Bot"},
                Text = content
            });
        }

        private Match CheckIfLastStringDoubleNewLine(string text)
        {
            return Regex.Matches(text, Regex.Escape("\n"))
                                               .Cast<Match>()
                                               .Skip(2)
                                               .FirstOrDefault();
        }
             
        private Match CheckIfLastStringSingleNewLine(string text)
        {
            return Regex.Matches(text, Regex.Escape("\n"))
                                               .Cast<Match>()
                                               .Skip(1)
                                               .FirstOrDefault();
        }

        private async Task PostMessageToQnAKBAsync(string text)
        {
            try
            {
                //Reference: https://github.com/Microsoft/BotBuilder-Samples/issues/699
                var body = JsonConvert.SerializeObject(
                    new
                    {
                        question = text,
                        top = 1,
                    }, Formatting.None);

                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage())
                    {
                        string uri = "https://mysim-qna.azurewebsites.net/qnamaker/knowledgebases/63f6bc49-9052-4d0b-8626-b48b4e13554e/generateAnswer";
                        request.Method = HttpMethod.Post;
                        request.RequestUri = new Uri(uri);
                        request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                        request.Headers.Add("Authorization", "EndpointKey 5550eeab-5935-47e0-ae02-73b318a6364c");

                        var response = await client.SendAsync(request);
                        var result = response.Content.ReadAsStringAsync().Result;

                        POSTResult = result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get response: " + ex.Message + " (Contact Administrator)");
            }
        }        

       /*  REUSE OF CODE (BELOW)
        *  Title: SyncfusionExamples/Chat-GettingStarted-in-Xamarin-Forms
        *  Author: vimal1994
        *  Date: 20 Jan 2021
        *  Code version: NA
        *  Type: Source Code
        *  Available at: https://github.com/SyncfusionExamples/Chat-GettingStarted-in-Xamarin-Forms
        */

        /// <summary>
        /// Gets or sets the group message conversation.
        /// </summary>
        public ObservableCollection<object> Messages
        {
            get
            {
                return this.messages;
            }

            set
            {
                this.messages = value;
            }
        }

        /// <summary>
        /// Gets or sets the current author.
        /// </summary>
        public Author CurrentUser
        {
            get
            {
                return this.currentUser;
            }
            set
            {
                this.currentUser = value;
                RaisePropertyChanged("CurrentUser");
            }
        }

        /// <summary>
        /// Property changed handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when property is changed.
        /// </summary>
        /// <param name="propName">changed property name</param>
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}