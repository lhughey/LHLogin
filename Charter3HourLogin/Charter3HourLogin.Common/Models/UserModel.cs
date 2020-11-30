using System;
using System.Globalization;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Charter3HourLogin.Common.Models
{
    public class UserModel : ReactiveObject
    {
        [Reactive] 
        [JsonProperty("firstName")] 
        public string FirstName { get; set; } = string.Empty;

        [Reactive] 
        [JsonProperty("lastName")] 
        public string LastName { get; set; } = string.Empty;

        [Reactive] 
        [JsonProperty("userName")] 
        public string UserName { get; set; } = string.Empty;

        //We could encrypt the password, but the auth would be done at the server level, so no need to store auth data locally
        [Reactive] 
        [JsonProperty("password")] 
        public string Password { get; set; } = string.Empty;

        [Reactive] 
        [JsonProperty("phone")] 
        public string Phone { get; set; } = string.Empty;

        [Reactive] 
        [JsonProperty("startDate")] 
        public DateTime StartDate { get; set; } = DateTime.MinValue;
    }
}