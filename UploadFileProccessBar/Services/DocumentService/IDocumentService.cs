using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UploadFileProccessBar.Models;
using UploadFileProccessBar.Models.DataBase;
using UploadFileProccessBar.Services.DocumentItemService;

namespace UploadFileProccessBar.Services.DocumentService
{
    public interface IDocumentService : ICrudService<Documents>
    {
        public Task<(Documents, DocumentItem, bool exists)> CreateOrNewDoc(string id, int countItem, string type);

    }

    public class DocumentService : BaseService<Documents>, IDocumentService
    {
        private readonly IDocumentItemService _documentItemService;
        public DocumentService(IOptions<FileUploadContext> mongoDatabaseSettings, IDocumentItemService documentItemService) : base(mongoDatabaseSettings)
        {
            _documentItemService = documentItemService;
        }

        public async Task<Documents?> GetAsync(Guid id) =>
            await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        public async Task UpdateAsync(Guid id, Documents updateModel)
          => await _collection.ReplaceOneAsync(x => x.Id == id, updateModel);


        public async Task UpdateAsync(List<Documents> updateModel)
        { }
        public async Task RemoveAsync(Guid id)
        { }


        public async Task RemoveAsync(List<Guid> id)
        {
            throw new NotImplementedException();
        }
        public async Task<(Documents,DocumentItem,  bool exists)> CreateOrNewDoc(string id, int CountItem, string type)
        {

            Documents doc = null;
            if (!string.IsNullOrWhiteSpace(id))
                if (Guid.TryParse(id.ToString(), out Guid ID))
                    doc = await GetAsync(ID);
                 if (doc != null) return ( doc, await _documentItemService.CalcFreeItem(doc.Id), false );
            else
                doc = new Documents()
                {
                    Id = new Guid(),
                    CountItem = CountItem,
                    DocumentType = type,
                    FileLength = 0,
                    LengthUpload = 0 ,
                    CreateAt=DateTime.Now
                   

                };

            await CreateAsync(doc);
            


            for (int i = 0; i < CountItem; i++)
            {

                await _documentItemService.CreateAsync(new DocumentItem()
                {
                    Id = Guid.NewGuid(),
                    Document =  new byte[0],
                    LengthUpload = 0,
                    DocumentId = doc.Id,
                    Time = DateTime.Now.AddSeconds(i), 
                    Index=i , 
                    Completed=false
                });
            }


            return (doc , await _documentItemService.CalcFreeItem(doc.Id), true);



        }
    }
}
