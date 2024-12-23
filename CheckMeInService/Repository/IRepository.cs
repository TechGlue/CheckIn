namespace CheckMeInService.Repository;

public interface IRepository<T>
{
    T? GetById(Guid id);
    IEnumerable<T> GetAll();
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
}