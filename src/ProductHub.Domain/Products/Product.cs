﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductHub.Domain.Products
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Units { get; set; }
    }
}
