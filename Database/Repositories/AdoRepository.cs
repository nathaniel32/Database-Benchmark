using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace csdb.Database.Repositories
{
    public static class AdoRepository
    {
        public static long? QueryBasic(int iterations, int warmUpIterations)
        {
            try
            {
                List<long> timings = new List<long>();
                string connectionString = DbConnector.GetConnectionString();

                for (int i = 0; i < iterations + warmUpIterations; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = @"SELECT a.Aufnr, a.AufDat, a.ErlDat, a.Dauer, m.MitName, k.KunName
                                        FROM Auftrag a
                                        LEFT JOIN Mitarbeiter m ON a.MitID = m.MitID
                                        JOIN Kunde k ON a.KunNr = k.KunNr
                                        WHERE a.AufDat >= @startDate AND a.AufDat < @endDate;";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@startDate", new DateTime(2024, 1, 1));
                            command.Parameters.AddWithValue("@endDate", new DateTime(2025, 1, 1));

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Materialisasi objek
                                    var obj = new
                                    {
                                        Aufnr = reader.GetInt32(0),
                                        AufDat = reader.GetDateTime(1),
                                        ErlDat = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
                                        Dauer = reader.IsDBNull(3) ? (decimal?)null : reader.GetDecimal(3),
                                        MitName = reader.IsDBNull(4) ? "N/A" : reader.GetString(4),
                                        KunName = reader.GetString(5)
                                    };
                                }
                            }
                        }
                    }
                    stopwatch.Stop();
                    if (i >= warmUpIterations)
                    {
                        timings.Add(stopwatch.ElapsedMilliseconds);
                    }
                }
                return (long)timings.Average();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        public static long? QueryAggregate(int iterations, int warmUpIterations)
        {
            try
            {
                List<long> timings = new List<long>();
                string connectionString = DbConnector.GetConnectionString();

                for (int i = 0; i < iterations + warmUpIterations; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = @"SELECT m.MitID, m.MitName, SUM(a.Dauer) AS TotalDauer, SUM(mg.Anzahl) AS TotalMontage
                                        FROM Mitarbeiter m
                                        LEFT JOIN Auftrag a ON m.MitID = a.MitID
                                        LEFT JOIN Montage mg ON a.Aufnr = mg.AufNr
                                        GROUP BY m.MitID, m.MitName
                                        ORDER BY TotalDauer DESC;";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Materialisasi objek
                                    var obj = new
                                    {
                                        MitID = reader.GetString(0),
                                        MitName = reader.GetString(1),
                                        TotalDauer = reader.IsDBNull(2) ? (decimal?)null : reader.GetDecimal(2),
                                        TotalMontage = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3)
                                    };
                                }
                            }
                        }
                    }
                    stopwatch.Stop();
                    if (i >= warmUpIterations)
                    {
                        timings.Add(stopwatch.ElapsedMilliseconds);
                    }
                }
                return (long)timings.Average();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        public static long? QueryDetail(int iterations, int warmUpIterations)
        {
            try
            {
                List<long> timings = new List<long>();
                string connectionString = DbConnector.GetConnectionString();

                for (int i = 0; i < iterations + warmUpIterations; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = @"SELECT a.Aufnr, a.AufDat, e.EtBezeichnung, mg.Anzahl, e.EtPreis, (mg.Anzahl * e.EtPreis) AS Gesamtpreis
                                        FROM Auftrag a
                                        JOIN Montage mg ON a.Aufnr = mg.AufNr
                                        JOIN Ersatzteil e ON mg.EtID = e.EtID
                                        WHERE a.AufDat BETWEEN @startDate AND @endDate;";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@startDate", new DateTime(2024, 1, 1));
                            command.Parameters.AddWithValue("@endDate", new DateTime(2024, 12, 31));

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Materialisasi objek
                                    var obj = new
                                    {
                                        Aufnr = reader.GetInt32(0),
                                        AufDat = reader.GetDateTime(1),
                                        EtBezeichnung = reader.GetString(2),
                                        Anzahl = reader.GetInt32(3),
                                        EtPreis = reader.GetDecimal(4),
                                        Gesamtpreis = reader.GetDecimal(5)
                                    };
                                }
                            }
                        }
                    }
                    stopwatch.Stop();
                    if (i >= warmUpIterations)
                    {
                        timings.Add(stopwatch.ElapsedMilliseconds);
                    }
                }
                return (long)timings.Average();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        public static long? QueryHeavyJoin(int iterations, int warmUpIterations)
        {
            try
            {
                List<long> timings = new List<long>();
                string connectionString = DbConnector.GetConnectionString();

                for (int i = 0; i < iterations + warmUpIterations; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = @"SELECT
                                            m.MitName,
                                            k.KunName,
                                            a.Aufnr,
                                            a.AufDat,
                                            e.EtBezeichnung,
                                            mg.Anzahl,
                                            (mg.Anzahl * e.EtPreis) AS Gesamtpreis
                                        FROM
                                            Montage mg
                                        JOIN
                                            Auftrag a ON mg.Aufnr = a.Aufnr
                                        JOIN
                                            Mitarbeiter m ON a.MitID = m.MitID
                                        JOIN
                                            Kunde k ON a.KunNr = k.KunNr
                                        JOIN
                                            Ersatzteil e ON mg.EtID = e.EtID
                                        WHERE
                                            a.AufDat BETWEEN @startDate AND @endDate
                                        ORDER BY
                                            a.AufDat DESC, m.MitName, k.KunName;";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@startDate", new DateTime(2023, 1, 1));
                            command.Parameters.AddWithValue("@endDate", new DateTime(2024, 12, 31));

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Materialisasi objek
                                    var obj = new
                                    {
                                        MitarbeiterName = reader.GetString(0),
                                        KundeName = reader.GetString(1),
                                        AuftragNr = reader.GetInt32(2),
                                        AuftragDatum = reader.GetDateTime(3),
                                        ErsatzteilBezeichnung = reader.GetString(4),
                                        Jumlah = reader.GetInt32(5),
                                        TotalHarga = reader.GetDecimal(6)
                                    };
                                }
                            }
                        }
                    }
                    stopwatch.Stop();
                    if (i >= warmUpIterations)
                    {
                        timings.Add(stopwatch.ElapsedMilliseconds);
                    }
                }
                return (long)timings.Average();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
    }
}