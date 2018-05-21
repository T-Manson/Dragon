using System;
using System.ComponentModel;
using Dragon.Framework.ApiCore.Common;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Dragon.Framework.ApiCore.Conventions
{
    /// <summary>
    /// Api Action参数约定
    /// </summary>
    public class ApiActionParameterBindingConvention : IActionModelConvention
    {
        /// <summary>
        /// 应用约定
        /// </summary>
        /// <param name="action"></param>
        public void Apply(ActionModel action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            //Api Action采用自定义模型绑定约定
            if (typeof(ApiController).IsAssignableFrom(action.Controller.ControllerType))
            {
                foreach (var parameter in action.Parameters)
                {
                    parameter.BindingInfo = parameter.BindingInfo ?? new BindingInfo();
                    parameter.BindingInfo.BindingSource = GetBindingSource(parameter.ParameterInfo.ParameterType);
                }
            }
        }

        private BindingSource GetBindingSource(Type paramenterType)
        {
            //基元类型都通过Path传递
            if (TypeDescriptor.GetConverter(paramenterType).CanConvertFrom(typeof(string))) return BindingSource.Path;

            //复杂类型通过Body传递
            return BindingSource.Body;
        }
    }
}
