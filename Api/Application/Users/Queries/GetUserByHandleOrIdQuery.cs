using System.Threading;
using System.Threading.Tasks;
using Api.Application.Common.Interfaces;
using Api.Domain.Entities;
using MediatR;

namespace Api.Application.Users.Queries
{
    public class GetUserByHandleOrIdQuery : IRequest<User?>
    {
        public string HandleOrId { get; }
        public GetUserByHandleOrIdQuery(string handleOrId)
        {
            HandleOrId = handleOrId;
        }
    }

    public class GetUserByHandleOrIdQueryHandler : IRequestHandler<GetUserByHandleOrIdQuery, User?>
    {
        private readonly IUserRepository _userRepository;
        public GetUserByHandleOrIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User?> Handle(GetUserByHandleOrIdQuery request, CancellationToken cancellationToken)
        {
            return await _userRepository.FindUserAsync(request.HandleOrId);
        }
    }
}
