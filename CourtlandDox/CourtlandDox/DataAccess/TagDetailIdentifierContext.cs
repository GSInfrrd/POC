using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using CourtlandDox.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CourtlandDox.DataAccess
{
    public class TagDetailIdentifierContext
    {
        private readonly IMongoDatabase _database;
        public TagDetailIdentifierContext()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("Courtland");
        }

        public bool Create(TagDetailIdentifierCollection tagDetail)
        {
            var returnRes = false;
            try
            {
                var tagDetailDocument = tagDetail.ToBsonDocument();
                var tagDetailCollection = _database.GetCollection<BsonDocument>("TagDetailIdentifierCollection");
                tagDetailCollection.InsertOne(tagDetailDocument);
                returnRes = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return returnRes;
        }

        public List<TagDetailIdentifierCollection> GetAll()
        {
            var returnRes = new List<TagDetailIdentifierCollection>();
            try
            {
                var tagDetailCollection = _database.GetCollection<BsonDocument>("TagDetailIdentifierCollection");
                var tagDetail = tagDetailCollection.FindSync<TagDetailIdentifierCollection>(Builders<BsonDocument>.Filter.Empty);
                returnRes = tagDetail.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return returnRes;
        }

        public List<TagDetailIdentifierCollection> GetByNoticeId(string id)
        {
            var returnRes = new List<TagDetailIdentifierCollection>();
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("NoticeId", id);
                var tagDetailCollection = _database.GetCollection<BsonDocument>("TagDetailIdentifierCollection");
                var tagDetail = tagDetailCollection.FindSync<TagDetailIdentifierCollection>(filter);
                returnRes = tagDetail.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return returnRes;
        }

        public List<Models.Tag> GetTagDetailIdentifier(string noticeId, string extractedText)
        {
            var returnRes = new List<Models.Tag>();
            try
            {
                var filteredTags = GetByNoticeId(noticeId);
                foreach (var tagDetail in filteredTags)
                {
                    if (Regex.IsMatch(extractedText, tagDetail.NameExpression,
                        RegexOptions.IgnoreCase))
                    {
                        var strSplitList = Regex.Split(extractedText, tagDetail.NameExpression, RegexOptions.IgnoreCase);
                        if (strSplitList.Length > 1)
                        {
                            if (Regex.IsMatch(strSplitList[1], tagDetail.ValueExpression,
                                RegexOptions.IgnoreCase))
                            {
                                var resTag = new Models.Tag { Name = tagDetail.NameField };
                                var res = Regex.Match(strSplitList[1], tagDetail.ValueExpression);
                                resTag.Value = res.Value;
                                if (!String.IsNullOrEmpty(resTag.Value))
                                    returnRes.Add(resTag);
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