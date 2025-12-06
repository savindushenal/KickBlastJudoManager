using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MySql.Data.MySqlClient;
using KickBlastJudoManager.Models;

namespace KickBlastJudoManager.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
        }

        #region Athlete Operations

        public void InsertAthlete(Athlete athlete)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"INSERT INTO athletes
                        (athlete_name, current_weight, plan_id, competition_category_id)
                        VALUES (@name, @weight, @planId, @categoryId)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", athlete.AthleteName);
                        cmd.Parameters.AddWithValue("@weight", athlete.CurrentWeight);
                        cmd.Parameters.AddWithValue("@planId", athlete.PlanId);
                        cmd.Parameters.AddWithValue("@categoryId", athlete.CompetitionCategoryId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving athlete: {ex.Message}", ex);
            }
        }

        public List<Athlete> GetAllAthletes()
        {
            var athletes = new List<Athlete>();

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"SELECT 
                                        a.athlete_id,
                                        a.athlete_name,
                                        a.current_weight,
                                        a.plan_id,
                                        a.competition_category_id,
                                        a.created_at,
                                        a.updated_at,
                                        tp.plan_name,
                                        tp.weekly_fee,
                                        tp.can_compete,
                                        wc.category_name,
                                        COUNT(DISTINCT c.competition_id) as total_competitions,
                                        COALESCE(SUM(cs.hours), 0) as total_coaching_hours
                                    FROM athletes a
                                    INNER JOIN training_plans tp ON a.plan_id = tp.plan_id
                                    INNER JOIN weight_categories wc ON a.competition_category_id = wc.category_id
                                    LEFT JOIN competitions c ON a.athlete_id = c.athlete_id 
                                        AND MONTH(c.competition_date) = MONTH(CURRENT_DATE())
                                        AND YEAR(c.competition_date) = YEAR(CURRENT_DATE())
                                    LEFT JOIN coaching_sessions cs ON a.athlete_id = cs.athlete_id
                                        AND MONTH(cs.session_date) = MONTH(CURRENT_DATE())
                                        AND YEAR(cs.session_date) = YEAR(CURRENT_DATE())
                                    GROUP BY a.athlete_id, a.athlete_name, a.current_weight, a.plan_id, 
                                             a.competition_category_id, a.created_at, a.updated_at,
                                             tp.plan_name, tp.weekly_fee, tp.can_compete, wc.category_name
                                    ORDER BY a.created_at DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var weeklyFee = reader.GetDecimal("weekly_fee");
                            var totalCompetitions = reader.GetInt32("total_competitions");
                            var totalCoachingHours = reader.GetDecimal("total_coaching_hours");

                            // Calculate monthly cost
                            var monthlyCost = (weeklyFee * 4) + 
                                            (totalCompetitions * 220) + 
                                            (totalCoachingHours * 90.50m);

                            athletes.Add(new Athlete
                            {
                                AthleteId = reader.GetInt32("athlete_id"),
                                AthleteName = reader.GetString("athlete_name"),
                                CurrentWeight = reader.GetDecimal("current_weight"),
                                PlanId = reader.GetInt32("plan_id"),
                                CompetitionCategoryId = reader.GetInt32("competition_category_id"),
                                CreatedAt = reader.GetDateTime("created_at"),
                                UpdatedAt = reader.GetDateTime("updated_at"),
                                TrainingPlanName = reader.GetString("plan_name"),
                                WeeklyFee = weeklyFee,
                                CanCompete = reader.GetBoolean("can_compete"),
                                WeightCategoryName = reader.GetString("category_name"),
                                TotalCompetitions = totalCompetitions,
                                TotalCoachingHours = totalCoachingHours,
                                MonthlyCost = monthlyCost
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading athletes: {ex.Message}", ex);
            }

            return athletes;
        }

        public Athlete GetAthleteById(int athleteId)
        {
            try
            {
                var athletes = GetAllAthletes();
                var athlete = athletes.FirstOrDefault(a => a.AthleteId == athleteId);
                if (athlete == null)
                    throw new Exception($"Athlete with ID {athleteId} not found.");
                return athlete;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving athlete: {ex.Message}", ex);
            }
        }

        public void UpdateAthlete(Athlete athlete)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"UPDATE athletes 
                        SET athlete_name = @name,
                            current_weight = @weight,
                            plan_id = @planId,
                            competition_category_id = @categoryId
                        WHERE athlete_id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", athlete.AthleteId);
                        cmd.Parameters.AddWithValue("@name", athlete.AthleteName);
                        cmd.Parameters.AddWithValue("@weight", athlete.CurrentWeight);
                        cmd.Parameters.AddWithValue("@planId", athlete.PlanId);
                        cmd.Parameters.AddWithValue("@categoryId", athlete.CompetitionCategoryId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                            throw new Exception($"Athlete with ID {athlete.AthleteId} not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating athlete: {ex.Message}", ex);
            }
        }

        public void DeleteAthlete(int athleteId)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    // Cascading deletes will handle competitions, coaching sessions, etc.
                    string query = "DELETE FROM athletes WHERE athlete_id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", athleteId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                            throw new Exception($"Athlete with ID {athleteId} not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting athlete: {ex.Message}", ex);
            }
        }

        #endregion

        #region Competition Operations

        public void AddCompetition(Competition competition)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"INSERT INTO competitions
                        (athlete_id, competition_date, competition_name, fee, result, notes)
                        VALUES (@athleteId, @date, @name, @fee, @result, @notes)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@athleteId", competition.AthleteId);
                        cmd.Parameters.AddWithValue("@date", competition.CompetitionDate);
                        cmd.Parameters.AddWithValue("@name", competition.CompetitionName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@fee", competition.Fee);
                        cmd.Parameters.AddWithValue("@result", competition.Result ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@notes", competition.Notes ?? (object)DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding competition: {ex.Message}", ex);
            }
        }

        public List<Competition> GetAthleteCompetitions(int athleteId)
        {
            var competitions = new List<Competition>();

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"SELECT * FROM competitions 
                                   WHERE athlete_id = @athleteId 
                                   ORDER BY competition_date DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@athleteId", athleteId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                competitions.Add(new Competition
                                {
                                    CompetitionId = reader.GetInt32("competition_id"),
                                    AthleteId = reader.GetInt32("athlete_id"),
                                    CompetitionDate = reader.GetDateTime("competition_date"),
                                    CompetitionName = reader.IsDBNull(reader.GetOrdinal("competition_name")) 
                                        ? null : reader.GetString("competition_name"),
                                    Fee = reader.GetDecimal("fee"),
                                    Result = reader.IsDBNull(reader.GetOrdinal("result")) 
                                        ? null : reader.GetString("result"),
                                    Notes = reader.IsDBNull(reader.GetOrdinal("notes")) 
                                        ? null : reader.GetString("notes"),
                                    CreatedAt = reader.GetDateTime("created_at")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading competitions: {ex.Message}", ex);
            }

            return competitions;
        }

        public void DeleteCompetition(int competitionId)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = "DELETE FROM competitions WHERE competition_id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", competitionId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting competition: {ex.Message}", ex);
            }
        }

        #endregion

        #region Coaching Session Operations

        public void AddCoachingSession(CoachingSession session)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"INSERT INTO coaching_sessions
                        (athlete_id, session_date, hours, hourly_rate, notes)
                        VALUES (@athleteId, @date, @hours, @rate, @notes)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@athleteId", session.AthleteId);
                        cmd.Parameters.AddWithValue("@date", session.SessionDate);
                        cmd.Parameters.AddWithValue("@hours", session.Hours);
                        cmd.Parameters.AddWithValue("@rate", session.HourlyRate);
                        cmd.Parameters.AddWithValue("@notes", session.Notes ?? (object)DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding coaching session: {ex.Message}", ex);
            }
        }

        public List<CoachingSession> GetAthleteCoachingSessions(int athleteId)
        {
            var sessions = new List<CoachingSession>();

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"SELECT * FROM coaching_sessions 
                                   WHERE athlete_id = @athleteId 
                                   ORDER BY session_date DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@athleteId", athleteId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sessions.Add(new CoachingSession
                                {
                                    SessionId = reader.GetInt32("session_id"),
                                    AthleteId = reader.GetInt32("athlete_id"),
                                    SessionDate = reader.GetDateTime("session_date"),
                                    Hours = reader.GetDecimal("hours"),
                                    HourlyRate = reader.GetDecimal("hourly_rate"),
                                    TotalFee = reader.GetDecimal("total_fee"),
                                    Notes = reader.IsDBNull(reader.GetOrdinal("notes")) 
                                        ? null : reader.GetString("notes"),
                                    CreatedAt = reader.GetDateTime("created_at")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading coaching sessions: {ex.Message}", ex);
            }

            return sessions;
        }

        public void DeleteCoachingSession(int sessionId)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = "DELETE FROM coaching_sessions WHERE session_id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", sessionId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting coaching session: {ex.Message}", ex);
            }
        }

        #endregion

        #region Reference Data

        public List<TrainingPlan> GetAllTrainingPlans()
        {
            var plans = new List<TrainingPlan>();

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM training_plans ORDER BY plan_id";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            plans.Add(new TrainingPlan
                            {
                                PlanId = reader.GetInt32("plan_id"),
                                PlanName = reader.GetString("plan_name"),
                                WeeklyFee = reader.GetDecimal("weekly_fee"),
                                CanCompete = reader.GetBoolean("can_compete")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading training plans: {ex.Message}", ex);
            }

            return plans;
        }

        public void InsertTrainingPlan(TrainingPlan plan)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"INSERT INTO training_plans
                        (plan_name, weekly_fee, can_compete)
                        VALUES (@name, @fee, @canCompete)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", plan.PlanName);
                        cmd.Parameters.AddWithValue("@fee", plan.WeeklyFee);
                        cmd.Parameters.AddWithValue("@canCompete", plan.CanCompete);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving training plan: {ex.Message}", ex);
            }
        }

        public void UpdateTrainingPlan(TrainingPlan plan)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"UPDATE training_plans
                        SET plan_name = @name,
                            weekly_fee = @fee,
                            can_compete = @canCompete
                        WHERE plan_id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", plan.PlanId);
                        cmd.Parameters.AddWithValue("@name", plan.PlanName);
                        cmd.Parameters.AddWithValue("@fee", plan.WeeklyFee);
                        cmd.Parameters.AddWithValue("@canCompete", plan.CanCompete);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating training plan: {ex.Message}", ex);
            }
        }

        public void DeleteTrainingPlan(int planId)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    // Check if any athletes use this plan
                    string checkQuery = "SELECT COUNT(*) FROM athletes WHERE plan_id = @id";
                    using (var checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@id", planId);
                        int athleteCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (athleteCount > 0)
                        {
                            throw new Exception($"Cannot delete this training plan. {athleteCount} athlete(s) are currently using it.");
                        }
                    }

                    string query = "DELETE FROM training_plans WHERE plan_id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", planId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting training plan: {ex.Message}", ex);
            }
        }

        public List<WeightCategory> GetAllWeightCategories()
        {
            var categories = new List<WeightCategory>();

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM weight_categories ORDER BY category_id";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new WeightCategory
                            {
                                CategoryId = reader.GetInt32("category_id"),
                                CategoryName = reader.GetString("category_name"),
                                UpperWeightLimit = reader.IsDBNull(reader.GetOrdinal("upper_weight_limit"))
                                    ? (decimal?)null : reader.GetDecimal("upper_weight_limit")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading weight categories: {ex.Message}", ex);
            }

            return categories;
        }

        #endregion

        #region Database Health

        public bool CheckTablesExist()
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"SELECT COUNT(*) FROM information_schema.tables 
                                   WHERE table_schema = DATABASE() 
                                   AND table_name IN ('training_plans', 'weight_categories', 'athletes', 
                                                      'competitions', 'coaching_sessions')";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        int tableCount = Convert.ToInt32(cmd.ExecuteScalar());
                        return tableCount == 5;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Competition Event Management

        public void RegisterCompetitionEvent(string competitionName, DateTime date)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    // Check if competition_events table exists, if not use old method
                    string checkTableQuery = @"SELECT COUNT(*) FROM information_schema.tables 
                                              WHERE table_schema = DATABASE() 
                                              AND table_name = 'competition_events'";
                    
                    using (var checkCmd = new MySqlCommand(checkTableQuery, conn))
                    {
                        var tableExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                        
                        if (tableExists)
                        {
                            // Use competition_events table
                            string query = @"INSERT INTO competition_events
                                (event_name, event_date, entry_fee)
                                VALUES (@name, @date, 220.00)
                                ON DUPLICATE KEY UPDATE event_name = event_name";

                            using (var cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@date", date);
                                cmd.Parameters.AddWithValue("@name", competitionName);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Fallback: just store in competitions table with dummy data
                            // This won't work with FK constraint, so we need to handle it
                            throw new Exception("Please run the database migration script first:\\n\\n" +
                                "supabase/migrations/20251206_add_competition_events.sql\\n\\n" +
                                "This will create the competition_events table.");
                        }
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1452) // Foreign key constraint fails
            {
                throw new Exception("Database schema needs updating. Please run:\\n\\n" +
                    "supabase/migrations/20251206_add_competition_events.sql\\n\\n" +
                    "This will create the required competition_events table.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error registering competition: {ex.Message}", ex);
            }
        }

        public List<CompetitionEvent> GetAllCompetitionsGrouped()
        {
            var competitions = new List<CompetitionEvent>();

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    // Check if competition_events table exists
                    string checkTableQuery = @"SELECT COUNT(*) FROM information_schema.tables 
                                              WHERE table_schema = DATABASE() 
                                              AND table_name = 'competition_events'";
                    
                    using (var checkCmd = new MySqlCommand(checkTableQuery, conn))
                    {
                        var tableExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                        
                        string query;
                        if (tableExists)
                        {
                            query = @"SELECT 
                                        ce.event_id as competition_id,
                                        ce.event_name as competition_name,
                                        ce.event_date as competition_date,
                                        COUNT(c.competition_id) as participant_count
                                    FROM competition_events ce
                                    LEFT JOIN competitions c ON c.event_id = ce.event_id
                                    GROUP BY ce.event_id, ce.event_name, ce.event_date
                                    ORDER BY ce.event_date DESC";
                        }
                        else
                        {
                            // Fallback to old method
                            query = @"SELECT 
                                        MIN(competition_id) as competition_id,
                                        competition_name,
                                        competition_date,
                                        COUNT(CASE WHEN athlete_id > 0 THEN 1 END) as participant_count
                                    FROM competitions
                                    GROUP BY competition_name, competition_date
                                    ORDER BY competition_date DESC";
                        }

                        using (var cmd = new MySqlCommand(query, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                competitions.Add(new CompetitionEvent
                                {
                                    CompetitionId = reader.GetInt32("competition_id"),
                                    CompetitionName = reader.GetString("competition_name"),
                                    CompetitionDate = reader.GetDateTime("competition_date"),
                                    ParticipantCount = reader.GetInt32("participant_count")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading competitions: {ex.Message}", ex);
            }

            return competitions;
        }

        #endregion
    }
}
