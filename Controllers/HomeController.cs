using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FaceAppMvcBrad2.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace FaceAppMvcBrad2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public static IFaceClient Authenticate(string endpoint, string key)
        {
            string SUBSCRIPTION_KEY = key;//Environment.GetEnvironmentVariable(key);
            string ENDPOINT = endpoint;//Environment.GetEnvironmentVariable(endpoint);
            return new FaceClient(new ApiKeyServiceClientCredentials(SUBSCRIPTION_KEY)) { Endpoint = ENDPOINT };
        }
        
        public static async Task<string> DetectFaceExtract(IFaceClient client, string url, string recognitionModel, ILogger log)
        {
            log.LogInformation("========DETECT FACES========");

            // Create a list of images
            // List<string> imageFileNames = new List<string>
            //                 {
            //                     "detection1.jpg",    // single female with glasses
            //                     // "detection2.jpg", // (optional: single man)
            //                     // "detection3.jpg", // (optional: single male construction worker)
            //                     // "detection4.jpg", // (optional: 3 people at cafe, 1 is blurred)
            //                     "detection5.jpg",    // family, woman child man
            //                     "detection6.jpg"     // elderly couple, male female
            //                 };

            // foreach (var imageFileName in imageFileNames)
            // {
                IList<DetectedFace> detectedFaces;

                // Detect faces with all attributes from image url.
                //detectedFaces = await client.Face.DetectWithStreamAsync(url,
                detectedFaces = await client.Face.DetectWithUrlAsync(url,
                        returnFaceAttributes: new List<FaceAttributeType> { FaceAttributeType.Accessories, FaceAttributeType.Age,
                        FaceAttributeType.Blur, FaceAttributeType.Emotion, FaceAttributeType.Exposure, FaceAttributeType.FacialHair,
                        FaceAttributeType.Gender, FaceAttributeType.Glasses, FaceAttributeType.Hair, FaceAttributeType.HeadPose,
                        FaceAttributeType.Makeup, FaceAttributeType.Noise, FaceAttributeType.Occlusion, FaceAttributeType.Smile },
                        recognitionModel: recognitionModel);

                var message = $"{detectedFaces.Count} face(s) detected from image `{url}`.";
                log.LogInformation(message);
                return message;
            // }
        }

        public async Task<IActionResult> Index()
        {
            var client = Authenticate("https://faceappfacebrad.cognitiveservices.azure.com/", "081bc555cb8a490980c230a0f4167878");
            var url = "https://csdx.blob.core.windows.net/resources/Face/Images/detection6.jpg";
            var message = await DetectFaceExtract(client, url, RecognitionModel.Recognition01, _logger);
            ViewBag.Message = message;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
