using Microsoft.EntityFrameworkCore;
using SeriLog.DB;
using System.Security.Cryptography;
using System.Text;

namespace SeriLogProject.BusinessLogic
{
    public static class ProcSeedingBL
    {
        public static void DbProcedureUpdateExecute(this IApplicationBuilder app, ref IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                using (ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    //Read New procedure from file location
                    var newScriptToRun = GetSqlScriptsFromFolder(env);
                    foreach (var script in newScriptToRun)
                    {
                       // var scriptHash = script.Value.GetHashString();
                       // var exists = context.DbProcedureUpdateLog.Any(w => w.ScriptHash == scriptHash && w.ProcedureName == script.Key && w.HasError == false);

                        //if (exists == false)
                        {
                            using (var tranScope = context.Database.BeginTransaction())
                            {
                                try
                                {
                                    context.Database.ExecuteSqlRaw(script.Value);
                                   /* context.DbProcedureUpdateLog.Add(new DbProcedureUpdateLog
                                    {
                                        ProcedureName = script.Key,
                                        ScriptHash = scriptHash,
                                        ExecutedDatetime = DateTime.Now,
                                        HasError = false,
                                        ErrorMessage = null
                                    });*/
                                    context.SaveChanges();
                                    tranScope.Commit();
                                }
                                catch (Exception e)
                                {
                                    tranScope.Rollback();
                            /*        context.DbProcedureUpdateLog.Add(new DbProcedureUpdateLog
                                    {
                                        ProcedureName = script.Key,
                                        ScriptHash = scriptHash,
                                        ExecutedDatetime = DateTime.Now,
                                        HasError = true,
                                        ErrorMessage = e.Message
                                    });*/
                                    context.SaveChanges();
                                    throw;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static Dictionary<string, string> GetSqlScriptsFromFolder(IWebHostEnvironment env)
        {
            //Retribe the the phisical procedure file from specific location
            string sqlScriptsDirPath = Path.Combine(env.ContentRootPath, "StoredProcedures");
            string storedProceduresSqlScriptsDirPath = Path.Combine(sqlScriptsDirPath, "Procedures");
            if (Directory.Exists(storedProceduresSqlScriptsDirPath) == false)
            {
                Directory.CreateDirectory(storedProceduresSqlScriptsDirPath);
            }

            return Directory.GetFiles(storedProceduresSqlScriptsDirPath, "*.sql", SearchOption.AllDirectories).ToDictionary(Path.GetFileName, File.ReadAllText);
        }

        public static string GetHashString(this string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString)) sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static byte[] GetHash(this string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
    }
}
