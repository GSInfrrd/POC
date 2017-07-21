using System;
using System.Collections.Generic;
using CourtlandDox.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CourtlandDox.DataAccess
{
    public class IdentifierContext
    {
        private readonly IMongoDatabase _database;
        public IdentifierContext()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("Courtland");
        }

        public bool Create(IdentifierCollection identifier)
        {
            bool returnRes = false;
            try
            {
                var identifierDocument = identifier.ToBsonDocument();
                var identifierCollection = _database.GetCollection<BsonDocument>("IdentifierCollection");
                identifierCollection.InsertOne(identifierDocument);
                returnRes = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return returnRes;
        }

        public List<IdentifierCollection> GetAll()
        {
            var returnRes = new List<IdentifierCollection>();
            try
            {
                var identifierCollection = _database.GetCollection<BsonDocument>("IdentifierCollection");
                var identifier = identifierCollection.FindSync<IdentifierCollection>(Builders<BsonDocument>.Filter.Empty);
                returnRes = identifier.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return returnRes;
        }
    }


}