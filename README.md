<div align="center">

<h1>🗄️🎞️ Silver Screen — Movie Discovery & Social Platform [API]</h1>
  
<p>Silver Screen API is the backend of the Silver Screen platform – a full-stack web application for discovering, organising and discussing movies and series with your friends. It delivers data to the companion <a href="https://github.com/vasilev17/silver-screen-ui/tree/master">Silver Screen UI</a>.

<div>
  <img src="https://img.shields.io/badge/Team%20-%20Project%20-%20gray?logo=codecrafters&labelColor=orange" style="height: 30px; width: auto;">
</div>

</div>

---

## 🧰 Tech Stack

| Layer            | Technology                                   |
|------------------|----------------------------------------------|
| Core             | ASP.NET Core, C# |
| Data Access      | Entity Framework Core, MySQL |
| Auth             | JWT |
| External API     | [TMDB](https://developer.themoviedb.org/docs/getting-started) |
| DevOps           | [Silver Screen Docker](https://github.com/vasilev17/silver-screen-docker), Bitbucket Pipelines |

---

## 🏁 Getting Started

### Prerequisites
- **.NET 6 SDK**
- **Docker Desktop** (or a local MySQL 8 instance)
- *(Optional)* TMDB API key for extended catalogue seeding

### Clone

```bash
git clone https://github.com/vasilev17/silver-screen-api.git
cd silver-screen-api/SilverScreen
```

### API Documentation

#### Do I need to do anything to create the database and the tables in it?
No. The Docker container initializes the database and the tables are initialized automatically when you run the app via migrations.

#### Where I can see the tables?
You can see them on the [phpMyAdmin](http://localhost:5050/) page or use [MySQL Workbench](https://www.mysql.com/products/workbench/).

#### How can I add, modify or delete a table?
There are multiple ways to modify a specific table:

- Use [phpMyAdmin](http://localhost:5050/) and modify the table via the UI.
- Use [MySQL Workbench](https://www.mysql.com/products/workbench/) and write a query that adds, modifies or deletes the table. 
- Modify the EF entities in the Models/Tables folder that corresponds to the table in the database.

> [!NOTE]
> <i>You need to update the database, otherwise it's not going to work.</i>

#### How to update the database?
This is a necessary task to complete if you modified the database in some way.

If you modified the databse externally via *phpMyAdmin* or *MySQL Workbench* you need to do the following */ skip if you updated the entities directly /*:

- Open the package manager console in Visual Studio
- Run this command: `Scaffold-DbContext "datasource=localhost;port=3306;database=SilverScreen;username=root;password=5w%q@IiwWQc8" MySql.EntityFrameworkCore -OutputDir "Models/Tables" -f` to update the EF entities. 

When the EF entities are updated via scaffolding, it replaces the original `SilverScreenContext.cs` file, which removes the configuration for the MySQL connection string, which will generate errors in some places.

> [!NOTE]
> <i>If you modified the database internally (modified the entities directly), you don't need to follow this instructions.</i>

Here what to do in case of scaffolding:

- Find `optionsBuilder.UseMySQL`, and replace the entire row with `optionsBuilder.UseMySQL($"datasource={Environment.GetEnvironmentVariable("MYSQL_DATABASE_IP") ?? "localhost"}; port=3306; database=SilverScreen; username=root; password=" + Environment.GetEnvironmentVariable("SSDbPass"));` and remove the warning that starts with the `#` symbol above.

After executing these steps or updating the entities directly, open the Package Manager console in Visual Studio and run this command: `Add-Migration NameOfTheMigraion`.

### Following these steps should result in a successfully updated database!

---

## 🎬 Showcase

### API Models Diagram
<p align="center">
  <img width="622" height="620" alt="API Models Diagram" src="https://github.com/user-attachments/assets/f77b74bc-f644-4079-a6be-e8ba02a4b9b0" />
</p>
