using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace DETP.services
{
    public partial class ViewRenderService
    {




        public static async Task<string> RenderViewToStringAsync(
    string viewName, object model,
    ControllerContext controllerContext,
    bool isPartial = false)
        {
            var actionContext = controllerContext as ActionContext;

            var serviceProvider = controllerContext.HttpContext.RequestServices;
            var razorViewEngine = serviceProvider.GetService(typeof(IRazorViewEngine)) as IRazorViewEngine;
            var tempDataProvider = serviceProvider.GetService(typeof(ITempDataProvider)) as ITempDataProvider;

            using (var sw = new StringWriter())
            {
                var viewResult = razorViewEngine.FindView(actionContext, viewName, !isPartial);

                if (viewResult?.View == null)
                    throw new ArgumentException($"{viewName} does not match any available view");

                var viewDictionary =
                    new ViewDataDictionary(new EmptyModelMetadataProvider(),
                        new ModelStateDictionary())
                    { Model = model };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }

    }
}
