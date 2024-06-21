namespace UploadFileProccessBar.Services
{
    public interface ICrudService<T> where T : class
    {
        public Task<List<T>> GetAsync();


        public Task<T?> GetAsync(Guid id);


        public Task CreateAsync(T model);
        public Task CreateAsync(List<T> models);

        public Task UpdateAsync(Guid id, T updateModel);
        public Task UpdateAsync(List<T> updateModel);

        public Task RemoveAsync(Guid id);

        public Task RemoveAsync(List<Guid> id);
    }
}
