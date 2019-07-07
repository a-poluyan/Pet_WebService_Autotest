using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using TechTalk.SpecFlow;

namespace Pet_WebService_Autotest.Features.StepDefinitions
{
    [Binding]
    public sealed class Pet_findByStatus_steps
    {
        private readonly string url = "http://petstore.swagger.io/v2/pet/findByStatus";                                 // URL к тестируемому методу веб-сервиса


        [Given(@"получаем список домашних животных со статусом ""(.*)"" в переменнную ""(.*)""")]
        public void ДопустимПолучаемСписокДомашнихЖивотныхСоСтатусомВПеременнную(string status, string var)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response    = client.GetAsync(new Uri($"{url}?status={status}")).Result;            // получаем ответ от сервиса
                string json                     = response.Content.ReadAsStringAsync().Result;                          // получаем только содержимое ответа (json)
                ScenarioContext.Current[var]    = JsonConvert.DeserializeObject<List<Pet>>(json);                       // десериализируем JSON в список животных и запоминаем в переменную var
            }
        }

        [Then(@"список домашних животных в переменной ""(.*)"" не должен быть пустым")]
        public void ТоСписокДомашнихЖивотныхВПеременнойНеДолженБытьПустым(string var)
        {
            List<Pet> pets = ScenarioContext.Current[var] as List<Pet>;
            if (pets.Count == 0) throw new Exception("Список домашних животных пуст");
        }
    }
}
