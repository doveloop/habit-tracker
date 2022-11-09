namespace Habit_Tracker___Doveloop.Data
{
    using Microsoft.Azure.Cosmos;
    using Habit_Tracker___Doveloop.Models;

    public class CosmosDbService : ICosmosDbService
    {
        private CosmosClient _client;
        private Container _habitLabelContainer;
        private PartitionKey _partitionKey;

        public CosmosDbService(CosmosClient dbClient, string habitLabelDbName, string containerName)
        {
            _client = dbClient;
            _habitLabelContainer = _client.GetContainer(habitLabelDbName, containerName);
        }

        public void SetUser(string user)
        {
            _partitionKey = new PartitionKey(user);
        }

        #region HabitsLabels
        public async Task AddHabitLabelAsync(HabitLabel habit)
        {
            await _habitLabelContainer.CreateItemAsync<HabitLabel>(habit, _partitionKey);
        }

        public async Task DeleteHabitLabelAsync(string id)
        {
            await _habitLabelContainer.DeleteItemAsync<HabitLabel>(id, _partitionKey);
        }

        public async Task<HabitLabel> GetHabitLabelAsync(string id)
        {
            try
            {
                ItemResponse<HabitLabel> response = await _habitLabelContainer.ReadItemAsync<HabitLabel>(id, _partitionKey);
                return response.Resource;
            } catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<HabitLabel>> GetHabitsLabelsAsync(string queryString)
        {
            var query = _habitLabelContainer.GetItemQueryIterator<HabitLabel>(new QueryDefinition(queryString));
            List<HabitLabel> results = new List<HabitLabel>();
            while(query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task UpdateHabitLabelAsync(HabitLabel habit)
        {
            await _habitLabelContainer.UpsertItemAsync(habit, _partitionKey);
        }
        #endregion
    }
}
