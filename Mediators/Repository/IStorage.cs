namespace Mediators.Repository;

public interface IStorage<TModelRef, TModel>
    where TModel : notnull
{
    Task<TModel?> TryGetAsync(TModelRef id, CancellationToken ct = default);

    Task SetAsync(TModel model, CancellationToken ct = default);

    Task<IReadOnlyList<TModel>> GetAllAsync(CancellationToken ct = default);

    Task<int> CountAsync(CancellationToken ct = default);
}
