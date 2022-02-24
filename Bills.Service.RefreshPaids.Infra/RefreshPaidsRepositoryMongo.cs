using Bills.Service.Common.VO;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bills.Service.RefreshPaids.Infra
{
    public class RefreshPaidsRepositoryMongo : IRefreshPaidsRepository
    {
        IConfiguration _configuration;
        IMongoDatabase _database;
        public RefreshPaidsRepositoryMongo(IConfiguration configuration)
        {
            _configuration = configuration;
            var client = new MongoClient(_configuration["ConnectionString"]);
            _database = client.GetDatabase("test");
        }

        public IEnumerable<BillsVO> GetAllResults()
        {
            var cars = _database.GetCollection<BillsVO>("bills").Find(_ => true).ToListAsync();

            return cars.Result;
        }

        public object SetAsPaidAsync(ObjectId id)
        {
            var updateResult = _database.GetCollection<BillsVO>("bills").UpdateOneAsync(
                rec => rec._id == id, Builders<BillsVO>
                .Update
                .Set(rec => rec.paid, true));

            return updateResult.Result.IsAcknowledged &&
                   updateResult.Result.ModifiedCount > 0;
        }

        public object SetAsUnPaidAsync(ObjectId id)
        {
            var updateResult = _database.GetCollection<BillsVO>("bills").UpdateOneAsync(
                 rec => rec._id == id, Builders<BillsVO>
                 .Update
                 .Set(rec => rec.paid, false));

            return updateResult.Result.IsAcknowledged &&
                   updateResult.Result.ModifiedCount > 0;
        }
    }
}
