Что такое SocketFlow, зачем он нужен.

В моём понимании SocketFlow это библиотека, которая позволяет обмениваться данными между компьютерами и программами на разных языках не сложнее чем вызов метода.
Большой упор ставится на **удобство**.

Я начал писать эту библиотеку только потому что не нашёл чего-то хорошего и удобного для того, чтобы запустить сервер на C# и подключиться к нему из браузерного Js (WebSocket)

Референсная реализация библиотек написана на C#

## Основные фичи (цели) либы
1. Работа на нескольких протоколах одновременно (Как правило чем ниже - тем лучше, не хочется слать события с 16 байтами через http)
2. События
    * `Send<T>(int, T)` - отправить
    * `Bind<T>(int, Action<T>)` - получить
3. Данные приходят программисту в готовом виде (**Не сложнее чем вызвать метод**)
4. `Json` - базовый тип данных внутри
5. Возможность форматировать данные по-своему
    * `DataWrapper` - Ресурс осуществляющий форматирование объекта в байты и обратно, можно создавать свои
6. Группы пользователей, для упрощённой и оптимизированной отправки

Кроме того думаю на счёт кластеризации, шифрования, авторизации и невосприимчивости к разрыву соединений

## Как это работает?
Библиотека событийно-ориентированная. То есть с обоих сторон у нас есть возможность подписать какому-то событию свой метод.

Например клиент создаёт экземпляр SocketFlow и подписывает на id события _11_ какой-то метод.
Когда сервер отправит событие _11_ то метод, подписанный на клиенте будет вызван с нужными данными уже в готовом виде.

Вам лишь требуется не ошибиться типом данных в методе, в случае строго-типизированных языков вам нужно держать синхронные типы на событиях по разные стороны.

## Что такое id события и почему числа?
Идентификатор события по-сути просто позволяет вызывать разные функции в зависимости от id

Id - это числа. Это кажется неудобно, ведь например есть та же библиотека Socket.io которая делает что-то подобное но со строками.

В Socket.io ты отправляешь также события, но ты называешь их строкой, например: `send("position", new Point(1, 1))`.
Кажется тут всё понятно в отличии от моего: `send(1, new Point(1, 1))` где непонятно что за точка отправляется.

Мой ответ - `enum`.

А вообще на это есть ряд причин:
* Меньше размер каждого события
* На порядок меньше шанс ошибиться (Ну ладно мы, конечно, можем сделать словарь, но всё же)
* Также можно читать названия событий
* Не требуется первичная инициализация (В случае если бы мы хотели сократить размер событий и сделать вид что мы строки, а внутри при подключении всё приводить к числам)

## Браузер и всё-всё-всё
Сервер SocketFlow должен уметь работать поверх нескольких протоколов одновременно, а это значит что вы можете настроить сервер так, что сможете подключиться одновременно из браузера (Поверх WebSocket) и десктопного приложения (Поверх tcp)

Дальше мне стало лень описывать подробно ¯\\\_(ツ)\_/¯