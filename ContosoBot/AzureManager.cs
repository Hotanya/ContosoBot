using Microsoft.WindowsAzure.MobileServices;
using ContosoBot.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoBot
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<Customer> timelineTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("https://contosobotmobile.azurewebsites.net");
            this.timelineTable = this.client.GetTable<Customer>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task AddTimeline(Customer timeline)
        {
            await this.timelineTable.InsertAsync(timeline);
        }

        public async Task<List<Customer>> GetTimelines()
        {
            return await this.timelineTable.ToListAsync();
        }

        public async Task DeleteTimeline(Customer timeline)
        {
            await this.timelineTable.DeleteAsync(timeline);
        }

        public async Task UpdateTimeline(Customer timeline)
        {
            await this.timelineTable.UpdateAsync(timeline);
        }
    }
}