# Todo.Core

Библиотека классов .NET 8 для управления списком задач (ToDo) с настроенным пайплайном CI/CD и публикацией NuGet-пакета в GitHub Packages.

Разработано в рамках практического занятия «Проектирование и публикация .NET-библиотеки с CI/CD».

---

## 📌 Содержание
- [Структура решения](#-структура-решения)
- [Функциональность](#-функциональность)
- [Сборка и запуск тестов](#-сборка-и-запуск-тестов)
- [Подключение пакета через GitHub Packages (NuGet)](#-подключение-пакета-через-github-packages-nuget)
  - [Способ 1: Через Visual Studio](#способ-1-через-visual-studio)
  - [Способ 2: Через .NET CLI и nuget.config](#способ-2-через-net-cli-и-nugetconfig)
- [Пример использования](#-пример-использования)
- [CI/CD Пайплайн](#-cicd-пайплайн)

---

## 📁 Структура решения

```text
TodoSolution/
├── .github/
│   └── workflows/
│       ├── ci.yml                 # Непрерывная интеграция (Build, Test, Pack)
│       └── publish-package.yml    # Публикация пакета в GitHub Packages
├── Todo.Core/
│   ├── TodoItem.cs                # Модель задачи
│   ├── TodoList.cs                # Коллекция задач и операции над ними
│   └── Todo.Core.csproj           # Проект библиотеки с метаданными NuGet
├── Todo.Core.Tests/
│   ├── TodoListTests.cs           # Модульные тесты (xUnit)
│   └── Todo.Core.Tests.csproj     # Проект тестов
├── .gitignore
├── README.md
└── TodoSolution.sln
```

---

## ✨ Функциональность

Библиотека `Todo.Core` предоставляет:
* **`TodoItem`**:
  * `Guid Id` — уникальный идентификатор задачи.
  * `string Title` — название задачи (с валидацией на пустоту).
  * `bool IsDone` — статус выполнения.
  * Методы `MarkDone()`, `MarkUndone()`, `Rename(string newTitle)`.
* **`TodoList`**:
  * Добавление задачи (`Add(string title)`).
  * Удаление задачи по ID (`Remove(Guid id)`).
  * Поиск задач по подстроке без учёта регистра (`Find(string substring)`).
  * Получение списка задач только для чтения (`Items`) и их количества (`Count`).
  * Сохранение и загрузка задач в формате JSON (`Save(string path)`, `Load(string path)`).

---

## 🛠 Сборка и запуск тестов

### Сборка решения:
```bash
dotnet restore
dotnet build -c Release
```

### Запуск модульных тестов:
```bash
dotnet test -c Release --verbosity normal
```

### Локальная упаковка NuGet-пакета:
```bash
dotnet pack Todo.Core/Todo.Core.csproj -c Release -o ./artifacts
```

---

## 📦 Подключение пакета через GitHub Packages (NuGet)

Пакет опубликован в GitHub Packages по адресу:
`https://nuget.pkg.github.com/ANYWALL1/index.json`

### Способ 1: Через Visual Studio
1. Откройте проект в Visual Studio.
2. Перейдите в меню **Tools** (Сервис) → **Options** (Параметры) → **NuGet Package Manager** → **Package Sources** (Источники пакетов).
3. Нажмите кнопку **+** (Добавить) и укажите:
   * **Name**: `GitHubPackages`
   * **Source**: `https://nuget.pkg.github.com/ANYWALL1/index.json`
4. Нажмите **Update** (Обновить) и затем **OK**.
5. При первой установке пакета NuGet запросит авторизацию:
   * **Username**: ваш GitHub логин (`ANYWALL1`)
   * **Password**: ваш Personal Access Token (PAT) с правами `read:packages`.
6. В окне **Manage NuGet Packages** выберите источник `GitHubPackages`, найдите `Todo.Core` и нажмите **Install**.

### Способ 2: Через .NET CLI и nuget.config
Создайте рядом со своим проектом файл `nuget.config`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="github" value="https://nuget.pkg.github.com/ANYWALL1/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="ANYWALL1" />
      <add key="ClearTextPassword" value="ВАШ_GITHUB_PAT" />
    </github>
  </packageSourceCredentials>
</configuration>
```

Затем установите пакет командой:
```bash
dotnet add package Todo.Core --version 0.1.0
```

---

## 💻 Пример использования

```csharp
using System;
using Todo.Core;

class Program
{
    static void Main()
    {
        var todoList = new TodoList();

        // Добавление задач
        var task1 = todoList.Add("Купить молоко");
        var task2 = todoList.Add("Сделать практическую работу по CI/CD");

        // Отметка задачи выполненной
        task1.MarkDone();

        // Поиск задач
        var found = todoList.Find("практическую");
        foreach (var item in found)
        {
            Console.WriteLine($"[{(item.IsDone ? "X" : " ")}] {item.Title}");
        }

        // Сохранение списка в JSON
        todoList.Save("todos.json");

        // Загрузка списка из JSON
        var restoredList = new TodoList();
        restoredList.Load("todos.json");
        Console.WriteLine($"Загружено задач: {restoredList.Count}");
    }
}
```

---

## 🚀 CI/CD Пайплайн

В проекте настроены два GitHub Actions workflow:
1. **`ci.yml` (CI / Build & Test)**:
   * Запускается при каждом `push` и `pull_request` в ветку `master`.
   * Выполняет: `Checkout` → `Setup .NET 8` → `Cache NuGet packages` → `dotnet restore` → `dotnet build` → `dotnet test` → `dotnet pack`.
2. **`publish-package.yml` (Publish package to GitHub Packages)**:
   * Запускается автоматически после успешного завершения пайплайна `CI / Build & Test`.
   * Собирает релизный NuGet-пакет и публикует его в GitHub Packages с использованием встроенного `${{ secrets.GITHUB_TOKEN }}`.
