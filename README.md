# BlogAPIs

## Overview

BlogAPIs is a robust and secure RESTful API designed for managing a blogging platform. Built with .NET Core 7, it offers comprehensive features for user authentication and blog post management. The API leverages JWT for secure authentication and authorization, ensuring a secure and reliable user experience.

## Features

- **User Management**: Seamless registration and login functionalities with JWT-based authentication.
- **Blog Post Management**: Full CRUD (Create, Read, Update, Delete) operations for blog posts.
- **Data Validation**: Ensures data integrity and prevents the processing of invalid data.
- **Secure Endpoints**: Protected routes accessible only with valid JWT Bearer tokens.
- **Database Integration**: Utilizes Entity Framework Core for efficient database management and relationships.

## Technologies Used

- **.NET Core 7**: The backbone of the application, providing a modern and scalable framework.
- **Entity Framework Core**: ORM for database operations and management.
- **JWT (JSON Web Tokens)**: For secure authentication and authorization.
- **SQL Server**: Primary database, with flexibility to use any database supported by EF Core.
- **ASP.NET Core API**: Framework for building and managing the API.

## Installation and Setup

1. **Clone the Repository**: Obtain the source code from the GitHub repository.
2. **Configure the Database**: Update the connection string in the configuration file to point to your database.
3. **Run Migrations**: Apply database migrations to set up the required schema.
4. **Start the Application**: Launch the API locally or deploy it to your desired hosting environment.

## API Endpoints

### User Authentication

- **POST /api/Authenticator/RegisterAsync**: *Register a new user.*
- **POST /api/Authenticator/LoginAsync**: *Login and receive a JWT.*
- **Authentication**:
All protected endpoints require a valid JWT Bearer token. Include the token in the `Authorization` header of your requests: [*Bearer* *your-token-here*]

### Blog Posts

- **GET /api/Blog/GetAllPosts**:  *Retrieve all blog posts.*
- **GET /api/Blog/GetPostByID**:  *Retrieve a specific blog post by ID.*
- **POST /api/Blog/CreatePost**:  *Create a new blog post.*
- **PUT /api/Blog/EditPost**:  *Update an existing blog post by ID.*
- **DELETE /api/Blog/DeletePost**:  *Delete a blog post by ID.*
- **GET /api/Blog/GetTopXPosts**:  *Retrieve the top X blog posts.*
- **GET/api/Blog/GetPostsByIdRange**:  *Retrieve blog posts within a specified ID range.*
- **GET/api/Blog/GetPostByKeywords**:  *Retrieve blog posts containing specific keywords.*



## Contributing

We welcome contributions to enhance the functionality and features of BlogAPIs. Please fork the repository and submit a pull request with your proposed changes.

## License

This project is licensed under the MIT License. Please refer to the LICENSE file for more details.

## Contact

For any questions, issues, or contributions, please contact [Zain Ahmed Refaat](mailto:zeinahmed04@gmail.com).
  
