using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelWriteApi.Helpers
{
    public class ApiExplorerIgnores : IActionModelConvention
    {
        public void Apply(ActionModel action) // Option to hide a controller from Swagger
        {
            if (action.Controller.ControllerName.Equals("WeatherForecast")) 
                action.ApiExplorer.IsVisible = false;
        }
    }
}
