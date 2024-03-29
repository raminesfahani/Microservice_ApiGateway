﻿using DataModel;
using DataModel.Models.User;
using Events.Users;
using FluentValidation;
using Infrastructure.Core;
using Infrastructure.Core.Commands;
using Infrastructure.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UsersService.Commands
{
    public class DeleteUserCommand
    {
        public class Command : ICommand
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.Id).NotEmpty();
            }
        }

        public class Handler(DatabaseContext db, IEventBus eventBus) : ICommandHandler<Command>
        {
            private readonly DatabaseContext _db = db;
            private readonly IEventBus _eventBus = eventBus;

            private async Task<User> GetUser(Guid id)
            {
                var query = from user in _db.Users
                            where user.Id == id
                            select user;

                var result = await query.FirstOrDefaultAsync();

                return result;
            }

            public async Task Handle(Command command, CancellationToken cancellationToken)
            {
                var user = await GetUser(command.Id);

                _db.Remove(user);

                var @event = Mapping.Map<User, UserDeletedEvent>(user);
                @event.UserId = user.Id;

                await _db.SaveChangesAndCommit(@event);
            }
        }
    }
}
