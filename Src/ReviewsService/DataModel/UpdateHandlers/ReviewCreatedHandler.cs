using DataModel.Models.Review;
using Events.Reviews;
using Infrastructure.Core;
using Infrastructure.Core.Events;
using System.Threading;
using System.Threading.Tasks;

namespace DataModel.UpdateHandlers
{
    public class ReviewCreatedHandler(DatabaseContext db) : IEventHandler<ReviewCreatedEvent>
    {
        private readonly DatabaseContext _db = db;

        public async Task Handle(ReviewCreatedEvent @event, CancellationToken cancellationToken)
        {
            var review = Mapping.Map<ReviewCreatedEvent, Review>(@event);
            review.Id = @event.ReviewId;

            await _db.AddAsync(review, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
