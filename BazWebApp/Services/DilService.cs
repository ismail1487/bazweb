using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baz.Model.Entity.Constants;

namespace BazWebApp.Services
{
    /// <summary>
    /// Dil işlemleri yöneten methodaların bulunduğu interface
    /// </summary>
    public interface IDilService 
    {
        /// <summary>
        ///  Dilleri listeleyen methodtur.
        /// </summary>
        /// <returns></returns>
        Result<List<ParamDiller>> List();
        Result<List<ParamYabanciDiller>> YabanciDilList();
    }
    /// <summary>
    /// Dil servisinin methodlarının yer aldığı sınıftır.
    /// </summary>
    /// <seealso cref="BazWebApp.Services.IDilService" />
    public class DilService : IDilService
    {
        private readonly IRequestHelper _requestHelper;
        /// <summary>
        /// Dil servisinin methodlarının yer aldığı sınıfın yapıcı methodu
        /// </summary>
        /// <param name="requestHelper"></param>
        public DilService(IRequestHelper requestHelper) 
        {
            _requestHelper = requestHelper;
        }
        /// <summary>
        /// Dilleri listeleyen methodtur.
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamDiller>> List()
        {
            var result = _requestHelper.Get<Result<List<ParamDiller>>>(LocalPortlar.IYSService+"/api/dil/list");
            return result.Result;
        }
        public Result<List<ParamYabanciDiller>> YabanciDilList()
        {
            var result = _requestHelper.Get<Result<List<ParamYabanciDiller>>>(LocalPortlar.IYSService+ "/api/dil/YabanciDilList");
            return result.Result;
        }
    }
}
