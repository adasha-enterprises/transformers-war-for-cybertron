using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WarForCybertron.Model;

namespace WarForCybertron.Repository
{
    public static class WarForCybertronContextExtensions
    {
        private static ILogger _logger = ApplicationLogging.CreateLogger("SearchHelpers");

        public static void UpdateDb(this WarForCybertronContext context)
        {
            try
            {
                var sqlScripts = new List<string>();

                sqlScripts.Add("USE [WarForCybertronTest]" +
                    Environment.NewLine +
                    "GO" +
                    Environment.NewLine +
                    "SET ANSI_NULLS ON" +
                    Environment.NewLine +
                    "GO" +
                    Environment.NewLine +
                    "SET QUOTED_IDENTIFIER ON" +
                    Environment.NewLine +
                    "GO" +
                    Environment.NewLine +
                    "IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetTransformerScore')" +
                    Environment.NewLine +
                    "DROP PROCEDURE GetTransformerScore" +
                    Environment.NewLine +
                    "GO" +
                    Environment.NewLine +
                    "CREATE PROCEDURE [dbo].[GetTransformerScore] @Id UNIQUEIDENTIFIER, @Score INT OUTPUT" +
                    Environment.NewLine +
                    "AS" +
                    Environment.NewLine +
                    "BEGIN" +
                    Environment.NewLine +
                    "SELECT @Score = Strength + Intelligence + Speed + Endurance + [Rank] + Courage + Firepower + Skill FROM Transformers WHERE Id = @Id" +
                    Environment.NewLine +
                    "END" +
                    Environment.NewLine +
                    "GO");

                sqlScripts.ForEach(sql =>
                    {
                        ExecuteSqlScript(context, sql);
                    }
                );
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update database: {e.Message}");
            }
        }

        public static void EnsureSeedData(this WarForCybertronContext context, string transformersWithGodMode)
        {
            try
            {
                if (!context.Transformers.Any())
                {
                    var transformers = GetFileContents<Transformer>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SeedData\\transformers.csv"));
                    var _ = transformersWithGodMode.Split(',');

                    transformers.ForEach(t =>
                        {
                            var transformer = t;
                            // aside from Optimus and Predaking, randomize all properties of the other Transformers
                            if (!_.Contains(t.Name))
                            {
                                transformer = new Transformer { Allegiance = t.Allegiance, Name = t.Name };
                                RandomizeTransformerProperties(transformer);
                            }

                            context.Transformers.Add(transformer);
                            context.SaveChanges();
                            transformer = null;
                        }
                    );
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to add seed data: {e.Message}");
            }
        }

        private static void ExecuteSqlScript(WarForCybertronContext context, string sql)
        {
            try
            {
                context.Database.ExecuteSqlRawAsync(sql);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to execute SQL script: {e.Message}\nsql");
            }
        }

        private static void RandomizeTransformerProperties(Transformer transformer)
        {
            var rand = new Random(Environment.TickCount);

            foreach (var prop in transformer.RatingProperties)
            {
                prop.SetValue(transformer, rand.Next(1, 10));
            };
        }

        private static List<T> GetFileContents<T>(string filePath) where T : new()
        {
            var fileContents = File.ReadAllText(filePath);
            CsvConfig.ItemDelimiterString = "\"";
            CsvConfig.ItemSeperatorString = ",";
            return CsvSerializer.DeserializeFromString<List<T>>(fileContents);
        }
    }
}
