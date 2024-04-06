using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;

namespace streamerCompanion
{
    class Twitch
    {
        
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> TwitchGetUser(string TwitchOAuth, string ClientID, string TargetUser)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.twitch.tv/helix/users?login=" + TargetUser.ToLower());
            requestMessage.Headers.Add("Client-ID", ClientID);
            requestMessage.Headers.Add("Authorization", "Bearer " + TwitchOAuth);

            var response = await client.SendAsync(requestMessage);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "";
            }

            var text = await response.Content.ReadAsStringAsync();
            dynamic array = JsonConvert.DeserializeObject(text);
            try
            {
                var txt = array["data"][0];
            }
            catch (Exception ex)
            {
                return "";
            }
            //var txt = array["data"][0];
            return array["data"][0].ToString();
        }

        public static async Task<string> TwitchGetLastActivity(string TwitchOAuth, string ClientID, string TargetUser)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.twitch.tv/helix/search/channels?query=" + TargetUser.ToLower() + "&first=1");
            requestMessage.Headers.Add("Client-ID", ClientID);
            requestMessage.Headers.Add("Authorization", "Bearer " + TwitchOAuth);

            var response = await client.SendAsync(requestMessage);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "";
            }

            var text = await response.Content.ReadAsStringAsync();
            dynamic array = JsonConvert.DeserializeObject(text);
            var txt = array["data"][0]["game_name"];
            return array["data"][0].ToString();
        }

        public static async Task<string> TwitchGetOauth(string TwitchOAuth, string ClientID, string ClientSecret)
        {
            string oauth = TwitchOAuth;
            int expTime = 0;
            if(TwitchOAuth != "")
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://id.twitch.tv/oauth2/validate");
                requestMessage.Headers.Add("Authorization", "Bearer " + TwitchOAuth);

                var response = await client.SendAsync(requestMessage);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    expTime = 0;
                }
                else
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return "";
                    }
                    var text = await response.Content.ReadAsStringAsync();
                    dynamic array = JsonConvert.DeserializeObject(text);
                    expTime = array["expires_in"];
                }
                
            }

            if(expTime < 100000 || TwitchOAuth == "")
            {
                string url = $"https://id.twitch.tv/oauth2/token?client_id={ClientID}&client_secret={ClientSecret}&grant_type=client_credentials&scope=user:read:email%20channel:moderate%20channel_editor%20chat:edit%20chat:read%20whispers:edit%20whispers:read%20channel:read:redemptions";
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                var response = await client.SendAsync(requestMessage);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return "";
                }
                var text = await response.Content.ReadAsStringAsync();
                dynamic array = JsonConvert.DeserializeObject(text);
                oauth = array["access_token"];
            }

            return oauth;
        }


        public static async Task<string> TwitchGetReward(string TwitchOAuth, string ClientID, string TargetUserID, string PointsID)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api.twitch.tv/helix/channel_points/custom_rewards?broadcaster_id={TargetUserID}&id={PointsID}");
            requestMessage.Headers.Add("Client-ID", ClientID);
            requestMessage.Headers.Add("Authorization", "Bearer " + TwitchOAuth);

            var response = await client.SendAsync(requestMessage);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "";
            }

            var text = await response.Content.ReadAsStringAsync();
            dynamic array = JsonConvert.DeserializeObject(text);
            return array["data"][0]["title"];
        }
    }
}
