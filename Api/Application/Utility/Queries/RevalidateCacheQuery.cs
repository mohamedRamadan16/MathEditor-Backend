using MediatR;

namespace Api.Application.Utility.Queries
{
    public class RevalidateCacheQuery : IRequest<RevalidateCacheResult>
    {
        public string? Path { get; set; }
        public string? Tag { get; set; }
        public RevalidateCacheQuery(string? path, string? tag)
        {
            Path = path;
            Tag = tag;
        }
    }

    public class RevalidateCacheResult
    {
        public bool Revalidated { get; set; }
        public long Now { get; set; }
        public string? Message { get; set; }
        public string? Value { get; set; }
    }
}
