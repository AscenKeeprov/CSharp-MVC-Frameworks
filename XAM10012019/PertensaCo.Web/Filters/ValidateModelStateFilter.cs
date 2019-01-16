using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static PertensaCo.Common.Constants.GlobalConstants;

namespace PertensaCo.Web.Filters
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class ValidateModelStateFilter : ActionFilterAttribute
	{
		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var modelState = context.ModelState;
			if (modelState.IsValid) await next();
			else
			{
				Controller controller = (Controller)context.Controller;
				var viewData = controller.ViewData;
				var modelErrors = modelState.Values.SelectMany(v => v.Errors);
				viewData[ErrorKey] = modelErrors.Select(modelError => modelError.ErrorMessage);
				viewData.Model = context.ActionArguments.FirstOrDefault(a
					=> a.Value.GetType().Name.ToLower().EndsWith("model")).Value;
				context.Result = controller.View(viewData.Model);
			}
		}
	}
}
