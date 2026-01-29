# NorbitSSO
SSO система для участия в мини-хакатоне от Норбит. Возможности: Регистрация, Авторизация, Получение данных о пользователе, блокировка аккаунта
# Запуск
## Серверная часть
Вариант 1: Docker (рекомендуется)
1. Полностью склонировать репозиторий
2. Перейти в папку src/NorbitSSO
3. Запустить Docker Compose (`docker compose up -d`)
4. API и веб-интерфейс готовы к работе
5. По умолчанию, сайт и API доступны по протоколу https на порту 2443. Для правильной работы рекомендуется использовать домен web.test. Дабы избежать ошибок проверки сертификата, необходимо добавить корневой сертификат [NorbitCA.crt](https://github.com/TrainzCity/NorbitSSO/blob/main/src/NoribtSSO/NorbitCA.crt "NorbitCA.crt") в хранилище доверенных сертификатов
Страница авторизации: `https://web.test:2443/Login`
Страница регистрации: `https://web.test:2443/Register`

Вариант 2: Запуск через Visual Studio
0. Развернуть Microsoft SQL Server и импортировать базу данных при помощи [скрипта](https://github.com/TrainzCity/NorbitSSO/blob/main/src/NorbitBase.sql "скрипта")
1. Полностью склонировать репозиторий
2. Открыть решение NorbitSSO.sln
3. Выбрать конфигурацию Debug
4. Запустить

## Клиентское приложение
1. Настроить перенаправление ip в файле [hosts](https://github.com/TrainzCity/NorbitSSO/blob/main/extra/hosts "hosts")
2. Импортировать корневой сертификат [NorbitCA.crt](https://github.com/TrainzCity/NorbitSSO/blob/main/src/NoribtSSO/NorbitCA.crt "NorbitCA.crt") в хранилище доверенных сертификатов

3.1. Запустить скомпилированную версию приложения из [Releases](https://github.com/TrainzCity/NorbitSSO/releases "Releases")
3.2. Склонировать репозиторий проекта, открыть решение NorbitSSO.sln, выбрать проект UserApp и запустить в режиме Debug | Any CPU.
