using Baz.Model.Entity;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager;
using Baz.RequestManager.Abstracts;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BazWebApp.Helpers;

namespace BazWebApp.Services
{
    /// <summary>
    /// Hedef kitle servis interface
    /// </summary>
    public interface ITargetGroupService
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Result<List<HedefKitleTanimlamalar>> GetListByKurum();

        /// <summary>
        /// Id'ye göre hedef kitleyi getiren metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Result<HedefKitle> SingleOrDefaultForView(int id);

        /// <summary>
        /// Hedef kitle ekleme güncelleme metodu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Result<bool> AddOrUpdate(HedefKitle item);

        /// <summary>
        /// Id ve tipe göre hedef kitle getirme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tip"></param>
        /// <returns></returns>
        public Result<List<KeyValueModel>> RunExpression(int id, string tip);

        /// <summary>
        /// Id ve tipe göre Kullanıcı hedef kitle getirme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tip"></param>
        /// <returns></returns>
        Result<List<KeyValueModel>> RunExpressionReturnUser(int id, string tip);

        /// <summary>
        /// Hedef kitle silme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Result<bool> HedefKitleSil(int id);

        /// <summary>
        /// Hedef kitle group id göre listeleme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Result<List<BasicKisiModel>> TargetGroupMembersListByGroupId(int id);

        /// <summary>
        /// Kurum hedef kitle listeleme metodu
        /// </summary>
        /// <returns></returns>
        Result<List<HedefKitleTanimlamalar>> GetKurumListByKurum();

        /// <summary>
        /// Kisi hedef kitle listeleme metodu
        /// </summary>
        /// <returns></returns>
        Result<List<HedefKitleTanimlamalar>> GetKisiListByKurum();
    }

    /// <summary>
    /// Hedef kitle servis sınıfı
    /// </summary>
    public class TargetGroupService : ITargetGroupService
    {
        private readonly IBazCookieService _bazCookieService;
        private readonly IRequestHelper _requestHedefKitleKurum;
        private readonly IRequestHelper _requestHedefKitleKisi;

        /// <summary>
        /// Hedef kitle konst.
        /// </summary>
        /// <param name="bazCookieService"></param>
        /// <param name="postaciIslemlerService"></param>
        /// <param name="provider"></param>
        public TargetGroupService(IBazCookieService bazCookieService, IPostaciIslemlerService postaciIslemlerService, IServiceProvider provider)
        {
            _bazCookieService = bazCookieService;
            _requestHedefKitleKurum = new RequestHelper(LocalPortlar.KurumService + "/api/HedefKitle/", new RequestManagerHeaderHelperForCookie(provider).SetDefaultHeader());
            _requestHedefKitleKisi = new RequestHelper(LocalPortlar.KisiServis + "/api/HedefKitle/", new RequestManagerHeaderHelperForCookie(provider).SetDefaultHeader());
        }

        /// <summary>
        /// Hedef kitle tanım listeleme metodu
        /// </summary>
        /// <returns></returns>
        public Result<List<HedefKitleTanimlamalar>> GetList()
        {
            try
            {
                var x = _requestHedefKitleKurum.Get<Result<List<HedefKitleTanimlamalar>>>("GetList");
                if (x.StatusCode == HttpStatusCode.OK)
                {
                    return x.Result;
                }
                else
                    return Results.Fail(x.Error.Message);
            }
            catch (Exception ex)
            {
                return Results.Fail(ex.InnerException.InnerException.Message);
            }
        }

        /// <summary>
        /// Hedef itle tanım kuruma göre listeleme metodu
        /// </summary>
        /// <returns></returns>
        public Result<List<HedefKitleTanimlamalar>> GetListByKurum()
        {
            var cookie = _bazCookieService.GetCookie().Result;
            var x = _requestHedefKitleKurum.Get<Result<List<HedefKitleTanimlamalar>>>("GetListByKurum/" + cookie.KurumId);

            return x.Result;
        }

        /// <summary>
        /// Hedef kitle tanımlamalarını kuruma göre listeleme metodu
        /// </summary>
        /// <returns></returns>
        public Result<List<HedefKitleTanimlamalar>> GetKurumListByKurum()
        {
            var cookie = _bazCookieService.GetCookie().Result;
            var x = _requestHedefKitleKurum.Get<Result<List<HedefKitleTanimlamalar>>>("GetKurumListByKurum/" + cookie.KurumId);

            return x.Result;
        }

        /// <summary>
        /// Hedef kitle tanımlamalarını kişiye göre listeleme metodu
        /// </summary>
        /// <returns></returns>
        public Result<List<HedefKitleTanimlamalar>> GetKisiListByKurum()
        {
            var cookie = _bazCookieService.GetCookie().Result;
            var x = _requestHedefKitleKurum.Get<Result<List<HedefKitleTanimlamalar>>>("GetKisiListByKurum/" + cookie.KurumId);

            return x.Result;
        }

        /// <summary>
        /// Id'ye göre hedef kitle getirme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<HedefKitle> SingleOrDefaultForView(int id)
        {
            var url = LocalPortlar.KurumService + "/api/HedefKitle/SingleOrDefaultForView/" + id;
            var x = _requestHedefKitleKurum.Get<Result<HedefKitle>>(url);
            return x.Result;
        }

        /// <summary>
        /// Hedef kitle ekleme ve güncelleme metodu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result<bool> AddOrUpdate(HedefKitle item)
        {
            var url = LocalPortlar.KurumService + "/api/HedefKitle/addorupdate";
            var x = _requestHedefKitleKurum.Post<Result<bool>>(url, item);

            return x.Result;
        }

        /// <summary>
        /// Id ve tipe göre hedef kitle getirme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tip"></param>
        /// <returns></returns>
        public Result<List<KeyValueModel>> RunExpression(int id, string tip)
        {
            if (tip == "kurum" || tip == "Kurum")
            {
                var x = _requestHedefKitleKurum.Get<Result<List<KeyValueModel>>>("RunExpression/" + id);
                return x.Result;
            }
            else
            {
                var x = _requestHedefKitleKisi.Get<Result<List<KeyValueModel>>>("RunExpression/" + id);
                return x.Result;
            }
        }

        /// <summary>
        /// Id ve tipe göre kişi hedef kitleleri getirme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tip"></param>
        /// <returns></returns>
        public Result<List<KeyValueModel>> RunExpressionReturnUser(int id, string tip)
        {
            if (tip == "kurum" || tip == "Kurum")
            {
                var x = _requestHedefKitleKurum.Get<Result<List<KeyValueModel>>>(LocalPortlar.KurumService + "/api/HedefKitle/RunExpressionReturnUser/" + id);
                return x.Result;
            }
            else
            {
                var x = _requestHedefKitleKisi.Get<Result<List<KeyValueModel>>>(LocalPortlar.KisiServis + "/api/HedefKitle/RunExpressionReturnUser/" + id);

                return x.Result;
            }
        }

        /// <summary>
        /// Hedef kitle silme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<bool> HedefKitleSil(int id)
        {
            var x = _requestHedefKitleKurum.Get<Result<bool>>("hedefkitlesil/" + id);

            return x.Result;
        }

        /// <summary>
        /// Grup Id'ye göre hedef kitle listeleme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<BasicKisiModel>> TargetGroupMembersListByGroupId(int id)
        {
            var x = _requestHedefKitleKisi.Get<Result<List<BasicKisiModel>>>("TargetGroupMembersListByGroupId/" + id);
            return x.Result;
        }
    }
}