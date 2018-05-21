using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Dragon.Framework.ApiCore.Conventions
{
    /// <summary>
    /// 全局Route前缀
    /// </summary>
    public class GlobalRoutePrefixConvention : IApplicationModelConvention
    {
        /// <summary>
        /// 前缀模板
        /// </summary>
        private readonly AttributeRouteModel _prefix;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="routeTemplateProvider">前缀模板对象</param>
        public GlobalRoutePrefixConvention(IRouteTemplateProvider routeTemplateProvider)
        {
            _prefix = new AttributeRouteModel(routeTemplateProvider);
        }

        /// <summary>
        /// 应用全局前缀
        /// </summary>
        /// <param name="application"></param>
        public void Apply(ApplicationModel application)
        {
            //遍历所有的Controller
            foreach (var controller in application.Controllers)
            {
                // 已经标记了RouteAttribute的Controller
                var matchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
                if (matchedSelectors.Any())
                {
                    foreach (var selectorModel in matchedSelectors)
                    {
                        // 在当前路由上再添加一个路由前缀
                        selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                            _prefix,
                            selectorModel.AttributeRouteModel);
                    }
                }

                // 没有标记 RouteAttribute 的 Controller
                var unmatchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel == null).ToList();
                if (unmatchedSelectors.Any())
                {
                    foreach (var selectorModel in unmatchedSelectors)
                    {
                        // 添加一个路由前缀
                        selectorModel.AttributeRouteModel = _prefix;
                    }
                }
            }
        }
    }
}