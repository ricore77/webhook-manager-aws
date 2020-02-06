//using Amazon;
//using Amazon.DynamoDBv2;
//using Amazon.DynamoDBv2.DataModel;
//using Amazon.DynamoDBv2.DocumentModel;
//using Amazon.Runtime;
//using WebhookManager.Helpers;
//using GrapeCity.Documents.Imaging;
//using Microsoft.Extensions.Logging;
//using Nest;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;

//namespace WebhookManager.Services
//{
//    public class ProfileService : IProfileService
//    {
       
//        private static string tableName = "Profile";
//        private Table _profilesTable;
//        DynamoDBContext _dynamodbContext;
//        private readonly IElasticClient _elasticClient;
//        private readonly ILogger _logger;
//        private AmazonDynamoDBClient _dynamoClient;

//        private  AmazonDynamoDBClient DynamoClient
//        {
//            get
//            {
//                if (_dynamoClient == null)
//                    _dynamoClient = AwsFactory.CreateClient<AmazonDynamoDBClient>();
//                return _dynamoClient;
//            }
//        }



//        private Table ProfilesTable
//        {
//            get
//            {
//                if (_profilesTable == null)
//                {
//                    _profilesTable = Table.LoadTable(DynamoClient, tableName);
//                }
//                return _profilesTable;
//            }
//            set
//            {
//                ProfilesTable = value;
//            }
//        }
       
//        private DynamoDBContext DynamoDBContext
//        {
//            get
//            {
//                if (_dynamodbContext == null)
//                {
//                    _dynamodbContext = new DynamoDBContext(DynamoClient);
//                    //ProfilesTable = Table.LoadTable(DynamoClient, tableName);

//                }
//                return _dynamodbContext;
//            }

//        }

//        public ProfileService(  IElasticClient elasticClient, ILogger<ProfileService> logger)
//        {


//            try
//            {
        
//                _logger = logger;
           
//                //_dynamoDbClient = new AmazonDynamoDBClient(new BasicAWSCredentials(cred.Key, cred.Secret), Amazon.RegionEndpoint.USEast1);

//                _logger.LogWarning("@@@@@@@@@@@@@@ vou instanciar o dynamo ^^^^&&&&");

//                //_logger.LogWarning($"***** AccessKey: {cred.Key}");
//                //_logger.LogWarning($"***** SecretKey: {cred.Secret}");
//                //_logger.LogWarning($"***** UseToken: {cred.GetCredentials()?.UseToken}");
//                //_logger.LogWarning($"***** AuthenticationRegion: {dynamoDbClient.Config.AuthenticationRegion}");
//                //_logger.LogWarning($"***** RegionEndpoint: {dynamoDbClient.Config.RegionEndpoint}");




//                _logger.LogCritical("$_dynamoDbClient&");

//                //_dynamodbContext = new DynamoDBContext(_dynamoDbClient);
//                //_profilesTable = Table.LoadTable(_dynamoDbClient, tableName);
//                _elasticClient = elasticClient;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError("$$$$$ ProfileService ^^^^&&&&");
//                _logger.LogCritical("ERROR: " + ex.InnerException);

//                throw;
//            }

//        }

//        public async Task<bool> Create(Model.Profile profile)
//        {
//            try
//            {
//                int index = 0;
//                profile.Id = Guid.NewGuid().ToString();
//                profile.DateOfRegister = DateTime.Now;
//                var imageParts = profile.ProfileThumbnail.Split(',').ToList<string>();

//                if (imageParts.Count >= 2) index = 1;

//                profile.ProfileThumbnail = GetConvertedImage(Base64ToImage(imageParts[index]));

//                await DynamoDBContext.SaveAsync<Model.Profile>(profile);
//                return true;
//            }
//            catch (Exception ex)
//            {

//                throw ex;
//            }

//        }

//        public async Task<List<Model.Profile>> GetAll()
//        {

//            ScanFilter scanFilter = new ScanFilter();
//            // scanFilter.AddCondition("Price", ScanOperator.LessThan, 0);

//            ScanOperationConfig config = new ScanOperationConfig()
//            {
//                //Filter = scanFilter,
//                //Select = SelectValues.SpecificAttributes,
//                //AttributesToGet = new List<string> { "Title", "Id" }
//            };

//            Search search = ProfilesTable.Scan(config);

//            List<Document> documentList = new List<Document>();
//            do
//            {
//                documentList.AddRange(await search.GetNextSetAsync());
//            } while (!search.IsDone);

//            return DynamoDBContext.FromDocuments<Model.Profile>(documentList).ToList();
//        }

//        public async Task<Model.Profile> GetProfileById(string id)
//        {

//            //GetItemOperationConfig config = new GetItemOperationConfig
//            //{

//            //    AttributesToGet = new List<string> { "Id", "LiveIn" }
//            //};

//            // Document document = await _profilesTable.GetItemAsync(id, "Rio de Janeiro");
//            // Model.Profile p =  dynamodbContext.FromDocument<Model.Profile>(document);

//            return await DynamoDBContext.LoadAsync<Model.Profile>(id);


//        }

