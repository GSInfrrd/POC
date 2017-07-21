using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CourtlandDox.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CourtlandDox.DataAccess
{
    public class InvoiceContext
    {
        private readonly IMongoDatabase _database;
        public InvoiceContext()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("AccumenData");
        }

        public bool Create(InvoiceCollection identifier)
        {
            bool returnRes = false;
            try
            {
                var identifierDocument = identifier.ToBsonDocument();
                var identifierCollection = _database.GetCollection<BsonDocument>("InvoiceCollection");
                identifierCollection.InsertOne(identifierDocument);
                returnRes = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return returnRes;
        }

        public List<InvoiceCollection> GetAll()
        {
            var returnRes = new List<InvoiceCollection>();
            try
            {
                var identifierCollection = _database.GetCollection<BsonDocument>("InvoiceCollection");
                var identifier = identifierCollection.FindSync<InvoiceCollection>(Builders<BsonDocument>.Filter.Empty);
                returnRes = identifier.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return returnRes;
        }

        public List<Models.Field> GetTagDetailIdentifier(string extractedText)
        {
            var returnRes = new List<Models.Field>();
            try
            {
                var filteredTags = GetAll();
                var addedTags = new List<string>();
                foreach (var tagDetail in filteredTags)
                {
                    if (Regex.IsMatch(extractedText, tagDetail.NameExpression,
                        RegexOptions.IgnoreCase)&&(!addedTags.Contains(tagDetail.NameField)))
                    {
                        var strSplitList = Regex.Split(extractedText, tagDetail.NameExpression, RegexOptions.IgnoreCase);
                        if (strSplitList.Length > 1)
                        {
                            if (Regex.IsMatch(strSplitList[1], tagDetail.ValueExpression,
                                RegexOptions.IgnoreCase))
                            {
                                var resTag = new Models.Field { name = tagDetail.NameField };
                                var res = Regex.Match(strSplitList[1], tagDetail.ValueExpression);
                                resTag.values = res.Value;
                                if (!String.IsNullOrEmpty(resTag.values))
                                {
                                    addedTags.Add(tagDetail.NameField);
                                    returnRes.Add(resTag);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return returnRes;
        }
    }
}