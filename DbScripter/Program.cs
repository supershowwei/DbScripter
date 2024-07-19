using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

namespace DbScripter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var arguments = new Arguments();

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                switch (arg.ToLower())
                {
                    case "-connectionstring":
                        arguments.ConnectionString = args[++i].Trim('"');
                        break;
                    case "-version":
                        arguments.Version = args[++i].Trim('"');
                        break;
                    case "-database":
                        arguments.Database = args[++i].Trim('"');
                        break;
                    case "-tables":
                        arguments.Tables = args[++i].Trim('"').Split(',');
                        break;
                    case "-includetriggers":
                        arguments.IncludeTriggers = true;
                        break;
                    case "-views":
                        arguments.Views = args[++i].Trim('"').Split(',');
                        break;
                    case "-storedprocedures":
                        arguments.StoredProcedures = args[++i].Trim('"').Split(',');
                        break;
                    case "-functions":
                        arguments.Functions = args[++i].Trim('"').Split(',');
                        break;
                    case "-type":
                        arguments.Type = args[++i].Trim('"');
                        break;
                    case "-dropandcreate":
                        arguments.DropAndCreate = true;
                        break;
                    case "-output":
                        arguments.Output = args[++i].Trim('"');
                        break;
                }
            }

            var server = new Server(new ServerConnection(new SqlConnection(arguments.ConnectionString)));

            var database = server.Databases[arguments.Database];

            var scripter = new Scripter(server)
            {
                Options =
                    {
                        AnsiPadding = false,                                            // ANSI 填補
                        AppendToFile = false,                                           // 附加至檔案
                        IncludeIfNotExists = false,                                     // 檢查物件是否存在
                        ContinueScriptingOnError = false,                               // 發生錯誤時繼續編寫指令碼
                        ConvertUserDefinedDataTypesToBaseType = false,                  // 將 UDDT 轉換為基底類型
                        WithDependencies = false,                                       // 產生相依物件的指令碼：產生該物件所需的相依物件，例如：View 相依於 Table，當產生 View 的指令碼時，會一併產生 Table 的指令碼。

                        IncludeHeaders = false,                                         // 包含描述性標頭
                                                                                        // /****** Object:  Table [dbo].[AbcTable]    Script Date: 2024/6/24 12:05:24 ******/

                        IncludeScriptingParametersHeader = false,                       // 包含指令碼參數標頭
                                                                                        // /*    ==指令碼參數==
                                                                                        // 
                                                                                        //     來源伺服器版本 : SQL Server 2022 (16.0.1115)
                                                                                        //     來源資料庫引擎版本 : Microsoft SQL Server Express Edition
                                                                                        //     來源資料庫引擎類型 : 獨立 SQL Server
                                                                                        // 
                                                                                        //     目標伺服器版本 : SQL Server 2022
                                                                                        //     目標資料庫引擎版本 : Microsoft SQL Server Express Edition
                                                                                        //     目標資料庫引擎類型 : 獨立 SQL Server
                                                                                        // */

                        DriIncludeSystemNames = false,                                  // 包含系統條件約束名稱
                        SchemaQualify = true,                                           // 結構描述會限定物件名稱
                        Bindings = false,                                               // 繫結選項
                        NoCollation = true,                                             // 指令碼定序：在欄位宣告後面加入 COLLATION
                        ScriptForCreateDrop = false,                                    // 編寫 DROP 和 CREATE 的指令碼：沒有直接一個選項，但是可以透過組合 ScriptDrops=True 和 ScriptDrops=False 的指令碼來完成。
                        ExtendedProperties = true,                                      // 編寫擴充屬性的指令碼
                        TargetServerVersion = SqlServerVersion.Version160,              // 針對伺服器版本編寫指令碼
                        TargetDatabaseEngineEdition = DatabaseEngineEdition.Standard,   // 資料庫引擎版本的指令碼 
                        TargetDatabaseEngineType = DatabaseEngineType.Standalone,       // 資料庫引擎類型的指令碼
                        ScriptOwner = false,                                            // 編寫擁有者的指令碼
                        IncludeDatabaseContext = false,                                 // 編寫 USE DATABASE 的指令碼

                        ScriptSchema = true,                                            // 要編寫指令碼的資料類型
                        ScriptData = false,

                        DriChecks = false,                                              // 編寫 Check 條件約束的指令碼（在 DriAllConstraints = True 時，此選項無效。）
                        DriDefaults = false,                                            // 編寫預設值的指令碼（在 DriAllConstraints = True 時，此選項無效。）
                        DriAllConstraints = true,                                       // （強制輸出所有的條件約束）
                        
                        ScriptXmlCompression = true,                                    // 編寫 XML 壓縮選項
                        DriPrimaryKey = true,                                           // 編寫主索引鍵的指令碼
                        DriForeignKeys = true,                                          // 編寫外部索引鍵的指令碼
                        FullTextIndexes = false,                                        // 編寫全文檢索索引的指令碼
                        Indexes = false,                                                // 編寫索引的指令碼
                        DriUniqueKeys = true,                                           // 編寫唯一索引鍵的指令碼
                        ScriptDataCompression = true,                                   // 編寫資料壓縮選項的指令碼
                        Triggers = arguments.IncludeTriggers,                           // 編寫觸發程序的指令碼
                        ChangeTracking = false,                                         // 編寫變更追蹤選項的指令碼

                                                                                        // 包括不支援的陳述式 (?)
                                                                                        // 編寫登入的指令碼 (?)
                                                                                        // 編寫物件層級權限的指令碼 (?)
                                                                                        // 編寫統計資料的指令碼 (?)
                        EnforceScriptingOptions = true
                    }
            };

            scripter.PrefetchObjects = true;

            if (!string.IsNullOrEmpty(arguments.Version) && int.TryParse(arguments.Version, out var version))
            {
                switch (version)
                {
                    case 80:
                        scripter.Options.TargetServerVersion = SqlServerVersion.Version80;
                        break;
                    case 90:
                        scripter.Options.TargetServerVersion = SqlServerVersion.Version90;
                        break;
                    case 100:
                        scripter.Options.TargetServerVersion = SqlServerVersion.Version100;
                        break;
                    case 105:
                        scripter.Options.TargetServerVersion = SqlServerVersion.Version105;
                        break;
                    case 110:
                        scripter.Options.TargetServerVersion = SqlServerVersion.Version110;
                        break;
                    case 120:
                        scripter.Options.TargetServerVersion = SqlServerVersion.Version120;
                        break;
                    case 130:
                        scripter.Options.TargetServerVersion = SqlServerVersion.Version130;
                        break;
                    case 140:
                        scripter.Options.TargetServerVersion = SqlServerVersion.Version140;
                        break;
                    case 150:
                        scripter.Options.TargetServerVersion = SqlServerVersion.Version150;
                        break;
                    case 160:
                        scripter.Options.TargetServerVersion = SqlServerVersion.Version160;
                        break;
                }
            }

            if (arguments.DropAndCreate)
            {
                scripter.Options.IncludeIfNotExists = true;
                scripter.Options.ScriptForCreateDrop = true;
            }

            if (arguments.Type != null && arguments.Type.Equals("SchemaAndData", StringComparison.OrdinalIgnoreCase))
            {
                scripter.Options.ScriptData = true;
            }
            else if (arguments.Type != null && arguments.Type.Equals("Data", StringComparison.OrdinalIgnoreCase))
            {
                scripter.Options.ScriptSchema = false;
                scripter.Options.ScriptData = true;
            }

            var urnList = new List<Urn>();

            if (arguments.Tables != null && arguments.Tables.Length > 0)
            {
                foreach (Table table in database.Tables)
                {
                    if (table.IsSystemObject) continue;

                    if (arguments.Tables.Any(a => a.Equals("*") || a.Equals(table.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        urnList.Add(table.Urn);
                    }
                }
            }
            
            if (arguments.Views != null && arguments.Views.Length > 0)
            {
                foreach (View view in database.Views)
                {
                    if (view.IsSystemObject) continue;

                    if (arguments.Views.Any(a => a.Equals("*") || a.Equals(view.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        urnList.Add(view.Urn);
                    }
                }
            }

            if (arguments.StoredProcedures != null && arguments.StoredProcedures.Length > 0)
            {
                foreach (StoredProcedure storedProcedure in database.StoredProcedures)
                {
                    if (storedProcedure.IsSystemObject) continue;

                    if (arguments.StoredProcedures.Any(a => a.Equals("*") || a.Equals(storedProcedure.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        urnList.Add(storedProcedure.Urn);
                    }
                }
            }

            if (arguments.Functions != null && arguments.Functions.Length > 0)
            {
                foreach (UserDefinedFunction function in database.UserDefinedFunctions)
                {
                    if (function.IsSystemObject) continue;

                    if (arguments.Functions.Any(a => a.Equals("*") || a.Equals(function.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        urnList.Add(function.Urn);
                    }
                }
            }

            var builder = new StringBuilder();

            if (scripter.Options.ScriptForCreateDrop)
            {
                scripter.Options.ScriptForCreateDrop = false;
                scripter.Options.ScriptDrops = true;

                WriteStatements(scripter, urnList, builder);

                scripter.Options.IncludeDatabaseContext = false;
                scripter.Options.ScriptDrops = false;

                WriteStatements(scripter, urnList, builder);
            }
            else
            {
                WriteStatements(scripter, urnList, builder);
            }
            
            File.WriteAllText(arguments.Output, builder.ToString());

            // https://stackoverflow.com/questions/483568/how-can-i-automate-the-generate-scripts-task-in-sql-server-management-studio-2
            // https://stackoverflow.com/questions/25410467/c-sharp-smo-not-scripting-constraints
            // https://www.mssqltips.com/sqlservertip/1503/options-for-scripting-sql-server-database-objects/
            // https://learn.microsoft.com/en-us/sql/ssms/scripting/generate-and-publish-scripts-wizard?view=sql-server-ver16
            // https://learn.microsoft.com/en-us/dotnet/api/microsoft.sqlserver.management.smo.scriptingoptions?view=sql-smo-160
            // https://stackoverflow.com/questions/37003017/scriptingoptions-sql-smo-does-not-support-scripting-data
        }

        private static void WriteStatements(Scripter scripter, List<Urn> urnList, StringBuilder builder)
        {
            foreach (var statement in scripter.EnumScript(urnList.ToArray()))
            {
                builder.AppendLine(statement.TrimEnd('\r', '\n'));
                builder.AppendLine("GO");
            }
        }
    }
}
