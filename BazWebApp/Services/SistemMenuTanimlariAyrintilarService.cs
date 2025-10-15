using Baz.Model.Entity;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using System.Collections.Generic;

namespace BazWebApp.Services
{
    public interface ISistemMenuTanimlariAyrintilar
    {
        Result<List<SistemMenuTanimlariWM>> SistemSayfaBasliklariGetir();
    }

    public class SistemMenuTanimlariAyrintilar : ISistemMenuTanimlariAyrintilar
    {
        private readonly IRequestHelper _requestHelper;
        private readonly IKurumService _kurumService;
        //private readonly string url = LocalPortlar.CleopatraService;
        public SistemMenuTanimlariAyrintilar(IRequestHelper requestHelper, IKurumService kurumService)
        {

            _requestHelper = requestHelper;
            _kurumService = kurumService;
        }

        public Result<List<SistemMenuTanimlariWM>> SistemSayfaBasliklariGetir()
        {
            var result = _requestHelper.Get<Result<List<SistemMenuTanimlariWM>>>(LocalPortlar.KurumService + "/api/KurumService/SistemSayfaBasliklariGetir");
            return result.Result;
        }
    }
}
