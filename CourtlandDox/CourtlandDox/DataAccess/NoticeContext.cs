using System;
using System.Collections.Generic;
using CourtlandDox.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CourtlandDox.DataAccess
{
    public class NoticeContext
    {
        private readonly IMongoDatabase _database;
        public NoticeContext()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("Courtland");
        }

        public bool Create(NoticeCollection notice)
        {
            bool returnRes = false;
            try
            {
                var noticeDocument = notice.ToBsonDocument();
                var noticeCollection = _database.GetCollection<BsonDocument>("NoticeCollection");
                noticeCollection.InsertOne(noticeDocument);
                returnRes = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return returnRes;
        }

        public NoticeCollection GetById(string id)
        {
            var returnRes = new NoticeCollection();
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
                var noticeCollection = _database.GetCollection<BsonDocument>("NoticeCollection");
                var notice = noticeCollection.FindSync<NoticeCollection>(filter);
                returnRes = notice.FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return returnRes;
        }

        public List<NoticeCollection> GetAll()
        {
            var returnRes = new List<NoticeCollection>();
            try
            {
                var noticeCollection = _database.GetCollection<BsonDocument>("NoticeCollection");
                var notice = noticeCollection.FindSync<NoticeCollection>(Builders<BsonDocument>.Filter.Empty);
                returnRes = notice.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return returnRes;
        }

        public (List<string>,List<string>) GetTypes(string extractedText, List<IdentifierCollection> allIdentifiers)
        {
            var returnRes = new List<string>();
            var resNotices=new List<string>();
            try
            {
                foreach (var identifier in allIdentifiers)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(extractedText, identifier.Expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        if (!resNotices.Contains(identifier.NoticeId))
                        {
                            var notice = GetById(identifier.NoticeId);
                            resNotices.Add(identifier.NoticeId);
                            returnRes.Add(notice.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return (returnRes,resNotices);
        }

    }
}