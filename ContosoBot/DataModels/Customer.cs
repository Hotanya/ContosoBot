using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ContosoBot.DataModels
{
    public class Customer
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string firstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string lastName { get; set; }

        [JsonProperty(PropertyName = "dateOfBirth")]
        public string dateOfBirth { get; set; }

        [JsonProperty(PropertyName = "mobileNumber")]
        public int mobileNumber { get; set; }
    }
}