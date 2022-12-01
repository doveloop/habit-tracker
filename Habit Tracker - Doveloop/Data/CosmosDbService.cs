namespace Habit_Tracker___Doveloop.Data
{
    using Microsoft.Azure.Cosmos;
    using Habit_Tracker___Doveloop.Models;

    public class CosmosDbService : ICosmosDbService
    {
        private CosmosClient _client;
        private Container _habitLabelContainer;
        private Container _profileContainer;
        private string _user;
        private PartitionKey _partitionKey;

        public CosmosDbService(CosmosClient dbClient, string DbName, string habitLabelContainerName, string profileContainerName)
        {
            _client = dbClient;
            _habitLabelContainer = _client.GetContainer(DbName, habitLabelContainerName);
            _profileContainer = _client.GetContainer(DbName, profileContainerName);
        }

        public void SetUser(string user)
        {
            _user = user;
            _partitionKey = new PartitionKey(user);
        }

        public async Task<UserProfile> GetProfileAsync()
        {
            var query = _profileContainer.GetItemQueryIterator<UserProfile>(new QueryDefinition("SELECT TOP 1 * FROM c WHERE c.user = \"" + _user + "\""));
            var response = await query.ReadNextAsync();
            return response.FirstOrDefault();
        }

        public async Task UpdateProfileAsync(UserProfile profile)
        {
            await _profileContainer.UpsertItemAsync(profile, _partitionKey);
        }

        #region HabitsLabels

        public async Task AddHabitEntryAsync(string id, DateTime time, float units)
        {
            HabitLabel habit = await GetHabitLabelAsync(id);
            habit.Entries.Insert(0, new HabitEntry
            {
                dateTime = time,
                Units = units
            });
            await UpdateHabitLabelAsync(habit);
        }
        public async Task AddHabitLabelAsync(HabitLabel habitLabel)
        {
            await _habitLabelContainer.CreateItemAsync<HabitLabel>(habitLabel, _partitionKey);
        }

        public async Task DeleteHabitLabelAsync(HabitLabel habitLabel)
        {
            //Remove relations
            foreach (Guid relationId in habitLabel.RelationIds ?? Array.Empty<Guid>().ToList())
            {
                HabitLabel relation = await GetHabitLabelAsync(relationId.ToString());
                relation.RelationIds.Remove(habitLabel.Id);
                await UpdateHabitLabelAsync(relation);
            }

            await _habitLabelContainer.DeleteItemAsync<HabitLabel>(habitLabel.Id.ToString(), _partitionKey);
        }

        public async Task<IEnumerable<HabitLabel>> GetHabitsAsync()
        {
            return await GetHabitsLabelsAsync("SELECT * FROM c WHERE c.user = \"" + _user + "\" AND c.type = \"habit\"");
        }

        public async Task<IEnumerable<HabitLabel>> GetLabelsAsync()
        {
            return await GetHabitsLabelsAsync("SELECT * FROM c WHERE c.user = \"" + _user + "\" AND c.type = \"label\"");
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

        public async Task<IEnumerable<HabitLabel>> GetHabitsLabelsAsync()
        {
            return await GetHabitsLabelsAsync("Select * FROM c WHERE c.user = \"" + _user + "\"");
        }

        public async Task<IEnumerable<HabitLabel>> GetHabitsLabelsAsync(string queryString)
        {
            var query = _habitLabelContainer.GetItemQueryIterator<HabitLabel>(new QueryDefinition(queryString));
            List<HabitLabel> results = new List<HabitLabel>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task UpdateHabitLabelAsync(HabitLabel habitLabel)
        {
            await _habitLabelContainer.UpsertItemAsync(habitLabel, _partitionKey);
        }

        public async Task UpdateHabitLabelAsync(HabitLabel habitLabel, List<Guid> oldRelationIds)
        {
            //delete old relations
            oldRelationIds.Where(oldId => !habitLabel.RelationIds.Contains(oldId)).ToList().ForEach(async oldId =>
            {
                HabitLabel oldRelation = await GetHabitLabelAsync(oldId.ToString());
                oldRelation.RelationIds.Remove(habitLabel.Id);
                await UpdateHabitLabelAsync(oldRelation);
            });
            //add new relations
            habitLabel.RelationIds.Where(newId => !oldRelationIds.Contains(newId)).ToList().ForEach(async newId =>
            {
                HabitLabel newRelation = await GetHabitLabelAsync(newId.ToString());
                newRelation.RelationIds.Add(habitLabel.Id);
                await UpdateHabitLabelAsync(newRelation);
            });
            await UpdateHabitLabelAsync(habitLabel);
        }
        #endregion
    }
}
