using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using BazWebApp.Services;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Controllers
{
    /// <summary>
    /// Hedef kitle kontrol metodlarının yer aldığı sınıftır.
    /// </summary>
    public class TargetGroupController : Controller
    {
        private readonly IKurumService _kurumService;
        private readonly IKisiService _kisiService;
        private readonly IParamService _paramService;
        private readonly IBazCookieService _bazCookieService;
        private readonly ITargetGroupService _targetGroupService;

        /// <summary>
        /// Hedef kitle kontrol metodlarının yer aldığı konst.
        /// </summary>
        /// <param name="kurumService"></param>
        /// <param name="kisiService"></param>
        /// <param name="paramService"></param>
        /// <param name="bazCookieService"></param>
        /// <param name="targetGroupService"></param>
        public TargetGroupController(IKurumService kurumService, IKisiService kisiService, IParamService paramService, IBazCookieService bazCookieService, ITargetGroupService targetGroupService)
        {
            _kurumService = kurumService;
            _kisiService = kisiService;
            _paramService = paramService;
            _bazCookieService = bazCookieService;
            _targetGroupService = targetGroupService;
        }

        #region View Methods

        /// <summary>
        /// Hedef kitle ekleme sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>
        [Route("/targetgroup/add")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/targetgroup/add")]
        public ActionResult Add()
        {
            return View("Add");
        }

        /// <summary>
        /// Hedef kitle güncelleme sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>
        [Route("/targetgroup/update/{id}")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/targetgroup/update/{id}")]
        public ActionResult Update() { return View("Update"); }

        /// <summary>
        /// Hedef kitle listeleme sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>
        [Route("/targetgroup/list")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/targetgroup/list")]
        public ActionResult List() { return View("List"); }

        /// <summary>
        /// Hedef kitle test sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>

        [Route("/targetgroup/Test")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/targetgroup/Test")]
        public ActionResult Test() { return View("Test"); }

        #endregion View Methods

        #region Api Methods


        /// <summary>
        /// hedef kitle listeleme metodu
        /// </summary>
        /// <param name="targetGroupType"></param>
        /// <returns></returns>
        [Route("/targetgroup/gettargetgroupfields/{targetGroupType}")]
        [HttpGet]
        public Result<List<HedefKitleField>> GetTargetGroupFields(string targetGroupType)
        {
            if (targetGroupType == "Kurum" || targetGroupType == "kurum")
            {
                return _kurumService.GetTargetGroupFields();
            }
            else
                return _kisiService.GetTargetGroupFields();
        }

        /// <summary>
        /// Parametre tanımına göre verileri getiren metod
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("/targetgroup/GetParamValue/{name}")]
        public Result<List<Baz.Model.Entity.ViewModel.KeyValueModel>> GetParamValue(string name)
        {

            if (name == null || name == "null")
                return new List<KeyValueModel>().ToResult();
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var liste = _paramService.List(new ParametreRequest()
            {
                ModelName = name,
                KurumId = Convert.ToInt32(cookie.KurumId),
                Tanim = "Test"
            });

            return liste.Value.Select(p => new KeyValueModel() { Key = p.Tanim, Value = p.TabloID.ToString() }).ToList().ToResult();
        }

        /// <summary>
        /// Üst Id'ye göre parametre verilerini getiren metod
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ustID"></param>
        /// <returns></returns>
        [Route("/targetgroup/GetParamValueWithUstID/{name}/{ustID}")]
        public Result<List<Baz.Model.Entity.ViewModel.KeyValueModel>> GetParamValueWithUstID(string name, int ustID)
        {

            if (name == null || name == "null")
                return new List<KeyValueModel>().ToResult();
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var liste = _paramService.List(new ParametreRequest()
            {
                ModelName = name,
                KurumId = Convert.ToInt32(cookie.KurumId),
                UstId = ustID
            });

            return liste.Value.Select(p => new KeyValueModel() { Key = p.Tanim, Value = p.TabloID.ToString() }).ToList().ToResult();
        }

        /// <summary>
        /// Id'ye göre kurum hedef kitle verilerini getiren metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("/targetgroup/GetDynamicValueForKurum/{id}")]
        public Result<List<Baz.Model.Entity.ViewModel.KeyValueModel>> GetDynamicValueForKurum(int id)
        {
            //var result = _kurumService.OlasiDegerlerList(id);
            //return result.Value.Select(p => new KeyValueModel() { Key = p.OlasiDegerAdi, Value = p.TabloID.ToString() }).ToList().ToResult();
            // Kod temizliğinden kalan parçacıklar, incelecek gerekliliğine göre ele alınacak.
            return null;
        }

        /// <summary>
        /// Id'ye göre kişi hedef kitle verilerini getiren metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("/targetgroup/GetDynamicValueForKisi/{id}")]
        public Result<List<Baz.Model.Entity.ViewModel.KeyValueModel>> GetDynamicValueForKisi(int id)
        {
            //var result = _kisiService.OlasiDegerlerList(id);
            //return result.Value.Select(p => new KeyValueModel() { Key = p.OlasiDegerAdi, Value = p.TabloID.ToString() }).ToList().ToResult();
            // Kod temizliğinden kalan parçacıklar, incelecek gerekliliğine göre ele alınacak.
            return null;
        }

        /// <summary>
        /// Hedef kitle ekleme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("/targetgroup/create")]
        [HttpPost]
        public Result<bool> Create([FromBody] Baz.Model.Entity.ViewModel.HedefKitle model)
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            model.KurumId = Convert.ToInt32(cookie.KurumId);
            model.KisiId = Convert.ToInt32(cookie.KisiId);
            return _targetGroupService.AddOrUpdate(model);
        }

        /// <summary>
        /// Id'ye göre hedef kitle verilerini getiren metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/targetgroup/SingleOrDefaultForView/{id}")]
        public Result<HedefKitle> SingleOrDefaultForView(int id)
        {
            return _targetGroupService.SingleOrDefaultForView(id);
        }

        /// <summary>
        /// Hedef kitle çalıştırma metodu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tip"></param>
        /// <returns></returns>
        [Route("/targetgroup/RunExpression/{id}/{tip}")]
        [HttpGet]
        public Result<List<KeyValueModel>> RunExpression(int id, string tip)
        {
            return _targetGroupService.RunExpression(id, tip);
        }

        /// <summary>
        /// Hedef kitle çalıştırma metodu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tip"></param>
        /// <returns></returns>
        [Route("/targetgroup/RunExpressionReturnUser/{id}/{tip}")]
        [HttpGet]
        public Result<List<KeyValueModel>> RunExpressionReturnUser(int id, string tip)
        {
            return _targetGroupService.RunExpressionReturnUser(id, tip);
        }

        /// <summary>
        /// Hedef kitle tanımlama metodu
        /// </summary>
        /// <returns></returns>
        [Route("/targetgroup/gettargetgroups")]
        [HttpGet]
        public Result<List<HedefKitleTanimlamalar>> GetTargetGroups()
        {
            return _targetGroupService.GetListByKurum();
        }

        /// <summary>
        /// Kişi hedef kitle tanımlama metodu
        /// </summary>
        /// <returns></returns>
        [Route("/targetgroup/GetPersonTargetGroups")]
        [HttpGet]
        public Result<List<HedefKitleTanimlamalar>> GetPersonTargetGroups()
        {
            return _targetGroupService.GetKisiListByKurum();
        }

        /// <summary>
        /// Kurum hedef kitle tanımlama metodu
        /// </summary>
        /// <returns></returns>
        [Route("/targetgroup/GetCompanyTargetGroups")]
        [HttpGet]
        public Result<List<HedefKitleTanimlamalar>> GetCompanyTargetGroups()
        {
            return _targetGroupService.GetKurumListByKurum();
        }

        /// <summary>
        /// Hedef kitle silme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("/targetgroup/hedefkitlesil/{id}")]
        [HttpGet]
        public Result<bool> HedefKitleSil(int id)
        {
            return _targetGroupService.HedefKitleSil(id);
        }

        /// <summary>
        /// Kurum Id'ye göre kurum hedef kitle getiren metod
        /// </summary>
        /// <returns></returns>
        [Route("/targetgroup/GetTargetGroupsByKurumId")]
        [HttpGet]
        public Result<List<HedefKitleTanimlamalar>> GetTargetGroupsByKurumId()
        {
            var cook = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var kurumId = Convert.ToInt32(cook.KurumId);
            var list = _targetGroupService.GetListByKurum();
            var res = new List<HedefKitleTanimlamalar>();
            foreach (var item in list.Value)
            {
                if (item.KurumID == kurumId)
                {
                    res.Add(item);
                }
            }
            return res.ToResult();
        }

        /// <summary>
        /// Grup Id'ye göre hedef kitleleri listeleyen metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("/targetgroup/TargetGroupMembersListByGroupId/{id}")]
        [HttpGet]
        public Result<List<BasicKisiModel>> TargetGroupMembersListByGroupId(int id)
        {
            var result = _targetGroupService.TargetGroupMembersListByGroupId(id);
            return result;
        }

        #endregion Api Methods
    }
}