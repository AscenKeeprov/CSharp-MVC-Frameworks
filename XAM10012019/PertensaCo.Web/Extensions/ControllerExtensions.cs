using Microsoft.AspNetCore.Mvc;

namespace PertensaCo.Web.Extensions
{
	public static class ControllerExtensions
	{
		public static string ControllerName(this Controller controller)
		{
			var controllerName = controller.ControllerContext
				.ActionDescriptor.ControllerName;
			if (string.IsNullOrWhiteSpace(controllerName))
			{
				var controllerType = controller.GetType();
				controllerName = controllerType.Name
					.Replace(typeof(Controller).Name, string.Empty);
			}
			return controllerName;
		}
	}
}
