using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Bills.Service.Common.VO
{
    [BsonIgnoreExtraElements]
    public class BillsVO
    {
        public ObjectId _id { get; set; }
        public string bill { get; set; }
        public int dueDate { get; set; }
        public int? dueLimit { get; set; }
        public int? lastPaidMonth { get; set; }
        public bool paid { get; set; }
    }
}
