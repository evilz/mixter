namespace Mixter.Domain.Identity
{
    [Repository]
    public interface IUserIdentitiesRepository
    {
        UserIdentity GetUserIdentity(UserId userId);
    }
}