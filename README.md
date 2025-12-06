# KickBlast Judo Training Fee Calculator

A C# WPF desktop application for managing athlete training fees and competition entries at KickBlast Judo club.

## Features

- Calculate monthly training costs based on training plan, competitions, and private coaching
- Validate athlete data including weight categories and competition eligibility
- Store athlete records in Supabase PostgreSQL database
- View all stored athletes in a data grid
- Professional GUI with grouped input sections and detailed cost breakdown

## Business Logic

### Training Plans
- **Beginner**: Rs. 250.00/week (cannot enter competitions)
- **Intermediate**: Rs. 300.00/week (can enter competitions)
- **Elite**: Rs. 350.00/week (can enter competitions)

### Weight Categories
- Heavyweight (Unlimited)
- Light-Heavyweight (100kg)
- Middleweight (90kg)
- Light-Middleweight (81kg)
- Lightweight (73kg)
- Flyweight (66kg)

### Costs
- Private tuition: Rs. 90.50 per hour (max 20 hours/month)
- Competition fee: Rs. 220 per competition
- Month = 4 weeks

## Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022 or JetBrains Rider
- Supabase database (already configured)

## Database Setup

The application uses a Supabase PostgreSQL database. The `athletes` table has already been created with the following schema:

```sql
CREATE TABLE athletes (
  id SERIAL PRIMARY KEY,
  athlete_name text NOT NULL,
  training_plan text NOT NULL,
  current_weight numeric(5,2) NOT NULL,
  weight_category text NOT NULL,
  competitions_entered integer DEFAULT 0,
  private_coaching_hours integer DEFAULT 0,
  monthly_cost numeric(10,2),
  created_at timestamptz DEFAULT now()
);
```

## Configuration

Before running the application, you need to update the database password in `App.config`:

1. Open `App.config`
2. Locate the connection string:
   ```xml
   <add name="SupabaseConnection"
        connectionString="Host=db.qcrvmxsbhenqqqmndpex.supabase.co;Database=postgres;Username=postgres;Password=YOUR_DATABASE_PASSWORD;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
        providerName="Npgsql" />
   ```
3. Replace `YOUR_DATABASE_PASSWORD` with your actual Supabase database password
4. You can find your database password in the Supabase dashboard under Settings > Database

## Building and Running

### Using Command Line

```bash
cd KickBlastJudoManager
dotnet restore
dotnet build
dotnet run
```

### Using Visual Studio

1. Open `KickBlastJudoManager.csproj` in Visual Studio
2. Press F5 to build and run

### Using JetBrains Rider

1. Open `KickBlastJudoManager.csproj` in Rider
2. Click the Run button or press Shift+F10

## Usage

1. **Enter Athlete Information**:
   - Fill in athlete name
   - Select training plan from dropdown
   - Enter current weight in kg
   - Select competition weight category
   - Enter number of competitions entered this month (0 if none)
   - Enter private coaching hours (0-20)

2. **Calculate Monthly Cost**:
   - Click "Calculate Monthly Cost" button
   - View the detailed cost breakdown in the output panel
   - See if athlete is within, under, or over their weight category

3. **Save to Database**:
   - After calculation, click "Save Athlete to SQL Database"
   - Athlete record will be saved to Supabase
   - Data grid will refresh automatically

4. **View All Athletes**:
   - All stored athletes appear in the bottom data grid
   - Click "Refresh Athletes List" to reload from database

## Example Output

```
Athlete: John Silva
Training Plan (Intermediate): 300.00 × 4 = Rs. 1200.00
Competitions (2 × 220.00): Rs. 440.00
Private Coaching (6 × 90.50): Rs. 543.00
---------------------------------------
Total Monthly Cost: Rs. 2183.00
Weight Status: Over category by 3kg
```

## Project Structure

```
KickBlastJudoManager/
├── Models/
│   ├── Athlete.cs              - Athlete data model
│   ├── TrainingPlan.cs         - Training plan logic
│   └── WeightCategory.cs       - Weight category validation
├── Services/
│   ├── CostCalculator.cs       - Business logic for cost calculation
│   └── DatabaseService.cs      - Database CRUD operations
├── MainWindow.xaml             - GUI layout
├── MainWindow.xaml.cs          - GUI event handlers
├── App.xaml                    - Application resources
├── App.xaml.cs                 - Application entry point
├── App.config                  - Database connection configuration
└── KickBlastJudoManager.csproj - Project file
```

## Validation Rules

- Athlete name is required
- All numeric fields must be valid numbers
- Current weight must be greater than zero
- Only Intermediate and Elite athletes can enter competitions
- Private coaching hours must be between 0 and 20
- All validation errors display in message boxes with retry option

## Technologies Used

- **Framework**: .NET 6.0 WPF
- **Language**: C# 10
- **Database**: PostgreSQL (Supabase)
- **Database Driver**: Npgsql 8.0.1
- **UI Framework**: Windows Presentation Foundation (WPF)

## License

This project is for educational purposes.
