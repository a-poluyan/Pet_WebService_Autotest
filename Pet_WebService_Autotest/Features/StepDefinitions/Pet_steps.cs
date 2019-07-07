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
    public sealed class Pet_steps
    {
        private readonly string url = "http://petstore.swagger.io/v2/pet";                                                  // URL к тестируемому методу веб-сервиса
        private readonly Random random = new Random();                                                                      // random


        [Given(@"добавляем новое домашнее животное со статусом ""(.*)"" и сохраняем его в переменнную ""(.*)""")]
        public void ДопустимДобавляемНовоеДомашнееЖивотноеСоСтатусомИСохраняемЕгоВПеременнную(string status, string var)
        {
            using (var client = new HttpClient())
            {
                // Создаём новое домашнее животное
                Pet new_pet = new Pet()
                {
                    id = random.Next(0, 1000000),
                    name = RandomString.Get(random.Next(1, 20)),
                    photoUrls = new List<string>() { RandomString.Get(random.Next(1, 20)), RandomString.Get(random.Next(1, 20)) },
                    status = "available",
                    tags = new List<Tag>()
                        {
                            new Tag() { id = 0, name = RandomString.Get(random.Next(1, 10)) },
                            new Tag() { id = 1, name = RandomString.Get(random.Next(1, 10)) }
                        },
                    category = new Category() { id = 0, name = RandomString.Get(random.Next(1, 10)) }
                };

                // Получаем контент для POST запроса
                var content = new StringContent(JsonConvert.SerializeObject(new_pet), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));       // хотим получить ответ в формате JSON (по умолчанию сервис вернёт XML)
                HttpResponseMessage response = client.PostAsync(url, content).Result;                                   // вызываем сервис

                string json = response.Content.ReadAsStringAsync().Result;                                              // получаем только содержимое ответа (json)
                Pet pet = JsonConvert.DeserializeObject<Pet>(json);                                                     // десериализируем JSON в животного (заодно отлавливаем ошибки маппинга полей)

                // Проверяем, что создалось корректное животное
                if (pet.id              != new_pet.id ||
                    pet.name            != new_pet.name ||
                    pet.photoUrls[0]    != new_pet.photoUrls[0] ||
                    pet.photoUrls[1]    != new_pet.photoUrls[1] ||
                    pet.status          != new_pet.status ||
                    pet.tags[0].id      != new_pet.tags[0].id ||
                    pet.tags[0].name    != new_pet.tags[0].name ||
                    pet.tags[1].id      != new_pet.tags[1].id ||
                    pet.tags[1].name    != new_pet.tags[1].name ||
                    pet.category.id     != new_pet.category.id ||
                    pet.category.name   != new_pet.category.name) throw new Exception("Не все поля запроса перетянулись в ответ");

                ScenarioContext.Current[var] = new_pet;                                                                 // запоминаем новое домашнее животное в переменную var
            }
        }


        [Then(@"домашнее животное ""(.*)"" (не)?[ ]?существует")]
        public void ТоДомашнееЖивотноеСуществует(string var, string exist)
        {
            Pet pet = ScenarioContext.Current[var] as Pet;                                                              // получаем ранее созданное домашнее животное
            using (var client = new HttpClient())
            {
                string url_with_id = $"{url}/{pet.id}";

                // Ветвление кода, если мы НЕ должны увидеть домашнее животное в системе, то отлавливаем код состояния 404, иначе делаем запрос и парсим ответ системы
                if (exist == "не")
                {
                    new Default_steps().ДопустимВызываемИПолучаемКодСостоянияHTTP(url_with_id, 404);
                } else
                {
                    HttpResponseMessage response = client.GetAsync(new Uri(url_with_id)).Result;                                   // получаем домашнее животное по ID
                    string json = response.Content.ReadAsStringAsync().Result;                                          // получаем только содержимое ответа (json)
                    Pet new_pet = JsonConvert.DeserializeObject<Pet>(json);                                             // десериализируем JSON в животное

                    // Проверяем, что создалось корректное животное
                    if (pet.id != new_pet.id ||
                        pet.name != new_pet.name ||
                        pet.photoUrls[0] != new_pet.photoUrls[0] ||
                        pet.photoUrls[1] != new_pet.photoUrls[1] ||
                        pet.status != new_pet.status ||
                        pet.tags[0].id != new_pet.tags[0].id ||
                        pet.tags[0].name != new_pet.tags[0].name ||
                        pet.tags[1].id != new_pet.tags[1].id ||
                        pet.tags[1].name != new_pet.tags[1].name ||
                        pet.category.id != new_pet.category.id ||
                        pet.category.name != new_pet.category.name) throw new Exception($"Домашнее животное с ID {new_pet.id} в системе и животное в локальной переменной {var} не совпадают.");
                }
            }
        }

        [Given(@"обновляем домашнее животное ""(.*)""")]
        public void ДопустимОбновляемДомашнееЖивотное(string var)
        {
            Pet pet             = ScenarioContext.Current[var] as Pet;                                                  // получаем домашнего питомца из хранилища
            string[] statuses   = { "available", "sold", "pending" };                                                   // инициализируем все возможные статусы домашнего животного в системе

            using (var client = new HttpClient())
            {
                string pet_new_name     = RandomString.Get(random.Next(1, 10));                                         // изменяем название домашнего животного
                string pet_new_status   = statuses[random.Next(0, statuses.Length - 1)];                                // изменяем статус домашнего животного

                // Создаём словарь с обновлёнными данными по домашнему животному, который мы отправим сервису 
                var values = new Dictionary<string, string>
                {
                    { "name", pet_new_name },
                    { "status", pet_new_status }
                };
                var content = new FormUrlEncodedContent(values);

                HttpResponseMessage response = client.PostAsync($"{url}/{pet.id}", content).Result;                     // делаем POST запрос

                // На Сваггере нет описания успешного обновления данных по животному.
                // Из всей доступной нам информации на Сваггере есть упоминание только о кодах 200 и 405, поэтому мы не можем использовать метод IsSuccessStatusCode класса HttpResponseMessage для проверки успешности запроса (в методе проверяются все коды 2хх, а нас интересует только 200)
                // Основываясь на вышенаписанном, захардкодимся только на код 200, как единственном успешном из описания в Сваггере.
                int status = (int)response.StatusCode;
                if (status != 200) throw new Exception($"Во время обновления данных по домашнему животному произошла ошибка. Код состояния HTTP: {status}");


                // Обновляем информацию по домашнему животному у себя (локально), чтобы потом сравнить ответ от сервиса с локальным объектом
                pet.name    = pet_new_name;
                pet.status  = pet_new_status;

                ScenarioContext.Current[var] = pet;                                                                     // обновляем информацию о домашнем животном в переменной var
            }
        }

        [Given(@"удаляем домашнее животное ""(.*)""")]
        public void ДопустимУдаляемДомашнееЖивотное(string var)
        {
            Pet pet = ScenarioContext.Current[var] as Pet;                                                              // получаем домашнего питомца из хранилища
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = client.DeleteAsync($"{url}/{pet.id}").Result;                            // делаем DELETE запрос

                // На Сваггере нет описания успешного удаления данных по животному.
                // Из всей доступной нам информации на Сваггере есть упоминание только о кодах 200, 400 и 404
                // Основываясь на вышенаписанном, захардкодимся только на код 200, как единственном успешном из описания в Сваггере.
                int status = (int)response.StatusCode;
                if (status != 200) throw new Exception($"Во время удаления данных по домашнему животному произошла ошибка. Код состояния HTTP: {status}");
            }
        }

    }
}
