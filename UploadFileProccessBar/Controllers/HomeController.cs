using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using UploadFileProccessBar.Models;
using UploadFileProccessBar.Services.DocumentItemService;
using UploadFileProccessBar.Services.DocumentService;
namespace UploadFileProccessBar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDocumentService documentService;
        private readonly IDocumentItemService documentItemService;
        public int BYTE_SIZE_UPLOAD = 184935 *2 ; //15000000;
        public HomeController(ILogger<HomeController> logger, IDocumentService documentService, IDocumentItemService documentItem)
        {
            _logger = logger;
            this.documentService = documentService;
            this.documentItemService = documentItem;
        }
        public IActionResult Index()
        { return View(); }
        //https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-8.0&tabs=visual-studio
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost]
        public async Task<IActionResult> Upload(string type)
        {

            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            const string totalByte = "totalByte";
            const string _persent = "persent";
            Request.Headers.TryGetValue(totalByte, out StringValues content_length);
            Request.Headers.TryGetValue(_persent, out StringValues persent);
            Request.Headers.TryGetValue("id", out StringValues id);
            Request.Headers.TryGetValue("streamPreview", out StringValues streamPreview);

            double _content_length = double.Parse(content_length);
            var count = (int)Math.Ceiling(_content_length > BYTE_SIZE_UPLOAD ? _content_length / BYTE_SIZE_UPLOAD : 1);
            (Documents, DocumentItem, bool) result = await documentService.CreateOrNewDoc(id.ToString(), count, type);
            var newId = result.Item1.Id;



            if (int.Parse(persent) >= 100)
            {
                Console.WriteLine(result.Item1.Id);
                return Json(new
                {
                    LengthUpload = result.Item1.LengthUpload,
                    persent,
                    id = newId,
                    status = "ok",
                    file = bool.Parse(streamPreview) ? await getFile(newId) : null
                    // file:null
                });
            }

            if (!HttpContext.Request.Body.CanSeek)
                HttpContext.Request.EnableBuffering();
            HttpContext.Request.Body.Position = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                await HttpContext.Request.Body.CopyToAsync(ms);
                var bodyArray = ms.ToArray();
                long CONTENTlENGTH = bodyArray.Length;
                if (result.Item3)
                {

                  //  Console.WriteLine(result.Item1.Id);
                    result.Item2.Document = new byte[CONTENTlENGTH];
                    result.Item2.LengthUpload = bodyArray.Length;
                    Array.Copy(bodyArray.ToArray(), 0, result.Item2.Document, 0, bodyArray.Length);
                    result.Item1.FileLength = _content_length;
                    result.Item2.LengthUpload = bodyArray.Length;

                    await documentService.UpdateAsync(result.Item1.Id, result.Item1);
                    await documentItemService.UpdateAsync(result.Item2.Id, result.Item2);

                    return Json(new
                    {
                        LengthUpload = result.Item1.LengthUpload,
                        persent,
                        id = newId,
                        status = "pending"
                    });
                }
                else
                {

                    await CalcAndSave(bodyArray, result.Item1);

                }
            }
            HttpContext.Request.Body.Position = 0;
            return Json(new
            {
                LengthUpload = result.Item1.LengthUpload,
                persent,
                id = newId,
                status = "pending",
                file = bool.Parse(streamPreview) ? await getFile(newId) : null


            });

        }

        private async Task CalcAndSave(Byte[] bodyArray, Documents doc)
        {




            var item = await documentItemService.CalcFreeItem(doc.Id);
            long LengthBody = bodyArray.Length;
           // Console.WriteLine($"item len :  {item.Document.Length} - item id  : {item.Id} ");
            long LengthItem = item.Document.Length;
            var freeSpace = BYTE_SIZE_UPLOAD - LengthItem;

            long sizeUpload = LengthBody <= freeSpace ? LengthBody : freeSpace;

            var temp = new byte[LengthItem + sizeUpload];

            Array.Copy(item.Document, 0, temp, 0, LengthItem);

            Array.Copy(bodyArray, 0, temp, LengthItem, sizeUpload);
            item.Document = new byte[temp.Length];
            Array.Copy(temp, 0, item.Document, 0, temp.Length);
            doc.LengthUpload += sizeUpload;
            item.LengthUpload += sizeUpload;
            if (item.Document.Length == BYTE_SIZE_UPLOAD)
                item.Completed = true;

            await documentService.UpdateAsync(doc.Id, doc);
            await documentItemService.UpdateAsync(item.Id, item);

            if (LengthBody > freeSpace)
            {
                Console.WriteLine( "body > freeSpace : " + item.Document.Length);
                var DivBody = new byte[LengthBody - sizeUpload];
                Array.Copy(bodyArray, sizeUpload, DivBody, 0, (LengthBody - sizeUpload));
                await CalcAndSave(DivBody, doc);
            }

    Console.WriteLine($"item len :  {item.Document.Length} - item id  : {item.Id} ");

        }

        public async Task Tets()
        {

            /*   */

        }





        public async Task<IActionResult> ShowFile(Guid ? id)
        {



            if (id.HasValue)
            {
    var docItem = await documentItemService.GetAsync(id.Value);
return File(docItem.Document, "image/jpeg");
            }

            var d = await documentService.GetAsync();
            var doc = d.OrderByDescending(x => x.CreateAt).FirstOrDefault();
              id = doc.Id;
            var _file = await documentItemService.GetByDocumentId(id.Value);
            foreach (var item in _file.OrderByDescending(x => x.Index))
            {
                Console.WriteLine(item.Index);
            }


            byte[] bytes = new byte[_file.Sum(x => x.Document.Length)];
            foreach (var item in _file.OrderByDescending(x => x.Index).Select(x => x.Document))
            {
                Array.Copy(item, 0, bytes, 0, item.Length);
            }

            // return Content ("data:image/jpeg;base64," + Convert.ToBase64String(bytes.ToArray(), 0, bytes.ToArray().Length));
            // var array= _file.OrderBy(x=>x.Time).Select(x=>x.Document); 
            Console.WriteLine(bytes.Length);
            return File(bytes, doc.DocumentType);
        }
        public async Task<string> getFile(Guid id)
        {
            var doc = await documentService.GetAsync(id);
            var _file = await documentItemService.GetByDocumentId(id);



            byte[] bytes = new byte[_file.Sum(x => x.Document.Length)];
            foreach (var item in _file.Select(x => x.Document))
            {

                Array.Copy(item, 0, bytes, 0, bytes.Length);
            }
            return $"data:{doc.DocumentType};base64,{Convert.ToBase64String(bytes.ToArray(), 0, bytes.ToArray().Length)}";

            ;
        }



        public async Task<string> getFile(Documents doc)
        {

            var _file = await documentItemService.GetByDocumentId(doc.Id);



            byte[] bytes = new byte[_file.Sum(x => x.Document.Length)];
            foreach (var item in _file.Select(x => x.Document))
            {

                Array.Copy(item, 0, bytes, 0, bytes.Length);
            }
            return $"data:{doc.DocumentType};base64,{Convert.ToBase64String(bytes.ToArray(), 0, bytes.ToArray().Length)}"

                     ;
        }

        public async Task<IActionResult> ListDocument()
        {
            var documents = await documentService.GetAsync();
            List<string> items = new List<string>();

            foreach (var item in documents)
            {
                items.Add(await getFile(item));

            }
            return View();



        }
    }
}