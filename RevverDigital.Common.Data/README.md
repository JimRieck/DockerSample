# Introduction 
This C# project is designed to provide common classes for a database repository and any helper methods. The project includes a couple generic class that can be used as a base for database repositories, with methods for performing common CRUD (Create, Read, Update, Delete) operations.

Using this project, developers can quickly and easily implement database repositories and perform tests against an in-memory database, allowing for faster and more efficient development and testing of database-driven applications.

Repositories
- EntityRepository
    - Contains both Read and Write
- ReadEntityRepository
    - ExistsAsync
    - GetAllAsync
    - GetByIdAsync
    - SearchAsync
- WriteEntityRepository
    - InsertAsync
    - UpdateAsync
    - DeleteByIdAsync
    - HardDeleteByIdAsync

Helpers
- InMemoryDBHelper
    - In addition to the repository class, the project also includes a test helper class for using an in-memory database with Entity Framework Core. This class makes it easy to create and seed an in-memory database for testing purposes, without having to worry about the details of setting up a test database or cleaning up after the test is done.