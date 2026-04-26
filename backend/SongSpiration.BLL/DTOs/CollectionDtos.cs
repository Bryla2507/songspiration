using System;
using System.Collections.Generic;

namespace SongSpiration.BLL.DTOs;

public class CollectionDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int PinCount { get; set; }
}

public class CreateCollectionDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateCollectionDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}