using DataModel;
using FluentValidation;
using Infrastructure.Core.Commands;
using Infrastructure.EventStores.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesService.Commands
{
    public class DeleteMoviesByUserIdCommand
    {
        public class Command : ICommand
        {
            public Guid UserId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.UserId).NotEmpty();
            }
        }

        public class Handler(IRepository<MovieAggregate> repository, DatabaseContext db) : ICommandHandler<Command>
        {
            private readonly IRepository<MovieAggregate> _repository = repository;
            private readonly DatabaseContext _db = db;

            private async Task<ICollection<Guid>> GetMovieIds(Guid userId)
            {
                var query = from movie in _db.Movies
                            where movie.UserId == userId
                            select movie.Id;

                var result = await query.ToListAsync();

                return result;
            }

            public async Task Handle(Command command, CancellationToken cancellationToken)
            {
                var movieIds = await GetMovieIds(command.UserId);

                var movies = await _repository.Find(movieIds);


                foreach (var movie in movies)
                {
                    movie.DeleteMovie(command.UserId);
                }

                await _repository.Delete(movies);
            }
        }
    }
}
