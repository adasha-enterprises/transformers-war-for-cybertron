# transformers-war-for-cybertron
Take-home assignment simulating the war between the Autobots and Decepticons

## Configuration Setup Instructions

* In a command/terminal window, run the following commands from repo root, replacing the values for user-name, password and server-host-name accordingly:
    * `cd WarForCybertron\WarForCybertron.API`
    * `dotnet user-secrets set "ConfigSettings:SQL_USER" "{{user-name}}"`
    * `dotnet user-secrets set "ConfigSettings:SQL_PASSWORD" "{{password}}"`
    * `dotnet user-secrets set "ConfigSettings:SQL_HOST" "{{server-host-name}}"`
    * `dotnet user-secrets set "ConfigSettings:SQL_DBNAME" "WarForCybertron"`

## Post Start-Up

* Open Objects\GetTransformerScore.sql from WarForCybertron.Repository project, copy the contents into a new query window in Microsoft SQL Server Management Studio and execute the query
* Import "War for Cybertron.postman_collection.json" into Postman and setup the environment to define variables for **`host`** e.g. https://localhost:44314, **`transformerId`**, **`bumblebee`** and **`jazz`**. The ids will be displayed in the response when the project runs. The id in the body of "Update Transformer" must match the id that is assigned to bumblebee.
* Open Test Explorer in Visual Studio and run all tests.
* View the API Documentation https://localhost:44314/swagger/index.html

## Packages

* Automapper: a simple library that helps to transform objects by defining mappings between properties. Primary purpose in this project is to map between domain objects and DTO.
* Serilog: File logging.
* Swashbuckle: Swagger Generator and Middleware for API documentation.
* ServiceStack: JSON and CSV Text Serializer, used to seed database from CSV file.
* Moq: Mocking framework used in unit tests.
* xunit: Developer testing framework used for creating unit tests.

## Decisions / Further Enhancements

* I have used the repository/unit-of-work pattern, due to its familiarity, although from my reading, I have learned that this may actually be less performant than using EF Core directly. With more time and research, I would look to remove this extra layer.
* The rules of battle are currently provided through a helper class. One way to improve upon this could be to create an interface that would allow different concrete implementations to be developed with a more sophisticated rules engine that could be registered at startup and made available to the service via dependency injection.
* Further constraints could be added to the Transformers table, such as making the Transformer name unique, to prevent duplicate names being entered e.g. Optimus and Predaking. An index could also be added to the Allegiance column.
* Being able to create the stored procedure via code, would remove the need for a manual step once the database has been created.