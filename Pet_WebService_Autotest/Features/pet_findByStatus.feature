# language: ru
@all @pet_findByStatus
Функционал: pet_findByStatus
	Автотестирование метода /pet/findByStatus
	Описание метода веб-сервиса находится по адресу: http://petstore.swagger.io/#/pet/findPetsByStatus

@get_pet
Структура сценария: Получение списка домашних животных
	Допустим получаем список домашних животных со статусом "<статус>" в переменнную "pets"
	То список домашних животных в переменной "pets" не должен быть пустым

	Примеры:
	|  статус   |
	| available |
	|  pending  |
	|   sold    |

@correct_status
Структура сценария: Проверка корректности входного параметра STATUS
	Допустим вызываем "<url>" и получаем код состояния HTTP "<код>"

	Примеры:
	|								url								  | код |
	| http://petstore.swagger.io/v2/pet/findByStatus?status=available | 200 |
	| http://petstore.swagger.io/v2/pet/findByStatus?status=pending   | 200 |
	| http://petstore.swagger.io/v2/pet/findByStatus?status=sold      | 200 |
	| http://petstore.swagger.io/v2/pet/findByStatus?status=notvalid  | 400 |
	| http://petstore.swagger.io/v2/pet/findByStatus				  | 400 |
