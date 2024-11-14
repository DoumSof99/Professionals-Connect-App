using System;

namespace API.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository {get;}
    IMessageRespository MessageRespository {get;}
    ILikesRepository LikesRepository {get;}
    Task<bool> Complete();
    bool HasChanges();
}
