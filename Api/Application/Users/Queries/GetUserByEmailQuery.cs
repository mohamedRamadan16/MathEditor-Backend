using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Entities;
using MediatR;

namespace Api.Application.Users.Queries
{
    public class GetUserByEmailQuery : IRequest<User?>
    {
        public string Email { get; }
        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }
    }

    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, User?>
    {
        private readonly Common.Interfaces.IUserRepository _userRepository;
        public GetUserByEmailQueryHandler(Common.Interfaces.IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _userRepository.FindUserByEmailAsync(request.Email);
        }
    }
}
