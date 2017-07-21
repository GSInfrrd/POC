using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CourtlandDox.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CourtlandDox.DataAccess
{
    public class BankContext
    {
        private readonly IMongoDatabase _database;
        public BankContext()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("Courtland");
        }

        public bool Create(BankCollection identifier)
        {
            var returnRes = false;
            try
            {
                var bankDocument = identifier.ToBsonDocument();
                var bankCollection = _database.GetCollection<BsonDocument>("BankCollection");
                bankCollection.InsertOne(bankDocument);
                returnRes = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return returnRes;
        }

        public List<BankCollection> GetAll()
        {
            var returnRes = new List<BankCollection>();
            try
            {
                var bankCollection = _database.GetCollection<BsonDocument>("BankCollection");
                var bank = bankCollection.FindSync<BankCollection>(Builders<BsonDocument>.Filter.Empty);
                returnRes = bank.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return returnRes;
        }


        public BankCollection GetBankDetail(string extractedText)
        {
            var res = new BankCollection();
            try
            {
                var banks = GetAll();
                var found = false;
                foreach (var bank in banks)
                {
                    if (Regex.IsMatch(extractedText, bank.Expression,
                        RegexOptions.IgnoreCase))
                    {
                        res = bank;
                        found = true;
                    }
                    if (found) break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return res;
        }
    }
}