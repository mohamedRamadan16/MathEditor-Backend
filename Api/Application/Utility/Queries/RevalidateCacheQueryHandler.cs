using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Application.Utility.Queries
{
    public class RevalidateCacheQueryHandler : IRequestHandler<RevalidateCacheQuery, RevalidateCacheResult>
    {
        public Task<RevalidateCacheResult> Handle(RevalidateCacheQuery request, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (string.IsNullOrEmpty(request.Path) && string.IsNullOrEmpty(request.Tag))
            {
                return Task.FromResult(new RevalidateCacheResult
                {
                    Revalidated = false,
                    Now = now,
                    Message = "Missing path or tag to revalidate"
                });
            }
            return Task.FromResult(new RevalidateCacheResult
            {
                Revalidated = true,
                Now = now,
                Value = request.Path ?? request.Tag
            });
        }
    }
}
