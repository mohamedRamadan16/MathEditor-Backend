using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Application.Revisions.DTOs;
using MediatR;
using Api.Application.Common.Interfaces;

namespace Api.Application.Revisions.Commands
{
    public class CreateRevisionCommand : IRequest<RevisionResponseDto?>
    {
        public CreateRevisionDto Dto { get; }
        public Guid UserId { get; }
        public string? UserEmail { get; }
        public string? UserName { get; }
        public CreateRevisionCommand(CreateRevisionDto dto, Guid userId, string? userEmail, string? userName)
        {
            Dto = dto;
            UserId = userId;
            UserEmail = userEmail;
            UserName = userName;
        }
    }
}
