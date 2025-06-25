# CarAz - Car Marketplace Application

A comprehensive car marketplace application built with ASP.NET Core, Entity Framework Core, and PostgreSQL.

## 🚗 Features

- **User Management**: Registration, authentication, and role-based access control
- **Car Listings**: Browse, search, and filter car listings
- **Booking System**: Test drive, rental, and purchase bookings
- **Favorites**: Save and manage favorite cars
- **Admin Panel**: Manage users, cars, and bookings
- **Dealer Portal**: Manage car listings and bookings

## 🏗️ Architecture

The project follows Clean Architecture principles with the following structure:

- **CarAz.Core**: Domain entities, interfaces, and DTOs
- **CarAz.Infrastructure**: Data access layer with Entity Framework Core
- **CarAz.Presentation**: ASP.NET Core MVC application

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8.0
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Authentication**: Custom implementation with JWT
- **Password Hashing**: BCrypt

## 📋 Prerequisites

- .NET 8.0 SDK
- PostgreSQL 15+

## 🚀 Quick Start

### 1. Clone the Repository

```bash
git clone <repository-url>
cd CarAz
```

### 2. Set Up Database

1. Install PostgreSQL:
   ```bash
   brew install postgresql@15
   brew services start postgresql@15
   ```

2. Create database:
   ```bash
   psql postgres -c "CREATE DATABASE CarAzDb;"
   ```

3. Update connection string in `CarAz.Presentation/appsettings.json`

### 3. Apply Entity Framework Migrations

```bash
dotnet ef database update --project CarAz.Infrastructure --startup-project CarAz.Presentation
```

### 4. Run the Application

```bash
dotnet run --project CarAz.Presentation
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`

## 📊 Database Management

Use the provided management script for common database operations:

```bash
# Apply migrations
./manage-db.sh update

# Create new migration
./manage-db.sh migrate AddNewFeature

# Check database status
./manage-db.sh status

# Backup database
./manage-db.sh backup

# Reset database (⚠️ destructive)
./manage-db.sh reset
```

## 🗄️ Database Schema

### Users Table
- Primary user information and authentication
- Role-based access (User, Admin, Dealer)
- Email verification and password reset functionality

### Cars Table
- Car listings with detailed specifications
- Owner relationship with Users
- Status tracking (For Sale, For Rent, Sold, Rented)

### Bookings Table
- Test drive, rental, and purchase bookings
- Date range and status management
- User and car relationships

### FavoriteCars Table
- Many-to-many relationship between Users and Cars
- Prevents duplicate favorites

## 🔐 Sample Users

The application comes with pre-seeded sample users:

- **Admin**: `admin@caraz.com` / `Admin123!`
- **Dealer**: `dealer@caraz.com` / `Dealer123!`
- **Customer**: `user@caraz.com` / `User123!`

## 🛠️ Development

### Adding New Entities

1. Create entity in `CarAz.Core/Entities/`
2. Add DbSet to `ApplicationDbContext`
3. Configure entity in `OnModelCreating`
4. Create migration:
   ```bash
   ./manage-db.sh migrate AddNewEntity
   ```
5. Apply migration:
   ```bash
   ./manage-db.sh update
   ```

### Repository Pattern

The application uses the repository pattern for data access:

- Interfaces defined in `CarAz.Core/Interfaces/`
- Implementations in `CarAz.Infrastructure/Repositories/`
- Dependency injection configured in `Program.cs`

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Check the [troubleshooting section](DATABASE_SETUP.md#troubleshooting)
- Review the documentation files
