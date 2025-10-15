using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager;
using Baz.RequestManager.Abstracts;
using BazWebApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BazWebApp.Services
{
    /// <summary>
    /// Arama servis sınıfı interface
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Arama metodu
        /// </summary>
        /// <param name="query"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        Result<List<Baz.Model.Entity.ViewModel.ElasticSearchIndexItem>> Search(string query, string indexName = "globalindex");

        /// <summary>
        /// mesajlar içeriğinde arama yapma metodu
        /// </summary>
        /// <param name="query">aranacak keliöme</param>
        /// <returns>İlgili sonuçları döndürür.</returns>
        public Result<List<MesajAramaSonucModel>> MesajlarIcindeArama(string query);
    }

    /// <summary>
    /// Arama servis sınıfı
    /// </summary>
    public class SearchService : ISearchService
    {
        private readonly IRequestHelper _requestHelper;

        /// <summary>
        /// Arama servis sınıfı konst.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public SearchService(IServiceProvider serviceProvider)
        {
            _requestHelper = new RequestHelper(LocalPortlar.SearchService + "/api/", new RequestManagerHeaderHelperForCookie(serviceProvider).SetDefaultHeader());
        }

        /// <summary>
        /// Arama metodu
        /// </summary>
        /// <param name="query"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public Result<List<ElasticSearchIndexItem>> Search(string query, string indexName = "globalindex")
        {
            string url = "ElasticSync/Search?query=" + query + "&indexName=" + indexName;
            var result = _requestHelper.Get<Result<List<Baz.Model.Entity.ViewModel.ElasticSearchIndexItem>>>(url);
            if (result.IsSuccess && result.Result.IsSuccess)
                return result.Result;
            return new List<Baz.Model.Entity.ViewModel.ElasticSearchIndexItem>().ToResult();
        }

        /// <summary>
        /// mesajlar içeriğinde arama yapma metodu
        /// </summary>
        /// <param name="query">aranacak keliöme</param>
        /// <returns>İlgili sonuçları döndürür.</returns>
        public Result<List<MesajAramaSonucModel>> MesajlarIcindeArama(string query)
        {
            var result = _requestHelper.Get<Result<List<MesajAramaSonucModel>>>(LocalPortlar.IYSService + "/api/IcerikGenel/MesajlarIcindeArama/" + query);
            return result.Result;
        }
    }
}