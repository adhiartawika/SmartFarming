
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace backend.Commons
{
    public interface IMailTemplateHelperService
    {
        Task<string> GetTemplateHtmlAsStringAsync<T>(
                              string viewName, T model);
    }
    public class MailTemplateHelperService : IMailTemplateHelperService
    {
        private IRazorViewEngine _razorViewEngine;
        private IServiceProvider _serviceProvider;
        private ITempDataProvider _tempDataProvider;
        private IConfiguration _configuration;
        public MailTemplateHelperService(
            IRazorViewEngine engine,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ITempDataProvider tempDataProvider)
        {
            this._configuration = configuration;
            this._razorViewEngine = engine;
            this._serviceProvider = serviceProvider;
            this._tempDataProvider = tempDataProvider;
        }

        public async Task<string> GetTemplateHtmlAsStringAsync<T>(
                string template, T model)
        {
            string FullPath = Path.Combine(this._configuration["Path:EmailTemplates"], template);
            StreamReader str = new StreamReader(FullPath);

            string mailText = str.ReadToEnd();

            str.Close();
            foreach(var prop in typeof(T).GetProperties())
            {
                var code = "@@"+prop.Name;
                string propValue = this.GetPropertyValue(model, prop.Name).ToString();
                mailText = mailText.Replace(code, propValue);
            }

            return mailText;

        }
        private object GetPropertyValue(object source, string propertyName)
        {
            PropertyInfo property = source.GetType().GetProperty(propertyName);
            return property.GetValue(source, null);
        }
    }
}
