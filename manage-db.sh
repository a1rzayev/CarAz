#!/bin/bash

# CarAz Database Management Script
# This script provides common database operations

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "🚗 CarAz Database Management"
echo "============================"

case "$1" in
    "update")
        echo "🔄 Updating database with latest migrations..."
        dotnet ef database update --project CarAz.Infrastructure --startup-project CarAz.Presentation
        ;;
    "migrate")
        if [ -z "$2" ]; then
            echo "❌ Please provide a migration name"
            echo "Usage: $0 migrate MigrationName"
            exit 1
        fi
        echo "📝 Creating new migration: $2"
        dotnet ef migrations add "$2" --project CarAz.Infrastructure --startup-project CarAz.Presentation
        ;;
    "remove")
        echo "🗑️  Removing last migration..."
        dotnet ef migrations remove --project CarAz.Infrastructure --startup-project CarAz.Presentation
        ;;
    "reset")
        echo "⚠️  This will drop and recreate the database. Are you sure? (y/N)"
        read -r response
        if [[ "$response" =~ ^([yY][eE][sS]|[yY])$ ]]; then
            echo "🗑️  Dropping database..."
            psql -U postgres -c "DROP DATABASE IF EXISTS CarAzDb;"
            echo "🔄 Creating database..."
            psql -U postgres -c "CREATE DATABASE CarAzDb;"
            echo "📝 Applying migrations..."
            dotnet ef database update --project CarAz.Infrastructure --startup-project CarAz.Presentation
            echo "✅ Database reset completed"
        else
            echo "❌ Operation cancelled"
        fi
        ;;
    "backup")
        if [ -z "$2" ]; then
            BACKUP_FILE="caraz_backup_$(date +%Y%m%d_%H%M%S).sql"
        else
            BACKUP_FILE="$2"
        fi
        echo "💾 Creating backup: $BACKUP_FILE"
        pg_dump -U postgres CarAzDb > "$BACKUP_FILE"
        echo "✅ Backup created: $BACKUP_FILE"
        ;;
    "restore")
        if [ -z "$2" ]; then
            echo "❌ Please provide backup file path"
            echo "Usage: $0 restore backup_file.sql"
            exit 1
        fi
        echo "🔄 Restoring from backup: $2"
        psql -U postgres CarAzDb < "$2"
        echo "✅ Database restored"
        ;;
    "status")
        echo "📊 Database Status:"
        echo "=================="
        echo "PostgreSQL Service:"
        brew services list | grep postgresql
        echo ""
        echo "Database Connection:"
        if psql -U postgres -d CarAzDb -c "SELECT version();" > /dev/null 2>&1; then
            echo "✅ Connected to CarAzDb"
        else
            echo "❌ Cannot connect to CarAzDb"
        fi
        echo ""
        echo "Tables:"
        psql -U postgres -d CarAzDb -c "\dt" 2>/dev/null || echo "❌ Cannot list tables"
        ;;
    "seed")
        echo "🌱 Seeding database with sample data..."
        dotnet run --project CarAz.Presentation
        ;;
    *)
        echo "Usage: $0 {update|migrate|remove|reset|backup|restore|status|seed}"
        echo ""
        echo "Commands:"
        echo "  update    - Apply pending migrations"
        echo "  migrate   - Create new migration (requires name)"
        echo "  remove    - Remove last migration"
        echo "  reset     - Drop and recreate database"
        echo "  backup    - Create database backup"
        echo "  restore   - Restore from backup file"
        echo "  status    - Show database status"
        echo "  seed      - Seed database with sample data"
        echo ""
        echo "Examples:"
        echo "  $0 update"
        echo "  $0 migrate AddUserTable"
        echo "  $0 backup my_backup.sql"
        echo "  $0 restore my_backup.sql"
        exit 1
        ;;
esac 