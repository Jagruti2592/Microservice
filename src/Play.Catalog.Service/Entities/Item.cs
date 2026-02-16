

using System;
using Play.Common;

namespace Play.Catalog.Service.Entities
{

    public record Item : IEntity
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}