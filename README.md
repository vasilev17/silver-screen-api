# SilverScreen API documentation #

### Why is this created? ###
To help you set up or modify the database.

### Do I need to do anything to create the database and the tables in it? ###
Nope. The Docker container initializes the database and the tables are initialized automatically when you run the app via migrations.

### Where I can see the tables? ###
You can see them on the [phpMyAdmin](http://localhost:5050/) page or use [MySQL Workbench](https://www.mysql.com/products/workbench/).

### How can I add, modify or delete a table? ###
There are multiple ways to modify a specific table:

* Use [phpMyAdmin](http://localhost:5050/) and modify the table via the UI.
* Use [MySQL Workbench](https://www.mysql.com/products/workbench/) and write a query that adds, modifies or deletes the table. 
* Modify the EF entities in the Models/Tables folder that corresponds to the table in the database.

**NOTE: You need to update the database, otherwise it's not going to work.**

### How to update the database? ###
This is a necessary task to complete if you modified the database in some way.

If you modified the databse externally via *phpMyAdmin* or *MySQL Workbench* you need to do the following */skip if you updated the entities directly/*:

* Open the package manager console in Visual Studio
* Run this command: `Scaffold-DbContext "datasource=localhost;port=3306;database=SilverScreen;username=root;password=5w%q@IiwWQc8" MySql.EntityFrameworkCore -OutputDir "Models/Tables" -f` to update the EF entities. 

When the EF entities are updated via scaffolding, it replaces the original `SilverScreenContext.cs` file, which removes the configuration for the MySQL connection string, which will generate errors in some places.

*Note that if you modified the database internally(modified the entities directly), you don't need to follow this instructions.* 

Here what to do in case of scaffolding:

* Import `SilverScreen.Services`

* Find `optionsBuilder.UseMySQL`, and replace the entire row with `optionsBuilder.UseMySQL("datasource=" + GetLocalIPService.GetLocalIPAddress() + "; port=3306; database=SilverScreen; username=root; password=" + Environment.GetEnvironmentVariable("SSDbPass"));` and remove the warning that starts with the # symbol above.

After doing all this steps or you have updated the entitied directly, open the Package Manager console in Visual Studio and run this command: `Add-Migration NameOfTheMigraion`.

**If you did followed these steps correctly, you have successfully updated the database!**

