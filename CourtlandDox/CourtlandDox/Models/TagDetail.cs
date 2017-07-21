using System.Collections.Generic;

namespace CourtlandDox.Models
{
    public class TagDetail
    {
        public TagDetail()
        {
            Tags = new List<Tag>();
        }
        public string FileName { get; set; }
        public string Bank { get; set; }
        public string NoticeType { get; set; }
        public List<Tag> Tags { get; set; }
        public bool Executed { get; set; }

    }

    public class Tag
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}