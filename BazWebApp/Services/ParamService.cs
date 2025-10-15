using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Services
{
    /// <summary>
    /// Parametrelere ait crud işlem metotlarının yer aldığı servis interface
    /// </summary>
    public interface IParamService
    {
        /// <summary>
        /// Parametre listeleme metodu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Result<List<ParametreResult>> List(ParametreRequest request);

        /// <summary>
        /// Parametre listeleme metodu 2
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Result<List<ParametreResult>> ListParam(ParametreRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Result<List<ParametreDilTanimlar>> ListParamDiller(ParametreRequest request);

        /// <summary>
        /// Parametre dil değerlerini kaydeder
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Result<bool> SaveParamDiller(ParametreDiller request);
        
        /// <summary>
        /// Parametre ekleme metodu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Result<int> Add(ParametreRequest item);

        /// <summary>
        /// Parametre güncelleme metodu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Result<int> Update(ParametreRequest item);

        /// <summary>
        /// Parametre silme metodu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Result<int> Delete(ParametreRequest item);
    }

    /// <summary>
    /// Parametre servis sınıfı
    /// </summary>
    public class ParamService : IParamService
    {
        private readonly IRequestHelper _helper;
        private readonly int cultureId;

        /// <summary>
        /// Parametre servis sınıfı konst.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="localizationService"></param>
        public ParamService(IRequestHelper helper, ILocalizationService localizationService)
        {
            _helper = helper;
            cultureId = localizationService.GetCultureId();
        }

        /// <summary>
        /// Parametre listeleme metotu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<ParametreResult>> List(ParametreRequest request)
        {
            request.DilID = cultureId;
            request.Tanim = "test";
            if (request.ModelName == "ParamDiller")
            {
                request.DilID = 0;
            }
            var url = LocalPortlar.IYSService + "/api/KureselParametreler/List";
            var result = _helper.Post<Result<List<ParametreResult>>>(url, request);

            return result.Result;
        } 


        /// <summary>
        /// Parametre listeleme metotu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<ParametreResult>> ListParam(ParametreRequest request)
        {
            var url = LocalPortlar.IYSService + "/api/KureselParametreler/ListParam";
            var result = _helper.Post<Result<List<ParametreResult>>>(url, request);

            return result.Result;
        }
        /// <summary>
        /// Parametre listeleme metotu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<ParametreDilTanimlar>> ListParamDiller(ParametreRequest request)
        {
            var url = LocalPortlar.IYSService + "/api/KureselParametreler/GetParamDiller";
            var result = _helper.Post<Result<List<ParametreDilTanimlar>>>(url, request);

            return result.Result;
        }
        /// <summary>
        /// Parametre listeleme metotu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<bool> SaveParamDiller(ParametreDiller request)
        {
            var url = LocalPortlar.IYSService + "/api/KureselParametreler/SaveParamDiller";
            var result = _helper.Post<Result<bool>>(url, request);

            return result.Result;
        }

        

        /// <summary>
        /// Parametre ekleme metotu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result<int> Add(ParametreRequest item)
        {
            var url = LocalPortlar.IYSService + "/api/KureselParametreler/Add";
            var result = _helper.Post<Result<int>>(url, item);

            return result.Result;
        }

        /// <summary>
        /// Parametre güncelleme metotu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result<int> Update(ParametreRequest item)
        {
            var url = LocalPortlar.IYSService + "/api/KureselParametreler/Update";
            var result = _helper.Post<Result<int>>(url, item);

            return result.Result;
        }

        /// <summary>
        /// Parametre silindi yapma metotu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result<int> Delete(ParametreRequest item)
        {
            var url = LocalPortlar.IYSService + "/api/KureselParametreler/Delete";
            var result = _helper.Post<Result<int>>(url, item);

            return result.Result;
        }
    }
}