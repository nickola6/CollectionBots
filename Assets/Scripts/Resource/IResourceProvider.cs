using System.Collections.Generic;

public interface IResourceProvider
{
    IEnumerable<Resource> Resources { get; }
    bool IsOccupied(Resource resource);
}