//        public async Task<bool> Update(Model.Profile profile)
//        {
//            await SavePost(profile);
//            await DynamoDBContext.SaveAsync<Model.Profile>(profile);
//            return true;

//        }

//        public static string GetConvertedImage(byte[] stream)
//        {
//            using (var bmp = new GcBitmap())
//            {
//                bmp.Load(stream);


//                var newImg = new GcBitmap();
//                newImg.Load(stream);



//                var resizedImage = bmp.Resize(100, 100, InterpolationMode.NearestNeighbor);
//                string img64 = GenerateImage(newImg);
//                return img64;//GetBase64(resizedImage);
//            }
//        }

//        public static string GenerateImage(GcBitmap img, int pixelWidth = 1024, int pixelHeight = 1024, bool opaque = true, float dpiX = 96, float dpiY = 96)
//        {
//            //we use solid color as background here 
//            //as some image formats may not support transparent background
//            Color backColor = Color.AntiqueWhite;
//            Color foreColor = Color.Beige;
//            const int bottom = 144, pad = 36;
//            int minSize = Math.Min(pixelWidth, pixelHeight) / 6, maxSize = (int)(Math.Min(pixelWidth, pixelHeight) / 1.5);

//            // Randomize some parameters of the sample:
//            var rnd = new Random();
//            GcBitmap bmpSrc = img;
//            // It is advisable to dispose bitmaps which are no longer needed,
//            //using (var bmpSrc = new GcBitmap(Path.Combine("Resources", "woman-brick-wall.jpg")))
//            //{
//            // Make sure source and target opacity match:
//            bmpSrc.Opaque = opaque;

//            // Coordinates and size of the clipping in the source image:
//            int x = 50, y = 0, w = Convert.ToInt16(img.Width), h = Convert.ToInt16(img.Height);

//            // Create a clipping region excluding all outside of the specified circle:
//            var rgn = new Region(new RectangularFigure(0, 0, bmpSrc.PixelWidth, bmpSrc.PixelHeight));
//            var ellipse = new EllipticFigure(x, y, w, h);
//            rgn.CombineWithRegion(new Region(ellipse), RegionCombineMode.Exclude, false);

//            // To clip using a Region, we need to use the BitmapRenderer:
//            bmpSrc.EnsureRendererCreated();
//            var renderer = bmpSrc.Renderer;
//            renderer.ClipRegion = rgn;
//            renderer.Clear(Color.Transparent); var size = rnd.Next(minSize, maxSize);
//            using (var bmpRound = bmpSrc.Clip(new Rectangle(x, y, w, h)))
//            using (var bmpSmall = bmpRound.Resize(size, size))
//            {
//                var bmp = new GcBitmap(pixelWidth, pixelHeight, opaque, dpiX, dpiY);
//                bmp.Clear(Color.Transparent);
//                bmp.BitBlt(bmpSmall,
//                    rnd.Next(pad, pixelWidth - pad - bmpSmall.PixelWidth),
//                    rnd.Next(pad, pixelHeight - pad - bottom - bmpSmall.PixelHeight));
//                return GetBase64(bmp);
//            }
//            //}
//        }
//        #region helper 
//        private static string GetBase64(GcBitmap bmp)
//        {
//            using (MemoryStream m = new MemoryStream())
//            {
//                bmp.SaveAsPng(m);
//                return Convert.ToBase64String(m.ToArray());
//            }
//        }

//        public byte[] Base64ToImage(string base64String)
//        {
//            // Convert base 64 string to byte[]
//            byte[] imageBytes = Convert.FromBase64String(base64String);
//            return imageBytes;
//            //var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
//            //return ms;
//            // Convert byte[] to Image
//            //using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
//            //{
//            //    Image image = Image.FromStream(ms, true);
//            //    return image;
//            //}
//        }


//        public async Task SavePost(Model.Profile profile)
//        {

//            //    await _elasticClient.UpdateAsync<Post>(post, u => u.Doc(post));
//            IndexResponse resp = await _elasticClient.IndexDocumentAsync<Model.Profile>(profile);
//            var error = resp.ServerError;
//        }

//        public async Task<bool> Like(string myProfileID, string profileILikeID)
//        {
//            try
//            {
//                Model.Profile profileILike = await DynamoDBContext.LoadAsync<Model.Profile>(profileILikeID);
//                Model.Profile profile = await DynamoDBContext.LoadAsync<Model.Profile>(myProfileID);


//                if (profileILike.ILike.Contains(myProfileID))
//                {
//                    bool ItsAMatch = true;
//                }

//                if (!profileILike.LikesMe.Contains(myProfileID))
//                    profileILike.LikesMe.Add(myProfileID);

//                if (!profile.ILike.Contains(profileILikeID))
//                    profile.ILike.Add(profileILikeID);


//                BatchWrite<Model.Profile> batch = DynamoDBContext.CreateBatchWrite<Model.Profile>();
//                batch.AddPutItems(new List<Model.Profile> { profile, profileILike });


//                await batch.ExecuteAsync();

//                return true;
//            }
//            catch (Exception ex)
//            {

//                throw ex;
//            }



//        }
//        #endregion
//    }

//}
