using Baz.ProcessResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using System.Reflection;
using System.Collections;

namespace BazWebApp.Handlers
{
    /// <summary>
    /// Model validation handler class'ı.
    /// </summary>
    public class ModelValidationFilter : IActionFilter
    {
        private XSS xss;
        private PropertyInfo propInfo;

        /// <summary>
        /// Model validation handler sınıfı yapıcı metodu.
        /// </summary>
        public ModelValidationFilter()
        {
            xss = new XSS();
        }

        /// <summary>
        /// IActionFilter nesnesinden kalıtılan metot, Action başlamadan önce, model binding sonrasında çalışır ve ModelState.IsValid kontrollerini gerçekleştirir.
        /// </summary>
        /// <param name="context">ActionExecutingContext nesnesi.</param>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var ps = context.ActionDescriptor.Parameters;
            if (context.ActionArguments.Count > 0)
            {
                foreach (var p in ps)
                {
                    if (context.ActionArguments[p.Name] != null)
                    {
                        //When the parameter is equal to the string
                        if (p.ParameterType.Equals(typeof(string)))
                        {
                            context.ActionArguments[p.Name] = xss.Filter(context.ActionArguments[p.Name].ToString());
                        }
                        else if (p.ParameterType.IsClass) // when the parameter is equal to the class
                        {
                            ModelFieldFilter(p.Name, p.ParameterType, context.ActionArguments[p.Name]);
                        }
                    }
                }
            }

            if (!context.ModelState.IsValid)
            {
                var err = new Error();

                foreach (var entry in context.ModelState)
                {
                    if (entry.Value.ValidationState != Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid)
                    {
                        err.Metadata.Add(entry.Key, entry.Value.Errors.FirstOrDefault().ErrorMessage);
                    }
                }
                var x = Results.Fail(err);
                var requestObject = new BadRequestObjectResult(x)
                {
                    StatusCode = Convert.ToInt32(ResultStatusCode.ModelIsNotValid)
                };
                context.Result = requestObject;
                //context.Result = new RedirectToRouteResult(RouteToDictionaryValue());     //properties ve private methods regionları açılırsa bu satır da açılabilir.
            }
        }

        /// <summary>
        /// IActionFilter nesnesinden kalıtılan metot, Action işlemi bittikten sonra çalışır. Şu an kullanılmadığı için içi boş ancak kalıtım nedeniyle eklenmesi gerektiği için buradadır.
        /// </summary>
        /// <param name="context">ActionExecutingContext nesnesi.</param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //something can be done after the action executes
        }

        /// <summary>
        ///Traversing string properties of modified classes Not: Vpn engellemesi nedeniyle script testi yapılamamaktadır.
        /// </summary>
        ///<param name = "key" > class name </param>
        ///<param name = "t" > data type </param>
        ///<param name = "obj" > object </param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private object ModelFieldFilter(string key, Type t, object obj)
        {
            //Get the property collection of a class
            var ats = t.GetCustomAttributes(typeof(ModelValidationFilter), false);

            //object anArray = propInfo.GetValue(obj, null);
            //ArrayList myList = anArray as ArrayList;

            if (obj != null)
            {
                //Get the property collection of a class
                //var pps = obj.GetType().GetProperties()
                //   .Where(p => p.GetIndexParameters().Length == 0);

                var pps = t.GetProperties();

                foreach (var pp in pps)
                {
                    if (pp.GetIndexParameters().Length > 0)
                    {
                        IList collection = (IList)obj;
                        if (collection != null)
                        {
                            for (int i = 0; i < collection.Count; i++)
                            {
                                var o = collection[i];
                                if (o.GetType() == typeof(string))
                                {
                                    collection[i] = xss.Filter(o.ToString());
                                }
                                else
                                {
                                    var ps = o.GetType().GetProperties();
                                    foreach (var p in ps)
                                    {
                                        if (p.GetValue(o) != null)
                                        {
                                            //When property equals string
                                            if (p.PropertyType.Equals(typeof(string)))
                                            {
                                                string value = p.GetValue(o).ToString();
                                                p.SetValue(o, xss.Filter(value));
                                            }
                                            else if (p.PropertyType.IsClass) // recursion when the property is equal to the class
                                            {
                                                p.SetValue(o, ModelFieldFilter(p.Name, p.PropertyType, p.GetValue(o)));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (pp.GetValue(obj) != null)
                        {
                            //When property equals string
                            if (pp.PropertyType.Equals(typeof(string)))
                            {
                                string value = pp.GetValue(obj).ToString();
                                pp.SetValue(obj, xss.Filter(value));
                            }
                            else if (pp.PropertyType.IsClass) // recursion when the property is equal to the class
                            {
                                pp.SetValue(obj, ModelFieldFilter(pp.Name, pp.PropertyType, pp.GetValue(obj)));
                            }
                        }
                    }
                }
            }

            return obj;
        }
    }
}