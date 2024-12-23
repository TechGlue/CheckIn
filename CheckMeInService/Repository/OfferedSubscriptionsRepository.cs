using CheckMeInService.Models;

namespace CheckMeInService.Repository;

public class OfferedSubscriptionsRepository: IRepository<OfferedSubscriptions> 
{
    private readonly CheckMeInContext _checkMeInContext;

    public OfferedSubscriptionsRepository(CheckMeInContext checkMeInContext)
    {
        _checkMeInContext = checkMeInContext;
    }
    
    public OfferedSubscriptions? GetById(Guid id)
    {
        return _checkMeInContext.OfferedSubscriptions.SingleOrDefault(of => of.SubscriptionId == id);
    }

    public IEnumerable<OfferedSubscriptions> GetAll()
    {
        return _checkMeInContext.OfferedSubscriptions.Select(of => of);
    }

    public void Create(OfferedSubscriptions entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException();
        }

        _checkMeInContext.OfferedSubscriptions.Add(entity);
        _checkMeInContext.SaveChangesAsync();
    }

    public void Update(OfferedSubscriptions entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException();
        }

        var existingOfferedSubscription =
            _checkMeInContext.OfferedSubscriptions.SingleOrDefault(of => of.SubscriptionId == entity.SubscriptionId);

        if (existingOfferedSubscription is not null)
        {
            existingOfferedSubscription.SubscriptionId = entity.SubscriptionId;
            existingOfferedSubscription.SubscriptionName = entity.SubscriptionName;
            _checkMeInContext.SaveChangesAsync();
        }
    }

    public void Delete(OfferedSubscriptions entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException();
        }
        
        var existingOfferedSubscription =
            _checkMeInContext.OfferedSubscriptions.SingleOrDefault(of => of.SubscriptionId == entity.SubscriptionId);

        if (existingOfferedSubscription is not null)
        {
            _checkMeInContext.OfferedSubscriptions.Remove(existingOfferedSubscription);
            _checkMeInContext.SaveChangesAsync();
        }
    }
}