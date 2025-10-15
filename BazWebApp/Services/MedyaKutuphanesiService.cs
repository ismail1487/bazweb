using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity;
using System.Net.Security;
using System.Threading;

namespace BazWebApp.Services
{
    
    /// <summary>
    /// Medya kütüphanesine ait metotların yer aldığı servis sınıfıdır
    /// </summary>
    public interface IMedyaKutuphanesiService
    {
        /// <summary>
        /// Resim yükleme metodu
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Result<MedyaKutuphanesi> ResimYukle(IFormFile file); 
        /// <summary>
        /// Resim yükleme metodu
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Result<MedyaKutuphanesi> ResimYukleIcerik(IFormFile file);

        /// <summary>
        /// Çoklu dosya yükleme metodu
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        Result<List<MedyaKutuphanesi>> UploadMultiple(List<IFormFile> files);

        /// <summary>
        /// Video kaydı için oluşturulan geçici viddeo parçalarını yükleme metodu
        /// </summary>
        /// <param name="chunk"></param>
        /// <returns></returns>
        Result<bool> UploadChunk(ChunkModel chunk);
    }

    /// <summary>
    /// Medya kütüphanesi servis sınıfı
    /// </summary>
    public class MedyaKutuphanesiService : IMedyaKutuphanesiService
    {
        private readonly IRequestHelper _requestHelper;
        private readonly bool isDevelopment = false;
        private readonly IBazCookieService _bazCookieService;

        /// <summary>
        /// Medya kütüphanesi servis sınıfı konst.
        /// </summary>
        /// <param name="requestHelper"></param>
        /// <param name="bazCookieService"></param>
        public MedyaKutuphanesiService(IRequestHelper requestHelper, IBazCookieService bazCookieService)
        {
            _bazCookieService = bazCookieService;
            _requestHelper = requestHelper;
#if DEBUG
            isDevelopment = true;
#endif
        }
        /// <summary>
        /// Resim yükleme metotu
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Result<MedyaKutuphanesi> ResimYukleIcerik(IFormFile file)
        {
            HttpClientHandler clientHandler = new();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
            {
                if (isDevelopment)
                {
                    return true; // for development, trust all certificates
                }
                return sslPolicyErrors == SslPolicyErrors.None;

                //&& .Contains(cert.GetCertHashString()); // Compliant: trust only some certificate

                //return true;
            };

            HttpClient _client = new(clientHandler);
            _client.Timeout = Timeout.InfiniteTimeSpan;
            using var content = new StreamContent(file.OpenReadStream());
            using var formData = new MultipartFormDataContent { { content, "file", file.FileName } };
            var response = _client.PostAsync(LocalPortlar.MedyaKutuphanesiService + "/MedyaKutuphanesi/UploadIcerik", formData).Result;
            Result<MedyaKutuphanesi> medya = JsonConvert.DeserializeObject<Result<MedyaKutuphanesi>>(response.Content.ReadAsStringAsync().Result);
            return medya;
        }

        /// <summary>
        /// Resim yükleme metotu
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Result<MedyaKutuphanesi> ResimYukle(IFormFile file)
        {
            HttpClientHandler clientHandler = new();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
            {
                if (isDevelopment)
                {
                    return true; // for development, trust all certificates
                }
                return sslPolicyErrors == SslPolicyErrors.None;

                //&& .Contains(cert.GetCertHashString()); // Compliant: trust only some certificate

                //return true;
            };

            HttpClient _client = new(clientHandler);
            _client.Timeout = Timeout.InfiniteTimeSpan;
            using var content = new StreamContent(file.OpenReadStream());
            using var formData = new MultipartFormDataContent { { content, "file", file.FileName } };
            var response = _client.PostAsync(LocalPortlar.MedyaKutuphanesiService + "/MedyaKutuphanesi/Upload", formData).Result;
            Result<MedyaKutuphanesi> medya = JsonConvert.DeserializeObject<Result<MedyaKutuphanesi>>(response.Content.ReadAsStringAsync().Result);
            return medya;
        }
        

        /// <summary>
        /// Çoklu Dosya yükleme metotu
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public Result<List<MedyaKutuphanesi>> UploadMultiple(List<IFormFile> files)
        {
            HttpClientHandler clientHandler = new()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    //if (isDevelopment)
                    //{
                    //    return true; // for development, trust all certificates
                    //}

                    //return sslPolicyErrors == SslPolicyErrors.None;
                    return true;
                }
            };

            HttpClient _client = new(clientHandler);
            var formData = new MultipartFormDataContent();
            //Form data çevirerek medya kütüphanesine yollanması
            foreach (var file in files)
            {
                var content = new StreamContent(file.OpenReadStream());
                formData.Add(content, "files", file.FileName);
            }

            var response = _client.PostAsync(LocalPortlar.MedyaKutuphanesiService + "/MedyaKutuphanesi/UploadMultiple", formData).Result;
            Result<List<MedyaKutuphanesi>> medya = JsonConvert.DeserializeObject<Result<List<MedyaKutuphanesi>>>(response.Content.ReadAsStringAsync().Result);
            return medya;
        }

        /// <summary>
        /// Video kaydı için oluşturulan geçici viddeo parçalarını yükleme metodu
        /// </summary>
        /// <param name="chunk"></param>
        /// <returns></returns>
        public Result<bool> UploadChunk(ChunkModel chunk)
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            HttpClientHandler clientHandler = new()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    //if (isDevelopment)
                    //{
                    //    return true; // for development, trust all certificates
                    //}

                    //return sslPolicyErrors == SslPolicyErrors.None;
                    return true;
                }
            };
            var sessionId = cookie.SessionId;
            HttpClient _client = new(clientHandler);
            var formData = new MultipartFormDataContent();
            //Form data çevirerek medya kütüphanesine yollanması
            var content = new StreamContent(chunk.file.OpenReadStream());
            formData.Add(content, "file", chunk.file.FileName);

            formData.Add(new StringContent(chunk.room), "room");
            formData.Add(new StringContent(chunk.isRecording.ToString()), "isRecording");
            formData.Add(new StringContent(chunk.kurumId.ToString()), "kurumId");
            formData.Add(new StringContent(chunk.index.ToString()), "index");
            formData.Add(new StringContent(chunk.kisiId.ToString()), "kisiId");
            formData.Add(new StringContent(chunk.randGuid), "randGuid");
            formData.Add(new StringContent(chunk.odaId.ToString()), "odaId");
            _client.DefaultRequestHeaders.Add("sessionID", sessionId);
            var response = _client.PostAsync(LocalPortlar.MedyaKutuphanesiService + "/MedyaKutuphanesi/UploadChunk", formData).Result;
            Result<bool> medya = JsonConvert.DeserializeObject<Result<bool>>(response.Content.ReadAsStringAsync().Result);
            return medya;
        }
    }

    /// <summary>
    /// Videokaydı sırasında oluşturulan geçici video parçalarını temsil eden model sınıfı.
    /// </summary>
    public class ChunkModel
    {
        /// <summary>
        ///
        /// </summary>
        public string room { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool isRecording { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int kurumId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int index { get; set; }

        /// <summary>
        ///
        /// </summary>
        public IFormFile file { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int kisiId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string randGuid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int odaId { get; set; }
    }
}