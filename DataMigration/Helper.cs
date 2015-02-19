using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace DataMigration
{
    public class Helper
    {
        private static Dictionary<string, string> applicationSettings = new Dictionary<string, string>() 
        { 
            { "applicatioName", ConfigurationManager.AppSettings["applicatioName"] }, 
            { "applicatioVersion", ConfigurationManager.AppSettings["applicatioVersion"] } 
        };

        public static void ProcessData(string[] selectedItems)
        {
            ItemDTO[] configItems = Deserialize();
            ProcessData(configItems, selectedItems);
        }

        private static ItemDTO[] Deserialize()
        {
            ConfigItemsDTO result = null;
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigItemsDTO));

            FileStream fRead = File.Open("ConfigItems.xml", FileMode.Open);
            XmlReader reader = XmlReader.Create(fRead);
            result = (ConfigItemsDTO)serializer.Deserialize(reader);

            reader.Close();
            fRead.Close();

            return result.Items;
        }

        private static string GenerateQuery(ItemDTO item)
        {
            string statement = "SELECT {0} FROM {1}";
            string tables = null;
            string columns = null;
            foreach (TableDTO table in item.Tables)
            {
                if(table.Join == null)
                {
                    tables =  table.Name;
                }
                else
                {
                    if(table.Join.Type.ToUpper() == "INNER")
                    {
                        tables = string.Concat(tables, " JOIN ");
                    }
                    else if (table.Join.Type.ToUpper() == "OUTER")
                    {
                        tables = string.Concat(tables, " LEFT JOIN ");
                    }

                    tables = string.Concat(tables, table.Name, " ON ");

                    int index = 1;
                    foreach (ColumnDTO column in table.Join.Columns)
                    {
                        if (index > 1)
                        {
                            tables = string.Concat(tables, " AND ");
                        }

                        tables = string.Concat(tables, column.MappedTable, ".", column.MappedColumn, " = ", table.Name, ".", column.Name);
                        index++;
                    }
                }

                foreach(ColumnDTO column in table.Columns)
                {
                    if (columns != null)
                    {
                        columns = string.Concat(columns, ", ");
                    }
                    columns = string.Concat(columns, table.Name, ".", column.Name);
                }
            }

            bool isFilterExists = false;
            string whereBuilder = null;
            if (item.Filters != null && item.Filters.Length > 0)
            {
                statement = string.Concat(statement, " WHERE {2}");
                foreach (ColumnDTO filter in item.Filters)
                {
                    if(whereBuilder != null)
                    {
                        whereBuilder = string.Concat(whereBuilder, " AND ");
                    }

                    if(filter.Value.Contains("@"))
                    {
                        filter.Value = applicationSettings[filter.Value.Substring(1, filter.Value.Length - 1)];
                    }

                    whereBuilder = string.Concat(whereBuilder, filter.Table, ".", filter.Name, " = ", "'", filter.Value, "'");
                }

                isFilterExists = true;
            }

            string sql = isFilterExists ? String.Format(statement, columns, tables, whereBuilder) : String.Format(statement, columns, tables);

            return sql;
        }

        private static void ProcessData(ItemDTO[] items, string[] selectedItems)
        {
            List<string> sqlQueries = new List<string>();
            if (items != null && items.Length > 0)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["AFSConfig"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (ItemDTO item in items)
                    {
                        if (selectedItems.Contains(item.Name))
                        {
                            string sql = GenerateQuery(item);
                            MigrateData(connection, sql, item.Name);
                        }
                    }

                    connection.Close();
                }
            }
        }

        private static void MigrateData(SqlConnection connection, string sql, string configurationItem)
        {
            DataTable result = null;
            SqlCommand command = new SqlCommand(sql, connection);
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                result = new DataTable();
                adapter.Fill(result);
            }

            switch (configurationItem)
            {
                case "Categories":
                    MigrateCategories(result);
                    break;

                case "DataItems":
                    MigrateDataItems(result);
                    break;

                case "Expressions":
                    MigrateExpressions(result);
                    break;

                case "ExtensionPoints":
                    MigrateExtensionPoints(result);
                    break;

                case "Pages":
                    MigratePages(result);
                    break;

                case "Rules":
                    MigrateRules(result);
                    break;
            }
        }

        private static void MigrateCategories(DataTable result)
        {
            if (result != null)
            {
                List<CategoryDTO> categories = new List<CategoryDTO>();
                foreach (DataRow row in result.Rows)
                {
                    CategoryDTO category = new CategoryDTO()
                    {
                        Description = Convert.ToString(row["T_CAT"], CultureInfo.CurrentCulture),
                        DisplayName = Convert.ToString(row["N_CAT_DSPLY"], CultureInfo.CurrentCulture),
                        GroupName = Convert.ToString(row["C_REF_DATA_TYP"], CultureInfo.CurrentCulture),
                        GroupDisplayName = Convert.ToString(row["T_REF_DATA_TYP"], CultureInfo.CurrentCulture),
                        LocaleId = Convert.ToInt32(row["V_LCL_ID"], CultureInfo.CurrentCulture),
                        Name = Convert.ToString(row["N_CAT"], CultureInfo.CurrentCulture)
                    };

                    categories.Add(category);
                }

                IEnumerable<IGrouping<string, CategoryDTO>> groups = categories.GroupBy(category => category.GroupDisplayName);

                foreach (IGrouping<string, CategoryDTO> group in groups)
                {
                    IEnumerable<IGrouping<int, CategoryDTO>> localizedGroups = group.GroupBy(locale => locale.LocaleId);
                    foreach (IGrouping<int, CategoryDTO> localizedGroup in localizedGroups)
                    {
                        string categoryJSON = new JavaScriptSerializer().Serialize(localizedGroup);
                    }
                }
            }
        }

        private static void MigrateDataItems(DataTable result)
        {
            if (result != null)
            {
                List<DataItemDTO> dataItems = new List<DataItemDTO>();
                foreach (DataRow row in result.Rows)
                {
                    DataItemDTO dataItem = new DataItemDTO()
                    {
                        Description = Convert.ToString(row["T_DATAITEM"], CultureInfo.CurrentCulture),
                        DisplayName = Convert.ToString(row["N_DATAITEM_DSPLY"], CultureInfo.CurrentCulture),
                        DataItemType = Convert.ToString(row["C_DATAITEM_TYP"], CultureInfo.CurrentCulture),
                        DataItemTypeDisplay = Convert.ToString(row["N_DATAITEM_TYP_DSPLY"], CultureInfo.CurrentCulture),
                        GroupName = Convert.ToString(row["C_FNCTL_DATA_GRP"], CultureInfo.CurrentCulture),
                        GroupDisplayName = Convert.ToString(row["T_FNCTL_DATA_GRP"], CultureInfo.CurrentCulture),
                        LocaleId = Convert.ToInt32(row["V_LCL_ID"], CultureInfo.CurrentCulture),
                        Name = Convert.ToString(row["N_DATAITEM"], CultureInfo.CurrentCulture)
                    };

                    dataItems.Add(dataItem);
                }

                IEnumerable<IGrouping<string, DataItemDTO>> groups = dataItems.GroupBy(dataItem => dataItem.GroupDisplayName);

                foreach (IGrouping<string, DataItemDTO> group in groups)
                {
                    IEnumerable<IGrouping<int, DataItemDTO>> localizedGroups = group.GroupBy(locale => locale.LocaleId);
                    foreach (IGrouping<int, DataItemDTO> localizedGroup in localizedGroups)
                    {
                        string dataItemJSON = new JavaScriptSerializer().Serialize(localizedGroup);
                    }
                }
            }
        }

        private static void MigrateExpressions(DataTable result)
        {
            if (result != null)
            {
                List<ExpressionDTO> expressions = new List<ExpressionDTO>();
                foreach (DataRow row in result.Rows)
                {
                    ExpressionDTO expression = new ExpressionDTO()
                    {
                        Description = Convert.ToString(row["T_EXPRSN"], CultureInfo.CurrentCulture),
                        DisplayName = Convert.ToString(row["N_EXPRSN_DSPLY"], CultureInfo.CurrentCulture),
                        GroupName = Convert.ToString(row["C_EXPRSN_GRP"], CultureInfo.CurrentCulture),
                        GroupDisplayName = Convert.ToString(row["T_EXPRSN_GRP"], CultureInfo.CurrentCulture),
                        LocaleId = Convert.ToInt32(row["V_LCL_ID"], CultureInfo.CurrentCulture),
                        Name = Convert.ToString(row["N_EXPRSN"], CultureInfo.CurrentCulture)
                    };

                    expressions.Add(expression);
                }

                IEnumerable<IGrouping<string, ExpressionDTO>> groups = expressions.GroupBy(expression => expression.GroupDisplayName);

                foreach (IGrouping<string, ExpressionDTO> group in groups)
                {
                    IEnumerable<IGrouping<int, ExpressionDTO>> localizedGroups = group.GroupBy(locale => locale.LocaleId);
                    foreach (IGrouping<int, ExpressionDTO> localizedGroup in localizedGroups)
                    {
                        string expressionJSON = new JavaScriptSerializer().Serialize(localizedGroup);
                    }
                }
            }
        }

        private static void MigrateExtensionPoints(DataTable result)
        {
            if (result != null)
            {
                List<ExtensionPointDTO> extensionPoints = new List<ExtensionPointDTO>();
                foreach (DataRow row in result.Rows)
                {
                    ExtensionPointDTO extensionPoint = new ExtensionPointDTO()
                    {
                        Description = Convert.ToString(row["T_EXTPT"], CultureInfo.CurrentCulture),
                        DisplayName = Convert.ToString(row["N_EXTPT_DSPLY"], CultureInfo.CurrentCulture),
                        ExtensionPointType = Convert.ToString(row["C_EXTPT_TYP"], CultureInfo.CurrentCulture),
                        ExtensionPointTypeDisplay = Convert.ToString(row["N_EXTPT_TYP_DSPLY"], CultureInfo.CurrentCulture),
                        GroupName = Convert.ToString(row["C_EXTPT_GRP"], CultureInfo.CurrentCulture),
                        GroupDisplayName = Convert.ToString(row["T_EXTPT_GRP"], CultureInfo.CurrentCulture),
                        LocaleId = Convert.ToInt32(row["V_LCL_ID"], CultureInfo.CurrentCulture),
                        Name = Convert.ToString(row["N_EXTPT"], CultureInfo.CurrentCulture)
                    };

                    extensionPoints.Add(extensionPoint);
                }

                IEnumerable<IGrouping<string, ExtensionPointDTO>> groups = extensionPoints.GroupBy(extensionPoint => extensionPoint.GroupDisplayName);

                foreach (IGrouping<string, ExtensionPointDTO> group in groups)
                {
                    IEnumerable<IGrouping<int, ExtensionPointDTO>> localizedGroups = group.GroupBy(locale => locale.LocaleId);
                    foreach (IGrouping<int, ExtensionPointDTO> localizedGroup in localizedGroups)
                    {
                        string extensionPointJSON = new JavaScriptSerializer().Serialize(localizedGroup);
                    }
                }
            }
        }

        private static void MigratePages(DataTable result)
        {
            if (result != null)
            {
                List<PageDTO> pages = new List<PageDTO>();
                foreach (DataRow row in result.Rows)
                {
                    PageDTO page = new PageDTO()
                    {
                        Description = Convert.ToString(row["T_PAGE_CLS"], CultureInfo.CurrentCulture),
                        DisplayName = Convert.ToString(row["N_PAGE_CLS_DSPLY"], CultureInfo.CurrentCulture),
                        GroupName = Convert.ToString(row["C_PAGE_CLS_GRP"], CultureInfo.CurrentCulture),
                        GroupDisplayName = Convert.ToString(row["T_PAGE_CLS_GRP"], CultureInfo.CurrentCulture),
                        LocaleId = Convert.ToInt32(row["V_LCL_ID"], CultureInfo.CurrentCulture),
                        Name = Convert.ToString(row["N_PAGE_CLS"], CultureInfo.CurrentCulture)
                    };

                    pages.Add(page);
                }

                IEnumerable<IGrouping<string, PageDTO>> groups = pages.GroupBy(page => page.GroupDisplayName);

                foreach (IGrouping<string, PageDTO> group in groups)
                {
                    IEnumerable<IGrouping<int, PageDTO>> localizedGroups = group.GroupBy(locale => locale.LocaleId);
                    foreach (IGrouping<int, PageDTO> localizedGroup in localizedGroups)
                    {
                        string pageJSON = new JavaScriptSerializer().Serialize(localizedGroup);
                    }
                }
            }
        }

        private static void MigrateRules(DataTable result)
        {
            if (result != null)
            {
                List<RuleDTO> rules = new List<RuleDTO>();
                foreach (DataRow row in result.Rows)
                {
                    RuleDTO rule = new RuleDTO()
                    {
                        Description = Convert.ToString(row["T_RLE"], CultureInfo.CurrentCulture),
                        DisplayName = Convert.ToString(row["N_RLE_DSPLY"], CultureInfo.CurrentCulture),
                        ExpressionName = Convert.ToString(row["N_EXPRSN"], CultureInfo.CurrentCulture),
                        ExpressionDisplayName = Convert.ToString(row["N_EXPRSN_DSPLY"], CultureInfo.CurrentCulture),
                        GroupName = Convert.ToString(row["C_RLE_GRP"], CultureInfo.CurrentCulture),
                        GroupDisplayName = Convert.ToString(row["T_RLE_GRP"], CultureInfo.CurrentCulture),
                        LocaleId = Convert.ToInt32(row["V_LCL_ID"], CultureInfo.CurrentCulture),
                        Name = Convert.ToString(row["N_RLE"], CultureInfo.CurrentCulture)
                    };

                    rules.Add(rule);
                }

                IEnumerable<IGrouping<string, RuleDTO>> groups = rules.GroupBy(rule => rule.GroupDisplayName);

                foreach (IGrouping<string, RuleDTO> group in groups)
                {
                    IEnumerable<IGrouping<int, RuleDTO>> localizedGroups = group.GroupBy(locale => locale.LocaleId);
                    foreach (IGrouping<int, RuleDTO> localizedGroup in localizedGroups)
                    {
                        string ruleJSON = new JavaScriptSerializer().Serialize(localizedGroup);
                    }
                }
            }
        }
    }
}
