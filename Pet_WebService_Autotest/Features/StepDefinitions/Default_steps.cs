using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using TechTalk.SpecFlow;

namespace Pet_WebService_Autotest.Features.StepDefinitions
{
    [Binding]
    public sealed class Default_steps
    {
        [Given(@"вызываем ""(.*)"" и получаем код состояния HTTP ""(.*)""")]
        public void ДопустимВызываемИПолучаемКодСостоянияHTTP(string url, int status_code)
        {
            using (var client = new HttpClient())
            {
                int response_status_code = (int)client.GetAsync(new Uri(url)).Result.StatusCode;            // смотрим код состояния HTTP запроса

                // Проверяем, что код состояния HTTP запроса совпадает с ожидаемым
                if (response_status_code != status_code) throw new Exception($"Код состояния HTTP отличается от ожидаемого.\nОжидаемый: {status_code},\nФактический: {response_status_code}");
            }
        }
    }
}
