using System;
using System.Diagnostics;
using System.Linq;
using csdb.Database.Models;
using NHibernate.Linq;

namespace csdb.Database.Repositories
{
    public static class OrmRepository
    {
        public static long? QueryBasic(int iterations, int warmUpIterations)
        {
            try
            {
                List<long> timings = new List<long>();
                for (int i = 0; i < iterations + warmUpIterations; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    using (var session = DbConnector.OpenSession())
                    {
                        var startDate = new DateTime(2024, 1, 1);
                        var endDate = new DateTime(2025, 1, 1);

                        var result = session.Query<Auftrag>()
                            .Where(a => a.AufDat >= startDate && a.AufDat < endDate)
                            .Select(a => new {
                                a.Aufnr,
                                a.AufDat,
                                a.ErlDat,
                                a.Dauer,
                                MitName = (a.Mitarbeiter == null) ? "N/A" : a.Mitarbeiter.MitName,
                                KunName = a.Kunde.KunName
                            })
                            .ToList();
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
                for (int i = 0; i < iterations + warmUpIterations; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    using (var session = DbConnector.OpenSession())
                    {
                        var result = (from m in session.Query<Mitarbeiter>()
                                      select new
                                      {
                                          m.MitID,
                                          m.MitName,
                                          TotalDauer = session.Query<Auftrag>()
                                                              .Where(a => a.Mitarbeiter.MitID == m.MitID)
                                                              .Sum(a => (decimal?)a.Dauer) ?? 0,
                                          TotalMontage = session.Query<Montage>()
                                                                .Where(mg => mg.Auftrag.Mitarbeiter.MitID == m.MitID)
                                                                .Sum(mg => (int?)mg.Anzahl) ?? 0
                                      })
                                      .OrderByDescending(x => x.TotalDauer)
                                      .ToList();
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
                for (int i = 0; i < iterations + warmUpIterations; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    using (var session = DbConnector.OpenSession())
                    {
                        var startDate = new DateTime(2024, 1, 1);
                        var endDate = new DateTime(2024, 12, 31);

                        var result = session.Query<Montage>()
                            .Where(mg => mg.Auftrag.AufDat >= startDate && mg.Auftrag.AufDat <= endDate)
                            .Select(mg => new {
                                mg.Auftrag.Aufnr,
                                mg.Auftrag.AufDat,
                                mg.Ersatzteil.EtBezeichnung,
                                mg.Anzahl,
                                mg.Ersatzteil.EtPreis,
                                Gesamtpreis = mg.Anzahl * mg.Ersatzteil.EtPreis
                            })
                            .ToList();
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
                for (int i = 0; i < iterations + warmUpIterations; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    using (var session = DbConnector.OpenSession())
                    {
                        var startDate = new DateTime(2022, 1, 1);
                        var endDate = new DateTime(2024, 12, 31);

                        var result = (from mg in session.Query<Montage>()
                                      where mg.Auftrag.AufDat >= startDate && mg.Auftrag.AufDat <= endDate
                                      orderby mg.Auftrag.AufDat descending, mg.Auftrag.Mitarbeiter.MitName, mg.Auftrag.Kunde.KunName
                                      select new
                                      {
                                          MitarbeiterName = mg.Auftrag.Mitarbeiter.MitName,
                                          KundeName = mg.Auftrag.Kunde.KunName,
                                          AuftragNr = mg.Auftrag.Aufnr,
                                          AuftragDatum = mg.Auftrag.AufDat,
                                          ErsatzteilBezeichnung = mg.Ersatzteil.EtBezeichnung,
                                          Jumlah = mg.Anzahl,
                                          TotalHarga = mg.Anzahl * mg.Ersatzteil.EtPreis
                                      })
                                      .ToList();
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