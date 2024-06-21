using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UploadFileProccessBar.Models;
using UploadFileProccessBar.Models.DataBase;

namespace UploadFileProccessBar.Services.DocumentItemService
{
    public interface IDocumentItemService : ICrudService<DocumentItem>
    {
        public Task<List<DocumentItem>> GetByDocumentId(Guid id);
        public Task<DocumentItem?> CalcFreeItem(Guid id);
    }
    public class DocumentItemService : BaseService<DocumentItem>, IDocumentItemService
    {
          

        public DocumentItemService(IOptions<FileUploadContext> mongoDatabaseSettings) : base(mongoDatabaseSettings)
        {
        }

        public async Task<DocumentItem?> GetAsync(Guid id)
            => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        public async Task<List<DocumentItem>> GetByDocumentId(Guid id)
         => await _collection.Find(x => x.DocumentId == id).ToListAsync();
        public async Task UpdateAsync(Guid id, DocumentItem updateModel)
            => await _collection.ReplaceOneAsync(x => x.Id == id, updateModel);

        public Task RemoveAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(List<Guid> id)
        {
            throw new NotImplementedException();
        }
        public Task UpdateAsync(List<DocumentItem> updateModel)
        {
            throw new NotImplementedException();
        }

        public async Task<DocumentItem?> CalcFreeItem(Guid id)
        {
          
            var l = await _collection.Find(x => x.DocumentId == id && !x.Completed).ToListAsync();
            return l.OrderBy(x => x.Index).FirstOrDefault();
        }
    }
}
