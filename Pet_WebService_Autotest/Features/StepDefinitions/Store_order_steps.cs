using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TechTalk.SpecFlow;

namespace Pet_WebService_Autotest.Features.StepDefinitions
{
    [Binding]
    public sealed class Store_order_steps
    {
        private readonly string url     = "http://petstore.swagger.io/v2/store/order";                                  // URL к тестируемому методу веб-сервиса
        private static readonly Random random = new Random();                                                           // random


        [Given(@"создаём новый заказ на покупку домашнего животного и сохраняем его в переменнную ""(.*)""")]
        public void ДопустимСоздаёмНовыйЗаказНаПокупкуДомашнегоЖивотногоИСохраняемЕгоВПеременнную(string var)
        {
            using (var client = new HttpClient())
            {
                // Создаём новый домашнее животное
                Order new_order = new Order()
                {
                    id = random.Next(1, 10),
                    petId = random.Next(0, 1000000),
                    quantity = random.Next(0, 100),
                    shipDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff+0000"),
                    status = "placed",
                    complete = "false"
                };


                // Получаем контент для POST запроса
                var content = new StringContent(JsonConvert.SerializeObject(new_order), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));       // хотим получить ответ в формате JSON (по умолчанию сервис вернёт XML)
                HttpResponseMessage response = client.PostAsync(url, content).Result;                                   // вызываем сервис

                string json = response.Content.ReadAsStringAsync().Result;                                              // получаем только содержимое ответа (json)
                Order order = JsonConvert.DeserializeObject<Order>(json);                                               // десериализируем JSON в заказ (заодно отлавливаем ошибки маппинга полей)

                // Проверяем, что создался корректный заказ
                if (order.id        != new_order.id ||
                    order.petId     != new_order.petId ||
                    order.quantity  != new_order.quantity ||
                    order.shipDate  != new_order.shipDate ||
                    order.status    != new_order.status ||
                    order.complete  != new_order.complete) throw new Exception("Не все поля запроса перетянулись в ответ");

                ScenarioContext.Current[var] = new_order;                                                               // запоминаем новый заказ в переменную var
            }
        }

        [Then(@"заказ ""(.*)"" существует")]
        public void ТоЗаказСуществует(string var)
        {
            Order order = ScenarioContext.Current[var] as Order;                                                        // получаем ранее созданный заказ
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(new Uri($"{url}/{order.id}")).Result;                    // получаем заказ по ID
                string json = response.Content.ReadAsStringAsync().Result;                                              // получаем только содержимое ответа (json)
                Order new_order = JsonConvert.DeserializeObject<Order>(json);                                             // десериализируем JSON в заказ

                // Проверяем, что создался корректный заказ
                if (order.id        != new_order.id ||
                    order.petId     != new_order.petId ||
                    order.quantity  != new_order.quantity ||
                    order.shipDate  != new_order.shipDate ||
                    order.status    != new_order.status ||
                    order.complete  != new_order.complete) throw new Exception($"Заказ с ID {new_order.id} в системе и заказ в локальной переменной {var} не совпадают.");
            }
        }

    }
}
