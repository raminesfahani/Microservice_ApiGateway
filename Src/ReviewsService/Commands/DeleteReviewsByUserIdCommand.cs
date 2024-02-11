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

namespace ReviewsService.Commands
{
    public class DeleteReviewsByUserIdCommand
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

        public class Handler(IRepository<ReviewAggregate> repository, DatabaseContext db) : ICommandHandler<Command>
        {
            private readonly IRepository<ReviewAggregate> _repository = repository;
            private readonly DatabaseContext _db = db;

            private async Task<ICollection<Guid>> GetReviewIds(Guid userId)
            {
                var query = from review in _db.Reviews
                            where review.UserId == userId
                            select review.Id;

                var result = await query.ToListAsync();

                return result;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var reviewIds = await GetReviewIds(request.UserId);

                var reviews = await _repository.Find(reviewIds);

                foreach (var review in reviews)
                {
                    review.DeleteReview();
                }

                await _repository.Delete(reviews);
            }
        }
    }
}
