# WORK IN PROGRESS....


# SFTP Checking Process

The service is designed to connect to the SFTP server and check for new files at regular intervals of 1 minute. This functionality is implemented using a timer logic that triggers the SFTP checking process.

## Configurable SFTP Server Settings

To provide flexibility, the SFTP server, file paths, and related settings are configurable. The service utilizes the IConfiguration interface to load these settings from the [appsettings.json](./appsettings.json) file. This allows easy modification of the SFTP server details without changing the code.

## Downloading New Files

The service is responsible for downloading all new files from the SFTP server to a designated local path. This ensures that any newly added files on the server are fetched and made available locally for further processing.

## Storing Downloaded File Paths in PostgreSQL

As part of the service's functionality, all downloaded file paths are stored in a [PostgreSQL](https://www.postgresql.org/) database. This ensures a record of the downloaded files and facilitates efficient tracking and management.

## Determining New vs. Old Files

To determine if a file is new or old, the service compares the file's creation time on the SFTP server with the stored records in the PostgreSQL database. By considering the file creation time, the service can distinguish between files that have been previously downloaded and new files that require processing.

## Database Interaction with Entity Framework

The service leverages [Entity Framework](https://docs.microsoft.com/en-us/ef/core/) - a popular object-relational mapping framework - to interact with the PostgreSQL database. This choice simplifies database operations and provides an intuitive and consistent approach to working with the data.

## Code-First Database Definition

The database schema and tables are defined using the code-first principle. With code-first migrations, the service automatically generates the required database structure based on the defined models and applies any necessary changes when the schema evolves.

## Resilient Service with Connection Handling

The service is designed to be resilient, capable of handling connection problems without crashing. It includes exception handling mechanisms to gracefully handle any connectivity issues that may arise during the SFTP operations. This ensures the service's stability and uninterrupted execution.

## Code Comments for Clarity

The code is enriched with meaningful comments that explain its functionality. These comments serve as documentation within the codebase, aiding in understanding and maintaining the service.

## Logging and Tracing

Although not explicitly demonstrated in the provided code snippet, it is recommended to implement logging and tracing mechanisms to enhance the service's observability. Popular logging frameworks such as [Serilog](https://serilog.net/) or [Microsoft.Extensions.Logging](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-6.0) can be utilized to achieve comprehensive and configurable logging, enabling clear visibility into the service's activities and troubleshooting.

## Dependency Injection Usage

The service follows the principle of dependency injection, promoting loosely coupled components and facilitating better code organization and testability. Dependency injection is utilized for the database context and the SFTP client, enabling seamless integration and flexibility in managing dependencies.

By adhering to these requirements and implementing the described functionalities, the backend service using .NET Core 6 ensures efficient handling of SFTP operations, reliable storage of downloaded file paths, seamless interaction with the PostgreSQL database, and robustness in the face of connection issues.


## PARTICIPATED/CONTACT

1. Daniel - [Developer](https://www.greatsampleresume.com/job-responsibilities/it-developer-responsibilities/) - *[LinkedIN](https://www.linkedin.com/in/danielsvas/)*
