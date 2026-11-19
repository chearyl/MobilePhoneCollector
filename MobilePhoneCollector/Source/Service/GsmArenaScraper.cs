using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MobilePhoneCollector.Source.Core.Constants;
using MobilePhoneCollector.Source.Core.Database;
using MobilePhoneCollector.Source.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.WebRequestMethods;

namespace MobilePhoneCollector.Source.Service
{
    public class GsmArenaScraper : IDisposable
    {
        private readonly Uri _baseUri = new("https://www.gsmarena.com/");
        private HttpClient _httpClient;
        private TimeSpan _minInterval = TimeSpan.FromSeconds(2);

        public GsmArenaScraper()
        {
            _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All }) { Timeout = TimeSpan.FromSeconds(30) };
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) PhoneVault/1.0 (personal collection app)");
            _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
        }

        public async Task<(Phone?, List<PhoneImage>)> SearchPhone(string url)
        {
            using var resp = await _httpClient.GetAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(resp.Content.ReadAsStringAsync().Result);
            var node = doc.DocumentNode.SelectSingleNode("//h1[@data-spec='modelname']");
            if (node is null) return (null, []);

            string brand = node.InnerHtml.Split(" ")[0];
            Phone phone = new()
            {
                Model = node!.InnerText.Replace(brand, "").Trim(),
                Brand = brand,
                InfoUrl = url
            };

            var infoNode = doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']")!;
            foreach (var aSpec in GsmArenaDataSpec.DataSpec)
            {
                if (aSpec.Equals("loudspeaker") || aSpec.Equals("headphonejack") || aSpec.Equals("charging") || aSpec.Equals("infrared"))
                {
                    string specialTerm = "";
                    if (aSpec.Equals("loudspeaker")) specialTerm = "loudspeaker";
                    else if (aSpec.Equals("headphonejack")) specialTerm = "audio-jack";
                    else if (aSpec.Equals("charging")) specialTerm = "battery-charging";
                    else if (aSpec.Equals("infrared")) specialTerm = "irda";

                    var specialNode = infoNode.SelectSingleNode($"//a[@href=\"glossary.php3?term={specialTerm}\"]");
                    if(specialNode is not null)
                    {
                        var temp = new Specs
                        {
                            Key = GsmArenaDataSpec.GsmToActual[aSpec],
                            Value = specialNode.ParentNode!.ParentNode!.Elements("td").ElementAt(1).InnerText.Trim(),
                            Category = GsmArenaDataSpec.SpecCategory[aSpec]
                        };
                        phone.SpecList.Add(temp);
                    }
                    continue;
                }

                var specNode = infoNode.SelectSingleNode($"//td[@data-spec='{aSpec}']");
                if(specNode is not null)
                {
                    var temp = new Specs
                    {
                        Key = GsmArenaDataSpec.GsmToActual[aSpec],
                        Value = specNode.InnerText,
                        Category = GsmArenaDataSpec.SpecCategory[aSpec]
                    };
                    phone.SpecList.Add(temp);
                }
            }

            List<PhoneImage> imageList = [];
            try
            {
                using var picResp = await _httpClient.GetAsync(url.Replace("-", "-pictures-"));
                doc.LoadHtml(picResp.Content.ReadAsStringAsync().Result);
                var picNode = doc.DocumentNode.SelectSingleNode("//div[@id='pictures-list']");
                if (picNode is not null)
                {
                    var imgs = picNode.Elements("img");
                    if (imgs is not null)
                    {
                        foreach (var img in imgs)
                        {
                            PhoneImage image = new();
                            FileData data = new();
                            string imageUrl = img.GetAttributeValue("src", "");
                            try
                            {
                                var imageResp = await _httpClient.GetAsync(imageUrl);
                                string mime = imageResp.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
                                data.Data = imageResp.Content.ReadAsByteArrayAsync().Result;
                                data.HashCode = HashUtil.ByteToMd5(data.Data);
                                image.Data = data;
                                image.FileName = Path.GetFileName(imageUrl);
                                image.FileType = mime;
                                image.CreatedDate = DateTime.Now.ToString();
                                imageList.Add(image);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex.Message);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            

            return (phone, imageList);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
